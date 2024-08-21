using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;



public class DecorationDatabaseEditor : EditorWindow
{
    private List<Decoration> decorations;
    private SerializedObject serializedObject;
    private ReorderableList reorderableList;

    // size params
    float lineHeight = EditorGUIUtility.singleLineHeight;



    [MenuItem("Window/Decoration Editor")]
    public static void ShowWindow()
    {
        GetWindow<DecorationDatabaseEditor>("Decoration Editor");
    }

    private void OnEnable()
    {
        // Загружаем все объекты Decoration из ресурсов
        decorations = new List<Decoration>(Resources.LoadAll<Decoration>("World/Decorations"));

        // Создаем SerializedObject для работы с сериализованными данными
        serializedObject = new SerializedObject(this);

        // Создаем ReorderableList для отображения и редактирования списка Decoration
        reorderableList = new ReorderableList(decorations, typeof(Decoration), true, true, false, false)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Decorations");
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var decoration = decorations[index];
                rect.y += 2;

                // Отображаем поля для редактирования Decoration
                //Rect prefabLabel = new Rect(rect.x, rect.y - lineHeight/6, 100, lineHeight + 5);
                //EditorGUI.LabelField(prefabLabel, "Prefab");

                Rect prefabSlot = new Rect(rect.x, rect.y + lineHeight, 150, lineHeight);
                decoration.Prefab = (GameObject)EditorGUI.ObjectField(prefabSlot, decoration.Prefab, typeof(GameObject), false);

                //Rect typeLabel = new Rect(rect.x + 160, rect.y, 100, lineHeight);
                //EditorGUI.LabelField(typeLabel, "Type");

                Rect typeSlot = new Rect(rect.x + 160, rect.y + lineHeight, 80, lineHeight);
                decoration.Type = (DecorationType)EditorGUI.EnumPopup(typeSlot, decoration.Type);


                // Отображаем BiomeSpawnChances
                float xOffset = 250;
                foreach (var biomeSpawnChance in decoration.BiomeSpawnChances)
                {
                    Rect biomeLabel = new Rect(rect.x + xOffset, rect.y, 50, lineHeight);
                    EditorGUI.LabelField(biomeLabel, biomeSpawnChance.BiomeType.ToString());

                    Rect spawnChance = new Rect(rect.x + xOffset, rect.y + lineHeight, 50, lineHeight);
                    biomeSpawnChance.SpawnChance = EditorGUI.FloatField(spawnChance, biomeSpawnChance.SpawnChance);
                    xOffset += 60;
                }
            },
            elementHeightCallback = (int index) =>
            {
                return EditorGUIUtility.singleLineHeight * 2 + 4;
            }
        };
    }

    private void OnGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}