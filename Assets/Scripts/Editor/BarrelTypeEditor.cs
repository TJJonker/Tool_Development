using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor( typeof( BarrelType ) )]
public class BarrelTypeEditor : Editor {

    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propDamage;
    SerializedProperty propColor;

    private void OnEnable () {
        so = serializedObject;
        propRadius = so.FindProperty( "radius" );
        propDamage = so.FindProperty( "damage" );
        propColor = so.FindProperty( "color" );
    }

    public override void OnInspectorGUI () {
        so.Update();
        EditorGUILayout.PropertyField( propRadius );
        EditorGUILayout.PropertyField( propDamage );
        EditorGUILayout.PropertyField( propColor );
        if ( so.ApplyModifiedProperties() ) ExplosiveBarrelManager.UpdateAllBarrelColors();
    }
}
