using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Stats")]
public class EnemyStatsSO : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float damage;
    public float viewDistance;
    public float fov;
    public float attackRange;
    public float attackCooldown;
}
