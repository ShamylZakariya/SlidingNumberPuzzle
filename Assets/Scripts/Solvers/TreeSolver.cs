using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Solvers
{
    /// <summary>
    /// Performs a naive depth-dirst tree search for a solution.
    /// </summary>
    public class TreeSolver
    {

        /// <summary>
        /// Vertex represents a game state in the tree, and an array of 
        /// moves to transition to zero or more subsequent game states
        /// </summary>
        public class Vertex
        {
            public Board board = null;
            public Edge[] moves = null;
            public Edge incomingEdge = null;
            internal int depth = 0;
        }

        /// <summary>
        /// Represents a "move" which transforms one game state to the 
        /// next; result is the Vertex resultant from the move
        /// </summary>
        public class Edge
        {
            public int move = Board.Empty;
            public Vertex result = null;
            public Vertex parent = null;
        }

        private Vertex _root;
        private List<int> _solution = null;
        private int _maxSearchDepth = 0;
        private int _stopAfterNSolutions = 1;
        private bool _canceled = false;
        private int _solutionsFound = 0;

        public TreeSolver(Board initialGameState, int maxSearchDepth = int.MaxValue - 1, int stopAfterNSolutionsFound = 1)
        {
            _maxSearchDepth = maxSearchDepth;
            _stopAfterNSolutions = stopAfterNSolutionsFound;
            Vertex root = new Vertex();
            root.board = initialGameState;
            Solve(root, 0);
        }

        public int[] Solution
        {
            get
            {
                if (_solution != null)
                {
                    return _solution.ToArray();
                }
                return null;
            }
        }

        public void Cancel()
        {
            Debug.LogFormat("[Solver] - canceling...");
            _canceled = true;
        }

        private void Solve(Vertex current, int depth)
        {
            // exit if we've been canceled
            if (_canceled) return;

            // we've already found enough solutions, we can bail
            if (_solutionsFound >= _stopAfterNSolutions) return;

            // if this board is in a solved state, we need to record the path
            if (current.board.IsSolved)
            {
                _solutionsFound++;

                // record solution if it's shorter than the current best solution
                if (_solution == null || current.depth < _solution.Count)
                {
                    // we have a solution, walk up the parentage to form a path from solution 
                    // to root, reverse it, and add it to _solutions
                    _solution = new List<int>();

                    Vertex it = current;
                    while (it != null && it.incomingEdge != null)
                    {
                        _solution.Add(it.incomingEdge.move);
                        it = it.incomingEdge.parent;
                    }

                    _solution.Reverse();

                    Debug.LogFormat("[Solver] - found solution: " + string.Join(", ", _solution));
                }

                return;
            }

            if (depth >= _maxSearchDepth)
            {
                return;
            }

            //
            //  Find the available moves for the current board state, remove any moves
            //  that would result in backtracking 
            //

            int[] plays = current.board.AvailableMoves;
            Vertex parent = current.incomingEdge != null ? current.incomingEdge.parent : null;

            // we need to prevent backtracking
            if (parent != null)
            {
                HashSet<int> previousPlays = new HashSet<int>(parent.moves.Select((e) => e.move));
                plays = plays.Where((p) => !previousPlays.Contains(p)).ToArray();
            }

            Board[] boards = plays.Select((p) => current.board.Play(p)).ToArray();
            var playsAndBoards = plays.Zip(boards, (p, b) => new { A = p, B = b });

            current.moves = playsAndBoards.Select((pb) =>
            {
                Edge e = new Edge()
                {
                    move = pb.A,
                    result = new Vertex()
                    {
                        board = pb.B,
                        moves = null,
                        depth = depth + 1
                    },
                    parent = current
                };
                e.result.incomingEdge = e;
                return e;
            }).ToArray();

            foreach (Edge e in current.moves)
            {
                Solve(e.result, depth + 1);
            }
        }
    }
}
