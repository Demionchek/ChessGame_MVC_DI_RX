namespace Core.Model
{
    public enum MoveResult
    {
        Invalid,
        Valid,
        Capture
    }

    public readonly struct Move
    {
        public readonly Position From;
        public readonly Position To;

        public Move(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}