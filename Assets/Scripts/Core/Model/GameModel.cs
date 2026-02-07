using Core.Rules;
using UniRx;

namespace Core.Model
{
    public class GameModel
    {
        private readonly ChessRules _rules;
        public BoardState Board { get; } = new();

        public Subject<BoardState> OnBoardChanged { get; } = new();
        public Subject<Move> OnMoveApplied { get; } = new();

        public PieceColor CurrentTurn { get; private set; } = PieceColor.White;

        public GameModel(ChessRules rules)
        {
            _rules = rules;
        }

        public void PlacePiece(Position pos, Piece piece)
        {
            Board.Set(pos, piece);
            OnBoardChanged.OnNext(Board);
        }

        public void TryMove(Position from, Position to)
        {
            var move = new Move(from, to);
            var result = _rules.Validate(Board, move, CurrentTurn);

            if (result == MoveResult.Invalid)
                return;

            Board.Move(from, to);

            OnMoveApplied.OnNext(move);
            OnBoardChanged.OnNext(Board);

            CurrentTurn = CurrentTurn == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;
        }

        public void SetupInitialPosition()
        {
            for (int x = 0; x < 8; x++)
            {
                Board.Set(new Position(x, 1), new Piece(PieceType.Pawn, PieceColor.White));
                Board.Set(new Position(x, 6), new Piece(PieceType.Pawn, PieceColor.Black));
            }

            OnBoardChanged.OnNext(Board);
        }
    }
}