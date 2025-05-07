using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3((float)0.7, 0,0);
    public Vector3 Bulletoffset = new Vector3(0,(float)0.109,0);
    private SpriteRenderer weaponSpriteRenderer;

    public bool facing;

    public GameObject projectilePrefab;
        List<Weapons> weapons = new List<Weapons>
        {
            //Adding weapons to List
            new Weapons("Pistol", 10),
            new Weapons("Shotgun", 15),
            new Weapons("Raygun", 20)
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

        //Shooting
        if (Input.GetMouseButtonDown(0)){

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorld - transform.position).normalized;

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 20.0f;

            // Set rotation to match direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);


        }

        transform.position = player.transform.position + offset;
        
        if (Input.GetKeyDown(KeyCode.Alpha1)){  
                //Once we have weapon assets switch the sprite being used here.
                Debug.Log("Equipped Weapon: " + weapons[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){  
                //Once we have weapon assets switch the sprite being used here.
                Debug.Log("Equipped Weapon: " + weapons[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)){  
                //Once we have weapon assets switch the sprite being used here.
                Debug.Log("Equipped Weapon: " + weapons[2]);
        }
        
    }
   
}
