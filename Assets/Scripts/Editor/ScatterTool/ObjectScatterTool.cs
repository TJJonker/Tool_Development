using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectScatterTool : EditorWindow {

    [MenuItem( "Tools / Jonkos / Object Scatter Tool %&A" )]
    public static void OpenWindow () => GetWindow<ObjectScatterTool>( "Object Scatter" );


    private const string RANGE_SAVE_STRING = "OBJECT_SCATTER_TOOL_RANGE";
    private const string AMOUNT_SAVE_STRING = "OBJECT_SCATTER_TOOL_AMOUNT";

    private const int MINIMUM_RANGE_VALUE = 1;
    private const int MAXIMUM_RANGE_VALUE = 30;
    private const int MINIMUM_AMOUNT_VALUE = 1;
    private const int MAXIMUM_AMOUNT_VALUE = 100;

    SerializedObject so;
    SerializedProperty propRange;
    [SerializeField] private float range;
    SerializedProperty propAmount;
    [SerializeField] private int amount;
    SerializedProperty propPrefab;
    [SerializeField] private GameObject prefab;

    private Vector2[] generatedPoints;


    private void OnEnable () {
        so = new SerializedObject( this );
        propRange = so.FindProperty( "range" );
        propAmount = so.FindProperty( "amount" );
        propPrefab = so.FindProperty( "prefab" );

        range = EditorPrefs.GetFloat( RANGE_SAVE_STRING, MINIMUM_RANGE_VALUE );
        amount = EditorPrefs.GetInt( AMOUNT_SAVE_STRING, MINIMUM_AMOUNT_VALUE );

        GenerateRandomPoints( amount );

        SceneView.duringSceneGui += DuringSceneGUI;
    }


    private void OnDisable () {

        EditorPrefs.SetFloat( RANGE_SAVE_STRING, range );
        EditorPrefs.SetInt( AMOUNT_SAVE_STRING, amount );

        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI () {
        so.Update();

        EditorGUILayout.PropertyField( propRange );
        propRange.floatValue = propRange.floatValue.Between( MINIMUM_RANGE_VALUE, MAXIMUM_RANGE_VALUE );

        EditorGUILayout.PropertyField( propAmount );
        propAmount.intValue = propAmount.intValue.Between( MINIMUM_AMOUNT_VALUE, MAXIMUM_AMOUNT_VALUE );

        EditorGUILayout.PropertyField( propPrefab );

        if ( so.ApplyModifiedProperties() ) {
            GenerateRandomPoints( amount );
            SceneView.RepaintAll();
        }

        if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 ) {
            GUI.FocusControl( null );
            Repaint();
        }
    }



    private void DuringSceneGUI ( SceneView sceneView ) {

        // Calculate hipoint and tangent space
        var cameraTransform = sceneView.camera.transform;
        Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
        Physics.Raycast( ray, out RaycastHit hit );

        var tangent = Vector3.Cross( hit.normal, cameraTransform.up ).normalized;
        var biTangent = Vector3.Cross( hit.normal, tangent );



        bool ctrlHeld = ( Event.current.modifiers & EventModifiers.Control ) != 0;
        if ( Event.current.type == EventType.ScrollWheel && ctrlHeld ) {
            float scrollDir = Mathf.Sign( Event.current.delta.y );

            so.Update();
            propRange.floatValue *= 1f + scrollDir * .075f;
            so.ApplyModifiedProperties();
            Repaint();
            Event.current.Use();
        }


        List<Pose> rayCastHits = new List<Pose>();
        foreach ( Vector2 p in generatedPoints ) {
            if ( Physics.Raycast( GetTangentRay( p ), out RaycastHit h ) ) {

                Quaternion randomRotation = Quaternion.Euler( 0f, 0f, Random.value * 360 );
                Quaternion rotation = Quaternion.LookRotation( h.normal ) * ( randomRotation * Quaternion.Euler( 90f, 0f, 0f ) );

                Pose pose = new Pose( h.point, rotation );
                rayCastHits.Add( pose );
            }
        }

        if ( ctrlHeld && Event.current.type == EventType.MouseDown )
            TrySpawningObjects( rayCastHits );


        if ( Event.current.type == EventType.MouseMove ) sceneView.Repaint();
        if ( Event.current.type == EventType.Repaint ) {

            #region Draw origin point and Tangent Space Gizmo

            Handles.zTest = CompareFunction.LessEqual;
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine( 5f, hit.point, hit.point + hit.normal );
            Handles.color = Color.red;
            Handles.DrawAAPolyLine( 5f, hit.point, hit.point + tangent );
            Handles.color = Color.green;
            Handles.DrawAAPolyLine( 5f, hit.point, hit.point + biTangent );
            #endregion

            #region Draw detailed circle which adapts to the surface
            const int circleDetail = 64;
            Vector3[] circlePosition = new Vector3[ circleDetail ];
            for ( int i = 0; i < circleDetail; i++ ) {
                // Split the circle up in equal parts/angles
                var p = 360f / circleDetail * i;
                var radP = p * Mathf.Deg2Rad;
                // Calculate the positions from the calculated angle
                Ray r = GetTangentRay( new Vector2( Mathf.Cos( radP ), Mathf.Sin( radP ) ) );
                // Save position in an array
                if ( Physics.Raycast( r, out RaycastHit h ) ) circlePosition[ i ] = h.point;
                else circlePosition[ i ] = r.origin;
            }

            Handles.color = Color.blue;
            Handles.DrawAAPolyLine( circlePosition );
            #endregion

            #region Draw generated points and their normals
            Handles.color = Color.white;

            Mesh mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            Material material = prefab.GetComponent<MeshRenderer>().sharedMaterial;
            material.SetPass( 0 );

            foreach ( Pose hp in rayCastHits ) {
                // Handles.DrawAAPolyLine( hp.point, hp.point + hp.normal );
                // Handles.DrawSolidDisc( hp.point, hp.normal, .1f );
                Graphics.DrawMeshNow( mesh, hp.position, hp.rotation );
            }
            #endregion
        }

        Ray GetTangentRay ( Vector2 pointPosition ) {
            var originalPos = hit.point + ( tangent * pointPosition.x + biTangent * pointPosition.y ) * range;
            return new Ray( originalPos + hit.normal * 3, -hit.normal );
        }
    }

    private void TrySpawningObjects ( List<Pose> hitpts ) {
        if ( prefab == null ) return;
        foreach ( Pose p in hitpts ) {
            GameObject spawnedObject = (GameObject) PrefabUtility.InstantiatePrefab( prefab );
            Undo.RegisterCreatedObjectUndo( spawnedObject, "Spawn Objects" );

            spawnedObject.transform.position = p.position;
            spawnedObject.transform.rotation = p.rotation;
        }
        GenerateRandomPoints( amount );
    }

    private void GenerateRandomPoints ( int amountOfPoints ) {
        generatedPoints = new Vector2[ amountOfPoints ];
        for ( int i = 0; i < amountOfPoints; i++ ) {
            generatedPoints[ i ] = Random.insideUnitCircle;
        }
    }

}
