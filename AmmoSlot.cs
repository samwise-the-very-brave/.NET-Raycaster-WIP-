namespace DDA
{
    class AmmoSlot
    {
        public int MaxAmmo { get; init; }
        public int CurrentAmmo { get; private set; }
        public AmmoType AmmoType { get; }

        public AmmoSlot(int maxAmmo, AmmoType type)
        {
            MaxAmmo = maxAmmo;
            CurrentAmmo = 0;
            AmmoType = type;
        }

        public void AddAmmo(int ammo)
        {
            if (CurrentAmmo + ammo > MaxAmmo)
            {
                CurrentAmmo = MaxAmmo;
                return;
            }

            CurrentAmmo += ammo;
        }
    }
}
