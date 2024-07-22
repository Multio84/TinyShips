using System;
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
    public Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();




    public void Initialize()
    {
        _mapSize = _mapWidth + _mapHeight - 1;
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
                
                Vector2Int tileCoords = new Vector2Int(x, y);
                _tiles.Add(new Vector2Int(x, y), SpawnTile(x, y));
                SetTileSize(_tiles[tileCoords]);
            }
        }
        Debug.Log("Spawned a map.");
    }

    // get left bottom (minX, minY) and right top (maxX, maxY) corners world positions of rhomb map
    public Vector3[] GetBoundaryCorners()
    {
        Vector3[] corners = new Vector3[2];
        
        Vector2Int firstCellIndex = new Vector2Int {
            x = _mapHeight - 1,
            y = 0
        };
        Vector2Int lastCellIndex = new Vector2Int {
            x = _mapSize - _mapHeight,
            y = _mapSize - 1
        };

        if (_tiles is null) {
            Debug.Log("There are no tiles yet!");
            return null;
        }
        corners[0] = _tiles[firstCellIndex]._tileObject.transform.position;
        corners[1] = _tiles[lastCellIndex]._tileObject.transform.position;

        return corners;
    }

}
