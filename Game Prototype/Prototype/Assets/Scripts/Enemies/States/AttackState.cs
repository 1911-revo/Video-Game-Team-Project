using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/States/Attack")]
public class AttackState : EnemyState
{
    private float lastAttackTime;
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered attack");
        lastAttackTime = 0;
    }

    public override void Tick(Enemy enemy)
    {
        if (Time.time - lastAttackTime >= enemy.stats.attackCooldown)
        {
            enemy.controller.attack?.Attack(enemy.transform, enemy.player.transform, enemy.stats);
            lastAttackTime = Time.time;
        }

        float distToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);

        if (distToPlayer > enemy.stats.attackRange)
        {
            enemy.controller.TransitionTo("TestEnemyChaseState");
        }
    }

    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited attack");
    }
}
