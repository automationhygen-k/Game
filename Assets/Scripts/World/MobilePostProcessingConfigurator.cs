using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenWorldRealisticMobileRacer.World
{
    /// <summary>
    /// High-quality mobile post stack with adaptive levels for mid/high/premium devices.
    /// Adds cinematic depth cues (motion blur, depth of field, tone mapping) with mobile-safe ranges.
    /// </summary>
    [RequireComponent(typeof(Volume))]
    public class MobilePostProcessingConfigurator : MonoBehaviour
    {
        private enum VisualTier
        {
            Mid,
            High,
            Premium
        }

        [Header("Device Thresholds")]
        [SerializeField] private int highRamGb = 8;
        [SerializeField] private int premiumRamGb = 12;
        [SerializeField] private int highGpuMb = 3072;
        [SerializeField] private int premiumGpuMb = 6144;

        [Header("Shared")]
        [SerializeField] private bool enableMotionBlur = true;
        [SerializeField] private bool enableDepthOfField = true;

        private Volume volume;
        private VolumeProfile profile;

        private void Awake()
        {
            volume = GetComponent<Volume>();
            profile = volume.profile != null ? volume.profile : ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            ApplyTierSettings(DetectTier());
        }

        private VisualTier DetectTier()
        {
            int ramGb = SystemInfo.systemMemorySize / 1024;
            int gpuMb = SystemInfo.graphicsMemorySize;

            if (ramGb >= premiumRamGb && gpuMb >= premiumGpuMb)
            {
                return VisualTier.Premium;
            }

            if (ramGb >= highRamGb && gpuMb >= highGpuMb)
            {
                return VisualTier.High;
            }

            return VisualTier.Mid;
        }

        private void ApplyTierSettings(VisualTier tier)
        {
            ColorAdjustments color = Ensure<ColorAdjustments>();
            Tonemapping tone = Ensure<Tonemapping>();
            Bloom bloom = Ensure<Bloom>();
            Vignette vignette = Ensure<Vignette>();
            MotionBlur motionBlur = Ensure<MotionBlur>();
            DepthOfField dof = Ensure<DepthOfField>();
            ChromaticAberration chroma = Ensure<ChromaticAberration>();
            FilmGrain grain = Ensure<FilmGrain>();
            WhiteBalance wb = Ensure<WhiteBalance>();

            tone.mode.Override(TonemappingMode.ACES);
            color.postExposure.Override(0.06f);
            color.contrast.Override(10f);
            color.saturation.Override(5f);
            wb.temperature.Override(2f);
            wb.tint.Override(1f);

            switch (tier)
            {
                case VisualTier.Premium:
                    bloom.intensity.Override(0.38f);
                    bloom.threshold.Override(0.92f);
                    bloom.scatter.Override(0.82f);
                    vignette.intensity.Override(0.18f);

                    motionBlur.active = enableMotionBlur;
                    motionBlur.quality.Override(MotionBlurQuality.High);
                    motionBlur.intensity.Override(0.18f);

                    dof.active = enableDepthOfField;
                    dof.mode.Override(DepthOfFieldMode.Gaussian);
                    dof.gaussianStart.Override(8f);
                    dof.gaussianEnd.Override(42f);
                    dof.gaussianMaxRadius.Override(1.4f);

                    chroma.intensity.Override(0.04f);
                    grain.intensity.Override(0.05f);
                    break;

                case VisualTier.High:
                    bloom.intensity.Override(0.32f);
                    bloom.threshold.Override(0.96f);
                    bloom.scatter.Override(0.72f);
                    vignette.intensity.Override(0.14f);

                    motionBlur.active = enableMotionBlur;
                    motionBlur.quality.Override(MotionBlurQuality.Medium);
                    motionBlur.intensity.Override(0.14f);

                    dof.active = enableDepthOfField;
                    dof.mode.Override(DepthOfFieldMode.Gaussian);
                    dof.gaussianStart.Override(10f);
                    dof.gaussianEnd.Override(34f);
                    dof.gaussianMaxRadius.Override(1f);

                    chroma.intensity.Override(0.025f);
                    grain.intensity.Override(0.03f);
                    break;

                default:
                    bloom.intensity.Override(0.22f);
                    bloom.threshold.Override(1.02f);
                    bloom.scatter.Override(0.62f);
                    vignette.intensity.Override(0.1f);

                    motionBlur.active = false;
                    dof.active = false;

                    chroma.intensity.Override(0.01f);
                    grain.intensity.Override(0.015f);
                    break;
            }
        }

        private T Ensure<T>() where T : VolumeComponent
        {
            if (!profile.TryGet(out T component))
            {
                component = profile.Add<T>(true);
            }

            component.active = true;
            return component;
        }
    }
}
