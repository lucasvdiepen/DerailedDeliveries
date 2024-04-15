using UnityEngine;
using UnityEngine.UI;

using DerailedDeliveries.Framework.PopupManagement;

namespace DerailedDeliveries.Framework.CoalOvenSystem
{
    public class CoalPopup : Popup
    {
        [SerializeField]
        private Image _coalImage;

        [SerializeField]
        private float _lowCoalAmount = 40f;

        private void OnEnable()
        {
            CoalOven.Instance.OnCoalAmountChanged += OnCoalAmountChanged;
            OnCoalAmountChanged(CoalOven.Instance.CoalAmount);
        }

        private void OnDisable() => CoalOven.Instance.OnCoalAmountChanged -= OnCoalAmountChanged;

        private void OnCoalAmountChanged(float coalAmount)
        {
            if(coalAmount <= _lowCoalAmount)
            {
                Show();
                return;
            }

            Close();
        }
    }
}