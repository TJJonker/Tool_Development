using UnityEditor;
using UnityEngine;

public static class Snapper {

    private const string SNAP_TEXT = "Snap Objects";

    [MenuItem( "Edit/Snap Selected GameObjects", isValidateFunction: true )]
    public static bool SnapObjectsValidate () {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem( "Edit/Snap Selected GameObjects" )]
    public static void SnapObjects () {
        foreach ( GameObject g in Selection.gameObjects ) {
            Undo.RecordObject( g.transform, SNAP_TEXT );
            g.transform.position = g.transform.position.Round();
        }
    }
}
