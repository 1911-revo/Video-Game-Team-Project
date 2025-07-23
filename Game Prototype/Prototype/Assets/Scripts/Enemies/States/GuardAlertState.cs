using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Instance of EnemyState scriptable object for patrolling behaviour of the standard guard enemy.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/States/GuardAlertState")]
public class GuardAlertState : EnemyState
{
    /// <summary>
    /// One off action to be performed when transitioning in to this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param> 
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered alert");
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
        Vector3 toPlayer = (enemy.player.position - enemy.transform.position).normalized;
        enemy.fov.SetViewDirection(toPlayer, 120);
        enemy.fov.RotateViewCone();
        enemy.fov.SetOrigin(enemy.agent.nextPosition);


        // Transition to patrol state if player exits field of view
        if (enemy.fov.percentRaysOnPlayer == 0)
        {
            enemy.controller.TransitionTo("GuardPatrolState");
        }

        // Transition to chasing state if player is visible enough
        if (enemy.fov.percentRaysOnPlayer >= 0.1)
        {
            enemy.controller.TransitionTo("GuardChaseState");
        }
    }

    /// <summary>
    /// One off action to be performed when transitioning out of this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited alert");
    }




}
