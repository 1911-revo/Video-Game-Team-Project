using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsSO stats;
    public Transform player;

    [Header("States")]
    public EnemyStateController controller;
    public EnemyState patrolState;
    public EnemyState chaseState;
    public EnemyState attackState;
    //public EnemyState dieState;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
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
