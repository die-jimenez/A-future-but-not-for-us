using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BodyPartAnimations))]
public class Editor_BodyPartAnimations: Editor
{
    SerializedProperty _hasArms;
    SerializedProperty _hasLegs;
    SerializedProperty _arms;
    SerializedProperty _legs;

    private void OnEnable()
    {
        // hook up the serialized properties
        _hasArms = serializedObject.FindProperty(nameof(BodyPartAnimations.hasArms));
        _hasLegs = serializedObject.FindProperty(nameof(BodyPartAnimations.hasLegs));
        _arms = serializedObject.FindProperty(nameof(BodyPartAnimations.arms));
        _legs = serializedObject.FindProperty(nameof(BodyPartAnimations.legsAnimator));


    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        if (_hasArms.boolValue)
        {
            EditorGUILayout.PropertyField(_arms);
        }
        if (_hasLegs.boolValue)
        {
            EditorGUILayout.PropertyField(_legs);
        }

        // Write back changed values
        // This also handles all marking dirty, saving, undo/redo etc
        serializedObject.ApplyModifiedProperties();
    }


}
