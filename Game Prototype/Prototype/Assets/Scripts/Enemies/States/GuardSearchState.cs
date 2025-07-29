using NUnit.Framework;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Instance of EnemyState scriptable object for searching behaviour of the standard guard enemy.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/States/GuardSearchState")]
public class GuardSearchState : EnemyState
{
    private enum SearchPhase { Travel, LookLeft, LookRight, Done }
    private SearchPhase currentPhase;

    private Vector3 lastPlayerPos;
    private float lookDuration = 2f;
    private float lookTimer = 0f;

    private float baseAngle;


    /// <summary>
    /// One off action to be performed when transitioning in to this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param> 
    public override void Enter(Enemy enemy)
    {
        Debug.Log("Entered search");
        enemy.agent.isStopped = false;

        // Begin moving to the player's last known location
        lastPlayerPos = enemy.player.position;
        enemy.agent.SetDestination(lastPlayerPos);
        currentPhase = SearchPhase.Travel;
    }

    /// <summary>
    /// Actions to be performed every frame while in this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Tick(Enemy enemy)
    {
        // Transition to chasing state if the player is spotted
        if (enemy.fov.percentRaysOnPlayer > 0)
        {
            enemy.controller.TransitionTo("GuardChaseState");
        }

        // Move through phases of the search process
        switch (currentPhase)
        {
            // Still moving to last known location of the player
            case SearchPhase.Travel:
                Vector3 direction = (lastPlayerPos - enemy.transform.position).normalized;
                enemy.fov.SetViewDirection(direction, 90);
                enemy.fov.RotateViewCone();
                // Point reached, begin looking left
                if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
                {
                    baseAngle = enemy.fov.GetCurrentAngle();
                    currentPhase = SearchPhase.LookLeft;
                    lookTimer = 0f;

                    // Set target direction 90 degrees left
                    float leftAngle = baseAngle - 85f;
                    enemy.fov.SetViewDirection(FieldOfView.VectorFromAngle(leftAngle + enemy.fov.fov / 2f), 90f);
                }
                break;

            case SearchPhase.LookLeft:
                // Slowly rotate view cone left over lookDuration seconds
                lookTimer += Time.deltaTime;
                enemy.fov.RotateViewCone();

                // Finished looking left, begin looking right
                if (lookTimer >= lookDuration)
                {
                    currentPhase = SearchPhase.LookRight;
                    lookTimer = 0f;

                    // Set target direction 90 degrees right of initial direction
                    float rightAngle = baseAngle + 85f;
                    enemy.fov.SetViewDirection(FieldOfView.VectorFromAngle(rightAngle + enemy.fov.fov / 2f), 90f);
                }
                break;

            case SearchPhase.LookRight:
                // Slowly rotate view cone right
                lookTimer += Time.deltaTime;
                enemy.fov.RotateViewCone();

                if (lookTimer >= lookDuration * 2)
                {
                    currentPhase = SearchPhase.Done;
                }
                break;

            case SearchPhase.Done:
                // Search completed without finding player, return to alerted state
                enemy.controller.TransitionTo("GuardAlertState");
                return;
        }

        enemy.fov.SetOrigin(enemy.agent.nextPosition);
    }

    /// <summary>
    /// One off action to be performed when transitioning out of this state
    /// </summary>
    /// <param name="enemy"> Enemy instance </param>
    public override void Exit(Enemy enemy)
    {
        Debug.Log("Exited search");
    }
}
