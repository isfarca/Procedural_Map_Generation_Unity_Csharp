using UnityEngine;

/// <summary>
/// Explicit definition that we have the directions north, east, south and west.
/// </summary>
public enum MazeDirection
{
    North,
    East,
    South,
    West
}

public static class MazeDirections 
{
    // Declare variables.
    public const int Count = 4;
    private static readonly IntVector2[] Directions = 
    { 
        new IntVector2(0, 1), // North.
        new IntVector2(1, 0), // East.
        new IntVector2(0, -1), // South.
        new IntVector2(-1, 0) // West.
    };
    private static readonly MazeDirection[] Opposites = 
    {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East
    };
    private static readonly Quaternion[] Rotations = 
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    /// <summary>
    /// Get random direction.
    /// </summary>
    public static MazeDirection RandomValue 
    {
        get { return (MazeDirection)Random.Range(0, Count); }
    }

    /// <summary>
    /// Convert direction to integer vector.
    /// </summary>
    /// <param name="direction">Direction.</param>
    /// <returns>Integer vector.</returns>
    public static IntVector2 ToIntVector2(this MazeDirection direction) 
    {
        return Directions[(int)direction];
    }

    /// <summary>
    /// Get the vector value for the opposites.
    /// </summary>
    /// <param name="direction">Direction from enum.</param>
    /// <returns>Vector values from opposites.</returns>
    public static MazeDirection GetOpposite(this MazeDirection direction) 
    {
        return Opposites[(int)direction];
    }
	
    /// <summary>
    /// Target rotation.
    /// </summary>
    /// <param name="direction">Direction from enum.</param>
    /// <returns>Vector values from target rotation.</returns>
    public static Quaternion ToRotation (this MazeDirection direction) 
    {
        return Rotations[(int)direction];
    }

    /// <summary>
    /// Give the next direction in clockwise.
    /// </summary>
    /// <param name="direction">Direction as compass.</param>
    /// <returns>Clockwise direction.</returns>
    public static MazeDirection GetNextClockwise(this MazeDirection direction) 
    {
        return (MazeDirection)(((int)direction + 1) % Count);
    }

    /// <summary>
    /// Give the next direction in counter clockwise.
    /// </summary>
    /// <param name="direction">Direction as compass.</param>
    /// <returns></returns>
    public static MazeDirection GetNextCounterclockwise(this MazeDirection direction) 
    {
        return (MazeDirection)(((int)direction + Count - 1) % Count);
    }
}