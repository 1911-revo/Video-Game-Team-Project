using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/States/GuardPatrolState")]
public class GuardPatrolState : EnemyState
{
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered patrol");
    }

    public override void Tick(Enemy enemy)
    {
        if (enemy.transform.position == enemy.controller.waypointManager.CurrentWaypoint().position)
        {
            enemy.controller.waypointManager.GetNextWaypoint();
        }
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.controller.waypointManager.CurrentWaypoint().position, enemy.stats.moveSpeed * Time.deltaTime);
    }

    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited patrol");
    }
}
