namespace DDA
{
    class Frenchman : Enemy
    {
        public float RelativeAngle { get; set; }

        public Frenchman(string spriteName, float x, float y) : base(spriteName, x, y, 100) { }

        public override void Update()
        {
            if (Dead)
                return;

            if (RelativeAngle > 0 && RelativeAngle < 90)
            {
                currentAnimationIndex = 0;
            }
            else if (RelativeAngle > 90 && RelativeAngle < 180)
            {
                currentAnimationIndex = 1;
            }
            else if (RelativeAngle < 0 && RelativeAngle > -90)
            {
                currentAnimationIndex = 2;
            }
            else
            {
                currentAnimationIndex = 3;
            }
        }

        protected override void OnDie()
        {
            currentAnimationIndex = 4;
        }
    }
}
