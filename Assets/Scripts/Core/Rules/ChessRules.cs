using System;
using System.Collections.Generic;
using Core.Model;

namespace Core.Rules
{
    public class ChessRules
    {
        public MoveResult Validate(BoardState board, Move move, PieceColor turn)
        {
            Piece piece = board.Get(move.From);
            if (piece == null) return MoveResult.Invalid;
            if (piece.Color != turn) return MoveResult.Invalid;

            Piece target = board.Get(move.To);
            if (target != null && target.Color == piece.Color)
                return MoveResult.Invalid;

            var legalMoves = GetLegalMoves(board, move.From, piece);

            bool found = false;
            foreach (var pos in legalMoves)
            {
                if (pos.X == move.To.X && pos.Y == move.To.Y)
                {
                    found = true;
                    break;
                }
            }

            if (!found) return MoveResult.Invalid;

            BoardState simulated = board.Clone();
            simulated.Move(move.From, move.To);

            if (IsKingInCheck(simulated, turn))
                return MoveResult.Invalid;

            return target == null ? MoveResult.Valid : MoveResult.Capture;
        }

        private List<Position> GetLegalMoves(BoardState board, Position from, Piece piece)
        {
            var pseudo = GetPseudoMoves(board, from, piece);
            var legal = new List<Position>();

            foreach (var to in pseudo)
            {
                var sim = board.Clone();
                sim.Move(from, to);

                if (!IsKingInCheck(sim, piece.Color))
                    legal.Add(to);
            }

            if (piece.Type == PieceType.King && !piece.HasMoved)
                AddCastlingMoves(board, from, piece, legal);

            return legal;
        }

        private List<Position> GetPseudoMoves(BoardState board, Position from, Piece piece)
        {
            return piece.Type switch
            {
                PieceType.Pawn => PawnMoves(board, from, piece),
                PieceType.Rook => RookMoves(board, from, piece),
                PieceType.Knight => KnightMoves(board, from, piece),
                PieceType.Bishop => BishopMoves(board, from, piece),
                PieceType.Queen => QueenMoves(board, from, piece),
                PieceType.King => KingMoves(board, from, piece),
                _ => new List<Position>()
            };
        }

        private List<Position> PawnMoves(BoardState board, Position from, Piece piece)
        {
            var moves = new List<Position>();
            int dir = piece.Color == PieceColor.White ? 1 : -1;

            Position forward = new Position(from.X, from.Y + dir);
            if (board.IsInside(forward) && board.IsEmpty(forward))
                moves.Add(forward);

            // двойной ход
            if (!piece.HasMoved)
            {
                Position doubleForward = new Position(from.X, from.Y + 2 * dir);
                if (board.IsEmpty(doubleForward))
                    moves.Add(doubleForward);
            }

            // атаки
            Position left = new Position(from.X - 1, from.Y + dir);
            Position right = new Position(from.X + 1, from.Y + dir);

            if (board.IsInside(left) && board.IsEnemy(left, piece.Color))
                moves.Add(left);

            if (board.IsInside(right) && board.IsEnemy(right, piece.Color))
                moves.Add(right);

            if (board is IEnPassantProvider ep && ep.EnPassantTarget.HasValue)
            {
                var epPos = ep.EnPassantTarget.Value;
                if (Math.Abs(epPos.X - from.X) == 1 && epPos.Y == from.Y + dir)
                {
                    moves.Add(epPos);
                }
            }

            return moves;
        }

        private List<Position> RookMoves(BoardState board, Position from, Piece piece)
            => RayMoves(board, from, piece, new[]
            {
                (1,0), (-1,0), (0,1), (0,-1)
            });

        private List<Position> BishopMoves(BoardState board, Position from, Piece piece)
            => RayMoves(board, from, piece, new[]
            {
                (1,1), (-1,1), (1,-1), (-1,-1)
            });

        private List<Position> QueenMoves(BoardState board, Position from, Piece piece)
            => RayMoves(board, from, piece, new[]
            {
                (1,0), (-1,0), (0,1), (0,-1),
                (1,1), (-1,1), (1,-1), (-1,-1)
            });

        private List<Position> RayMoves(BoardState board, Position from, Piece piece, (int, int)[] dirs)
        {
            var moves = new List<Position>();

            foreach (var (dx, dy) in dirs)
            {
                int x = from.X + dx;
                int y = from.Y + dy;

                while (true)
                {
                    var pos = new Position(x, y);
                    if (!board.IsInside(pos)) break;

                    if (board.IsEmpty(pos))
                    {
                        moves.Add(pos);
                    }
                    else
                    {
                        if (board.IsEnemy(pos, piece.Color))
                            moves.Add(pos);
                        break;
                    }


                    x += dx;
                    y += dy;
                }
            }

            return moves;
        }

        private List<Position> KnightMoves(BoardState board, Position from, Piece piece)
        {
            var moves = new List<Position>();
            var offsets = new (int, int)[]
            {
                (1,2), (2,1), (-1,2), (-2,1),
                (1,-2), (2,-1), (-1,-2), (-2,-1)
            };

            foreach (var (dx, dy) in offsets)
            {
                Position pos = new Position(from.X + dx, from.Y + dy);
                if (!board.IsInside(pos)) continue;

                if (board.IsEmpty(pos) || board.IsEnemy(pos, piece.Color))
                    moves.Add(pos);
            }

            return moves;
        }

        private List<Position> KingMoves(BoardState board, Position from, Piece piece)
        {
            var moves = new List<Position>();

            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Position pos = new Position(from.X + dx, from.Y + dy);
                    if (!board.IsInside(pos)) continue;

                    if (board.IsEmpty(pos) || board.IsEnemy(pos, piece.Color))
                        moves.Add(pos);
                }

            return moves;
        }

        private void AddCastlingMoves(BoardState board, Position from, Piece piece, List<Position> moves)
        {
            if (!piece.HasMoved && !IsKingInCheck(board, piece.Color))
            {
                TryAddCastling(board, from, piece, moves, true);
                TryAddCastling(board, from, piece, moves, false);
            }
        }

        private bool IsKingInCheck(BoardState board, PieceColor color)
        {
            Position kingPos = default;

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board.Get(new Position(x, y));
                    if (piece != null &&
                        piece.Type == PieceType.King &&
                        piece.Color == color)
                    {
                        kingPos = new Position(x, y);
                    }
                }

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    Piece piece = board.Get(pos);
                    if (piece == null || piece.Color == color) continue;

                    var moves = GetPseudoMoves(board, pos, piece);
                    foreach (var move in moves)
                    {
                        if (move.X == kingPos.X && move.Y == kingPos.Y)
                            return true;
                    }
                }

            return false;
        }

        private void TryAddCastling(BoardState board, Position kingPos, Piece king,
                                    List<Position> moves, bool kingSide)
        {
            int y = kingPos.Y;
            int rookX = kingSide ? 7 : 0;
            int dir = kingSide ? 1 : -1;

            var rookPos = new Position(rookX, y);
            var rook = board.Get(rookPos);

            if (rook == null || rook.Type != PieceType.Rook || rook.HasMoved)
                return;

            for (int x = kingPos.X + dir; x != rookX; x += dir)
            {
                if (!board.IsEmpty(new Position(x, y)))
                    return;
            }

            for (int x = kingPos.X; x != kingPos.X + 2 * dir; x += dir)
            {
                var sim = board.Clone();
                sim.Move(kingPos, new Position(x, y));
                if (IsKingInCheck(sim, king.Color))
                    return;
            }

            moves.Add(new Position(kingPos.X + 2 * dir, y));
        }
    }
}