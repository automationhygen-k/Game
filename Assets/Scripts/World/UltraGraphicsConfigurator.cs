using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenWorldRealisticMobileRacer.World
{
    /// <summary>
    /// Applies a visual tier tuned for mid -> premium mobile processors.
    /// Premium devices get near-max fidelity; mid-tier gets stable high quality.
    /// </summary>
    public class UltraGraphicsConfigurator : MonoBehaviour
    {
        private enum DeviceTier
        {
            Mid,
            High,
            Premium
        }

        [Header("Device Tier Thresholds")]
        [SerializeField] private int midRamGb = 6;
        [SerializeField] private int highRamGb = 8;
        [SerializeField] private int premiumRamGb = 12;
        [SerializeField] private int highGpuMb = 3072;
        [SerializeField] private int premiumGpuMb = 6144;

        [Header("General")]
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;
        [SerializeField] private bool enableVSyncOnPremium = true;

        [Header("Frame Targets")]
        [SerializeField] private int midTargetFps = 45;
        [SerializeField] private int highTargetFps = 60;
        [SerializeField] private int premiumTargetFps = 90;

        private void Start()
        {
            DeviceTier tier = DetectTier();
            ApplyTier(tier);
        }

        private DeviceTier DetectTier()
        {
            int ramGb = SystemInfo.systemMemorySize / 1024;
            int gpuMb = SystemInfo.graphicsMemorySize;

            if (ramGb >= premiumRamGb && gpuMb >= premiumGpuMb)
            {
                return DeviceTier.Premium;
            }

            if (ramGb >= highRamGb && gpuMb >= highGpuMb)
            {
                return DeviceTier.High;
            }

            if (ramGb >= midRamGb)
            {
                return DeviceTier.Mid;
            }

            return DeviceTier.Mid;
        }

        private void ApplyTier(DeviceTier tier)
        {
            switch (tier)
            {
                case DeviceTier.Premium:
                    ApplyQuality(qualityIndex: 5, shadowDistance: 180f, lodBias: 2.6f, aa: 4, textureLimit: 0, anisotropic: AnisotropicFiltering.ForceEnable);
                    ApplyUrp(renderScale: 1.2f, msaa: 4, supportsHdr: true, mainLightShadows: true, additionalLightShadows: true, softShadows: true);
                    SetTargetFps(premiumTargetFps, enableVSyncOnPremium ? 1 : 0);
                    break;

                case DeviceTier.High:
                    ApplyQuality(qualityIndex: 4, shadowDistance: 130f, lodBias: 1.9f, aa: 4, textureLimit: 0, anisotropic: AnisotropicFiltering.ForceEnable);
                    ApplyUrp(renderScale: 1.05f, msaa: 4, supportsHdr: true, mainLightShadows: true, additionalLightShadows: true, softShadows: true);
                    SetTargetFps(highTargetFps, 0);
                    break;

                default:
                    ApplyQuality(qualityIndex: 3, shadowDistance: 95f, lodBias: 1.4f, aa: 2, textureLimit: 1, anisotropic: AnisotropicFiltering.Enable);
                    ApplyUrp(renderScale: 0.92f, msaa: 2, supportsHdr: true, mainLightShadows: true, additionalLightShadows: false, softShadows: false);
                    SetTargetFps(midTargetFps, 0);
                    break;
            }
        }

        private static void ApplyQuality(int qualityIndex, float shadowDistance, float lodBias, int aa, int textureLimit, AnisotropicFiltering anisotropic)
        {
            if (qualityIndex >= 0 && qualityIndex < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(qualityIndex, true);
            }

            QualitySettings.shadowDistance = shadowDistance;
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.lodBias = lodBias;
            QualitySettings.antiAliasing = aa;
            QualitySettings.masterTextureLimit = textureLimit;
            QualitySettings.anisotropicFiltering = anisotropic;
        }

        private void ApplyUrp(float renderScale, int msaa, bool supportsHdr, bool mainLightShadows, bool additionalLightShadows, bool softShadows)
        {
            UniversalRenderPipelineAsset activeUrp = urpAsset != null
                ? urpAsset
                : GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (activeUrp == null)
            {
                return;
            }

            activeUrp.renderScale = Mathf.Clamp(renderScale, 0.7f, 1.4f);
            activeUrp.msaaSampleCount = msaa;
            activeUrp.supportsHDR = supportsHdr;
            activeUrp.supportsMainLightShadows = mainLightShadows;
            activeUrp.supportsAdditionalLightShadows = additionalLightShadows;
            activeUrp.supportsSoftShadows = softShadows;
            activeUrp.supportsCameraDepthTexture = true;
            activeUrp.supportsCameraOpaqueTexture = false;
        }

        private static void SetTargetFps(int targetFps, int vSyncCount)
        {
            QualitySettings.vSyncCount = vSyncCount;
            Application.targetFrameRate = targetFps;
        }
    }
}
