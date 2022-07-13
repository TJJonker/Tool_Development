using UnityEditor;
using UnityEngine;

[CustomEditor( typeof( BarrelType ) )]
public class BarrelTypeEditor : Editor {
    private enum Sounds { Bip, Boop, Bap }
    Sounds sound;

    private float soundVolume;

    public override void OnInspectorGUI () {

        GUILayout.Label( "Bip Machine", EditorStyles.boldLabel );

        using ( new GUILayout.HorizontalScope() ) {
            GUILayout.Label( "Choose your sound:" );
            sound = (Sounds) EditorGUILayout.EnumPopup( sound );
        }

        soundVolume = GUILayout.HorizontalSlider( soundVolume, 0f, 8f );

        if ( GUILayout.Button( "Make Sound" ) ) Debug.Log( sound.ToString() );
    }
}
