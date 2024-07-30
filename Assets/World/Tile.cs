using System.Collections.Generic;
using UnityEngine;


public enum TileType {
    Water,
    Land,
    Reef
}

// class to set spawn chance of the set decoration type
[System.Serializable]
public class DecorationTypeSpawnChance
{
    public DecorationType DecorationType;
    public float SpawnChance;   
}

// square object, spawned in the world to create terrain
[CreateAssetMenu(fileName = "Tile", menuName = "Terrain/Tile")]
public class Tile : TerrainObject
{
    [SerializeField] private TileType _type;
    [SerializeField] private BiomeType _biomeType;
    [SerializeField] private List <DecorationTypeSpawnChance> _spawnChances = new List <DecorationTypeSpawnChance>();
    [SerializeField] private GameObject _prefab;

    public TileType Type => _type;
    public BiomeType BiomeType => _biomeType;
    public List <DecorationTypeSpawnChance> SpawnChances => _spawnChances;
    public override GameObject Prefab => _prefab;
        
    // tile X & Z size
    public float Size { get => WorldGenerator._tileSize; }

    // tile Y size
    const float TileHeightModifier = 0.05f;
    public float Height {
        get {
            if (_prefab is null) {
                Debug.Log("There's no tile object assigned to Tile");
                return 0;
            }
            else {
                return _prefab.transform.localScale.y * TileHeightModifier;
            }
        }
    }

    
    private void OnEnable()
    {
        InitAllDecorationsChance();
    }

    // inits spawn chance for each type of decoration as 0, if it's not set yet
    private void InitAllDecorationsChance()
    {
        foreach (DecorationType decorationType in System.Enum.GetValues(typeof(DecorationType)))
        {
            bool chanceIsSet = false;
            foreach (var spawnChance in _spawnChances) {
                if (spawnChance.DecorationType == decorationType) {
                    chanceIsSet = true;
                    break;
                }
            }

            if (!chanceIsSet) {
                _spawnChances.Add(new DecorationTypeSpawnChance { DecorationType = decorationType, SpawnChance = 0f });
            }
        }
    }

}