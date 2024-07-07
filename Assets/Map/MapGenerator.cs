using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

//using System.Numerics;
using UnityEngine;

// class for diagonal-shape map generation.
public class MapGenerator : MonoBehaviour
{
    int _mapSize;    // length of a square side (counted in cells), in which resulting rhomb map will be inscribed
    const float _tileSize = 1; // scale of a square side(x & z) of a tile, can be serialized, if needed
    public int _mapWidth;
    public int _mapHeight;

    public List<GameObject> _tilePrefabs = new List<GameObject>();
    public GameObject _tilePrefab;
    public GameObject _gridObject;
    private Grid _grid;
    public Tile[,] _tiles;



    void Start()
    {
        Initialize();
        GenerateMap();
    }

    void Initialize()
    {
        _mapSize = _mapWidth + _mapHeight - 1;
        _tiles = new Tile[_mapSize, _mapSize];
        _grid = _gridObject.GetComponent<Grid>();
        _grid.cellSize = new Vector3 (_tileSize, 1, _tileSize);
    }

    Tile SpawnTile(int x, int y)
    {
        var cell = new Vector3Int(x, 0, y);
        var worldPos = _grid.CellToWorld(cell);

        GameObject tile = _tilePrefabs[0];
        var tileSpawned = Instantiate(tile, worldPos, Quaternion.identity, _grid.transform);
        Debug.Log($"Spawned tile in ({x}, {y})");

        return tileSpawned.GetComponent<Tile>();
    }

    void SetTileSize(Tile tile)
    {
        tile._tileObject.transform.localScale = new Vector3 (_tileSize, 1, _tileSize);
    }

    void GenerateMap()
    {
        for (int y = 0; y < _mapSize; y++) {
            for (int x = 0; x < _mapSize; x++) {
                // exclude corner cells
                if (x + y < _mapHeight - 1 ||       // left bottom
                _mapSize + x - y < _mapWidth ||     // left top
                _mapSize - x + y < _mapWidth ||     // right bottom
                (_mapSize - 1) * 2 - x - y < _mapHeight - 1)    // right top
                continue;                

                 _tiles[x, y] = SpawnTile(x, y);
                 SetTileSize(_tiles[x, y]);
            }
        }
    }










}
