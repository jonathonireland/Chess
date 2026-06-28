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
        public Player Winner { get; private set; } = Player.None;
        public bool IsGameOver => Winner != Player.None;
        public bool IsCheck { get; private set; }

        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (IsGameOver || Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board)
                .Where(move => MoveIsLegal(move, CurrentPlayer));
        }

        public void MakeMove(Move move)
        {
            if (IsGameOver)
            {
                return;
            }

            Piece capturedPiece = Board[move.ToPos];
            Player movingPlayer = CurrentPlayer;

            move.Execute(Board);

            CurrentPlayer = CurrentPlayer.Opponent();

            IsCheck = IsInCheck(CurrentPlayer, Board);
        }

        public IEnumerable<Move> LegalMovesForPlayer(Player player)
        {
            if (IsGameOver)
            {
                return Enumerable.Empty<Move>();
            }

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

                    moves.AddRange(
                        piece.GetMoves(pos, Board)
                            .Where(move => MoveIsLegal(move, player))
                    );
                }
            }

            return moves;
        }

        private bool MoveIsLegal(Move move, Player player)
        {
            Board copy = Board.Copy();
            move.Execute(copy);
            return !IsInCheck(player, copy);
        }

        private Position FindKing(Player player, Board board)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);
                    Piece piece = board[pos];

                    if (piece != null &&
                        piece.Color == player &&
                        piece.Type == PieceType.King)
                    {
                        return pos;
                    }
                }
            }

            return null;
        }

        private bool IsInCheck(Player player, Board board)
        {
            Position kingPos = FindKing(player, board);

            if (kingPos == null)
            {
                return true;
            }

            Player opponent = player.Opponent();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);
                    Piece piece = board[pos];

                    if (piece == null || piece.Color != opponent)
                    {
                        continue;
                    }

                    IEnumerable<Move> opponentMoves = piece.GetMoves(pos, board);

                    if (opponentMoves.Any(move => move.ToPos == kingPos))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ChangePlayer()
        {
            CurrentPlayer = CurrentPlayer.Opponent();

            if (IsInCheck(CurrentPlayer, Board))
            {
                // Raise event or set a flag
            }
        }
    }
}
