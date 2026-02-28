using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenWorldRealisticMobileRacer.World
{
    /// <summary>
    /// Lightweight runtime scaler for mobile: keeps visual quality high but adapts when frame-time spikes.
    /// </summary>
    public class AdaptiveQualityRuntime : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;

        [Header("Frame Budget")]
        [SerializeField] private int targetFps = 60;
        [SerializeField] private float toleranceMs = 2.4f;
        [SerializeField] private float sampleSmoothing = 0.08f;

        [Header("Render Scale Bounds")]
        [SerializeField] private float minRenderScale = 0.82f;
        [SerializeField] private float maxRenderScale = 1.18f;
        [SerializeField] private float scaleStep = 0.02f;

        [Header("Shadow Bounds")]
        [SerializeField] private float minShadowDistance = 70f;
        [SerializeField] private float maxShadowDistance = 170f;
        [SerializeField] private float shadowStep = 6f;

        private float smoothedDelta;
        private float timeUntilNextAdjust;

        private void Awake()
        {
            if (urpAsset == null)
            {
                urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            }

            targetFps = Mathf.Clamp(targetFps, 30, 120);
        }

        private void Update()
        {
            smoothedDelta = Mathf.Lerp(smoothedDelta, Time.unscaledDeltaTime, sampleSmoothing);
            timeUntilNextAdjust -= Time.unscaledDeltaTime;

            if (timeUntilNextAdjust > 0f)
            {
                return;
            }

            timeUntilNextAdjust = 0.33f;

            float budgetMs = 1000f / targetFps;
            float currentMs = smoothedDelta * 1000f;

            if (currentMs > budgetMs + toleranceMs)
            {
                ReduceCost();
            }
            else if (currentMs < budgetMs - toleranceMs)
            {
                RaiseQuality();
            }
        }

        private void ReduceCost()
        {
            if (urpAsset != null)
            {
                urpAsset.renderScale = Mathf.Max(minRenderScale, urpAsset.renderScale - scaleStep);
            }

            QualitySettings.shadowDistance = Mathf.Max(minShadowDistance, QualitySettings.shadowDistance - shadowStep);
            ScalableBufferManager.ResizeBuffers(0.95f, 0.95f);
        }

        private void RaiseQuality()
        {
            if (urpAsset != null)
            {
                urpAsset.renderScale = Mathf.Min(maxRenderScale, urpAsset.renderScale + scaleStep);
            }

            QualitySettings.shadowDistance = Mathf.Min(maxShadowDistance, QualitySettings.shadowDistance + shadowStep);
            ScalableBufferManager.ResizeBuffers(1f, 1f);
        }
    }
}
