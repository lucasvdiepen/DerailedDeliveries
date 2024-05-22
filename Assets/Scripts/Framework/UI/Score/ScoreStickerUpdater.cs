using UnityEngine;

using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.Score
{
    /// <summary>
    /// A class that is responsible for updating the score sticker.
    /// </summary>
    public class ScoreStickerUpdater : MonoBehaviour
    {
        [SerializeField]
        private GameObject _niceJobSticker;

        [SerializeField]
        private GameObject _failedSticker;

        [SerializeField]
        private int _percentageToWin = 55;

        private void OnEnable()
        {
            _niceJobSticker.SetActive(LevelTracker.Instance.CurrentScore >= _percentageToWin);
            _failedSticker.SetActive(LevelTracker.Instance.CurrentScore < _percentageToWin);
        }
    }
}