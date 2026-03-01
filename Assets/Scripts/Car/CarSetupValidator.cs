using UnityEngine;

namespace OpenWorldRealisticMobileRacer.Car
{
    public class CarSetupValidator : MonoBehaviour
    {
        [SerializeField] private WheelCollider[] wheelColliders;

        private void OnValidate()
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                if (wheel == null)
                {
                    continue;
                }

                JointSpring spring = wheel.suspensionSpring;
                spring.spring = 38000f;
                spring.damper = 5200f;
                spring.targetPosition = 0.5f;
                wheel.suspensionSpring = spring;
                wheel.mass = 35f;
                wheel.wheelDampingRate = 0.25f;

                WheelFrictionCurve forward = wheel.forwardFriction;
                forward.extremumSlip = 0.42f;
                forward.extremumValue = 1f;
                forward.asymptoteSlip = 0.8f;
                forward.asymptoteValue = 0.72f;
                forward.stiffness = 1.45f;
                wheel.forwardFriction = forward;

                WheelFrictionCurve sideways = wheel.sidewaysFriction;
                sideways.extremumSlip = 0.34f;
                sideways.extremumValue = 1f;
                sideways.asymptoteSlip = 0.7f;
                sideways.asymptoteValue = 0.75f;
                sideways.stiffness = 1.35f;
                wheel.sidewaysFriction = sideways;
            }
        }
    }
}
