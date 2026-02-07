using System.Collections.Generic;
using Core.Model;

namespace Core.Rules
{
    public class ChessRules
    {
        public MoveResult Validate(BoardState board, Move move, PieceColor turn)
        {
            var fromPiece = board.Get(move.From);
            if (fromPiece == null) return MoveResult.Invalid;

            var toPiece = board.Get(move.To);
            if (toPiece != null && toPiece.Color == fromPiece.Color)
                return MoveResult.Invalid;

            return toPiece == null ? MoveResult.Valid : MoveResult.Capture;
        }
    }
}