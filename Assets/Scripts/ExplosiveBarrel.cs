using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour {

    private readonly int shaderPropertyColor = Shader.PropertyToID( "_Color" );

    public BarrelType type;

    private MaterialPropertyBlock mpb;
    public MaterialPropertyBlock Mpb {
        get {
            if ( mpb == null ) mpb = new MaterialPropertyBlock();
            return mpb; 
        }
    }


    private void OnDrawGizmosSelected () {
        if ( type == null ) return;

        Handles.color = type.color;
        Handles.DrawWireDisc( transform.position, transform.up, type.radius );
        Handles.color = Color.white;
    }

    private void OnEnable () => ExplosiveBarrelManager.allExplosiveBarrels.Add( this );
    private void OnDisable () => ExplosiveBarrelManager.allExplosiveBarrels.Remove( this );
    private void OnValidate () => TryApplyColor();

    public void TryApplyColor () {
        if ( type == null ) return;
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Mpb.SetColor( shaderPropertyColor, type.color );
        renderer.SetPropertyBlock( Mpb );
    }
}
