using System.Collections.Generic;
using UnityEngine;

/*
 * class to spawn decorations in the world
 * each tile can have 1-3 decorations
 * position of spawned decorations a being collected from "spawners"
 * - objects with empty objects in hierarchy, called "places"
 * there are spawnerSingle, double & triple - named by number of places in them
*/

public class TerrainDecorator : MonoBehaviour
{
    public GameObject _spawnerTriplePrefab;
    


    // [SerializeField] List<GameObject> _sandDecorations;
    // [SerializeField] List<GameObject> _grass1Decorations;
    // [SerializeField] List<GameObject> _grass3Decorations;

    // private List<Decoration> _decorations;
    


    // // void Start()
    // // {
    // //     LoadDecorations();
    // // }

    // void LoadDecorations()
    // {
    //     _decorations = new List<Decoration>(Resources.LoadAll<Decoration>(""));
    // }

    // public List<Decoration> GetAllDecorations()
    // {
    //     return _decorations;
    // }







    /*
    вероятность появления деревьев:

    У каждого дерева есть вероятность появления в том или ином биоме
    У каждой клетки есть вероятность появления дерева в определённом количестве: 0, 1, 2 или 3


    */

    // decides how many decorations will be spawned, based on decorations
    // GameObject SelectSpawner(float chance)
    // {
        
    // }


    // GameObject PlaceSpawner(Tile tile)
    // {
    //     var tileHeight = tile.Height;
    //     var spawner = Instantiate(_spawnerTriplePrefab, tile.transform);
    //     spawner.transform.position += new Vector3(0, tileHeight, 0);

    //     return spawner;
    // }

    // GameObject GetRandomDecorationPrefab(List<GameObject> decorations)
    // {
    //     int randomIndex = Random.Range(0, decorations.Count);
    //     return decorations[randomIndex];
    // }


    // void SpawnDecoration(List<GameObject> decorations, Tile tile)
    // {
    //     GameObject spawner = PlaceSpawner(tile);
    //     GameObject[] placesToSpawn = spawner.GetComponent<DecorationSpawnerTriple>().GetPlaces();
    //     List<Decoration> decorations = GetAllDecorations();

    //     foreach (var place in placesToSpawn) {
    //         var decorationPrefab = GetRandomDecorationPrefab(decorations);
    //         decoration = Instantiate(decorationPrefab, place.transform);
    //         //var tileObject = tile.gameObject;
    //         decoration.transform.SetParent(tile.gameObject.transform);
    //         Debug.Log($"Spawned {decoration} in pos = {decoration.transform.localPosition} with rot = {decoration.transform.localRotation}");
    //     }

    //     //Destroy(spawner);
    // }

    
    // public void SpawnDecorations(Dictionary<Vector2Int, Tile> _tiles)
    // {
    //     LoadDecorations();

    //     foreach(var tileEntry in _tiles) {
    //         //var tilePos = tileEntry.Key;
    //         var tile = tileEntry.Value;

    //         if (tile.BiomeType == TileBiomeType.Grass1) {
    //             SpawnDecoration(_grass1Decorations, tile);
    //         }
            
    //     }

    // }
}
