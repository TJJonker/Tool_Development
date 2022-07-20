using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

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
        Handles.zTest = CompareFunction.LessEqual;
        Handles.color = Color.gray;

        const float range = 15; 
        int amountOfLines = (int)( range * 2 / gridSize);
        if ( amountOfLines % 2 == 0 ) amountOfLines++;

        int halfPoint = amountOfLines / 2;

        for(int i = 0; i < amountOfLines; i++ ) {
            int lineIndex = i - halfPoint;

            Vector3 pointAA = new Vector3( lineIndex * gridSize, 0, -range );
            Vector3 pointAB = new Vector3( lineIndex * gridSize, 0, +range );
            
            Vector3 pointBA = new Vector3( -range, 0, lineIndex * gridSize );
            Vector3 pointBB = new Vector3( +range, 0, lineIndex * gridSize );

            

            Handles.DrawAAPolyLine( pointAA, pointAB );
            Handles.DrawAAPolyLine( pointBA, pointBB );
        }
    }

    private void Snap () {
        foreach ( GameObject g in Selection.gameObjects ) {
            Undo.RecordObject( g.transform, UNDO_SNAP_TEXT );
            g.transform.position = g.transform.position.Round( gridSize );
        }
    }
}
