using UnityEngine;

public static class ExtensionMethods {

    #region Vector3 Extensions
    public static Vector3 Round ( this Vector3 v ) {
        v.x = Mathf.Round( v.x );
        v.y = Mathf.Round( v.y );
        v.z = Mathf.Round( v.z );
        return v;
    }

    public static Vector3 Round ( this Vector3 v, float roundingPoint ) => ( v / roundingPoint ).Round() * roundingPoint;
    #endregion

    #region Float Extensions
    public static float Round ( this float f, float roundingPoint ) => Mathf.Round( f / roundingPoint ) * roundingPoint;
    public static float AtLeast ( this float f, float min ) => Mathf.Max( f, min );
    public static float AtMax ( this float f, float max ) => Mathf.Min( f, max );
    public static float Between ( this float f, float min, float max ) => f.AtLeast( min ).AtMax( max );
    #endregion

    #region Int Extensions
    public static int AtLeast ( this int i, int min ) => Mathf.Max( i, min );
    public static int AtMax ( this int i, int max ) => Mathf.Min( i, max );
    public static int Between ( this int i, int min, int max ) => i.AtLeast( min ).AtMax( max );
    #endregion
}
