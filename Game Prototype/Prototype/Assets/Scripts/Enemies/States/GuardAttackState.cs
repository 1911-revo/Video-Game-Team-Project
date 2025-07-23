using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Instance of EnemyState scriptable object for patrolling behaviour of the standard guard enemy.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/States/GuardAttackState")]
public class GuardAttackState : EnemyState
{
    private float lastAttackTime = 0;

    /// <summary>
    /// One off action to be performed when transitioning in to this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param> 
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered attack");
        enemy.agent.stoppingDistance = 1.5f;
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

        if (Time.time - lastAttackTime >= enemy.stats.attackCooldown)
        {
            enemy.controller.attack?.Attack(enemy.transform, enemy.player.transform, enemy.stats);
            lastAttackTime = Time.time;
        }


        // Transition to searching state if player exits field of view
        if (enemy.fov.percentRaysOnPlayer == 0)
        {
            enemy.controller.TransitionTo("GuardSearchState");
        }

        // Transition to chasing state if player exits attack range
        if ((enemy.player.position - enemy.transform.position).magnitude > enemy.stats.attackRange)
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
        Debug.Log("Exited attack");
        enemy.agent.stoppingDistance = 0;
    }
}
