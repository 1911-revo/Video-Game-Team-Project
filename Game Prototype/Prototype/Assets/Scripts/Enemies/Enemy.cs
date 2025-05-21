using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  General enemy class to hold enemy information
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyStatsSO stats;
    [HideInInspector] public Transform player;

    public EnemyStateController controller;
    [HideInInspector] public NavMeshAgent agent;
    public FieldOfView fov;


    private void Start()
    {
        controller = GetComponent<EnemyStateController>();
        if (controller == null)
        {
            Debug.Log("EnemyStateController not found on enemy!");
        }

        player = GameObject.FindWithTag("Player").transform;
        if (player == null)
        {
            Debug.Log("Player transform not found in scene!");
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.Log("Enemy AI agent not found!");
        }
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = stats.moveSpeed;
        fov.fov = stats.fov;
        fov.viewDistance = stats.viewDistance;
    }
}
