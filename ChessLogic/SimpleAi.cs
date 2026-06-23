using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public static class SimpleAi
    {
        private static readonly Random random = new();

        public static Move ChooseMove(GameState gameState)
        {
            List<Move> moves = gameState
                .LegalMovesForPlayer(Player.Black)
                .ToList();

            if (!moves.Any())
            {
                return null;
            }

            int bestScore = int.MinValue;
            List<Move> bestMoves = new();

            foreach (Move move in moves)
            {
                int score = ScoreMove(gameState, move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(move);
                }
            }

            return bestMoves[random.Next(bestMoves.Count)];
        }

        private static int GetPieceValue(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => 1,
                PieceType.Knight => 3,
                PieceType.Bishop => 3,
                PieceType.Rook => 5,
                PieceType.Queen => 9,
                PieceType.King => 1000,
                _ => 0
            };
        }

        private static int EvaluateBoard(Board board)
        {
            int score = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);

                    if (board.IsEmpty(pos))
                    {
                        continue;
                    }

                    Piece piece = board[pos];
                    int value = GetPieceValue(piece.Type);

                    if (piece.Color == Player.Black)
                    {
                        score += value;
                    }
                    else
                    {
                        score -= value;
                    }
                }
            }

            return score;
        }

        private static int ScoreMove(GameState gameState, Move move)
        {
            Board boardCopy = gameState.Board.Copy();

            move.Execute(boardCopy);

            return EvaluateBoard(boardCopy);
        }
    }
}