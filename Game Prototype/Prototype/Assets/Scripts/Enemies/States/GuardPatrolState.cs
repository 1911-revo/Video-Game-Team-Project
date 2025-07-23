using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Instance of EnemyState scriptable object for patrolling behaviour of the standard guard enemy.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/States/GuardPatrolState")]
public class GuardPatrolState : EnemyState
{
    /// <summary>
    /// One off action to be performed when transitioning in to this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param> 
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered patrol");

    }

    /// <summary>
    /// Actions to be performed every frame while in this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Tick(Enemy enemy)
    {
        // Enemy has reached its waypoint and needs new path
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.controller.waypointManager.GetNextWaypoint();
            enemy.agent.SetDestination(enemy.controller.waypointManager.CurrentWaypoint().position);
        }
        // Update the field of view cone
        enemy.fov.SetViewDirection(enemy.agent.velocity, 180);
        enemy.fov.RotateViewCone();
        enemy.fov.SetOrigin(enemy.agent.nextPosition);

        // Transition to alert state if player enters field of view
        if (enemy.fov.percentRaysOnPlayer > 0)
        {
            enemy.controller.TransitionTo("GuardAlertState");
        }
    }

    /// <summary>
    /// One off action to be performed when transitioning out of this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited patrol");
    }




}
