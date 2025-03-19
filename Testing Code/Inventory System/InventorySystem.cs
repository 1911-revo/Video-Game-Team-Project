using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public List<GameObject> inventorySystem = new List<GameObject>(); // Player's inventory
    public List<GameObject> items = new List<GameObject>(); // Available items

    private GameObject medkit;
    private GameObject key;
    private GameObject gun;
    private GameObject ammo;
    private GameObject food;
    private GameObject water;
    private GameObject flashlight;
    private GameObject map;
    private GameObject compass;
    private GameObject radio;
    private GameObject knife;

    private int indexOfItemSlot = 0; // Tracks selected item

    void Start()
    {
        medkit = GameObject.Find("Medkit");
        key = GameObject.Find("Key");
        gun = GameObject.Find("Gun");
        ammo = GameObject.Find("Ammo");
        food = GameObject.Find("Food");
        water = GameObject.Find("Water");
        flashlight = GameObject.Find("Flashlight");
        map = GameObject.Find("Map");
        compass = GameObject.Find("Compass");
        radio = GameObject.Find("Radio");
        knife = GameObject.Find("Knife");

        items.Add(medkit);
        items.Add(key);
        items.Add(gun);
        items.Add(ammo);
        items.Add(food);
        items.Add(water);
        items.Add(flashlight);
        items.Add(map);
        items.Add(compass);
        items.Add(radio);
        items.Add(knife);

        Debug.Log("Press B to open the inventory");
        Debug.Log("Press H to see what item is in your hand");
        Debug.Log("Press A and D to change the item you have");
        Debug.Log("Press E to store and press L to abandon the item in the inventory");
        Debug.Log("we have " + items.Count + " items in the list to choose");
    }



    void Update()
    {
        // Open inventory
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (inventorySystem.Count > 0)
            {
                Debug.Log("Inventory contains:");
                foreach (var item in inventorySystem)
                {
                    Debug.Log(item.name);
                }
            }
            else
            {
                Debug.Log("Inventory is empty");
            }
        }

        // See what item is in hand
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (items.Count > 0)
            {
                Debug.Log("The item in your hand is " + items[indexOfItemSlot].name);
            }
            else
            {
                Debug.Log("No items available.");
            }
        }

        // Change item in hand (left)
        if (Input.GetKeyDown(KeyCode.A) && items.Count > 0)
        {
            if (indexOfItemSlot > 0)
            {
                indexOfItemSlot--;
                Debug.Log("The item in your hand is " + items[indexOfItemSlot].name);
            }
            else
            {
                Debug.Log("Already at the first item.");
            }
        }

        // Change item in hand (right)
        if (Input.GetKeyDown(KeyCode.D) && items.Count > 0)
        {
            if (indexOfItemSlot < items.Count - 1)
            {
                indexOfItemSlot++;
                Debug.Log("The item in your hand is " + items[indexOfItemSlot].name);
            }
            else
            {
                Debug.Log("Already at the last item.");
            }
        }

        // Store item in inventory
        if (Input.GetKeyDown(KeyCode.E) && items.Count > 0)
        {
            GameObject selectedItem = items[indexOfItemSlot];

            if (!inventorySystem.Contains(selectedItem)) // Prevent duplicate storage
            {
                inventorySystem.Add(selectedItem);
                Debug.Log("You have stored " + selectedItem.name + " in your inventory.");
            }
            else
            {
                Debug.Log(selectedItem.name + " is already in your inventory.");
            }
        }
    }
}
