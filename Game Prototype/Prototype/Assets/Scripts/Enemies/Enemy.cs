using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStatsSO stats;
    public Transform player;
    public BoxCollider2D playerBC;

    [Header("States")]
    public EnemyStateController controller;
    public EnemyState patrolState;
    public EnemyState chaseState;
    public EnemyState attackState;
    //public EnemyState dieState;

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
