using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public EnemyState initialState;
    private EnemyState currentState;
    private Enemy enemy;

    [SerializeReference] public ScriptableObject attackSO;

    public IAttackBehaviour attack { get; private set; }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        attack = attackSO as IAttackBehaviour;
    }

    private void Start()
    {
        TransitionTo(initialState);
    }

    private void Update()
    {
        currentState?.Tick(enemy);
    }

    public void TransitionTo(EnemyState newState)
    {
        currentState?.Exit(enemy);
        currentState = newState;
        currentState?.Enter(enemy);
    }
}
