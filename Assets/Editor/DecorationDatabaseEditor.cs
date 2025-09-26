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
    private float decorationWidth = 130;
    private float prefabWidth = 180;
    private float typeWidth = 80;
    private float biomeWidth = 50;
    private float cellSpacing = 10;

    // Background color for lines
    private Color lineBackgroundColor = new Color32(51, 51, 51, 255);   // gray



    [MenuItem("Custom Tools/Decoration Editor")]
    public static void ShowWindow()
    {
        GetWindow<DecorationDatabaseEditor>("Decoration Editor");
    }

    private void OnEnable()
    {
        ReloadDecorations();
    }

    private void ReloadDecorations()
    {
        decorations = new List<Decoration>(Resources.LoadAll<Decoration>("World/Decorations"));
        reorderableList = new ReorderableList(decorations, typeof(Decoration), true, true, false, false)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                // no title
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var decoration = decorations[index];

                // Draw background rectangle
                Rect backgroundRect = new Rect(rect.x, rect.y + 1, rect.width, rect.height - 2);
                EditorGUI.DrawRect(backgroundRect, lineBackgroundColor);

                // display the Decoration name
                Rect decorationName = new Rect(rect.x + cellSpacing, rect.y + cellSpacing / 5, prefabWidth, lineHeight);
                EditorGUI.LabelField(decorationName, decoration.name);

                EditorGUI.BeginChangeCheck();

                // display the Decoration field
                Rect prefabSlot = new Rect(rect.x + decorationWidth + cellSpacing * 2, rect.y + cellSpacing / 3, prefabWidth, lineHeight);
                decoration.Prefab = (GameObject)EditorGUI.ObjectField(prefabSlot, decoration.Prefab, typeof(GameObject), false);

                Rect typeSlot = new Rect(rect.x + decorationWidth + prefabWidth + cellSpacing * 3, rect.y + cellSpacing / 3, typeWidth, lineHeight);
                decoration.Type = (DecorationType)EditorGUI.EnumPopup(typeSlot, decoration.Type);

                //  display the Decoration BiomeSpawnChances
                float xOffset = decorationWidth + prefabWidth + typeWidth + cellSpacing * 4;
                foreach (var biomeSpawnChance in decoration.BiomeSpawnChances)
                {
                    Rect spawnChance = new Rect(rect.x + xOffset, rect.y + cellSpacing / 3, biomeWidth, lineHeight);
                    biomeSpawnChance.SpawnChance = EditorGUI.FloatField(spawnChance, biomeSpawnChance.SpawnChance);
                    xOffset += biomeWidth + cellSpacing / 2;
                }

                // If any property was changed, mark the object as dirty
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(decoration);
                }
            },
            // 
            elementHeightCallback = (int index) =>
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }
        };
    }

    private void OnGUI()
    {
        serializedObject = new SerializedObject(this);
        serializedObject.Update();

        // display the title
        GUILayout.BeginHorizontal();
        GUILayout.Space(cellSpacing * 3); // Отступ слева
        GUILayout.Label("Decoration", GUILayout.Width(decorationWidth));
        GUILayout.Space(8); // Отступ слева
        GUILayout.Label("Prefab", GUILayout.Width(prefabWidth));
        GUILayout.Space(6); // Отступ слева
        GUILayout.Label("Type", GUILayout.Width(typeWidth + cellSpacing / 2));

        // display the titles of BiomeSpawnChances
        foreach (var biomeType in System.Enum.GetValues(typeof(BiomeType)))
        {
            GUILayout.Label(biomeType.ToString(), GUILayout.Width(biomeWidth + cellSpacing / 10));
        }
        GUILayout.EndHorizontal();

        // Scrolling area for the list
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndScrollView();
    }
}