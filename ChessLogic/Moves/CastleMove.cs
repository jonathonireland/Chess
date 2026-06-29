using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class CastleMove : Move
    {
        public override MoveType Type { get; }
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public CastleMove(MoveType type, Position kingFromPos, Position kingToPos, Position rookFromPos, Position rookToPos)
        {
            Type = type;
            FromPos = kingFromPos;
            ToPos = kingToPos;
            this.rookFromPos = rookFromPos;
            this.rookToPos = rookToPos;
        }

        public override void Execute(Board board)
        {
            Piece king = board[FromPos];
            Piece rook = board[rookFromPos];

            board[ToPos] = king;
            board[FromPos] = null;

            board[rookToPos] = rook;
            board[rookFromPos] = null;

            king.HasMoved = true;
            rook.HasMoved = true;
        }
    }
}
