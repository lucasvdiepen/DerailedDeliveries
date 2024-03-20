using UnityEngine;

namespace DerailedDeliveries.Framework.Utils
{
    /// <summary>
    /// An AbstractSingleton class that other classes can derive from, will automatically be reachable through the get Instance of this class.
    /// </summary>
    /// <typeparam name="T">The class that is deriving from the AbstractSingleton.</typeparam>
    public class AbstractSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        /// <summary>
        /// Gets the instance of this class by either returning a valid static variable or making a new Singleton object.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();

                if (_instance != null)
                    return _instance;

                GameObject container = new(typeof(T).Name);
                _instance = container.AddComponent<T>();

                return _instance;
            }
        }
    }
}