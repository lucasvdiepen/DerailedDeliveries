using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.Networking
{
    public class Destroyer : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] _components;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if(base.IsOwner)
            {
                Destroy(this);
                return;
            }

            foreach(MonoBehaviour component in _components)
                Destroy(component);

            Destroy(this);
        }
    }
}