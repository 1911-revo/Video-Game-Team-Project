using UnityEngine;

public class Weapons
{

    public string name { get; set; }

    public int damage { get; set; }

    

    public Weapons (string Name, int Damage){

        name = Name;

        damage = Damage;

    }

    public override string ToString()
    {
        return $"Weapon: {name}, Damage: {damage}";
    }
    
}
