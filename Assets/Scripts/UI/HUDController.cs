using TMPro;
using UnityEngine;
using OpenWorldRealisticMobileRacer.Car;

namespace OpenWorldRealisticMobileRacer.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text rpmText;
        [SerializeField] private GameObject pausePanel;

        private bool paused;

        private void Update()
        {
            if (car != null)
            {
                if (speedText != null)
                {
                    speedText.text = $"{Mathf.RoundToInt(car.SpeedKph)} km/h";
                }

                if (rpmText != null)
                {
                    rpmText.text = $"RPM {Mathf.RoundToInt(car.CurrentEngineRpm)}";
                }
            }
        }

        public void TogglePause()
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(paused);
            }
        }
    }
}
