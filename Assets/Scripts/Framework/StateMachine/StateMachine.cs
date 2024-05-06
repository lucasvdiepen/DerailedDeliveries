using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DerailedDeliveries.Framework.StateMachine.Attributes;

namespace DerailedDeliveries.Framework.StateMachine
{
    /// <summary>
    /// A class responsible for managing all the states.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// The instance of the state machine.
        /// </summary>
        private static StateMachine _instance;

        /// <summary>
        /// Whether the state machine has been initialized.
        /// </summary>
        private static bool _hasBeenInitialized;

        /// <summary>
        /// Gets the instance of the state machine. The instance will only be created once.
        /// </summary>
        public static StateMachine Instance
        {
            get
            {
                if (_instance != null)
                {
                    _hasBeenInitialized = true;
                    return _instance;
                }

                _instance = FindObjectOfType<StateMachine>();

                if (_instance != null)
                {
                    _hasBeenInitialized = true;
                    return _instance;
                }

                if (_hasBeenInitialized)
                    return null;

                GameObject container = new(typeof(StateMachine).Name);
                _instance = container.AddComponent<StateMachine>();

                _hasBeenInitialized = true;

                return _instance;
            }
        }

        /// <summary>
        /// The maximum size of the navigation history.
        /// </summary>
        [SerializeField]
        private int _maxHistorySize = 10;

        /// <summary>
        /// The registered states.
        /// </summary>
        private readonly Dictionary<Type, State> _states = new();

        /// <summary>
        /// The navigation history.
        /// </summary>
        private readonly List<Type> _navigationHistory = new();

        /// <summary>
        /// Invoked when the state has changed.
        /// </summary>
        public Action<State> OnStateChanged;

        /// <summary>
        /// The current state.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// The target state.
        /// </summary>
        public State TargetState { get; private set; }

        /// <summary>
        /// The previous state.
        /// </summary>
        public State PreviousState
        {
            get
            {
                if(_navigationHistory.Count < 2)
                    return null;

                // Tries to get the second to last state in the navigation history.
                if (!TryGetState(_navigationHistory[^2], out State state))
                    return null;

                return state;
            }
        }

        /// <summary>
        /// Checks if the given state is the current state.
        /// </summary>
        /// <typeparam name="T">The type of the state to check.</typeparam>
        /// <returns>True if the given state is the current state, otherwise false.</returns>
        public bool IsCurrentState<T>() where T : State => CurrentState.GetType() == typeof(T);

        /// <summary>
        /// Checks if the given state is the current state.
        /// </summary>
        /// <param name="state">The type of the state to check.</param>
        /// <returns>True if the given state is the current state, otherwise false.</returns>
        public bool IsCurrentState(State state)
            => CurrentState.GetType() == state.GetType();

        /// <summary>
        /// Checks if the given state is the previous state.
        /// </summary>
        /// <typeparam name="T">The type of the state to check.</typeparam>
        /// <returns>True if the given state is the previous state, otherwise false.</returns>
        public bool IsPreviousState<T>() where T : State
            => PreviousState.GetType() == typeof(T);

        /// <summary>
        /// Checks if the given state is the previous state.
        /// </summary>
        /// <param name="state">The type of the state to check.</param>
        /// <returns>True if the given state is the previous state, otherwise false.</returns>
        public bool IsPreviousState(State state)
            => PreviousState.GetType() == state.GetType();

        /// <summary>
        /// Checks if the given state is the target state.
        /// </summary>
        /// <typeparam name="T">The type of the state to check.</typeparam>
        /// <returns>True if the given state is the target state, otherwise false.</returns>
        public bool IsTargetState<T>() where T : State
            => TargetState.GetType() == typeof(T);

        /// <summary>
        /// Checks if the given state is the target state.
        /// </summary>
        /// <param name="state">The type of the state to check.</param>
        /// <returns>True if the given state is the target state, otherwise false.</returns>
        public bool IsTargetState(State state)
            => TargetState.GetType() == state.GetType();

        /// <summary>
        /// Goes to the given state.
        /// </summary>
        /// <typeparam name="T">The target state.</typeparam>
        /// <param name="shouldAddToHistory">Whether to add the state to the navigation history.</param>
        public void GoToState<T>(bool shouldAddToHistory = true) where T : State
            => GoToState(typeof(T), shouldAddToHistory);

