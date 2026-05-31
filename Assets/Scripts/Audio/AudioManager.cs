using UnityEngine;

namespace GravityFlip.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private AudioSource sfxSource;

        [Header("Clips (assign in Inspector)")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip flipClip;
        [SerializeField] private AudioClip collectClip;
        [SerializeField] private AudioClip deathClip;
        [SerializeField] private AudioClip doorUnlockClip;
        [SerializeField] private AudioClip levelCompleteClip;
        [SerializeField] private AudioClip levelResetClip;

        [Header("Mix")]
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        private void Awake()
        {
            if (sfxSource == null)
            {
                sfxSource = GetComponent<AudioSource>();
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }

        public void PlayJump() => PlayOneShot(jumpClip);

        public void PlayFlip() => PlayOneShot(flipClip);

        public void PlayCollect() => PlayOneShot(collectClip);

        public void PlayDeath() => PlayOneShot(deathClip);

        public void PlayDoorUnlock() => PlayOneShot(doorUnlockClip);

        public void PlayLevelComplete() => PlayOneShot(levelCompleteClip);

        public void PlayLevelReset() => PlayOneShot(levelResetClip);

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null || sfxSource == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }
}
