using UnityEngine;
using UnityEngine.EventSystems;

namespace OpenWorldRealisticMobileRacer.InputSystem
{
    public class TouchInputProvider : MonoBehaviour
    {
        [Header("Sensitivity")]
        [SerializeField, Range(0.25f, 3f)] private float steeringSensitivity = 1.1f;
        [SerializeField, Range(0.1f, 0.95f)] private float steeringSmoothing = 0.22f;
        [SerializeField, Range(20f, 220f)] private float maxSpeedForSteeringReduction = 140f;
        [SerializeField, Range(0.15f, 1f)] private float minSteeringFactorAtTopSpeed = 0.4f;

        [Header("Fallback Keyboard (Editor Only)")]
        [SerializeField] private bool allowKeyboardFallback = true;

        public float Steering { get; private set; }
        public float Throttle { get; private set; }
        public float Brake { get; private set; }
        public bool Handbrake { get; private set; }

        private float steeringTarget;
        private float smoothedSteering;
        private float currentSpeedKph;

        public void SetSteeringFromWheel(float normalizedSteering)
        {
            steeringTarget = Mathf.Clamp(normalizedSteering, -1f, 1f);
        }

        public void SetThrottle(bool pressed)
        {
            Throttle = pressed ? 1f : 0f;
        }

        public void SetBrake(bool pressed)
        {
            Brake = pressed ? 1f : 0f;
        }

        public void SetHandbrake(bool pressed)
        {
            Handbrake = pressed;
        }

        public void SetVehicleSpeed(float speedKph)
        {
            currentSpeedKph = Mathf.Max(0f, speedKph);
        }

        public void SetSensitivity(float value)
        {
            steeringSensitivity = Mathf.Clamp(value, 0.25f, 3f);
        }

        private void Update()
        {
            float speedT = Mathf.Clamp01(currentSpeedKph / maxSpeedForSteeringReduction);
            float speedFactor = Mathf.Lerp(1f, minSteeringFactorAtTopSpeed, speedT);
            float target = steeringTarget * steeringSensitivity * speedFactor;
            smoothedSteering = Mathf.Lerp(smoothedSteering, target, 1f - steeringSmoothing);
            Steering = Mathf.Clamp(smoothedSteering, -1f, 1f);

#if UNITY_EDITOR
            if (allowKeyboardFallback && !EventSystem.current.IsPointerOverGameObject())
            {
                float h = UnityEngine.Input.GetAxis("Horizontal");
                float v = UnityEngine.Input.GetAxis("Vertical");
                Steering = Mathf.Lerp(Steering, h, 0.1f);
                Throttle = Mathf.Clamp01(v);
                Brake = Mathf.Clamp01(-v);
                Handbrake = UnityEngine.Input.GetKey(KeyCode.Space);
            }
#endif
        }
    }
}
