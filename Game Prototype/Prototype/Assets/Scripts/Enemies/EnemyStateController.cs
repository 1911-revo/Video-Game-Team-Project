using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General state machine to control enemy behaviour
/// </summary>
public class EnemyStateController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeReference] public ScriptableObject attackSO;
    [SerializeField] private EnemyState[] states;
    public WaypointManager waypointManager;

    private Dictionary<string, EnemyState> enemyStates;
    private EnemyState currentState;
    private Enemy enemy;

    public IAttackBehaviour attack { get; private set; }

    private void Awake()
    {
        // Initialise controller
        enemy = GetComponent<Enemy>();
        attack = attackSO as IAttackBehaviour;
        enemyStates = new Dictionary<string, EnemyState>();
        foreach (var state in states)
        {
            if (state != null)
            {
                enemyStates.Add(state.name, state);
            }
        }
    }

    private void Start()
    {
        // Use first state provided as initial state
        if (states.Length > 0)
        {
            TransitionTo(states[0].name);
        }
    }

    private void Update()
    {
        currentState?.Tick(enemy);
    }

    /// <summary>
    /// Attempt to transition to the provided state
    /// </summary>
    /// <param name="stateName"> New state </param>
    public void TransitionTo(string stateName)
    {
        // Check state exists
        if (enemyStates.TryGetValue(stateName, out var newState))
        {
            // Make transition
            currentState?.Exit(enemy);
            currentState = newState;
            currentState?.Enter(enemy);
        }
        else
        {
            Debug.Log($"State '{stateName}' not found.");
        }
    }
}
