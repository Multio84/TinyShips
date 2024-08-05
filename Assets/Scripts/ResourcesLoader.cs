using UnityEngine;


public static class ResourcesLoader
{
    public static T[] LoadAllFromResources<T>(string folderPath) where T : ScriptableObject
    {
        return Resources.LoadAll<T>(folderPath);
    }
}