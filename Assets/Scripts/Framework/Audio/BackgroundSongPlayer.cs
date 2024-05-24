using UnityEngine;

namespace DerailedDeliveries.Framework.Audio
{
    /// <summary>
    /// Class responsible for playing the background music track at initial start of game.
    /// </summary>
    public class BackgroundSongPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _backgroundTrack;

        [SerializeField]
        private float _musicVolume = .45f;

        private void Start() => AudioSystem.Instance.PlaySong(_backgroundTrack, _musicVolume);
    }
}
