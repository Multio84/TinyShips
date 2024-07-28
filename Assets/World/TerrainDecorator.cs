using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    [SerializeField] List<GameObject> _sandDecorations;
    [SerializeField] List<GameObject> _grass1Decorations;
    [SerializeField] List<GameObject> _grass3Decorations;



    GameObject PlaceSpawner(Tile tile)
    {
        var tilePos = tile.transform.position;
        var tileHeight = tile.Height;
        var spawner = Instantiate(_spawnerTriplePrefab, tile.transform);
        spawner.transform.position += new Vector3(0, tileHeight, 0);

        return spawner;
    }

    GameObject GetRandomDecorationPrefab(List<GameObject> decorations)
    {
        int randomIndex = Random.Range(0, decorations.Count);
        return decorations[randomIndex];
    }


    void SpawnDecoration(List<GameObject> decorations, Tile tile)
    {
        // Vector3 tilePos = tile.gameObject.transform.position;
        // float tileHeight = tile.Height;
        // Vector3 decorationPos = new Vector3(tilePos.x, tilePos.y + tileHeight, tilePos.z);

        // float randomYRot = Random.Range(0, 360f);
        // Quaternion decorationRot = Quaternion.Euler(0, randomYRot, 0);


        GameObject spawner = PlaceSpawner(tile);
        GameObject[] places = spawner.GetComponent<DecorationSpawnerTriple>().GetPlaces();

        GameObject decoration;

        foreach (var place in places) {
            var decorationPrefab = GetRandomDecorationPrefab(decorations);
            decoration = Instantiate(decorationPrefab, place.transform);
            //var tileObject = tile.gameObject;
            decoration.transform.SetParent(tile.gameObject.transform);
            Debug.Log($"Spawned {decoration} in pos = {decoration.transform.localPosition} with rot = {decoration.transform.localRotation}");
        }

        //Destroy(spawner);


        // int randomIndex = Random.Range(0, decorations.Count);
        // var decorationPrefab = decorations[randomIndex];
        // var decoration = Instantiate(decorationPrefab, decorationPos, decorationRot);
        // decoration = transform.parent.gameObject;


    }

    
    public void SpawnDecorations(Dictionary<Vector2Int, Tile> _tiles)
    {
        foreach(var tileEntry in _tiles) {
            //var tilePos = tileEntry.Key;
            var tile = tileEntry.Value;

            if (tile.BiomType == TileBiomType.Grass1) {
                SpawnDecoration(_grass1Decorations, tile);
            }
            
        }

    }
}
