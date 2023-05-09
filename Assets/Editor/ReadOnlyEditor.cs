using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Cinemachine;

#region ReadOnly Attribute
/*
    Thanks to It3ration for this argueably garbage solution used for making a public variable read only.
    https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
*/
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endregion

public class ReadOnlyEditor : MonoBehaviour
{
    
}
