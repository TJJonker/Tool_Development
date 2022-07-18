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

    public static Vector3 Round ( this Vector3 v ) {
        v.x = Mathf.Round( v.x );
        v.y = Mathf.Round( v.y );
        v.z = Mathf.Round( v.z );
        return v;
    }
}
