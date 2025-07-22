using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3((float)0.7, 0,0);
    public Vector3 Bulletoffset = new Vector3(0,(float)0.109,0);
    private SpriteRenderer weaponSpriteRenderer;

    public bool facing;

    public GameObject projectilePrefab;
    
    public WeaponDatabase weaponDatabase;

    public Weapons equippedWeapon;

    public float chargeTimeRequired = 3f;
    private float chargeTimer = 0f;
    private bool isCharging = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        weaponDatabase.weapons.Add(new Weapons("Pistol",5));
        weaponDatabase.weapons.Add(new Weapons("Shotgun",10));
        weaponDatabase.weapons.Add(new Weapons("RayGun",15));
        //This can be removed once we have our weapon assets

        Debug.Log("Equipped Weapon: " + weaponDatabase.weapons[0]);  
        equippedWeapon = weaponDatabase.weapons[0];
    }

    void shoot(){
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorld - transform.position).normalized;

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 20.0f;

            // Set rotation to match direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
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

        //Shooting
        if (equippedWeapon == weaponDatabase.weapons[2]){
            // Start Charging
            if (Input.GetMouseButtonDown(0))
            {
                isCharging = true;
                chargeTimer = 0f;
                Debug.Log("Started charging...");
            }

            // Charging in progress
            if (isCharging && Input.GetMouseButton(0))
            {
                chargeTimer += Time.deltaTime;
            }

            // Released Mouse Button
            if (isCharging && Input.GetMouseButtonUp(0))
            {
                isCharging = false;

                if (chargeTimer >= chargeTimeRequired)
                {
                    shoot();
                }
                else
                {
                    Debug.Log("Charge too short! (" + chargeTimer.ToString("F2") + "s)");
                    // Optionally play fail sound or effect
                }
            }
        } else if (Input.GetMouseButtonDown(0)){
            shoot();

        }

        transform.position = player.transform.position;
        
        if (Input.GetKeyDown(KeyCode.Alpha1)){  
                //Once we have weapon assets switch the sprite being used here.
                equippedWeapon = weaponDatabase.weapons[0];
                Debug.Log("Equipped Weapon: " + weaponDatabase.weapons[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){  
                //Once we have weapon assets switch the sprite being used here.
                equippedWeapon = weaponDatabase.weapons[1];
                Debug.Log("Equipped Weapon: " + weaponDatabase.weapons[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)){  
                //Once we have weapon assets switch the sprite being used here.
                equippedWeapon = weaponDatabase.weapons[2];
                Debug.Log("Equipped Weapon: " + weaponDatabase.weapons[2]);
        }
        
    }
   
}
