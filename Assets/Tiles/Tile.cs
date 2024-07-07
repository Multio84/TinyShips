using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    Water,
    Land,
    Reef
}

// square object, tiled on the map
public class Tile : MonoBehaviour
{
    public TileType _tileType;
    public float _height;

    public GameObject _tileObject;

    
}
