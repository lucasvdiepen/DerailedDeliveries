using System;

namespace DerailedDeliveries.Framework.StateMachine.Attributes
{
    /// <summary>
    /// An attribute that marks a state as the default state in the state machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultStateAttribute : Attribute { }
}