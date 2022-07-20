using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Snappertool : EditorWindow {

    private const string UNDO_SNAP_TEXT = "Snap Objects";
    private enum GridType {
        Cartesian,
        Polar
    }


    SerializedObject so;

    [SerializeField] float gridSize = 1f;
    SerializedProperty propGridSize;

    [SerializeField] GridType gridType;
    SerializedProperty propGridType;

    [SerializeField] int angularDivision;
    SerializedProperty propAngularDivision;


    [MenuItem( "Tools / Snapper %&S" )]
    public static void OpenWindow () => GetWindow<Snappertool>( "Snapper" );


    private void OnEnable () {
        so = new SerializedObject( this );
        propGridSize = so.FindProperty( "gridSize" );
        propGridType = so.FindProperty( "gridType" );
        propAngularDivision = so.FindProperty( "angularDivision" );

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable () {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }



    private void OnGUI () {
        so.Update();
        EditorGUILayout.PropertyField( propGridType );

        EditorGUILayout.PropertyField( propGridSize );
        propGridSize.floatValue = Mathf.Max( .1f, propGridSize.floatValue );

        if ( gridType == GridType.Polar ) {
            EditorGUILayout.PropertyField( propAngularDivision );
            propAngularDivision.intValue = Mathf.Max( 2, propAngularDivision.intValue );
        }

        so.ApplyModifiedProperties();

        using ( new EditorGUI.DisabledScope( Selection.gameObjects.Length == 0 ) )
            if ( GUILayout.Button( "Snap Selection" ) ) Snap();
    }

    private void DuringSceneGUI ( SceneView view ) {
        if ( Event.current.type != EventType.Repaint ) return;

        Handles.zTest = CompareFunction.LessEqual;
        Handles.color = Color.gray;

        const float range = 15;
        if ( gridType == GridType.Cartesian ) DrawGridCartesian( range );
        if ( gridType == GridType.Polar ) DrawGridPolar( range );
    }

    private void DrawGridCartesian ( float range ) {
        int amountOfLines = (int) ( range * 2 / gridSize );
        if ( amountOfLines % 2 == 0 ) amountOfLines++;

        int halfPoint = amountOfLines / 2;

        for ( int i = 0; i < amountOfLines; i++ ) {
            int lineIndex = i - halfPoint;

            Vector3 pointAA = new Vector3( lineIndex * gridSize, 0, -range );
            Vector3 pointAB = new Vector3( lineIndex * gridSize, 0, +range );

            Vector3 pointBA = new Vector3( -range, 0, lineIndex * gridSize );
            Vector3 pointBB = new Vector3( +range, 0, lineIndex * gridSize );

            Handles.DrawAAPolyLine( pointAA, pointAB );
            Handles.DrawAAPolyLine( pointBA, pointBB );
        }
    }

    private void DrawGridPolar ( float range ) {

        // Drawing rings
        int amountOfRings = (int) ( range / gridSize ) + 1;
        for ( int i = 1; i < amountOfRings; i++ ) {
            Handles.DrawWireDisc( Vector3.zero, Vector3.up, i * gridSize );
        }

        // Drawing angled lines
        float angleBetweenDivisions = 360f / angularDivision;
        for ( int i = 0; i < angularDivision; i++ ) {
            var rads = Mathf.Deg2Rad * angleBetweenDivisions * i;
            var x = Mathf.Cos( rads );
            var y = Mathf.Sin( rads );
            var direction = new Vector3( x, 0, y );
            Handles.DrawAAPolyLine( Vector3.zero, direction * gridSize * (amountOfRings - 1) );
        }
    }

    private void Snap () {
        foreach ( GameObject g in Selection.gameObjects ) {
            Undo.RecordObject( g.transform, UNDO_SNAP_TEXT );
            g.transform.position = GetSnappedPosition(g.transform.position);
        }
    }

    private Vector3 GetSnappedPosition(Vector3 originalPosition ) {
        
        // Cartesian Snapping 
        if ( gridType == GridType.Cartesian ) 
            return originalPosition.Round( gridSize );

        // Polar Snapping
        if(gridType == GridType.Polar ) {

            // Ranged snapping
            var distance = originalPosition.magnitude;
            var snappedDistance = distance.Round( gridSize );

            // Angular snapping
            var rad = Mathf.Atan2( originalPosition.z, originalPosition.x );
            var angleBetweenDivisions = 360f / angularDivision;
            var snappedAngle = ( rad ).Round( angleBetweenDivisions * Mathf.Deg2Rad );

            var newPos = new Vector2( Mathf.Cos( snappedAngle ), Mathf.Sin( snappedAngle ) ) * snappedDistance;
            return new Vector3( newPos.x, originalPosition.y, newPos.y );
        }

        return default;
    }
}
