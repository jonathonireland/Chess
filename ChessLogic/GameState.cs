using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }

        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board);
        }

        public void MakeMove(Move move) 
        {
            move.Execute(Board);
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        public IEnumerable<Move> LegalMovesForPlayer(Player player)
        {
            List<Move> moves = new();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);

                    if (Board.IsEmpty(pos))
                    {
                        continue;
                    }

                    Piece piece = Board[pos];

                    if (piece.Color != player)
                    {
                        continue;
                    }

                    moves.AddRange(piece.GetMoves(pos, Board));
                }
            }

            return moves;
        }
    }
}
