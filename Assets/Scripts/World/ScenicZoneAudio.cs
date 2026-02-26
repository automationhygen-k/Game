using UnityEngine;

namespace OpenWorldRealisticMobileRacer.World
{
    [RequireComponent(typeof(Collider))]
    public class ScenicZoneAudio : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private float fadeSpeed = 1.8f;
        [SerializeField] private float maxVolume = 0.85f;

        private bool inside;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            if (ambienceSource == null)
            {
                return;
            }

            float target = inside ? maxVolume : 0f;
            ambienceSource.volume = Mathf.MoveTowards(ambienceSource.volume, target, fadeSpeed * Time.deltaTime);

            if (!ambienceSource.isPlaying)
            {
                ambienceSource.loop = true;
                ambienceSource.Play();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                inside = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                inside = false;
            }
        }
    }
}
