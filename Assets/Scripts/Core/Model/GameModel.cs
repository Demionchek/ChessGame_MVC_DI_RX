using System;
using Core.Rules;
using UniRx;

namespace Core.Model
{
    public class GameModel : IEnPassantProvider
    {
        private readonly ChessRules _rules;
        public BoardState Board { get; } = new();
        public Subject<BoardState> OnBoardChanged { get; } = new();
        public Subject<Move> OnMoveApplied { get; } = new();
        public PieceColor CurrentTurn { get; private set; } = PieceColor.White;

        public Position? EnPassantTarget { get; private set; }

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

            if (EnPassantTarget.HasValue && to.Equals(EnPassantTarget.Value))
            {
                int dir = CurrentTurn == PieceColor.White ? -1 : 1;
                var capturedPawnPos = new Position(to.X, to.Y + dir);
                Board.Set(capturedPawnPos, null);
            }

            Board.Move(from, to);

            EnPassantTarget = null;

            var movedPiece = Board.Get(to);
            if (movedPiece.Type == PieceType.Pawn)
            {
                int deltaY = Math.Abs(to.Y - from.Y);
                if (deltaY == 2)
                {
                    int dir = movedPiece.Color == PieceColor.White ? 1 : -1;
                    EnPassantTarget = new Position(to.X, to.Y - dir);
                }
            }

            var pieceAfter = Board.Get(to);
            if (pieceAfter.Type == PieceType.King)
            {
                int dx = to.X - from.X;
                if (Math.Abs(dx) == 2)
                {
                    bool kingSide = dx > 0;
                    int rookFromX = kingSide ? 7 : 0;
                    int rookToX = kingSide ? to.X - 1 : to.X + 1;

                    var rookFrom = new Position(rookFromX, from.Y);
                    var rookTo = new Position(rookToX, from.Y);

                    Board.Move(rookFrom, rookTo);
                }
            }

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