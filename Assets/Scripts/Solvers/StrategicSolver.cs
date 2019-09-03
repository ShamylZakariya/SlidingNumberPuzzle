using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solvers {

    public class StrategicSolver {

        /// <summary>
        /// Compute the next move to make given the current board state
        /// </summary>
        /// <returns>Tuple(int,Board) representing the move played, and the resultant board state</returns>
        public static (int, Board) Next(Board currentState)
        {
            return (0, currentState);
        }

    }

}