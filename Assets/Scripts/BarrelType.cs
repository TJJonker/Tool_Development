using UnityEngine;

[CreateAssetMenu]
public class BarrelType : ScriptableObject {

    [Range( 1f, 8f )] public float radius = 1f;
    public float damage = 10f;
    public Color color = Color.red;

}
