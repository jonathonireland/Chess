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
            List<Move> moves =
                gameState.LegalMovesForPlayer(Player.Black)
                    .ToList();

            if(!moves.Any())
            {
                return null;
            }

            return moves[random.Next(moves.Count)];
        }
    }
}
