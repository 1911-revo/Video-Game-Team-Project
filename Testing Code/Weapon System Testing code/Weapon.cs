public class Weapon{

    public string name { get; set; }

    public int damage { get; set; }

    

    public Weapon (string Name, int Damage){

        name = Name;

        damage = Damage;

    }

    public override string ToString()
    {
        return $"Weapon: {name}, Damage: {damage}";
    }




}