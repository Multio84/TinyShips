using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


/*
 * class to spawn decorations in the world
 * each tile can have 1-3 decorations
 * position of spawned decorations a being collected from "spawners"
 * - objects with empty objects in hierarchy, called "places"
 * there are spawnerSingle, double & triple - named by number of places in them
*/

public class TerrainDecorator : MonoBehaviour
{
    [SerializeField] GameObject[] _spawners;
    Decoration[] _decorations;
    GameObject _currentSpawner;



    public void Decorate(Dictionary<Vector2Int, TileComponent> tiles)
    {
        LoadDecorations();
        if (_decorations.Length < 1) Debug.Log("No decorations were loaded.");
        
        foreach (var element in tiles) {
            var tile = element.Value;
            SpawnAllDecorations(tile);
        }
    }

    void LoadDecorations()
    {   
        _decorations = ResourcesLoader.LoadAllFromResources<Decoration>("World/Decorations");
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
        Transform[] places = GetPlacesToSpawn(tile);

        foreach (var place in places) {
            var decoration = SelectDecorationOfType(tile, type);
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

    GameObject SelectDecorationOfType(TileComponent tile, DecorationType type)
    {
        List<Decoration> allDecorationsOfType = new List<Decoration>();

        // collect all decorations of type
        foreach (var decoration in _decorations) {
            if (decoration.Type == type) {
                allDecorationsOfType.Add(decoration);
            }
        }

        List<Decoration> decorationsToSpawn = new List<Decoration>();

        foreach (var decoration in allDecorationsOfType) {
            // collect decorations, that have a chance for the tile's biome
            foreach (var biomeSpawnChance in decoration.BiomeSpawnChances) {
                if (biomeSpawnChance.BiomeType == tile.BiomeType) {
                    if (ResultChance(biomeSpawnChance.SpawnChance)) {
                        decorationsToSpawn.Add(decoration);
                        break;
                    }
                }
            }
        }

        if (decorationsToSpawn.Count() < 1) return null;
        
        return GetRandomDecorationPrefab(decorationsToSpawn);
    }

    GameObject GetRandomDecorationPrefab(List<Decoration> decorations)
    {
        int randomIndex = UnityEngine.Random.Range(0, decorations.Count);
        return decorations[randomIndex].Prefab;
    }

}
