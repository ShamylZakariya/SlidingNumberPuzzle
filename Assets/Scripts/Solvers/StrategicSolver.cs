using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solvers {

    public class StrategicSolver {

        /// <summary>
        /// Compute the next move to make given the current board state
        /// </summary>
        /// <returns>Tuple(int,Board) representing the move played, and the resultant board state, or (-1, null) if board is solved</returns>
        public static (int, Board) Next(Board board)
        {
            int currentIdx = IndexOfTileToActOn(board);
            if (currentIdx == -1) {
                // the board is solved
                return (-1,null);
            }

            var (emptyRow, emptyCol) = board.FindEmptySpace();
            var (currentRow, currentCol) = board.Find(currentIdx);
            var (destRow, destCol) = DestinationForTileIndex(currentIdx, board);

            // we'll move the tile to the correct column, and then to the currect row
            if (destCol != currentCol) {

            }

            return (0, board);
        }

        /// <summary>
        /// The strategy is to move tile 0 to (0,0), tile 1 to (0,1), and so on, one by one.
        /// This returns the index of the first tile which isn't in place. For a freshly jumbled
        /// board, this will (probably) be tile index 0.
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        static int IndexOfTileToActOn(Board currentState) {
            int end = currentState.Size * currentState.Size;
            for (int i = 0; i < end; i++) {
                if (currentState.Get(i) != i && i < end) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get the (row,col) destination for a given tile index. tile index 0 goes to (0,0), tile 1 to (0,1), etc
        /// </summary>
        static (int, int) DestinationForTileIndex(int index, Board board) {
            return (index / board.Size, index % board.Size);
        }

    }

}