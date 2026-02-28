using UnityEngine;
using UnityEngine.Audio;

namespace OpenWorldRealisticMobileRacer.Audio
{
    [RequireComponent(typeof(Collider))]
    public class TunnelAudioZone : MonoBehaviour
    {
        [SerializeField] private AudioMixerSnapshot outsideSnapshot;
        [SerializeField] private AudioMixerSnapshot tunnelSnapshot;
        [SerializeField] private float transitionTime = 0.7f;
        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            Collider c = GetComponent<Collider>();
            c.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag) || tunnelSnapshot == null)
            {
                return;
            }

            tunnelSnapshot.TransitionTo(transitionTime);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(playerTag) || outsideSnapshot == null)
            {
                return;
            }

            outsideSnapshot.TransitionTo(transitionTime);
        }
    }
}
