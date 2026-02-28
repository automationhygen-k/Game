using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OpenWorldRealisticMobileRacer.InputSystem;

namespace OpenWorldRealisticMobileRacer.UI
{
    public class VirtualSteeringWheel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform wheelTransform;
        [SerializeField] private Image wheelForeground;
        [SerializeField] private float maxRotation = 140f;
        [SerializeField] private float returnSpeed = 180f;
        [SerializeField] private TouchInputProvider inputProvider;

        private int controllingPointer = -1;
        private float currentRotation;
        private bool pointerActive;

        public void OnPointerDown(PointerEventData eventData)
        {
            controllingPointer = eventData.pointerId;
            pointerActive = true;
            UpdateWheel(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!pointerActive || controllingPointer != eventData.pointerId)
            {
                return;
            }

            UpdateWheel(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (controllingPointer != eventData.pointerId)
            {
                return;
            }

            controllingPointer = -1;
            pointerActive = false;
        }

        private void Update()
        {
            if (!pointerActive)
            {
                currentRotation = Mathf.MoveTowards(currentRotation, 0f, returnSpeed * Time.deltaTime);
                ApplyVisuals();
                inputProvider.SetSteeringFromWheel(Mathf.InverseLerp(-maxRotation, maxRotation, currentRotation) * 2f - 1f);
            }
        }

        private void UpdateWheel(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(wheelTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                return;
            }

            float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
            float clamped = Mathf.Clamp(90f - angle, -maxRotation, maxRotation);
            currentRotation = clamped;
            inputProvider.SetSteeringFromWheel(currentRotation / maxRotation);
            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            if (wheelForeground != null)
            {
                wheelForeground.rectTransform.localEulerAngles = new Vector3(0f, 0f, -currentRotation);
            }
        }
    }
}
