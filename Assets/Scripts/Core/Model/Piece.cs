namespace Core.Model
{
    public enum PieceType
    {
        Pawn, Rook, Knight, Bishop, Queen, King
    }

    public enum PieceColor
    {
        White, Black
    }

    public class Piece
    {
        public PieceType Type { get; }
        public PieceColor Color { get; }

        public bool HasMoved { get; set; }

        public Piece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }
    }
}