using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;



public class DecorationDatabaseEditor : EditorWindow
{
    private List<Decoration> decorations;
    private SerializedObject serializedObject;
    private ReorderableList reorderableList;
    private Vector2 scrollPosition;

    // element's sizes and spacings
    private float lineHeight = EditorGUIUtility.singleLineHeight;
    private float prefabWidth = 150;
    private float typeWidth = 80;
    private float biomeWidth = 50;
    private float headerHeight = EditorGUIUtility.singleLineHeight + 4;
    private float cellSpacing = 10;



    [MenuItem("Window/Decoration Editor")]
    public static void ShowWindow()
    {
        GetWindow<DecorationDatabaseEditor>("Decoration Editor");
    }

    private void OnEnable()
    {
        decorations = new List<Decoration>(Resources.LoadAll<Decoration>("World/Decorations"));

        // creating SerializedObject for serialized data manipulations
        serializedObject = new SerializedObject(this);

        // creating ReorderableList for displaying and editing Decorations list
        reorderableList = new ReorderableList(decorations, typeof(Decoration), true, true, false, false)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                // no title
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var decoration = decorations[index];

                // display the Decoration fields
                Rect prefabSlot = new Rect(rect.x, rect.y + cellSpacing / 3, prefabWidth, lineHeight);
                decoration.Prefab = (GameObject)EditorGUI.ObjectField(prefabSlot, decoration.Prefab, typeof(GameObject), false);

                Rect typeSlot = new Rect(rect.x + prefabWidth + cellSpacing, rect.y + cellSpacing / 3, typeWidth, lineHeight);
                decoration.Type = (DecorationType)EditorGUI.EnumPopup(typeSlot, decoration.Type);

                //  display the Decoration BiomeSpawnChances
                float xOffset = prefabWidth + typeWidth + cellSpacing * 2;
                foreach (var biomeSpawnChance in decoration.BiomeSpawnChances)
                {
                    Rect spawnChance = new Rect(rect.x + xOffset, rect.y + cellSpacing / 3, biomeWidth, lineHeight);
                    biomeSpawnChance.SpawnChance = EditorGUI.FloatField(spawnChance, biomeSpawnChance.SpawnChance);
                    xOffset += biomeWidth + cellSpacing / 2;
                }
            },
            elementHeightCallback = (int index) =>
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }
        };
    }

    private void OnGUI()
    {
        // display the title
        GUILayout.BeginHorizontal();
        GUILayout.Space(20); // Отступ слева
        GUILayout.Label("Prefab", GUILayout.Width(prefabWidth));
        GUILayout.Space(8); // Отступ слева
        GUILayout.Label("Type", GUILayout.Width(typeWidth + cellSpacing / 3));

        // display the titles of BiomeSpawnChances
        foreach (var biomeType in System.Enum.GetValues(typeof(BiomeType)))
        {
            GUILayout.Label(biomeType.ToString(), GUILayout.Width(biomeWidth + cellSpacing / 10));
        }
        GUILayout.EndHorizontal();

        // Scrolling area for the list
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        GUILayout.EndScrollView();
    }
}