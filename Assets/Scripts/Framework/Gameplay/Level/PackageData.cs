using UnityEngine;
using TMPro;
using FishNet.Object;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A class that is responsible for holding all the data related to a deliverable package and updating visuals.
    /// </summary>
    public class PackageData : NetworkBehaviour
    {
        [SerializeField]
        private int _packageID = -1;

        [SerializeField]
        private string _packageLabel;

        [SerializeField]
        private TextMeshProUGUI[] _textDisplays;

        /// <summary>
        /// A getter that is used to return the package's ID.
        /// </summary>
        public int PackageID => _packageID;

        /// <summary>
        /// A function that updates the packageLabel and packageID.
        /// </summary>
        /// <param name="label">The new package label.</param>
        /// <param name="id">The new package ID.</param>
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        public void UpdateLabelAndID(string label, int id)
        {
            _packageLabel = label;
            _packageID = id;

            for (int i = 0; i < _textDisplays.Length; i++)
                _textDisplays[i].text = _packageLabel;
        }
    }
}