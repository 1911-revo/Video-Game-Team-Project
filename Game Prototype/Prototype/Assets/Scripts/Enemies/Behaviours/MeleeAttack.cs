using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Attacks/Melee")]
public class MeleeAttack : ScriptableObject, IAttackBehaviour
{
    public void Attack(Transform enemyTransform, Transform playerTransform, EnemyStatsSO stats)
    {
        if (Vector2.Distance(enemyTransform.position, playerTransform.position) <= 1.1)
        {
            Debug.Log($"Melee attack dealt {stats.damage} to player!");
        }
    }
}
