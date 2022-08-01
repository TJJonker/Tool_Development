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

    private Vector2[] generatedPoints;
    private Vector3[] positions;


    private void OnEnable () {
        so = new SerializedObject( this );
        propRange = so.FindProperty( "range" );
        propAmount = so.FindProperty( "amount" );

        range = EditorPrefs.GetFloat( RANGE_SAVE_STRING, MINIMUM_RANGE_VALUE );
        amount = EditorPrefs.GetInt( AMOUNT_SAVE_STRING, MINIMUM_AMOUNT_VALUE );

        GenerateRandomPoints();

        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable () {

        EditorPrefs.SetFloat( RANGE_SAVE_STRING, range );
        EditorPrefs.SetInt( AMOUNT_SAVE_STRING, amount );

        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnGUI () {
        so.Update();

        EditorGUILayout.PropertyField( propRange );
        propRange.floatValue = propRange.floatValue.Between( MINIMUM_RANGE_VALUE, MAXIMUM_RANGE_VALUE );

        EditorGUILayout.PropertyField( propAmount );
        propAmount.intValue = propAmount.intValue.Between( MINIMUM_AMOUNT_VALUE, MAXIMUM_AMOUNT_VALUE );

        if ( so.ApplyModifiedProperties() ) {
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }

        if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 ) {
            GUI.FocusControl( null );
            Repaint();
        }
    }

    private void DuringSceneGUI ( SceneView sceneView ) {
        if ( Event.current.type == EventType.MouseMove ) sceneView.Repaint();


        bool ctrlHeld = ( Event.current.modifiers & EventModifiers.Control ) != 0;
        if ( Event.current.type == EventType.ScrollWheel && ctrlHeld ) {
            float scrollDir = Mathf.Sign( Event.current.delta.y );

            so.Update();
            propRange.floatValue *= 1f + scrollDir * .075f;
            so.ApplyModifiedProperties();
            Repaint();
            Event.current.Use();
        }

        if ( Event.current.type != EventType.Repaint ) return;

        var cameraTransform = sceneView.camera.transform;
        Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
        //Ray ray = new Ray( cameraTransform.position, cameraTransform.forward );
        Physics.Raycast( ray, out RaycastHit hit );

        var tangent = Vector3.Cross( hit.normal, cameraTransform.up ).normalized;
        var biTangent = Vector3.Cross( hit.normal, tangent );

        Handles.zTest = CompareFunction.LessEqual;
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine( 5f, hit.point, hit.point + hit.normal );
        Handles.color = Color.red;
        Handles.DrawAAPolyLine( 5f, hit.point, hit.point + tangent );
        Handles.color = Color.green;
        Handles.DrawAAPolyLine( 5f, hit.point, hit.point + biTangent );

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

        Ray GetTangentRay ( Vector2 pointPosition ) {
            var originalPos = hit.point + ( tangent * pointPosition.x + biTangent * pointPosition.y ) * range;
            return new Ray( originalPos + hit.normal * 3, -hit.normal );
        }

        Handles.color = Color.white;
        foreach ( Vector2 p in generatedPoints ) {
            if ( Physics.Raycast( GetTangentRay( p ), out RaycastHit newPosHit ) ) {
                Handles.DrawAAPolyLine( newPosHit.point, newPosHit.point + newPosHit.normal );
                Handles.DrawSolidDisc( newPosHit.point, newPosHit.normal, .1f );
            }
        }
    }

    private void GenerateRandomPoints () {
        generatedPoints = new Vector2[ amount ];
        for ( int i = 0; i < amount; i++ )
            generatedPoints[ i ] = Random.insideUnitCircle;
    }

}
