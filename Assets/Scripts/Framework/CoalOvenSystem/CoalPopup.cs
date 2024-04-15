using DerailedDeliveries.Framework.PopupManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DerailedDeliveries.Framework.CoalOvenSystem
{
    public class CoalPopup : Popup
    {
        [SerializeField]
        private Sprite _lowCoalSprite;

        [SerializeField]
        private Sprite _criticalLowCoalSprite;

        [SerializeField]
        private Image _coalImage;

        [SerializeField]
        private float _lowCoalAmount = 40f;

        [SerializeField]
        private float _criticalLowCoalAmount = 20f;

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
                _coalImage.sprite = _lowCoalSprite;
                Show();
                return;
            }

            if(coalAmount <= _criticalLowCoalAmount)
            {
                _coalImage.sprite = _criticalLowCoalSprite;
                Show();
                return;
            }

            Close();
        }
    }
}