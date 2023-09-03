using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kit
{
    /// <summary>Allows to play &amp; pool sounds and group them into <see cref="GameAudioSource" />s. Handles background music.</summary>
    public class AudioManager : Singleton<AudioManager>
    {
        #region Fields

        /// <summary>Handler for music since we're always fading it.</summary>
        public AudioSource GameAudioSource;

        public AudioSource UIAudioSource;

        /// <summary>Default volume level to use for the background music.</summary>
        public const float DefaultMusicVolume = 0.5f;

        private GameObject audioGameObject;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        #endregion

        #region AudioClip (Unpooled) playback

        /// <summary>Play an audio with a dedicated <see cref="GameAudioSource" /> and destroy it after it ends (if it's not looping).</summary>
        /// <returns>The <see cref="GameAudioSource" /> instantiated.</returns>
        public void PlayGameSound(AudioClip clip, bool loop = false, bool is3D = false)
        {
            if (clip != null)
            {
                PlayDedicated(clip, GameAudioSource, loop, is3D);
            }
        }

        public void PlayUI(AudioClip clip, bool loop = false, bool is3D = false)
        {
            if (clip != null)
            {
                UIAudioSource.PlayOneShot(clip);
            }
        }

        private void PlayDedicated(AudioClip clip, AudioSource audioSource, bool loop, bool is3D)
        {
            audioSource.clip = clip;
            audioSource.loop = loop;

            if (is3D)
                audioSource.spatialBlend = 1;

            if(audioSource.isPlaying)
                audioSource.Stop();
            audioSource.Play();
        }

        #endregion

        #region Public properties

        /// <summary>Returns whether a background music is playing.</summary>
        public bool IsMusicPlaying => GameAudioSource.isPlaying;

        #endregion
    }
}