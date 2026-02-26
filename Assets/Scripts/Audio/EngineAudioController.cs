using UnityEngine;
using OpenWorldRealisticMobileRacer.Car;
using OpenWorldRealisticMobileRacer.InputSystem;

namespace OpenWorldRealisticMobileRacer.Audio
{
    public class EngineAudioController : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private TouchInputProvider inputProvider;

        [Header("Engine Layers")]
        [SerializeField] private AudioSource idleLayer;
        [SerializeField] private AudioSource lowLayer;
        [SerializeField] private AudioSource midLayer;
        [SerializeField] private AudioSource highLayer;

        [Header("Skid / Shift")]
        [SerializeField] private AudioSource skidSource;
        [SerializeField] private AudioSource shiftDipSource;
        [SerializeField] private float skidSlipThreshold = 0.23f;

        [Header("RPM Mapping")]
        [SerializeField] private float minRpm = 850f;
        [SerializeField] private float maxRpm = 7800f;
        [SerializeField] private Vector2 pitchRange = new Vector2(0.75f, 2f);

        [Header("Variation")]
        [SerializeField] private float randomPitchJitter = 0.02f;
        [SerializeField] private float jitterSpeed = 2.4f;

        private float jitterTime;
        private int previousGear;

        private void Start()
        {
            EnsureLooping(idleLayer, lowLayer, midLayer, highLayer);
            previousGear = 1;
        }

        private void Update()
        {
            float rpm = Mathf.Clamp(car.CurrentEngineRpm, minRpm, maxRpm);
            float rpm01 = Mathf.InverseLerp(minRpm, maxRpm, rpm);
            float throttle = inputProvider.Throttle;

            jitterTime += Time.deltaTime * jitterSpeed;
            float jitter = (Mathf.PerlinNoise(jitterTime, 0f) - 0.5f) * 2f * randomPitchJitter;
            float basePitch = Mathf.Lerp(pitchRange.x, pitchRange.y, rpm01) + jitter;

            idleLayer.pitch = Mathf.Lerp(0.8f, 1.1f, rpm01 * 0.35f) + jitter;
            lowLayer.pitch = basePitch * 0.82f;
            midLayer.pitch = basePitch;
            highLayer.pitch = basePitch * 1.18f;

            float idleVol = Mathf.Lerp(0.9f, 0.25f, rpm01);
            float lowVol = Mathf.Clamp01(1f - Mathf.Abs(rpm01 - 0.25f) * 4f);
            float midVol = Mathf.Clamp01(1f - Mathf.Abs(rpm01 - 0.55f) * 3.2f);
            float highVol = Mathf.Clamp01(Mathf.InverseLerp(0.55f, 1f, rpm01));

            float throttleBoost = Mathf.Lerp(0.85f, 1.2f, throttle);
            idleLayer.volume = idleVol;
            lowLayer.volume = lowVol * throttleBoost;
            midLayer.volume = midVol * throttleBoost;
            highLayer.volume = highVol * throttleBoost;

            int gear = Mathf.Clamp(Mathf.FloorToInt(Mathf.Lerp(1f, 6f, rpm01)), 1, 6);
            if (gear != previousGear)
            {
                if (shiftDipSource != null)
                {
                    shiftDipSource.pitch = Random.Range(0.95f, 1.05f);
                    shiftDipSource.Play();
                }

                previousGear = gear;
            }

            if (skidSource != null)
            {
                bool shouldSkid = car.LateralSlip > skidSlipThreshold && car.SpeedKph > 30f;
                if (shouldSkid)
                {
                    if (!skidSource.isPlaying)
                    {
                        skidSource.Play();
                    }

                    skidSource.volume = Mathf.InverseLerp(skidSlipThreshold, skidSlipThreshold + 0.75f, car.LateralSlip);
                    skidSource.pitch = Mathf.Lerp(0.85f, 1.2f, Mathf.Clamp01(car.LateralSlip));
                }
                else if (skidSource.isPlaying)
                {
                    skidSource.Stop();
                }
            }
        }

        private static void EnsureLooping(params AudioSource[] sources)
        {
            foreach (AudioSource source in sources)
            {
                if (source == null)
                {
                    continue;
                }

                source.loop = true;
                if (!source.isPlaying)
                {
                    source.Play();
                }
            }
        }
    }
}
