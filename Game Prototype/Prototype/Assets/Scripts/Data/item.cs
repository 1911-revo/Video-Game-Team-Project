using UnityEngine;

/// <summary>
/// A basic struct that stores item data.
/// </summary>
public struct item
{
    public string id;
    public string name;
    public bool isKeyItem;
    public bool isConsumed;

    public item(string id, string name, bool isKeyItem, bool isConsumed)
    {
        this.id = id;
        this.name = name;
        this.isKeyItem = isKeyItem;
        this.isConsumed = isConsumed;
    } 
}
