using UnityEngine;

public class Weapons
{

    public string name { get; set; }

    public int damage { get; set; }

    // Extended functionality

    public int spread { get; set; }

    public int knockback { get; set; }

    public int fireRate { get; set; }



    public Weapons (string Name, int Damage, int Spread, int Knockback, int FireRate)
    {

        name = Name;

        damage = Damage;

        spread = Spread;

        knockback = Knockback;

        fireRate = FireRate;
    }

    public override string ToString()
    {
        return $"Weapon: {name}, Damage: {damage}, FireRate: {fireRate}, Spread: {spread}, Knockback {knockback}";
    }
    
}
