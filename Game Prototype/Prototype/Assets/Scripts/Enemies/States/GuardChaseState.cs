using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Instance of EnemyState scriptable object for patrolling behaviour of the standard guard enemy.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/States/GuardChaseState")]
public class GuardChaseState : EnemyState
{
    /// <summary>
    /// One off action to be performed when transitioning in to this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param> 
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered chase");
    }

    /// <summary>
    /// Actions to be performed every frame while in this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Tick(Enemy enemy)
    {
        enemy.agent.SetDestination(enemy.player.position);

        // Update the field of view cone
        Vector3 toPlayer = (enemy.player.position - enemy.transform.position).normalized;
        enemy.fov.SetViewDirection(toPlayer, 120);
        enemy.fov.RotateViewCone();
        enemy.fov.SetOrigin(enemy.agent.nextPosition);


        // Transition to searching state if player exits field of view
        if (enemy.fov.percentRaysOnPlayer == 0)
        {
            enemy.controller.TransitionTo("GuardSearchState");
        }

        // Transition to attacking state if player is within attack range
        if ((enemy.player.position - enemy.transform.position).magnitude < enemy.stats.attackRange)
        {
            enemy.controller.TransitionTo("GuardAttackState");
        }
    }

    /// <summary>
    /// One off action to be performed when transitioning out of this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited chase");
    }




}
