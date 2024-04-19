using UnityEngine;
using UnityEngine.UI;

using DerailedDeliveries.Framework.PopupManagement;

namespace DerailedDeliveries.Framework.CoalOvenSystem
{
    /// <summary>
    /// A <see cref="Popup"/> class that handles the coal bar popup logic.
    /// </summary>
    public class CoalBarPopup : Popup
    {
        [SerializeField]
        private Image _coalProgressionBarImage;

        private void OnEnable()
        {
            CoalOven.Instance.OnCoalAmountChanged += OnCoalAmountChanged;
            OnCoalAmountChanged(CoalOven.Instance.CoalAmount);
        }

        private void OnDisable() => CoalOven.Instance.OnCoalAmountChanged -= OnCoalAmountChanged;

        private void OnCoalAmountChanged(float coalAmount)
        {
            float percentage = coalAmount / CoalOven.Instance.MaxCoalAmount;
            _coalProgressionBarImage.fillAmount = percentage;
        }
    }
}