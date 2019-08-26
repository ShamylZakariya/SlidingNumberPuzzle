using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class Tile : MonoBehaviour
{
    TextMeshPro _label;
    int _index = -1;

    public int Index {
        get {
            return _index;
        }
        set {
            _index = value;
            _label.text = _index.ToString();
        }
    }

    public int Row = 0;
    public int Col = 0;

    void Awake()
    {
        _label = GetComponentInChildren<TextMeshPro>();
    }
}
