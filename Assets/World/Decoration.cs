using System.Collections.Generic;
using UnityEngine;


public enum DecorationType
{
    Tree,   // trees & bushes
    Grass,  // grass, flowers, and other small flora
    Stone   // stones and other small objects
}

// class to set spawn chance of decoration on a biome
[System.Serializable]
public class BiomeSpawnChance
{
    public BiomeType BiomeType;
    public float SpawnChance;   
}

[CreateAssetMenu(fileName = "Decoration", menuName = "Terrain/Decoration")]
public class Decoration : TerrainObject
{
    [SerializeField] private DecorationType _decorationType;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private List <BiomeSpawnChance> _spawnChances = new List <BiomeSpawnChance>();

    public DecorationType DecorationType => _decorationType;
    public override GameObject Prefab => _prefab;
    public List <BiomeSpawnChance> SpawnChances => _spawnChances;

    private void OnEnable()
    {
        InitAllBiomesChance();
    }

    // inits spawn chance on each biom as 0, if it's not set yet
    private void InitAllBiomesChance()
    {
        foreach (BiomeType biomeType in System.Enum.GetValues(typeof(BiomeType)))
        {
            bool chanceIsSet = false;
            foreach (var spawnChance in _spawnChances) {
                if (spawnChance.BiomeType == biomeType) {
                    chanceIsSet = true;
                    break;
                }
            }

            if (!chanceIsSet) {
                _spawnChances.Add(new BiomeSpawnChance { BiomeType = biomeType, SpawnChance = 0f });
            }
        }
    }

}
