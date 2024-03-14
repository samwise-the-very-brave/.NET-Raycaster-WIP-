using System.Drawing;

namespace DDA
{
    class AmmoPickup : Sprite
    {
        public AmmoType AmmoType { get; init; }
        public int Ammo { get; private set; }

        public AmmoPickup(string spriteName, float x, float y, int ammo, AmmoType ammoType) : base(spriteName, x, y, true)
        {
            Ammo = ammo;
            AmmoType = ammoType;
        }

        public override void Start()
        {
            HitboxSize = new SizeF(0.5f, 0.5f);
        }

        public override void OnCollisionWithPlayer()
        {
            PlayerResourceManager.AddAmmo(4, AmmoType);
            AudioManager.Play("pickups", "pickup");
            Main.ScreenFlash(0.25f, Color.FromArgb(128, Color.Yellow));
            SpriteManager.Destroy(this);
        }
    }
}
