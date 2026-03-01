using UnityEngine;
using OpenWorldRealisticMobileRacer.Car;

namespace OpenWorldRealisticMobileRacer.CameraSystem
{
    public class ChaseCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private CarController car;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3.4f, -6.8f);
        [SerializeField] private float positionDamping = 6f;
        [SerializeField] private float rotationDamping = 5f;
        [SerializeField] private float minFov = 62f;
        [SerializeField] private float maxFov = 82f;
        [SerializeField] private float fovResponseSpeed = 4.5f;
        [SerializeField] private float driftShake = 0.1f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.TransformPoint(offset);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionDamping);

            Vector3 lookTarget = target.position + Vector3.up * 1.1f;
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);

            float speed01 = car == null ? 0f : Mathf.Clamp01(car.SpeedKph / 220f);
            float desiredFov = Mathf.Lerp(minFov, maxFov, speed01);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFov, Time.deltaTime * fovResponseSpeed);

            if (car != null && car.IsDrifting)
            {
                transform.position += Random.insideUnitSphere * driftShake;
            }
        }
    }
}
