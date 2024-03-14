namespace DDA
{
    class Weapon : Sprite
    {
        public int FireDelay { get; }
        public AmmoType AmmoType { get; }

        public Weapon(string spriteName, int fireDelay, AmmoType type) : base(spriteName, 0, Main.gameWindowHeight * Main.graphicsScale - 150 * Main.graphicsScale, false)
        {
            FireDelay = fireDelay;
            AmmoType = type;

            PlayerResourceManager.AddWeapon(this);
        }

        public void Fire()
        {
            currentAnimationIndex = 1;
            CurrentAnimation.Reset();
        }
    }
}
