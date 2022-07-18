using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExplosiveBarrelManager : MonoBehaviour {
        
    public static List<ExplosiveBarrel> allExplosiveBarrels = new List<ExplosiveBarrel>();

    public static void UpdateAllBarrelColors () {
        foreach(ExplosiveBarrel barrel in allExplosiveBarrels ) {
            barrel.TryApplyColor();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos () {
        foreach ( var barrel in allExplosiveBarrels ) {
            if ( barrel.type == null ) continue;

            Handles.zTest = CompareFunction.LessEqual;

            Vector3 managerPos = transform.position;
            Vector3 barrelPos = barrel.transform.position;
            float halfHeight = ( managerPos.y - barrelPos.y ) / 2f;
            Vector3 offset = Vector3.up * halfHeight;

            Handles.DrawBezier( managerPos, barrelPos, managerPos - offset, barrelPos + offset, barrel.type.color, EditorGUIUtility.whiteTexture, 1f );
        }
    }
#endif
}
