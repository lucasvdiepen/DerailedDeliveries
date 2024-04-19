using UnityEngine;

using DerailedDeliveries.Framework.ParentingSystem;

namespace DerailedDeliveries.Framework.Utils.ObjectParenting
{
    /// <summary>
    /// An util class for object parenting.
    /// </summary>
    public static class ObjectParentUtils
    {
        /// <summary>
        /// Tries to get the <see cref="ObjectParent"/> of the given <see cref="GameObject"/>. This will also look over all connected parents until the root is reached.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to get the <see cref="ObjectParent"/> from.</param>
        /// <param name="objectParent">The object parent when found.</param>
        /// <returns>Whether the <see cref="ObjectParent"/> was found.</returns>
        public static bool TryGetObjectParent(GameObject gameObject, out ObjectParent objectParent)
        {
            if(gameObject.TryGetComponent(out objectParent))
                return true;

            objectParent = gameObject.GetComponentInParent<ObjectParent>();

            return objectParent != null;
        }
    }
}