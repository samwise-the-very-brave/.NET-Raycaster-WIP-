using System.Collections.Generic;

namespace DDA
{
    static class PlayerResourceManager
    {
        public static int Health { get; private set; }
        public static Dictionary<string, AmmoSlot> Ammo { get; private set; }
        public static int Armor { get; private set; }
        public static int CurrentWeaponIndex { get; private set; }

        static List<Weapon> weapons = new List<Weapon>();

        public static void Init()
        {
            Health = 100;
            Ammo = new Dictionary<string, AmmoSlot>()
            {
                {"None", new AmmoSlot(0, AmmoType.None) },
                {"Shells", new AmmoSlot(100, AmmoType.Shells) },
                {"NineMM", new AmmoSlot(200, AmmoType.NineMM) },
                {"Grenades", new AmmoSlot(50, AmmoType.Grenades) },
                {"Stars", new AmmoSlot(200, AmmoType.Stars) },
                {"PizzaSlices", new AmmoSlot(15, AmmoType.PizzaSlices) },
            };
            Armor = 0;
        }
        public static void Damage(int amount)
        {
            int absorbedByArmor = amount / 2;

            if (absorbedByArmor > Armor)
            {
                int surplus = absorbedByArmor - Armor;
                absorbedByArmor = Armor;
                Health -= surplus;
            }

            Armor -= absorbedByArmor;
            Health -= amount / 2;
        }
        public static void AddAmmo(int ammo, AmmoType type)
        {
            Ammo[type.ToString()].AddAmmo(ammo);
        }
        public static void AddHealth(int health, HealthAndArmorAddingMode mode)
        {
            if (mode == HealthAndArmorAddingMode.ClampTo100 && Health + health > 100)
            {
                Health = 100;
                return;
            }

            if (mode == HealthAndArmorAddingMode.ClampTo200 && Health + health > 200)
            {
                Health = 200;
                return;
            }

            Health += health;
        }
        public static void AddArmor(int armor, HealthAndArmorAddingMode mode)
        {
            if (mode == HealthAndArmorAddingMode.ClampTo100 && Armor + armor > 100)
            {
                Armor = 100;
                return;
            }

            if (mode == HealthAndArmorAddingMode.ClampTo200 && Armor + armor > 200)
            {
                Armor = 200;
                return;
            }

            Armor += armor;
        }
        public static void SetWeapon(int index)
        {
            if (index > weapons.Count - 1)
                return;

            CurrentWeaponIndex = index;
        }
        public static Weapon GetWeapon()
        {
            return weapons[CurrentWeaponIndex];
        }
        public static void AddWeapon(Weapon weapon)
        {
            weapons.Add(weapon);
        }
    }

    enum HealthAndArmorAddingMode
    {
        ClampTo100,
        ClampTo200
    }

    enum AmmoType
    {
        Shells,
        NineMM,
        Stars,
        Grenades,
        PizzaSlices,
        None
    }
}
