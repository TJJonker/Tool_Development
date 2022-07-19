using UnityEditor;
using UnityEngine;

public class Snappertool : EditorWindow {

    private const string UNDO_SNAP_TEXT = "Snap Objects";

    SerializedObject so;
    public float gridSize = 1f;
    SerializedProperty propGridSize;



    [MenuItem( "Tools / Snapper %&S" )]
    public static void OpenWindow () => GetWindow<Snappertool>( "Snapper" );


    private void OnEnable () {
        so = new SerializedObject( this );
        propGridSize = so.FindProperty( "gridSize" );

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable () {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI () {
        so.Update();
        EditorGUILayout.PropertyField( propGridSize );
        so.ApplyModifiedProperties();

        using ( new EditorGUI.DisabledScope( Selection.gameObjects.Length == 0 ) )
            if ( GUILayout.Button( "Snap Selection" ) ) Snap();
    }

    private void DuringSceneGUI(SceneView view ) {

    }

    private void Snap () {
        foreach ( GameObject g in Selection.gameObjects ) {
            Undo.RecordObject( g.transform, UNDO_SNAP_TEXT );
            g.transform.position = g.transform.position.Round( gridSize );
        }
    }
}
