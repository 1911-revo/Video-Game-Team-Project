using UnityEngine;

public class collectable : MonoBehaviour
{
    [SerializeField] public string id;
    [SerializeField] public string itemName;
    [SerializeField] public bool isKeyItem;
    [SerializeField] public bool isConsumed;

    public item thisItem;

    /// <summary>
    /// Creates an item attached to a gameobject that can be referenced later.
    /// </summary>
    private void Start()
    {
        thisItem = new item(id, itemName, isKeyItem, isConsumed);
    }
}
