using System;

[Serializable] public struct IntVector2 
{
    // Declare variables.
    public int X, Z;
	
    /// <summary>
    /// Manipulate the coordinates as a single value, but with whole numbers instead of comma numbers.
    /// </summary>
    /// <param name="x">Size x.</param>
    /// <param name="z">Size z.</param>
    public IntVector2(int x, int z) 
    {
        // Initializing.
        X = x;
        Z = z;
    }
    
    /// <summary>
    /// Add vectors.
    /// </summary>
    /// <param name="a">First delivery.</param>
    /// <param name="b">Second delivery.</param>
    /// <returns>Result.</returns>
    public static IntVector2 operator + (IntVector2 a, IntVector2 b) 
    {
        // Addition operation.
        a.X += b.X;
        a.Z += b.Z;
        
        return a;
    }
}