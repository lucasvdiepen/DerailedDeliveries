using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.StateMachine
{
    /// <summary>
    /// A class that represents a state in the state machine.
    /// </summary>
    public abstract class State : MonoBehaviour
    {
        /// <summary>
        /// Wheter the state is active or not.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Registers the state when it is created.
        /// </summary>
        private protected virtual void Awake() => RegisterState();

        /// <summary>
        /// Unregisters the state.
        /// </summary>
        private protected virtual void OnDestroy() => UnregisterState();

        /// <summary>
        /// Registers the state to the state machine.
        /// </summary>
        private protected virtual void RegisterState() => StateMachine.Instance.RegisterState(this);

#pragma warning disable UNT0008
        /// <summary>
        /// Unregisters the state from the state machine.
        /// </summary>
        private protected virtual void UnregisterState() => StateMachine.Instance?.UnregisterState(this);
#pragma warning restore UNT0008

        /// <summary>
        /// Method that gets called when the state is entered.
        /// </summary>
        public virtual IEnumerator OnStateEnter()
        {
            gameObject.SetActive(true);
            IsActive = true;

            yield return null;
        }

        /// <summary>
        /// Method that gets called when the state is exited.
        /// </summary>
        public virtual IEnumerator OnStateExit()
        {
            gameObject.SetActive(false);
            IsActive = false;

            yield return null;
        }

        /// <summary>
        /// Method that gets called when the state is entered from a child state.
        /// </summary>
        public virtual IEnumerator OnStateEnteredFromChild() => null;

        /// <summary>
        /// Method that gets called when the state is exited to a child state.
        /// </summary>
        public virtual IEnumerator OnStateExitedToChild() => null;
    }
}