using UnityEngine;
using UnityEngine.UI;

using DerailedDeliveries.Framework.PopupManagement;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    /// <summary>
    /// A <see cref="Popup"/> responsible for showing the repair popup.
    /// </summary>
    [RequireComponent(typeof(Damageable))]
    public class RepairPopup : Popup
    {
        [SerializeField]
        private Image _repairPopupImage;

        [SerializeField]
        private Sprite _lowHealthSprite;

        [SerializeField]
        private Sprite _criticalLowHealthSprite;

        [SerializeField]
        private int _lowHealthThreshold = 40;

        [SerializeField]
        private int _criticalLowHealthThreshold = 20;

        private Damageable _damageable;

        private protected override void Awake()
        {
            base.Awake();

            _damageable = GetComponent<Damageable>();
        }

        private void OnEnable()
        {
            _damageable.OnHealthChanged += OnHealthChanged;
            OnHealthChanged(_damageable.Health);
        }

        private void OnDisable() => _damageable.OnHealthChanged -= OnHealthChanged;

        private void OnHealthChanged(int health)
        {
            if(health <= _criticalLowHealthThreshold)
            {
                _repairPopupImage.sprite = _criticalLowHealthSprite;
                Show();
                return;
            }

            if(health <= _lowHealthThreshold)
            {
                _repairPopupImage.sprite = _lowHealthSprite;
                Show();
                return;
            }

            Close();
        }
    }
}