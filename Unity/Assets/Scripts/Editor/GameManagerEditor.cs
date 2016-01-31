using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditorInternal;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private ReorderableList listSlidesIntro;

    private void OnEnable()
    {
        listSlidesIntro = new ReorderableList(serializedObject,
                serializedObject.FindProperty("sprIntroShots"),
                true, true, true, true);

        listSlidesIntro.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) => {
        var element = listSlidesIntro.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        
    };
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        serializedObject.Update();
        listSlidesIntro.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}