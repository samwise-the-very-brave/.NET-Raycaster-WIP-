namespace DDA
{
    abstract class Enemy : Sprite, IDamagable
    {
        public int Health { get; private set; }
        protected bool Dead { get; set; }

        public Enemy(string spriteName, float x, float y, int health) : base(spriteName, x, y, true)
        {
            Health = health;
            HitboxSize = new System.Drawing.SizeF(1, 1);
            EnemyManager.Add(this);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Dead = true;
                OnDie();
            }
        }

        protected abstract void OnDie();
    }
}
