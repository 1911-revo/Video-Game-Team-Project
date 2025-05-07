using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(menuName = "Enemies/States/Patrol")]
public class PatrolState : EnemyState
{
    public float patrolRadius = 5f;
    public float timeBetweenTargets = 2f;
    private float nextTargetTime;
    private Vector2 currentTarget;
    private Vector2 origin;
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered patrol");
        origin = enemy.transform.position;
        PickNewTarget(enemy);
        nextTargetTime = Time.time + timeBetweenTargets;
    }

    public override void Tick(Enemy enemy)
    {
        // Move towards current target
        Vector2 pos = enemy.transform.position;
        Vector2 dir = (currentTarget - pos).normalized;
        enemy.transform.position += (Vector3)(dir * enemy.stats.moveSpeed / 2 * Time.deltaTime);

        // Reached target or timed out
        if (Vector2.Distance(pos, currentTarget) < 0.1f || Time.time >= nextTargetTime)
        {
            PickNewTarget(enemy);
            nextTargetTime = Time.time + timeBetweenTargets;
        }

        // Transition to chase player if close enough
        float distToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distToPlayer <= enemy.stats.chaseRange)
        {
            enemy.controller.TransitionTo("TestEnemyChaseState");
        }
    }

    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited patrol");
    }

    private void PickNewTarget(Enemy enemy)
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        currentTarget = origin + randomOffset;
    }
}
