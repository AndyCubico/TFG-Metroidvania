using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Attribute
public class ShowIfAttribute : PropertyAttribute
{
    public string ConditionFieldName;

    public ShowIfAttribute(string conditionFieldName)
    {
        ConditionFieldName = conditionFieldName;
    }
}

#if UNITY_EDITOR
// PropertyDrawer, only on editor
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.ConditionFieldName);

        if (conditionProperty != null && conditionProperty.boolValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.ConditionFieldName);

        if (conditionProperty != null && conditionProperty.boolValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        return -EditorGUIUtility.standardVerticalSpacing; // Hide the property
    }
}
#endif

