using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("The player transform to teleport")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    /// <summary>
    /// Teleports the player to the specified destination.
    /// </summary>
    /// <param name="destination">The transform to teleport the player to</param>
    public void TeleportPlayer(Transform destination)
    {
        playerTransform.position = new Vector3(
            destination.position.x,
            destination.position.y,
            playerTransform.position.z
        );

        cameraTransform.position = new Vector3(
            destination.position.x,
            destination.position.y,
            cameraTransform.position.z
        );
    }
}