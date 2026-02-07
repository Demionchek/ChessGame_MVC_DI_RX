namespace Core.Model
{
    public class BoardState
    {
        private readonly Piece[,] _board = new Piece[8, 8];

        public Piece Get(Position pos) => _board[pos.X, pos.Y];

        public void Set(Position pos, Piece piece) => _board[pos.X, pos.Y] = piece;

        public bool IsEmpty(Position pos) => Get(pos) == null;

        public bool IsEnemy(Position pos, PieceColor color)
        {
            var piece = Get(pos);
            return piece != null && piece.Color != color;
        }

        public bool IsInside(Position pos) => pos.IsInside();

        public void Move(Position from, Position to)
        {
            var piece = Get(from);
            Set(to, piece);
            Set(from, null);
            piece.HasMoved = true;
        }
    }
}