        /// <summary>
        /// Goes to the given state.
        /// </summary>
        /// <param name="state">The target state.</param>
        /// <param name="shouldAddToHistory">Whether to add the state to the navigation history.</param>
        public void GoToState(State state, bool shouldAddToHistory = true)
            => GoToState(state.GetType(), shouldAddToHistory);

        /// <summary>
        /// Goes to the given state.
        /// </summary>
        /// <param name="state">The target state</param>
        /// <param name="shouldAddToHistory">Whether to add the state to the navigation history.</param>
        public void GoToState(Type state, bool shouldAddToHistory = true)
        {
            if (CurrentState != null && CurrentState.GetType() == state)
            {
                Debug.LogError($"State of type {state} is already active");
                return;
            }

            if (!TryGetState(state, out State targetState))
            {
                Debug.LogError($"State of type {state} is not registered");
                return;
            }

            StartCoroutine(TransitionToState(targetState, shouldAddToHistory));
        }

        /// <summary>
        /// Goes back to the previous state.
        /// </summary>
        public void GoBack()
        {
            if (_navigationHistory.Count < 2)
            {
                Debug.LogError("No previous state to go back to");
                return;
            }

            GoToState(_navigationHistory[^2], false);

            _navigationHistory.RemoveAt(_navigationHistory.Count - 1);
        }

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        public void ClearHistory()
        {
            _navigationHistory.Clear();
            AddToHistory(CurrentState.GetType());
        }

        /// <summary>
        /// Adds the given state to the navigation history.
        /// </summary>
        /// <param name="state">The state to add to the navigation history.</param>
        private void AddToHistory(Type state)
        {
            if (_navigationHistory.Count >= _maxHistorySize)
                _navigationHistory.RemoveAt(0);

            _navigationHistory.Add(state);
        }

        /// <summary>
        /// Registers the given state.
        /// </summary>
        /// <param name="state">The state to register.</param>
        /// <param name="isDefault">Whether the given state should be the default state.</param>
        public void RegisterState(State state, bool isDefault = false)
        {
            if (IsStateRegistered(state))
            {
                Debug.LogError($"State of type {state.GetType()} is already registered");
                return;
            }
            
            _states.Add(state.GetType(), state);

            if(!isDefault && !IsDefaultState(state))
                return;

            SetDefaultState(state);
        }

        /// <summary>
        /// Unregisters the given state.
        /// </summary>
        /// <param name="state">The state to unregister.</param>
        public void UnregisterState(State state)
        {
            if (!IsStateRegistered(state))
            {
                Debug.LogError($"State of type {state.GetType()} is not registered");
                return;
            }

            _states.Remove(state.GetType());
        }

        /// <summary>
        /// Transitions to the given state.
        /// </summary>
        /// <param name="state">The target state.</param>
        /// <param name="shouldAddToHistory">Whether to add the state to the navigation history.</param>
        private IEnumerator TransitionToState(State state, bool shouldAddToHistory = true)
        {
            // Set the new target state.
            TargetState = state;

            // Get the current and target state trees.
            List<State> currentStateTree = null;
            List<State> targetStateTree = GetStateTree(TargetState);

            // Exit the current states.
            if (CurrentState != null)
            {
                currentStateTree = GetStateTree(CurrentState);

                yield return ExitStates(currentStateTree, targetStateTree);
            }

            // Set the target state to the current state.
            CurrentState = TargetState;

            // Adds the new state to the navigation history.
            if(shouldAddToHistory)
                AddToHistory(CurrentState.GetType());

            // Enter the target states.
            yield return EnterStates(currentStateTree, targetStateTree);

            // Invokes the state changed event.
            OnStateChanged?.Invoke(CurrentState);
        }

        /// <summary>
        /// Enters the given states minus the states that are already entered.
        /// </summary>
        /// <param name="currentStateTree">The currently active state tree.</param>
        /// <param name="targetStateTree">The target state tree.</param>
        private IEnumerator EnterStates(List<State> currentStateTree, List<State> targetStateTree)
        {
            State[] statesToEnter;

            if (currentStateTree != null)
            {
                statesToEnter = targetStateTree.Except(currentStateTree).ToArray();

                State lastCurrentState = currentStateTree[^1];

                // Check if the current state is a parent of the new target state.
                if (targetStateTree.Contains(lastCurrentState))
                    yield return lastCurrentState.OnStateExitedToChild();

                State lastTargetState = targetStateTree[^1];

                // Check if the target state was a parent of the current state.
                if (currentStateTree.Contains(lastTargetState))
                    yield return lastTargetState.OnStateEnteredFromChild();
            }
            else
            {
                statesToEnter = targetStateTree.ToArray();
            }

            for (int i = 0; i < statesToEnter.Length; i++)
            {
                yield return statesToEnter[i].OnStateEnter();

                if (i < statesToEnter.Length - 1)
                    yield return statesToEnter[i].OnStateExitedToChild();
            }
        }

