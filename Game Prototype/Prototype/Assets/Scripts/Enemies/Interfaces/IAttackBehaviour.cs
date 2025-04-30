using UnityEngine;

public interface IAttackBehaviour
{
    void Attack(Transform enemyTransform, Transform playerTransform, EnemyStatsSO stats);
}
