using UnityEngine;

namespace OpenWorldRealisticMobileRacer.World
{
    /// <summary>
    /// Pushes high-end visual settings by default, then gracefully scales down for low-memory devices.
    /// Attach once in the startup scene.
    /// </summary>
    public class UltraGraphicsConfigurator : MonoBehaviour
    {
        [Header("Performance Guard Rails")]
        [SerializeField] private int minSystemMemoryGbForUltra = 8;
        [SerializeField] private int minGraphicsMemoryMbForUltra = 4096;

        [Header("Ultra")]
        [SerializeField] private int ultraQualityLevel = 5;
        [SerializeField] private float ultraShadowDistance = 160f;
        [SerializeField] private int ultraTextureLimit = 0;

        [Header("High Fallback")]
        [SerializeField] private int highQualityLevel = 3;
        [SerializeField] private float highShadowDistance = 110f;
        [SerializeField] private int highTextureLimit = 0;

        [Header("Balanced Fallback")]
        [SerializeField] private int balancedQualityLevel = 2;
        [SerializeField] private float balancedShadowDistance = 85f;
        [SerializeField] private int balancedTextureLimit = 1;

        private void Start()
        {
            ApplyTier();
        }

        private void ApplyTier()
        {
            int memoryGb = SystemInfo.systemMemorySize / 1024;
            int gfxMb = SystemInfo.graphicsMemorySize;

            bool ultra = memoryGb >= minSystemMemoryGbForUltra && gfxMb >= minGraphicsMemoryMbForUltra;
            bool high = memoryGb >= 6 && gfxMb >= 2048;

            if (ultra)
            {
                SetQuality(ultraQualityLevel, ultraShadowDistance, ultraTextureLimit, 2.2f, 4);
                return;
            }

            if (high)
            {
                SetQuality(highQualityLevel, highShadowDistance, highTextureLimit, 1.65f, 2);
                return;
            }

            SetQuality(balancedQualityLevel, balancedShadowDistance, balancedTextureLimit, 1.25f, 2);
        }

        private static void SetQuality(int qualityIndex, float shadows, int masterTextureLimit, float lodBias, int aa)
        {
            if (qualityIndex >= 0 && qualityIndex < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(qualityIndex, true);
            }

            QualitySettings.shadowDistance = shadows;
            QualitySettings.masterTextureLimit = masterTextureLimit;
            QualitySettings.lodBias = lodBias;
            QualitySettings.antiAliasing = aa;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }
    }
}
