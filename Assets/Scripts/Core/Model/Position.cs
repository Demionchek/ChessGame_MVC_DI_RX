namespace Core.Model
{
    public readonly struct Position
    {
        public readonly int X;
        public readonly int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsInside() => X >= 0 && X < 8 && Y >= 0 && Y < 8;
    }
}