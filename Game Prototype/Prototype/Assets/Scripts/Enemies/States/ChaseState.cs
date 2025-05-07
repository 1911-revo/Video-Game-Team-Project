using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/States/Chase")]
public class ChaseState : EnemyState
{
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered chase");
    }

    public override void Tick(Enemy enemy)
    {
        Vector2 dir = (enemy.player.position - enemy.transform.position).normalized;
        enemy.transform.position += (Vector3)(dir * enemy.stats.moveSpeed * Time.deltaTime);

        float distToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);

        if (distToPlayer < enemy.stats.attackRange)
        {
            enemy.controller.TransitionTo(enemy.attackState);
        }

        if (distToPlayer > enemy.stats.chaseRange)
        {
            enemy.controller.TransitionTo(enemy.patrolState);
        }
    }

    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited chase");
    }
}
