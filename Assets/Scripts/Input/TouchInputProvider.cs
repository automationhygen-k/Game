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

        [Header("Swipe Steering (Optional)")]
        [SerializeField] private bool enableSwipeSteering;
        [SerializeField] private float swipeDeadZonePixels = 12f;
        [SerializeField] private float swipeToFullSteerPixels = 220f;

        [Header("Fallback Keyboard (Editor Only)")]
        [SerializeField] private bool allowKeyboardFallback = true;

        public float Steering { get; private set; }
        public float Throttle { get; private set; }
        public float Brake { get; private set; }
        public bool Handbrake { get; private set; }

        private float steeringTarget;
        private float smoothedSteering;
        private float currentSpeedKph;

        private bool swipeActive;
        private int swipeFingerId = -1;
        private Vector2 swipeStart;

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
            if (enableSwipeSteering)
            {
                HandleSwipeSteering();
            }

            float speedT = Mathf.Clamp01(currentSpeedKph / maxSpeedForSteeringReduction);
            float speedFactor = Mathf.Lerp(1f, minSteeringFactorAtTopSpeed, speedT);
            float target = steeringTarget * steeringSensitivity * speedFactor;
            smoothedSteering = Mathf.Lerp(smoothedSteering, target, 1f - steeringSmoothing);
            Steering = Mathf.Clamp(smoothedSteering, -1f, 1f);

#if UNITY_EDITOR
            bool pointerOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (allowKeyboardFallback && !pointerOverUi)
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

        private void HandleSwipeSteering()
        {
            if (Input.touchCount == 0)
            {
                swipeActive = false;
                swipeFingerId = -1;
                return;
            }

            if (!swipeActive)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase != TouchPhase.Began)
                    {
                        continue;
                    }

                    swipeActive = true;
                    swipeFingerId = touch.fingerId;
                    swipeStart = touch.position;
                    break;
                }

                return;
            }

            bool found = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId != swipeFingerId)
                {
                    continue;
                }

                found = true;
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    swipeActive = false;
                    swipeFingerId = -1;
                    steeringTarget = 0f;
                    return;
                }

                float deltaX = touch.position.x - swipeStart.x;
                if (Mathf.Abs(deltaX) < swipeDeadZonePixels)
                {
                    steeringTarget = 0f;
                    return;
                }

                steeringTarget = Mathf.Clamp(deltaX / swipeToFullSteerPixels, -1f, 1f);
                return;
            }

            if (!found)
            {
                swipeActive = false;
                swipeFingerId = -1;
            }
        }
    }
}
