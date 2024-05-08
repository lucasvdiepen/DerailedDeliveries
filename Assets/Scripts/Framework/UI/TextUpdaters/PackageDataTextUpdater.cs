using UnityEngine;

using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.Gameplay.Level;

namespace DerailedDeliveries.Framework.UI.TextUpdaters
{
    /// <summary>
    /// A <see cref="TextUpdater"/> class which updates the text on the package.
    /// </summary>
    public class PackageDataTextUpdater : TextUpdater
    {
        [SerializeField]
        private Damageable _boxDamageable;

        [SerializeField]
        private PackageData _packageData;

        private void OnEnable()
        {
            _boxDamageable.OnHealthChanged += UpdateHealthText;
            _packageData.OnPackageDataChanged += UpdateLabelText;

            UpdateHealthText(_boxDamageable.Health);
            UpdateLabelText(_packageData.PackageID, _packageData.PackageLabel);
        }

        private void OnDisable()
        {
            _boxDamageable.OnHealthChanged -= UpdateHealthText;
            _packageData.OnPackageDataChanged -= UpdateLabelText;
        }

        private void UpdateHealthText(int health) => ReplaceTag("[health]", health.ToString());

        private void UpdateLabelText(int id, string label) => ReplaceTag("[label]", label);
    }
}