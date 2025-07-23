using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3((float)0.7, 0,0);
    public Vector3 Bulletoffset = new Vector3(0,(float)0.109,0);
    private SpriteRenderer weaponSpriteRenderer;

    public bool facing;


    private bool isShooting = false;
    private int currentWeapon = 0;
    private float lastFireTime = 0f;

    private float currentRecoil = 0f;

    public GameObject projectilePrefab;
        List<Weapons> weapons = new List<Weapons>
        {
            //Adding weapons to List
            new Weapons("Pistol", 10, 1, 1, 5),
            new Weapons("Shotgun", 15, 20, 15, 1),
            new Weapons("Raygun", 20, 0, 1, 2)
        };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        //This can be removed once we have our weapon assets.
        Debug.Log("Equipped Weapon: " + weapons[0]);  
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gunDirection = mousePos - transform.position;
        float gunAngle = Mathf.Atan2(gunDirection.y, gunDirection.x) * Mathf.Rad2Deg;

        // Rotate the prefab to face the mouse
        transform.rotation = Quaternion.Euler(0f, 0f, gunAngle);

        // Flip the sprite if looking left
        weaponSpriteRenderer.flipY = gunAngle > 90f || gunAngle < -90f;


        transform.position = player.transform.position + offset - offset; // We can manipulate the offset to simulate recoil.


        handleShootingInput();

        handleWeaponSwitch();
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
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - transform.position).normalized;


        float currentRecoil = 10; //Made up for now to simulate how guns will behave when you fire them for too long
        float currentSpread = currentRecoil * weapons[currentWeapon].spread;

        float spreadAngle = UnityEngine.Random.Range(-currentSpread, currentSpread);
        Debug.Log("Spread angle: " + spreadAngle);

        // Apply spread
        float originalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float newAngle = originalAngle + spreadAngle;

        // Convert back to direction vector with spread applied
        Vector2 spreadDirection = new Vector2(
            Mathf.Cos(newAngle * Mathf.Deg2Rad),
            Mathf.Sin(newAngle * Mathf.Deg2Rad)
        ).normalized;

        // Apply bullet offset for spawn position
        Vector3 spawnPosition = transform.position + Bulletoffset;

        GameObject bullet = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Set velocity with spread applied
        bullet.GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * 20.0f;

        // Set rotation to match the spread direction
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private void handleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Once we have weapon assets switch the sprite being used here.
            Debug.Log("Equipped Weapon: " + weapons[0]);
            currentWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Once we have weapon assets switch the sprite being used here.
            Debug.Log("Equipped Weapon: " + weapons[1]);
            currentWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Once we have weapon assets switch the sprite being used here.
            Debug.Log("Equipped Weapon: " + weapons[2]);
            currentWeapon = 2;
        }
    }
}
