using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



// class for diagonal-shape map generation.
public class MapGenerator : MonoBehaviour
{   
    // map generation
    [Header("Terrain")]
    int _mapSize;    // length of a square side (counted in cells), in which resulting rhomb map will be inscribed
    const float _tileSize = 1; // scale of a square side(x & z) of a tile, can be serialized, if needed
    public int _mapWidth;   // width length of rhomb map in cells
    public int _mapHeight;  // height length of rhomb map in cells

    // noise generation
    [Header("Noise")]
    float[,] _noiseMap;
    public float _noiseScale;
    public int _octaves;     // default ~ 4
    [Range (0, 1)] public float _persistance;   // default 0.5
    public float _lacunarity;    // default ~ 2
    public int _seed;
    public Vector2 _offset;


    // objects
    public List<GameObject> _tilePrefabs = new List<GameObject>();
    public GameObject _tilePrefab;
    public GameObject _gridObject;
    private Grid _grid;
    public Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
    //float _noiseScale = 3f;

    // tiles
    [Serializable]
    struct TerrainType {
        public GameObject _tilePrefab;
        public float _height;
    }

    [SerializeField]
    TerrainType[] _tileSet;

    float[,] GenerateNoiseMap() {
     // generate noiseMap
        return NoiseMap.GenerateNoiseMap(_mapSize, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);
    }

    public void Initialize()
    {
        _mapSize = _mapWidth + _mapHeight - 1;
        _grid = _gridObject.GetComponent<Grid>();
        _grid.cellSize = new Vector3 (_tileSize, 1, _tileSize);
    }
    
    void SetTileSize(GameObject tile)
    {
        tile.transform.localScale = new Vector3 (_tileSize, 1, _tileSize);
    }

    Tile SpawnTile(int x, int y, float height)
    {
        var cell = new Vector3Int(x, 0, y);
        var worldPos = _grid.CellToWorld(cell);
        GameObject tileToSpawn = null;

        for (int i = 0; i < _tileSet.Length; i++) {
            if (height <= _tileSet[i]._height) {
                tileToSpawn = _tileSet[i]._tilePrefab;
                //Debug.Log($"Spawned one of set: {tileToSpawn}, height = {height}.");
                break;
            }
        }
        
        if (tileToSpawn is null) {
            Debug.LogError("There is no tile to spawn.");
            return null;
        }

        var tileSpawned = Instantiate(tileToSpawn, worldPos, Quaternion.identity, _grid.transform);
        SetTileSize(tileSpawned);

        return tileSpawned.GetComponent<Tile>();
    }

    public bool IsCellInGrid(int x, int y)
    {
        // exclude corner cells
        if (x + y < _mapHeight - 1 ||       // left bottom
        _mapSize + x - y < _mapWidth ||     // left top
        _mapSize - x + y < _mapWidth ||     // right bottom
        (_mapSize - 1) * 2 - x - y < _mapHeight - 1)    // right top
            return false;
        
        return true;
    }

    public void GenerateTerrain()
    {
        _noiseMap = GenerateNoiseMap();

        for (int y = 0; y < _mapSize; y++) {
            for (int x = 0; x < _mapSize; x++) {
                if (IsCellInGrid(x, y)) {
                    float height = _noiseMap[x, y];
                    _tiles.Add(new Vector2Int(x, y), SpawnTile(x, y, height));
                }
            }
        }
        Debug.Log("Terrain created.");
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

        corners[0] = _tiles[firstCellIndex]._tileObject.transform.position;
        corners[1] = _tiles[lastCellIndex]._tileObject.transform.position;

        return corners;
    }

}
