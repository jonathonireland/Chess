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

        private readonly List<Piece> capturedWhitePieces = new();
        private readonly List<Piece> capturedBlackPieces = new();

        public IReadOnlyList<Piece> CapturedWhitePieces => capturedWhitePieces;
        public IReadOnlyList<Piece> CapturedBlackPieces => capturedBlackPieces;

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
            return PseudoLegalMovesForPiece(pos, piece)
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

            if (capturedPiece != null)
            {
                if (capturedPiece.Color == Player.White)
                {
                    capturedWhitePieces.Add(capturedPiece);
                }
                else if (capturedPiece.Color == Player.Black)
                {
                    capturedBlackPieces.Add(capturedPiece);
                }
            }

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
                        PseudoLegalMovesForPiece(pos, piece)
                            .Where(move => MoveIsLegal(move, player))
                    );
                }
            }

            return moves;
        }


        private IEnumerable<Move> PseudoLegalMovesForPiece(Position pos, Piece piece)
        {
            foreach (Move move in piece.GetMoves(pos, Board))
            {
                yield return move;
            }

            if (piece.Type == PieceType.King)
            {
                foreach (Move castleMove in CastleMovesForKing(pos, piece))
                {
                    yield return castleMove;
                }
            }
        }

        private IEnumerable<Move> CastleMovesForKing(Position kingPos, Piece king)
        {
            if (king.HasMoved || IsInCheck(king.Color, Board))
            {
                yield break;
            }

            int row = king.Color == Player.White ? 7 : 0;

            if (kingPos.Row != row || kingPos.Column != 4)
            {
                yield break;
            }

            Move kingSide = TryCreateCastleMove(
                king,
                MoveType.CastlingKS,
                kingPos,
                new Position(row, 7),
                new Position(row, 6),
                new Position(row, 5),
                new[] { new Position(row, 5), new Position(row, 6) });

            if (kingSide != null)
            {
                yield return kingSide;
            }

            Move queenSide = TryCreateCastleMove(
                king,
                MoveType.CastlingQS,
                kingPos,
                new Position(row, 0),
                new Position(row, 2),
                new Position(row, 3),
                new[] { new Position(row, 1), new Position(row, 2), new Position(row, 3) });

            if (queenSide != null)
            {
                yield return queenSide;
            }
        }

        private Move TryCreateCastleMove(
            Piece king,
            MoveType moveType,
            Position kingFrom,
            Position rookFrom,
            Position kingTo,
            Position rookTo,
            IEnumerable<Position> emptySquares)
        {
            Piece rook = Board[rookFrom];

            if (rook == null || rook.Type != PieceType.Rook || rook.Color != king.Color || rook.HasMoved)
            {
                return null;
            }

            if (emptySquares.Any(pos => !Board.IsEmpty(pos)))
            {
                return null;
            }

            Position firstKingStep = new Position(kingFrom.Row, rookFrom.Column == 7 ? 5 : 3);

            if (SquareIsAttacked(firstKingStep, king.Color.Opponent(), Board) ||
                SquareIsAttacked(kingTo, king.Color.Opponent(), Board))
            {
                return null;
            }

            return new CastleMove(moveType, kingFrom, kingTo, rookFrom, rookTo);
        }

        private bool SquareIsAttacked(Position square, Player attackingPlayer, Board board)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);
                    Piece piece = board[pos];

                    if (piece == null || piece.Color != attackingPlayer)
                    {
                        continue;
                    }

                    if (AttackedSquaresForPiece(pos, piece, board).Any(attackedSquare => attackedSquare == square))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private IEnumerable<Position> AttackedSquaresForPiece(Position pos, Piece piece, Board board)
        {
            if (piece.Type == PieceType.Pawn)
            {
                Direction forward = piece.Color == Player.White ? Direction.North : Direction.South;
                Position leftAttack = pos + forward + Direction.West;
                Position rightAttack = pos + forward + Direction.East;

                if (Board.IsInside(leftAttack))
                {
                    yield return leftAttack;
                }

                if (Board.IsInside(rightAttack))
                {
                    yield return rightAttack;
                }

                yield break;
            }

            foreach (Move move in piece.GetMoves(pos, board))
            {
                yield return move.ToPos;
            }
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

            return SquareIsAttacked(kingPos, player.Opponent(), board);
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
