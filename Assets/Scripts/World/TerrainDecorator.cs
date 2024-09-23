using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


/*
 * class to spawn decorations in the world
 * each tile can have 1-4 decorations
 * positions of spawned decorations a being collected from "spawners"
 * - objects with empty objects in hierarchy, called "places"
 * there are spawnerSingle, double, triple & quadro - named by number of places in them
*/

public class TerrainDecorator : MonoBehaviour
{
    [SerializeField] GameObject[] _spawners;
    Decoration[] _decorations;
    GameObject _currentSpawner;



    public void Decorate(Dictionary<Vector2Int, TileComponent> tiles)
    {
        LoadDecorations();
        if (_decorations.Length == 0) {
            Debug.Log("No decorations were loaded.");
            return;
        }
        
        foreach (var element in tiles) {
            var tile = element.Value;
            SpawnAllDecorations(tile);
        }
    }

    void LoadDecorations()
    {   
        _decorations = Resources.LoadAll<Decoration>("World/Decorations");
    }

    void SpawnAllDecorations(TileComponent tile)
    {
        foreach (var element in tile.SpawnChances) {
            if (ResultChance(element.SpawnChance)) {
                SpawnDecorationsOfType(tile, element.DecorationType);
            }
        }
    }

    bool ResultChance(float chance)
    {
        if (chance == 0) return false;
        if (chance < 0 || chance > 1) 
            throw new ArgumentOutOfRangeException(nameof(chance), "Chance must be between 0 and 1.");      

        var randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= chance) return true;

        return false;
    }

    void SpawnDecorationsOfType(TileComponent tile, DecorationType type)
    {
        
        var decorationsToSpawn = CollectDecorationsOfType(tile, type);
        if (decorationsToSpawn is null) { return; }

        Transform[] places = GetPlacesToSpawn(tile);

        foreach (var place in places) {
            var decoration = GetRandomDecorationPrefab(decorationsToSpawn);
            if (decoration is null) {
                Destroy(_currentSpawner);
                continue;
            }

            var decorationSpawned = Instantiate(decoration, place);
            decorationSpawned.transform.SetParent(tile.transform);

            Destroy(_currentSpawner);
            //Debug.Log($"Spawned {decoration} on {tile.BiomeType} in pos ({tile.transform.position})");
        }
    }
    
    Transform[] GetPlacesToSpawn(TileComponent tile)
    {
        var spawnerComponent = PlaceSpawner(tile).GetComponent<DecorationSpawner>();
        Transform[] placesToSpawn = spawnerComponent.GetPlacesTransform();

        return placesToSpawn;
    }

    GameObject PlaceSpawner(TileComponent tile)
    {
        var spawnerPrefab = SelectSpawnerPrefab();
        var tileHeight = tile.Height;

        _currentSpawner = Instantiate(spawnerPrefab, tile.transform);
        _currentSpawner.transform.position += new Vector3(0, tileHeight, 0);

        return _currentSpawner;
    }

    GameObject SelectSpawnerPrefab()
    {
        int randomIndex = UnityEngine.Random.Range(0, _spawners.Length);
        return _spawners[randomIndex];
    }

    List<Decoration> CollectDecorationsOfType(TileComponent tile, DecorationType type)
    {
        List<Decoration> allDecorationsOfType = new List<Decoration>();
                
        foreach (var decoration in _decorations) {
            // collect all decorations of type, that have prefabs assigned
            if (decoration.Type == type && decoration.Prefab != null) {
                allDecorationsOfType.Add(decoration);
            }
        }
        if (allDecorationsOfType.Count() == 0) return null;

        List<Decoration> decorationsToSpawn = new List<Decoration>();

        foreach (var decoration in allDecorationsOfType)
        {
            // collect decorations, which spawn chance for the tile's biome came true
            foreach (var biomeSpawnChance in decoration.BiomeSpawnChances) {
                if (biomeSpawnChance.BiomeType == tile.BiomeType) {
                    if (ResultChance(biomeSpawnChance.SpawnChance)) {
                        decorationsToSpawn.Add(decoration);
                        break;
                    }
                }
            }
        }

        if (decorationsToSpawn.Count() == 0) { return null; }

        return decorationsToSpawn;
    }

    GameObject GetRandomDecorationPrefab(List<Decoration> decorations)
    {
        int randomIndex = UnityEngine.Random.Range(0, decorations.Count);
        return decorations[randomIndex].Prefab;
    }

}
