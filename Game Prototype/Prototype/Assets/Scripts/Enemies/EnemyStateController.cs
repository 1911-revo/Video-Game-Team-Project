using System.Collections.Generic;
using UnityEngine;

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
        if (states.Length > 0)
        {
            TransitionTo(states[0].name);
        }
    }

    private void Update()
    {
        currentState?.Tick(enemy);
    }

    public void TransitionTo(string stateName)
    {
        if (enemyStates.TryGetValue(stateName, out var newState))
        {
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
