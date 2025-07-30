using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3((float)0.7, 0, 0);
    public Vector3 Bulletoffset = new Vector3(0, (float)0.109, 0);
    private SpriteRenderer weaponSpriteRenderer;

    public bool facing;

    private bool isShooting = false;
    private int currentWeapon = 0;
    private float lastFireTime = 0f;

    // Recoil System
    [Header("Recoil Settings")]
    [SerializeField] private float recoilBuildRate = 15f; // How much recoil builds per shot
    [SerializeField] private float recoilDecayRate = 500f; // How fast recoil decays
    [SerializeField] private float maxRecoil = 100f; // Maximum recoil value

    [Header("Visual Recoil")]
    [SerializeField] private float visualKickStrength = 0.08f; // Visual kick per shot
    [SerializeField] private float visualReturnSpeed = 12f; // Return to center speed
    [SerializeField] private float maxVisualKickback = 0.25f; // Max visual displacement
    [SerializeField] private float rotationKickStrength = 4f; // Rotation per shot
    [SerializeField] private float maxRotationKick = 12f; // Max rotation

    // Runtime recoil values
    private float currentRecoil = 0f;
    private Vector3 visualRecoilOffset = Vector3.zero;
    private float visualRecoilRotation = 0f;

    public GameObject projectilePrefab;
    List<Weapons> weapons = new List<Weapons>
    {
        //Adding weapons to List
        new Weapons("Pistol", 10, 10, 10, 5),
        new Weapons("Shotgun", 15, 20, 15, 1),
        new Weapons("Raygun", 20, 0, 1, 2)
    };

    void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log("Equipped Weapon: " + weapons[0]);
    }

    void Update()
    {
        // Position gun at player center
        transform.position = player.transform.position;

        // Handle rotation
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 gunDirection = mousePos - transform.position;
        float gunAngle = Mathf.Atan2(gunDirection.y, gunDirection.x) * Mathf.Rad2Deg;

        // Update recoil
        updateRecoil();

        // Apply rotation with visual recoil
        transform.rotation = Quaternion.Euler(0f, 0f, gunAngle + visualRecoilRotation);

        // Flip sprite if looking left
        weaponSpriteRenderer.flipY = gunAngle > 90f || gunAngle < -90f;

        // Apply visual recoil offset
        Vector3 recoilPush = transform.right * visualRecoilOffset.x + transform.up * visualRecoilOffset.y;
        transform.position = player.transform.position + recoilPush;

        handleShootingInput();
        handleWeaponSwitch();
    }

    private void updateRecoil()
    {
        // Decay gameplay recoil - much faster when not shooting
        if (currentRecoil > 0)
        {
            if (!isShooting)
            {
                // Almost instant reset when not shooting
                currentRecoil = Mathf.Max(0, currentRecoil - 1000f * Time.deltaTime);
            }
            else
            {
                // Slower decay while shooting
                currentRecoil = Mathf.Max(0, currentRecoil - recoilDecayRate * 0.2f * Time.deltaTime);
            }
        }

        // Return visual recoil to center
        visualRecoilOffset = Vector3.Lerp(visualRecoilOffset, Vector3.zero, visualReturnSpeed * Time.deltaTime);
        visualRecoilRotation = Mathf.Lerp(visualRecoilRotation, 0f, visualReturnSpeed * Time.deltaTime);
    }

    private void applyRecoilKick()
    {
        // Build up gameplay recoil
        float recoilIncrease = recoilBuildRate * (1f + weapons[currentWeapon].spread * 0.02f);
        currentRecoil = Mathf.Min(currentRecoil + recoilIncrease, maxRecoil);

        // Calculate visual kick based on current recoil level
        float recoilPercent = currentRecoil / maxRecoil;
        float diminishingFactor = 1f - (recoilPercent * 0.5f); // Less visual kick at high recoil

        // Apply visual kickback
        float visualKick = visualKickStrength * diminishingFactor;
        visualRecoilOffset.x = Mathf.Max(visualRecoilOffset.x - visualKick, -maxVisualKickback);

        // Add vertical variation
        visualRecoilOffset.y += UnityEngine.Random.Range(-0.01f, 0.01f) * diminishingFactor;

        // Apply rotation kick
        float rotKick = rotationKickStrength * diminishingFactor;
        visualRecoilRotation = Mathf.Min(visualRecoilRotation + UnityEngine.Random.Range(rotKick * 0.7f, rotKick), maxRotationKick);
    }

    private void handleShootingInput()
    {
        if (Input.GetMouseButton(0))
        {
            isShooting = true;
            tryShooting();
        }
        else
        {
            isShooting = false;
        }
    }

    private void tryShooting()
    {
        float fireRate = weapons[currentWeapon].fireRate;
        float fireInterval = 1f / fireRate;

        if (Time.time >= lastFireTime + fireInterval)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    private void Shoot()
    {
        // Apply recoil
        applyRecoilKick();

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - transform.position).normalized;

        // Calculate spread based on current recoil
        float recoilPercent = currentRecoil / maxRecoil;
        float currentSpread = recoilPercent * weapons[currentWeapon].spread;

        float spreadAngle = UnityEngine.Random.Range(-currentSpread, currentSpread);
        Debug.Log($"Current Recoil: {currentRecoil:F1}/{maxRecoil} ({recoilPercent * 100:F0}%) | Spread angle: {spreadAngle:F2}");

        // Apply spread
        float originalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float newAngle = originalAngle + spreadAngle;

        // Convert back to direction vector
        Vector2 spreadDirection = new Vector2(
            Mathf.Cos(newAngle * Mathf.Deg2Rad),
            Mathf.Sin(newAngle * Mathf.Deg2Rad)
        ).normalized;

        // Spawn bullet
        Vector3 spawnPosition = transform.position + transform.TransformDirection(Bulletoffset);
        GameObject bullet = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Set velocity
        bullet.GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * 20.0f;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private void handleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Equipped Weapon: " + weapons[0]);
            currentWeapon = 0;
            resetRecoil();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Equipped Weapon: " + weapons[1]);
            currentWeapon = 1;
            resetRecoil();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Equipped Weapon: " + weapons[2]);
            currentWeapon = 2;
            resetRecoil();
        }
    }

    private void resetRecoil()
    {
        currentRecoil = 0f;
        visualRecoilOffset = Vector3.zero;
        visualRecoilRotation = 0f;
    }
}