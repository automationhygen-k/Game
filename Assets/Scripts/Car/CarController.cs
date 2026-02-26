using UnityEngine;
using OpenWorldRealisticMobileRacer.InputSystem;

namespace OpenWorldRealisticMobileRacer.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        [Header("Wheel Colliders")]
        [SerializeField] private WheelCollider frontLeft;
        [SerializeField] private WheelCollider frontRight;
        [SerializeField] private WheelCollider rearLeft;
        [SerializeField] private WheelCollider rearRight;

        [Header("Wheel Meshes")]
        [SerializeField] private Transform frontLeftVisual;
        [SerializeField] private Transform frontRightVisual;
        [SerializeField] private Transform rearLeftVisual;
        [SerializeField] private Transform rearRightVisual;

        [Header("Powertrain")]
        [SerializeField] private float engineForce = 6500f;
        [SerializeField] private float brakeForce = 4800f;
        [SerializeField] private float handbrakeForce = 7200f;
        [SerializeField] private float maxSteerAngle = 34f;
        [SerializeField] private float topSpeedKph = 240f;

        [Header("Handling")]
        [SerializeField] private float baseForwardStiffness = 1.45f;
        [SerializeField] private float baseSidewaysStiffness = 1.35f;
        [SerializeField] private float driftSidewaysStiffness = 0.7f;
        [SerializeField] private float highSpeedGripReduction = 0.78f;
        [SerializeField] private float tractionRecoverySpeed = 2.5f;
        [SerializeField] private AnimationCurve steeringBySpeed = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.35f);

        [Header("Input")]
        [SerializeField] private TouchInputProvider inputProvider;

        public float SpeedKph { get; private set; }
        public float CurrentEngineRpm { get; private set; }
        public float LateralSlip { get; private set; }
        public bool IsDrifting { get; private set; }

        private Rigidbody rb;
        private float driftBlend;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0f, -0.45f, 0f);
        }

        private void FixedUpdate()
        {
            SpeedKph = rb.velocity.magnitude * 3.6f;
            inputProvider.SetVehicleSpeed(SpeedKph);

            ApplySteering();
            ApplyMotorAndBrakes();
            ApplyGripModel();
            LimitTopSpeed();
            SyncWheelVisuals();
            UpdateTelemetry();
        }

        private void ApplySteering()
        {
            float speed01 = Mathf.Clamp01(SpeedKph / topSpeedKph);
            float steerLimit = maxSteerAngle * steeringBySpeed.Evaluate(speed01);
            float steerInput = inputProvider.Steering;

            frontLeft.steerAngle = steerInput * steerLimit;
            frontRight.steerAngle = steerInput * steerLimit;
        }

        private void ApplyMotorAndBrakes()
        {
            float throttle = inputProvider.Throttle;
            float brake = inputProvider.Brake;
            bool handbrake = inputProvider.Handbrake;

            float torque = throttle * engineForce;
            rearLeft.motorTorque = torque;
            rearRight.motorTorque = torque;

            float totalBrake = brake * brakeForce;
            if (handbrake)
            {
                totalBrake += handbrakeForce;
            }

            frontLeft.brakeTorque = brake * brakeForce;
            frontRight.brakeTorque = brake * brakeForce;
            rearLeft.brakeTorque = totalBrake;
            rearRight.brakeTorque = totalBrake;
        }

        private void ApplyGripModel()
        {
            WheelFrictionCurve fl = frontLeft.sidewaysFriction;
            WheelFrictionCurve fr = frontRight.sidewaysFriction;
            WheelFrictionCurve rl = rearLeft.sidewaysFriction;
            WheelFrictionCurve rr = rearRight.sidewaysFriction;

            float speedFactor = Mathf.Clamp01(SpeedKph / topSpeedKph);
            float speedGrip = Mathf.Lerp(1f, highSpeedGripReduction, speedFactor);

            WheelHit hit;
            rearLeft.GetGroundHit(out hit);
            float leftSlip = Mathf.Abs(hit.sidewaysSlip);
            rearRight.GetGroundHit(out hit);
            float rightSlip = Mathf.Abs(hit.sidewaysSlip);
            LateralSlip = Mathf.Max(leftSlip, rightSlip);

            bool wantsDrift = inputProvider.Handbrake || (Mathf.Abs(inputProvider.Steering) > 0.45f && inputProvider.Throttle > 0.65f && SpeedKph > 35f);
            float targetBlend = wantsDrift ? 1f : 0f;
            driftBlend = Mathf.MoveTowards(driftBlend, targetBlend, tractionRecoverySpeed * Time.fixedDeltaTime);

            float rearStiffness = Mathf.Lerp(baseSidewaysStiffness * speedGrip, driftSidewaysStiffness, driftBlend);
            float frontStiffness = Mathf.Lerp(baseSidewaysStiffness * speedGrip, baseSidewaysStiffness * 0.85f, driftBlend * 0.6f);

            fl.stiffness = frontStiffness;
            fr.stiffness = frontStiffness;
            rl.stiffness = rearStiffness;
            rr.stiffness = rearStiffness;

            WheelFrictionCurve ffl = frontLeft.forwardFriction;
            WheelFrictionCurve ffr = frontRight.forwardFriction;
            WheelFrictionCurve frl = rearLeft.forwardFriction;
            WheelFrictionCurve frr = rearRight.forwardFriction;

            float forwardGrip = baseForwardStiffness * Mathf.Lerp(1f, 0.78f, driftBlend * 0.75f);
            ffl.stiffness = forwardGrip;
            ffr.stiffness = forwardGrip;
            frl.stiffness = forwardGrip;
            frr.stiffness = forwardGrip;

            frontLeft.sidewaysFriction = fl;
            frontRight.sidewaysFriction = fr;
            rearLeft.sidewaysFriction = rl;
            rearRight.sidewaysFriction = rr;

            frontLeft.forwardFriction = ffl;
            frontRight.forwardFriction = ffr;
            rearLeft.forwardFriction = frl;
            rearRight.forwardFriction = frr;

            IsDrifting = driftBlend > 0.35f && LateralSlip > 0.2f;
        }

        private void LimitTopSpeed()
        {
            if (SpeedKph <= topSpeedKph)
            {
                return;
            }

            rb.velocity = rb.velocity.normalized * (topSpeedKph / 3.6f);
        }

        private void UpdateTelemetry()
        {
            float wheelRpm = (Mathf.Abs(rearLeft.rpm) + Mathf.Abs(rearRight.rpm)) * 0.5f;
            float gearInfluence = Mathf.Lerp(2.4f, 0.9f, Mathf.Clamp01(SpeedKph / topSpeedKph));
            CurrentEngineRpm = Mathf.Clamp(900f + wheelRpm * gearInfluence * 30f + inputProvider.Throttle * 900f, 850f, 7800f);
        }

        private void SyncWheelVisuals()
        {
            UpdateWheelPose(frontLeft, frontLeftVisual);
            UpdateWheelPose(frontRight, frontRightVisual);
            UpdateWheelPose(rearLeft, rearLeftVisual);
            UpdateWheelPose(rearRight, rearRightVisual);
        }

        private static void UpdateWheelPose(WheelCollider col, Transform visual)
        {
            if (col == null || visual == null)
            {
                return;
            }

            Vector3 pos;
            Quaternion rot;
            col.GetWorldPose(out pos, out rot);
            visual.position = pos;
            visual.rotation = rot;
        }
    }
}
