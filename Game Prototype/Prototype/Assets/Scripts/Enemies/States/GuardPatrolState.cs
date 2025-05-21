using NUnit.Framework;
using Unity.VisualScripting;
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
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.controller.waypointManager.GetNextWaypoint();
            enemy.agent.SetDestination(enemy.controller.waypointManager.CurrentWaypoint().position);
        }
        
        enemy.fov.SetViewDirection(enemy.agent.velocity);
        enemy.fov.SetOrigin(enemy.agent.nextPosition);

        if (enemy.fov.percentRaysOnPlayer > 0)
        {
            enemy.controller.TransitionTo("GuardAlertState");
        }
    }

    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited patrol");
    }


}
