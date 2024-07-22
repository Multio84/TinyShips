using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// class for diagonal-shape map generation.
public class MapGenerator : MonoBehaviour
{
    int _mapSize;    // length of a square side (counted in cells), in which resulting rhomb map will be inscribed
    const float _tileSize = 1; // scale of a square side(x & z) of a tile, can be serialized, if needed
    public int _mapWidth;   // width length of rhomb map in cells
    public int _mapHeight;  // height length of rhomb map in cells

    public List<GameObject> _tilePrefabs = new List<GameObject>();
    public GameObject _tilePrefab;
    public GameObject _gridObject;
    private Grid _grid;
    public Tile[,] _tiles;

    public Action MapCreated;

    // void Start()
    // {
    //     Initialize();
    //     GenerateMap();
    // }

    public void Initialize()
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
        
        return tileSpawned.GetComponent<Tile>();
    }

    void SetTileSize(Tile tile)
    {
        tile._tileObject.transform.localScale = new Vector3 (_tileSize, 1, _tileSize);
    }

    public void GenerateMap()
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
        Debug.Log("Spawned a map.");
        //MapCreated?.Invoke();
    }

    // get left bottom (minX, minY) and right top (maxX, maxY) corners world positions of rhomb map
    public Vector3[] GetBoundaryCorners()
    {
        Vector3[] corners = new Vector3[2];
        Vector2Int lastCellIndex = new Vector2Int {
            x = _mapSize - _mapWidth - 1,
            y = _mapHeight
        };

        corners[0] = _tiles[0, 0]._tileObject.transform.position;
        corners[1] = _tiles[lastCellIndex.x, lastCellIndex.y]._tileObject.transform.position;

        return corners;
    }

}
