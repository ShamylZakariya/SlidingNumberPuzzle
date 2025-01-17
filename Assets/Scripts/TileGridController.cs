﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class TileGridController : MonoBehaviour
{
    [SerializeField] GameObject _platform;
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] float _tileSeparation = 0.03f;
    [SerializeField] int _size = 3;
    [SerializeField] LayerMask _tileLayer = 0;

    [SerializeField] bool _randomize = false;
    [SerializeField] int _maxSolverSearchDepth = 1024;
    [SerializeField] int _stopAfterNSolutionsFound = 256;

    Dictionary<int, Tile> _tiles = new Dictionary<int, Tile>();
    TreeSolver _treeSolver = null;
    Thread _solverThread = null;

    void Start()
    {
        int[] indices = new int[_size * _size - 1];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        if (_randomize)
        {
            System.Random rng = new System.Random();
            rng.Shuffle(indices);
        }

        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                int i = row * _size + col;
                if (i < (_size * _size - 1))
                {
                    Tile tile = Instantiate(_tilePrefab).GetComponent<Tile>();
                    tile.Index = indices[i];
                    tile.Row = row;
                    tile.Col = col;

                    _tiles[tile.Index] = tile;
                    UpdateTile(tile);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (_treeSolver != null)
        {
            _treeSolver.Cancel();
        }
        if (_solverThread != null)
        {
            try
            {
                _solverThread.Abort();
            }
            catch (ThreadAbortException)
            { }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogFormat("Current board state:\n{0}", CurrentBoardState.ToString());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Solve_TreeSolver();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, float.PositiveInfinity, _tileLayer))
            {
                Tile tile = hit.transform.parent.gameObject.GetComponent<Tile>();
                Play(tile);
            }
        }
    }

    public Board CurrentBoardState
    {
        get
        {
            Board b = new Board(_size);
            for (int i = 0; i < (_size * _size - 1); i++)
            {
                Tile tile = _tiles[i];
                b.Set(tile.Row, tile.Col, tile != null ? tile.Index : Board.Empty);
            }
            return b;
        }
    }

    public void Play(Tile tile)
    {
        Board b = this.CurrentBoardState;
        b = b.Play(tile.Row, tile.Col);
        if (b != null)
        {
            UpdateTiles(b);
        }
    }

    void UpdateTile(Tile tile)
    {
        Bounds bnds = _platform.transform.CalculateBounds();
        float left = bnds.center.x - bnds.extents.x;
        float top = bnds.center.z - bnds.extents.z;
        float surface = bnds.center.y + bnds.extents.y;
        float size = _size;
        float scaledInset = _tileSeparation / size;

        tile.transform.position = new Vector3(left + (tile.Row + 0.5f) / size, surface, top + (tile.Col + 0.5f) / size);
        tile.transform.localScale = new Vector3((1f / size) - (2 * scaledInset), 1, (1f / size) - (2 * scaledInset));
    }

    void UpdateTiles(Board board)
    {
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                int index = board.Get(row, col);
                if (index != Board.Empty)
                {
                    Tile tile = _tiles[index];
                    tile.Row = row;
                    tile.Col = col;
                    UpdateTile(tile);
                }
            }
        }
    }

    private void Solve_TreeSolver()
    {
        if (_treeSolver == null && _solverThread == null)
        {
            _solverThread = new Thread(() =>
            {
                Debug.LogFormat("[TileGridController] - WORKER THREAD - Starting solver...");

                _treeSolver = new Solvers.TreeSolver(CurrentBoardState, _maxSolverSearchDepth, _stopAfterNSolutionsFound);
                int[] solution = _treeSolver.Solution;

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    _treeSolver = null;
                    _solverThread = null;

                    if (solution != null)
                    {
                        Debug.LogFormat("[TileGridController] - MAIN THREAD solution (length:{0}): [{1}]", solution.Length, string.Join(", ", solution));
                    }
                    else
                    {
                        Debug.LogFormat("[TileGridController] - MAIN THREAD found no solution for current board state");
                    }
                });
            });

            _solverThread.Start();
        }
    }
}
