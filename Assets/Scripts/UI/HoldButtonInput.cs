using UnityEngine;
using UnityEngine.EventSystems;
using OpenWorldRealisticMobileRacer.InputSystem;

namespace OpenWorldRealisticMobileRacer.UI
{
    public class HoldButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public enum InputType
        {
            Throttle,
            Brake,
            Handbrake
        }

        [SerializeField] private InputType inputType;
        [SerializeField] private TouchInputProvider inputProvider;

        public void OnPointerDown(PointerEventData eventData)
        {
            SetPressed(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetPressed(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerPress == gameObject)
            {
                SetPressed(false);
            }
        }

        private void SetPressed(bool pressed)
        {
            switch (inputType)
            {
                case InputType.Throttle:
                    inputProvider.SetThrottle(pressed);
                    break;
                case InputType.Brake:
                    inputProvider.SetBrake(pressed);
                    break;
                case InputType.Handbrake:
                    inputProvider.SetHandbrake(pressed);
                    break;
            }
        }
    }
}
