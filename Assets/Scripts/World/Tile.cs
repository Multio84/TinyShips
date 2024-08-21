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
    public List <DecorationTypeSpawnChance> SpawnChances => _spawnChances;  // spawn chances of each decoration type to be spawned on this biome
    public override GameObject Prefab
    {
        get => _prefab;
        set => _prefab = value;
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