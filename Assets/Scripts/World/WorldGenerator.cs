using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



// class for diagonal-shape map generation.
public class WorldGenerator : MonoBehaviour
{   
    // map
    [Header("Terrain")]
    int _mapSize;    // length of a square side (counted in cells), in which resulting rhomb map will be inscribed
    // max _mapWidth x _mapHeight are 120 x 70
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
    [Header("Objects")]
    [SerializeField] GameObject Grid;
    Grid _gridComponent;

    // tiles
    [HideInInspector] public const float TileSize = 1; // scale of a square side(x & z) of a tile, can be serialized, if needed
    [HideInInspector] public const float TileHeightModifier = 0.05f;
    [Serializable]
    class TileHeight {
        [SerializeField] Tile _tile;
        [SerializeField] float _height;

        public Tile Tile => _tile;
        public float Height => _height;
    }
    [SerializeField] TileHeight[] _tileSet;     // tiles to spawn, divided by height levels
    Dictionary<Vector2Int, TileComponent> _tiles = new Dictionary<Vector2Int, TileComponent>(); // spawned tiles
    public Dictionary<Vector2Int, TileComponent> Tiles { get => _tiles; }
        



    float[,] GenerateNoiseMap() {
        return NoiseMap.GenerateNoiseMap(_mapSize, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);
    }

    public void Initialize()
    {
        _mapSize = _mapWidth + _mapHeight - 1;
        _gridComponent = Grid.GetComponent <Grid> ();
        _gridComponent.cellSize = new Vector3 (TileSize, 1, TileSize);
    }


    
    void SetTileSize(GameObject tile)
    {
        tile.transform.localScale = _gridComponent.cellSize;
    }

    public bool IsCellInGrid(int x, int y)
    {
        // exclude corner cells to get rhomb-like grid
        if (x + y < _mapHeight - 1 ||       // left bottom
        _mapSize + x - y < _mapWidth ||     // left top
        _mapSize - x + y < _mapWidth ||     // right bottom
        (_mapSize - 1) * 2 - x - y < _mapHeight - 1)    // right top
            return false;
        
        return true;
    }

    Tile GetTileByHeight(float height)
    {
        if (_tileSet is null) {
            Debug.LogError("TileSet for terrain generation is empty");
            return null;
        }

        for (int i = 0; i < _tileSet.Length; i++) {
            if (height <= _tileSet[i].Height) {
                return _tileSet[i].Tile;
            }
        }
        Debug.LogError($"Tile with height < {height} not found.");
        return null;
    }

    TileComponent SpawnTile(int x, int y, float height)
    {
        var cell = new Vector3Int(x, 0, y);
        var worldPos = _gridComponent.CellToWorld(cell);

        Tile tileToSpawn = GetTileByHeight(height);
        if (tileToSpawn is null) {
            Debug.LogError("There is no tile to spawn.");
            return null;
        }

        var tileSpawned = Instantiate(tileToSpawn.Prefab, worldPos, Quaternion.identity, _gridComponent.transform);
        SetTileSize(tileSpawned);

        TileComponent tileComponent = tileSpawned.GetComponent<TileComponent>();
        tileComponent.Initialize(tileToSpawn);

        return tileComponent;
    }

    public void GenerateTerrain()
    {
        for (int y = 0; y < _mapSize; y++) {
            for (int x = 0; x < _mapSize; x++) {
                if (IsCellInGrid(x, y)) {
                    float height = _noiseMap[x, y];
                    Tiles.Add(new Vector2Int(x, y), SpawnTile(x, y, height));
                }
            }
        }
        Debug.Log("Terrain created.");
    }

    bool IsValid()
    {
        foreach (var element in _tileSet) {
            if (element.Tile is null) {
                Debug.LogError("TileSet for terrain is empty. Generation cancelled.");
                return false;
            }
        }
        return true;
    }

    public void GenerateWorld()
    {
        if (!IsValid()) {
            return;
        }
        _noiseMap = GenerateNoiseMap();
        GenerateTerrain();
        GameManager.Instance.TerrainDecorator.Decorate(Tiles);

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

        corners[0] = Tiles[firstCellIndex].gameObject.transform.position;
        corners[1] = Tiles[lastCellIndex].gameObject.transform.position;

        return corners;
    }

}
