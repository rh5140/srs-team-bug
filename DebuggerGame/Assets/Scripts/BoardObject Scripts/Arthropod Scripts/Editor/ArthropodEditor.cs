using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(Arthropod))]
public class ArthropodEditor : Editor
{
    bool behaviorFoldout = true;
    List<FieldInfo> behaviorFieldInfos = new List<FieldInfo>();
    List<string> behaviorPropertyNames = new List<string>();
    List<SerializedProperty> behaviorProperties = new List<SerializedProperty>();

    private void OnEnable()
    {
        var fields = typeof(Arthropod).GetFields();
        foreach(var field in fields)
        {
            if (field.FieldType.IsSubclassOf(typeof(ArthropodBehavior)))
            {
                var property = serializedObject.FindProperty(field.Name);

                if (property == null)
                {
                    Debug.LogWarningFormat(
                        "Field `{0}` on Arthropod is an ArthropodBehavior "
                        + "but is not serialized. Make sure it is public and the class "
                        + "is serializable. ",
                        field.Name
                    );
                }
                else
                {
                    behaviorFieldInfos.Add(field);
                    behaviorPropertyNames.Add(
                        field.Name
                    );

                    behaviorProperties.Add(
                        serializedObject.FindProperty(field.Name)
                    );
                }
            }
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        behaviorFoldout = EditorGUILayout.Foldout(behaviorFoldout, "Behaviors");
        if (behaviorFoldout)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < behaviorProperties.Count; i++)
            {
                var behaviorProperty = behaviorProperties[i];
                var behavior = (ArthropodBehavior)behaviorFieldInfos[i].GetValue(target);
                
                EditorGUILayout.BeginHorizontal();

                
                EditorGUI.BeginChangeCheck();
                var newEnabled = EditorGUILayout.ToggleLeft(
                    GUIContent.none,
                    behavior.enabled,
                    GUILayout.Width(
                        new GUIStyle(GUI.skin.toggle).CalcSize(GUIContent.none).x * 2f
                    )
                );
                if(EditorGUI.EndChangeCheck()){
                    Undo.RecordObject(target, "Enabled behavior");
                    behavior.enabled = newEnabled;
                }
                

                if (behavior.enabled)
                {
                    EditorGUILayout.PropertyField(
                        behaviorProperty,
                        GUILayout.ExpandWidth(false)
                    );
                }
                else
                {
                    EditorGUILayout.LabelField(behaviorProperty.displayName);
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        DrawPropertiesExcluding(serializedObject, behaviorPropertyNames.ToArray());

        serializedObject.ApplyModifiedProperties();
    }
}
