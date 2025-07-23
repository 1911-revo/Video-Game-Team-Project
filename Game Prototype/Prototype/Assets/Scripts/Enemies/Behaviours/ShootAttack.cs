using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Attacks/Shoot")]
public class ShootAttack : ScriptableObject, IAttackBehaviour
{
    public void Attack(Transform enemyTransform, Transform playerTransform, EnemyStatsSO stats)
    {
        Debug.Log("Shoot at player");
    }
}
