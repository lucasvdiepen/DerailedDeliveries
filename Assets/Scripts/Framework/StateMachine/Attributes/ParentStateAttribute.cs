using System;

namespace DerailedDeliveries.Framework.StateMachine.Attributes
{
    /// <summary>
    /// An attribute that marks its parent state.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ParentStateAttribute : Attribute
    {
        /// <summary>
        /// The parent state of the state.
        /// </summary>
        public Type ParentState { get; }

        public ParentStateAttribute(Type parentStateType) => ParentState = parentStateType;
    }
}