        /// <summary>
        /// Exits the given states minus the states that should remain active.
        /// </summary>
        /// <param name="currentStateTree">The currently active state tree.</param>
        /// <param name="targetStateTree">The target state tree.</param>
        private IEnumerator ExitStates(List<State> currentStateTree, List<State> targetStateTree)
        {
            State[] statesToExit = currentStateTree.Except(targetStateTree).ToArray();

            for (int i = statesToExit.Length - 1; i >= 0; i--)
                yield return statesToExit[i].OnStateExit();
        }

        /// <summary>
        /// Gets the state tree of the given state. 
        /// The root parent state is first in the list.
        /// The target state is last in the list.
        /// </summary>
        /// <param name="state">The state to get the tree of.</param>
        /// <returns>The state tree of the given state.</returns>
        private List<State> GetStateTree(State state)
        {
            List<State> tree = new()
            {
                state
            };

            State currentState = state;

            while(true)
            {
                ParentStateAttribute parentStateAttribute = 
                    (ParentStateAttribute)Attribute.GetCustomAttribute(currentState.GetType(), typeof(ParentStateAttribute));

                if (parentStateAttribute == null)
                    break;

                if (parentStateAttribute.ParentState == null ||
                    !TryGetState(parentStateAttribute.ParentState, out State parentState))
                {
                    break;
                }
                
                tree.Add(parentState);
                currentState = parentState;
            }

            tree.Reverse();

            return tree;
        }

        /// <summary>
        /// Invoked when all states are initialized and the state machine is ready.
        /// </summary>
        private void Start() => DisableAllStates();

        /// <summary>
        /// Disables all the states except the current state.
        /// </summary>
        private void DisableAllStates()
        {
            foreach (State state in _states.Values)
            {
                if(state == CurrentState)
                    continue;

                state.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Whether the given state has the default state attribute.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>True if the given state has the default state attribute, otherwise false.</returns>
        private bool IsDefaultState(State state)
        {
            DefaultStateAttribute defaultStateAttribute =
                (DefaultStateAttribute)Attribute.GetCustomAttribute(state.GetType(), typeof(DefaultStateAttribute));

            return defaultStateAttribute != null;
        }

        /// <summary>
        /// Sets the default state if it hasn't been set yet.
        /// </summary>
        /// <param name="state">The state to set as the default state.</param>
        private void SetDefaultState(State state)
        {
            if(CurrentState != null)
            {
                Debug.LogError($"Default state is already set to {CurrentState.GetType()}");
                return;
            }

            StartCoroutine(TransitionToState(state));
        }

        /// <summary>
        /// Checks if the given state is registered.
        /// </summary>
        /// <typeparam name="T">The type of the state to check.</typeparam>
        /// <returns>True if the given state is registered, otherwise false.</returns>
        private bool IsStateRegistered<T>() where T : State
            => _states.ContainsKey(typeof(T));

        /// <summary>
        /// Checks if the given state is registered.
        /// </summary>
        /// <param name="state">The type of the state to check.</param>
        /// <returns>True if the given state is registered, otherwise false.</returns>
        private bool IsStateRegistered(State state)
            => _states.ContainsKey(state.GetType());

        /// <summary>
        /// Tries to get the state of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the state to get.</typeparam>
        /// <param name="state">The output state.</param>
        /// <returns>True if the state was found, otherwise false.</returns>
        private bool TryGetState<T>(out State state) where T : State
            => TryGetState(typeof(T), out state);

        /// <summary>
        /// Tries to get the state of the given type.
        /// </summary>
        /// <param name="type">The type of the state to get.</param>
        /// <param name="state">The output state.</param>
        /// <returns>True if the state was found, otherwise false.</returns>
        private bool TryGetState(Type type, out State state)
            => _states.TryGetValue(type, out state);
    }
}
