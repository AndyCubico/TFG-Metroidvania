using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Attribute
public class ShowIfAttribute : PropertyAttribute
{
    public string conditionFieldName;
    public object expectedValue;

    public ShowIfAttribute(string conditionFieldName, object expectedValue)
    {
        this.conditionFieldName = conditionFieldName;
        this.expectedValue = expectedValue;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return ShouldShow(property) ? EditorGUI.GetPropertyHeight(property, label, true) : -EditorGUIUtility.standardVerticalSpacing;
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionFieldName);

        if (conditionProperty == null)
        {
            Debug.LogWarning($"Propery not found {showIf.conditionFieldName}");
            return true;
        }

        switch (conditionProperty.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return conditionProperty.boolValue.Equals(showIf.expectedValue);
            case SerializedPropertyType.Enum:
                return conditionProperty.enumValueIndex.Equals(System.Convert.ToInt32(showIf.expectedValue));
            default:
                Debug.LogWarning($"ShowIf doesn't support {conditionProperty.propertyType} yet.");
                return true;
        }
    }
}
#endif


