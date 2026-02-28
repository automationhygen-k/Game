using UnityEngine;

namespace OpenWorldRealisticMobileRacer.World
{
    public class WorldBootstrap : MonoBehaviour
    {
        [Header("Atmosphere")]
        [SerializeField] private Material skyboxMaterial;
        [SerializeField] private Color fogColor = new Color(0.67f, 0.73f, 0.82f);
        [SerializeField] private bool enableFog = true;
        [SerializeField] private float fogDensity = 0.0028f;

        [Header("High-End Visual Targets")]
        [SerializeField] private float shadowDistance = 140f;
        [SerializeField] private ShadowResolution shadowResolution = ShadowResolution.VeryHigh;
        [SerializeField] private ShadowQuality shadowQuality = ShadowQuality.All;
        [SerializeField] private int antiAliasing = 4;
        [SerializeField] private float lodBias = 2.0f;
        [SerializeField] private AnisotropicFiltering anisotropicFiltering = AnisotropicFiltering.ForceEnable;

        [Header("Frame Targets")]
        [SerializeField] private bool prefer120HzWhenAvailable = true;
        [SerializeField] private int fallbackFrameRate = 60;

        private void Awake()
        {
            if (skyboxMaterial != null)
            {
                RenderSettings.skybox = skyboxMaterial;
            }

            RenderSettings.fog = enableFog;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = fogColor;

            QualitySettings.shadowDistance = shadowDistance;
            QualitySettings.shadowResolution = shadowResolution;
            QualitySettings.shadows = shadowQuality;
            QualitySettings.antiAliasing = antiAliasing;
            QualitySettings.lodBias = lodBias;
            QualitySettings.anisotropicFiltering = anisotropicFiltering;

            int target = fallbackFrameRate;
            if (prefer120HzWhenAvailable)
            {
#if UNITY_2022_2_OR_NEWER
                if (Screen.currentResolution.refreshRateRatio.value >= 118f)
                {
                    target = 120;
                }
#else
                if (Screen.currentResolution.refreshRate >= 118)
                {
                    target = 120;
                }
#endif
            }

            Application.targetFrameRate = target;
        }
    }
}
