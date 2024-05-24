using UnityEngine;
using UnityEngine.UI;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.UI.Lever
{
    /// <summary>
    /// A class responsible for updating the lever indicator.
    /// </summary>
    public class LeverUpdater : MonoBehaviour
    {
        [SerializeField]
        private Image _leverIndicatorImage;

        [SerializeField]
        private Sprite _leverLeftSprite;

        [SerializeField]
        private Sprite _leverRightSprite;

        private void OnEnable()
        {
            TrainEngine.Instance.OnDirectionChanged += UpdateLeverIndicator;
            UpdateLeverIndicator(TrainEngine.Instance.CurrentSplitDirection);
        }

        private void OnDisable()
        {
            if(TrainEngine.Instance != null)
                TrainEngine.Instance.OnDirectionChanged -= UpdateLeverIndicator;
        }

        private void UpdateLeverIndicator(bool isRight)
            => _leverIndicatorImage.sprite = isRight ? _leverRightSprite : _leverLeftSprite;
    }
}