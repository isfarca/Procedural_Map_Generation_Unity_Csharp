using UnityEngine;

public class MazeWall : MazeCellEdge 
{
    // Declare variables.
    [SerializeField] private Transform _wallTransform;

    /// <inheritdoc cref="MazeCellEdge.Initialize"/>
    public override void Initialize(MazeCell mazeCell, MazeCell otherMazeCell, MazeDirection direction) 
    {
        base.Initialize(mazeCell, otherMazeCell, direction);
        _wallTransform.GetComponent<Renderer>().material = mazeCell.Room.Settings.WallMaterial;
    }
}