using FishNet.Object;
using System;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A class that is responsible for holding all the data related to a deliverable package and updating visuals.
    /// </summary>
    public class PackageData : NetworkBehaviour
    {
        /// <summary>
        /// Invoked when the package data changes.
        /// </summary>
        public Action<int, string> OnPackageDataChanged;

        /// <summary>
        /// A getter that is used to return the package's ID.
        /// </summary>
        public int PackageID { get; private set; } = -1;

        /// <summary>
        /// A getter that is used to return the package's label.
        /// </summary>
        public string PackageLabel { get; private set; }

        /// <summary>
        /// A function that updates the packageLabel and packageID.
        /// </summary>
        /// <param name="label">The new package label.</param>
        /// <param name="id">The new package ID.</param>
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        public void UpdateLabelAndID(string label, int id)
        {
            PackageLabel = label;
            PackageID = id;

            OnPackageDataChanged?.Invoke(PackageID, PackageLabel);
        }
    }
}