using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OpenWorldRealisticMobileRacer.Car;

namespace OpenWorldRealisticMobileRacer.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text rpmText;
        [SerializeField] private TMP_Text gearText;
        [SerializeField] private Image driftFill;
        [SerializeField] private GameObject pausePanel;

        [Header("Smoothing")]
        [SerializeField] private float speedLerp = 8f;
        [SerializeField] private float rpmLerp = 7f;

        private bool paused;
        private float uiSpeed;
        private float uiRpm;

        private void Update()
        {
            if (car != null)
            {
                uiSpeed = Mathf.Lerp(uiSpeed, car.SpeedKph, Time.deltaTime * speedLerp);
                uiRpm = Mathf.Lerp(uiRpm, car.CurrentEngineRpm, Time.deltaTime * rpmLerp);

                if (speedText != null)
                {
                    speedText.text = $"{Mathf.RoundToInt(uiSpeed)} km/h";
                }

                if (rpmText != null)
                {
                    rpmText.text = $"RPM {Mathf.RoundToInt(uiRpm)}";
                }

                if (gearText != null)
                {
                    int gear = Mathf.Clamp(Mathf.FloorToInt(Mathf.InverseLerp(0f, 7800f, uiRpm) * 6f) + 1, 1, 6);
                    gearText.text = gear.ToString();
                }

                if (driftFill != null)
                {
                    driftFill.fillAmount = Mathf.Clamp01(car.LateralSlip / 1.2f);
                    driftFill.color = Color.Lerp(new Color(0.1f, 0.9f, 0.2f), new Color(1f, 0.2f, 0.1f), driftFill.fillAmount);
                }
            }
        }

        public void TogglePause()
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
            AudioListener.pause = paused;
            if (pausePanel != null)
            {
                pausePanel.SetActive(paused);
            }
        }
    }
}
