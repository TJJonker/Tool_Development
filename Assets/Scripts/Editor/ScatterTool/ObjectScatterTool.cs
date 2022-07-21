using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectScatterTool : EditorWindow {

    [MenuItem( "Tools / Jonkos / Object Scatter Tool %&A" )]
    public static void OpenWindow () => GetWindow<ObjectScatterTool>( "Object Scatter" );


    private const string RANGE_SAVE_STRING = "OBJECT_SCATTER_TOOL_RANGE";
    private const string AMOUNT_SAVE_STRING = "OBJECT_SCATTER_TOOL_AMOUNT";

    private const int MINIMUM_RANGE_VALUE   = 1;
    private const int MAXIMUM_RANGE_VALUE   = 30;
    private const int MINIMUM_AMOUNT_VALUE  = 1;
    private const int MAXIMUM_AMOUNT_VALUE  = 100;

    SerializedObject    so;
    SerializedProperty  propRange;
    [SerializeField]    private float range;
    SerializedProperty  propAmount;
    [SerializeField]    private int amount;


    private void OnEnable () {
        so = new SerializedObject( this );
        propRange = so.FindProperty( "range" );
        propAmount = so.FindProperty( "amount" );

        range  = EditorPrefs.GetFloat( RANGE_SAVE_STRING, MINIMUM_RANGE_VALUE );
        amount = EditorPrefs.GetInt( AMOUNT_SAVE_STRING, MINIMUM_AMOUNT_VALUE );

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

        so.ApplyModifiedProperties();
    }

    private void DuringSceneGUI(SceneView sceneView ) {
        var cameraTransform = sceneView.camera.transform;
        Ray ray = new Ray( cameraTransform.position, cameraTransform.forward );
        Physics.Raycast( ray, out RaycastHit hit);

        var tangent = Vector3.Cross( hit.normal, cameraTransform.up ).normalized;
        var biTangent = Vector3.Cross( hit.normal, tangent );




        Handles.zTest = CompareFunction.LessEqual;
        Handles.color = Color.blue;
        Handles.DrawWireDisc( hit.point, hit.normal, range );
        Handles.DrawAAPolyLine(5f, hit.point, hit.point + hit.normal );
        Handles.color = Color.red;
        Handles.DrawAAPolyLine(5f, hit.point, hit.point + tangent);
        Handles.color = Color.green;
        Handles.DrawAAPolyLine( 5f, hit.point, hit.point + biTangent );
    }

}
