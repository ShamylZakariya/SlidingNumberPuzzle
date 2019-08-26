public class Board
{

    public static int Empty = -1;

    int _size;
    int[] _spaces;

    public Board(int size)
    {
        _size = size;
        _spaces = new int[size * size];
        for (int i = 0; i < _spaces.Length; i++)
        {
            _spaces[i] = Board.Empty;
        }
    }

    public Board(Board src)
    {
        _size = src._size;
        _spaces = new int[_size * _size];
        for (int i = 0; i < _spaces.Length; i++)
        {
            _spaces[i] = src._spaces[i];
        }
    }

    public Board Clone() { return new Board(this); }

    public void Set(int row, int col, int value)
    {
        _spaces[row * _size + col] = value;
    }

    public int Get(int row, int col)
    {
        return _spaces[row * _size + col];
    }

    public int Size { get { return _size; } }

    public bool IsSolved
    {
        get
        {
            int expected = 0;
            for (int i = 0; i < _spaces.Length; i++)
            {
                if (_spaces[i] == expected)
                {
                    // advance expected token to next, or Empty since in winning state the final space is empty
                    expected = i == _spaces.Length - 2 ? Board.Empty : i + 1;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    internal void Swap(int srcRow, int srcCol, int dstRow, int dstCol)
    {
        int srcOffset = srcRow * _size + srcCol;
        int dstOffset = dstRow * _size + dstCol;
        int t = _spaces[dstOffset];
        _spaces[dstOffset] = _spaces[srcOffset];
        _spaces[srcOffset] = t;
    }

    public override string ToString()
    {
        string[] rows = new string[_size];
        for (int row = 0; row < _size; row++)
        {
            string rowStr = "[";
            for (int col = 0; col < _size; col++)
            {
                int v = Get(row, col);
                rowStr += v != Board.Empty ? v.ToString() : " ";
                if (col < _size - 1) { rowStr += ", "; }
            }
            rowStr += "]";
            rows[row] = rowStr;
        }

        return string.Join(", ", rows) + " (Solved: " + IsSolved + ")";
    }


    /// <summary>
    /// Play the move on the tile at (row,col). 
    /// </summary>
    /// <param name="row">Row of the tile to play</param>
    /// <param name="col">Col of the tile to play</param>
    /// <returns>If a legal move, returns a new Board representing the new play state, else, null</returns>
    public Board Play(int row, int col)
    {
        Board result = null;

        // check to left
        if (row > 0 && Get(row - 1, col) == Board.Empty)
        {
            result = Clone();
            result.Swap(row, col, row - 1, col);
        }

        // check to right
        if (row < _size - 1 && Get(row + 1, col) == Board.Empty)
        {
            result = Clone();
            result.Swap(row, col, row + 1, col);
        }

        // check to up
        if (col > 0 && Get(row, col - 1) == Board.Empty)
        {
            result = Clone();
            result.Swap(row, col, row, col - 1);
        }

        // check to down
        if (col < _size - 1 && Get(row, col + 1) == Board.Empty)
        {
            result = Clone();
            result.Swap(row, col, row, col + 1);
        }

        return result;
    }

}