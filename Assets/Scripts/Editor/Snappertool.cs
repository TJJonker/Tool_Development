using System;
using UnityEditor;
using UnityEngine;

public class Snappertool : EditorWindow {

    private const string UNDO_SNAP_TEXT = "Snap Objects";

    [MenuItem( "Tools / Snapper %&S" )]
    public static void OpenWindow () => GetWindow<Snappertool>( "Snapper" );

    private void OnGUI () {
        if ( GUILayout.Button( "Snap Selection" ) ) Snap();
    }

    private void Snap () {
        foreach ( GameObject g in Selection.gameObjects ) {
            Undo.RecordObject( g.transform, UNDO_SNAP_TEXT );
            g.transform.position = g.transform.position.Round();
        }
    }
}
