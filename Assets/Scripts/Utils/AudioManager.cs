using UnityEngine;

namespace LuckyCharm.Utils
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] crackLightClips;
        [SerializeField] private AudioClip crackBreakClip;
        [SerializeField] private AudioClip paperWhooshClip;
        [SerializeField] private AudioClip paperRustleClip;
        [SerializeField] private AudioClip paperSettleClip;
        [SerializeField] private AudioClip uiTapClip;

        private AudioSource _sfxSource;
        private AudioSource _loopSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // GameManager handles DontDestroyOnLoad for the root parent

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            _loopSource = gameObject.AddComponent<AudioSource>();
            _loopSource.playOnAwake = false;
            _loopSource.loop = true;
        }

        private bool IsSoundEnabled()
        {
            return Core.SaveManager.Instance?.Data?.soundEnabled ?? true;
        }

        public void PlayCrackLight(int tapIndex)
        {
            if (!IsSoundEnabled()) return;
            if (crackLightClips != null && crackLightClips.Length > 0)
            {
                int index = Mathf.Clamp(tapIndex, 0, crackLightClips.Length - 1);
                _sfxSource.PlayOneShot(crackLightClips[index]);
            }
        }

        public void PlayCrackBreak()
        {
            if (!IsSoundEnabled()) return;
            if (crackBreakClip != null)
            {
                _sfxSource.PlayOneShot(crackBreakClip);
            }
        }

        public void PlayPaperWhoosh()
        {
            if (!IsSoundEnabled()) return;
            if (paperWhooshClip != null)
            {
                _sfxSource.PlayOneShot(paperWhooshClip);
            }
        }

        public void StartPaperRustle()
        {
            if (!IsSoundEnabled()) return;
            if (paperRustleClip != null && !_loopSource.isPlaying)
            {
                _loopSource.clip = paperRustleClip;
                _loopSource.Play();
            }
        }

        public void StopPaperRustle()
        {
            _loopSource.Stop();
        }

        public void PlayPaperSettle()
        {
            if (!IsSoundEnabled()) return;
            if (paperSettleClip != null)
            {
                _sfxSource.PlayOneShot(paperSettleClip);
            }
        }

        public void PlayUITap()
        {
            if (!IsSoundEnabled()) return;
            if (uiTapClip != null)
            {
                _sfxSource.PlayOneShot(uiTapClip);
            }
        }
    }
}
