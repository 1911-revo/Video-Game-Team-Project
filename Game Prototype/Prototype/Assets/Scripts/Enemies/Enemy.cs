using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsSO stats;
    public Transform player;

    public EnemyStateController controller;

    private void Start()
    {
        controller = GetComponent<EnemyStateController>();
        if (controller == null )
        {
            Debug.Log("EnemyStateController not found on enemy!");
        }

        player = GameObject.FindWithTag("Player").transform;
        if (player == null )
        {
            Debug.Log("Player transform not found in scene!");
        }
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        if (stats.health <= 0) 
        {
            Destroy(gameObject);
            //controller.TransitionTo(dieState);
        }
    }
}
