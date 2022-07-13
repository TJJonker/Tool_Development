using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( BarrelType ) )]
public class BarrelTypeEditor : Editor {
    private enum Sounds { Bip, Boop, Bap }
    Sounds sound;

    private float soundVolume;

    public override void OnInspectorGUI () {

        GUILayout.Label( "Bip Machine", EditorStyles.boldLabel );

        using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) ) {
            using ( new GUILayout.HorizontalScope() ) {
                GUILayout.Label( "Sound:" );
                sound = (Sounds) EditorGUILayout.EnumPopup( sound );
            }

            using(new GUILayout.HorizontalScope() ) {
                GUILayout.Label( "Sound Volume:" );
                soundVolume = GUILayout.HorizontalSlider( soundVolume, 0f, 8f, GUILayout.Height( 20 ) );
            }

            if ( GUILayout.Button( "Make Sound" ) ) Debug.Log( sound.ToString() );
        }
    }
}
