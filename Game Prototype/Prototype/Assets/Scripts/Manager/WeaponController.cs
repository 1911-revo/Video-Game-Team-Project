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

        
        if (Input.GetMouseButtonDown(0)){
            GameObject bullet = Instantiate(projectilePrefab, transform.position + Bulletoffset,projectilePrefab.transform.rotation);
            bulletMovement bulletScript = bullet.GetComponent<bulletMovement>();
            bulletScript.setFacing(facing);
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
    private void FixedUpdate()
    {
        SetFacingDirection();
    }

    private void SetFacingDirection()
    {
        Vector3 cursorPos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(player.transform.position);

        if (cursorPos.x < playerScreenPoint.x)
        {
            facing = false;
            weaponSpriteRenderer.flipX = true;
            offset = new Vector3((float)-0.7, 0,0);
        } 
        else
        {
            facing = true;
            weaponSpriteRenderer.flipX = false;
            offset = new Vector3((float)0.7, 0,0);
        }
    }
}
