using System;

class WeaponSystem
{
    static void Main(string[] args)
    {

        int currentWeapon = 0;
        string input = " ";
        List<Weapon> weapons = new List<Weapon>();
        

        //Adding weapons to List
        weapons.Add(new Weapon ("Fists", 1));
        weapons.Add(new Weapon ("Gun", 10));
        weapons.Add(new Weapon ("Knife", 5));
        weapons.Add(new Weapon ("Sword", 7));
        weapons.Add(new Weapon ("Raygun", 20));

        Console.WriteLine("Equipped Weapon: " + weapons[currentWeapon].ToString());
        
 

        while (input != "q"){
            
            input = Console.ReadLine();

            if (input == "f"){
                if (currentWeapon == 4){
                    currentWeapon = 0;
                } else {
                    currentWeapon = currentWeapon + 1;
                }
                Console.WriteLine("Equipped Weapon: " + weapons[currentWeapon].ToString());

            } else {
                Console.WriteLine("Press f to switch weapons or q to Quit!");
            }

        }

        
        

    }
}
