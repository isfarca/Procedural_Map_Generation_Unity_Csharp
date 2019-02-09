using UnityEngine;

public abstract class MazeCellEdge : MonoBehaviour 
{
    // Declare variables.
    public MazeCell Cell, OtherCell;
    public MazeDirection Direction;
    
    /// <summary>
    /// Make edges to cells of their cells and place them in the same place.
    /// </summary>
    /// <param name="cell">Current cell point.</param>
    /// <param name="otherCell">Neighbour cell point.</param>
    /// <param name="direction">Direction for continue the cell building.</param>
    public virtual void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction) 
    {
        // Initializing.
        Cell = cell;
        OtherCell = otherCell;
        Direction = direction;
        
        // Informs the cell that an edge has been created.
        cell.SetEdge(direction, this);
        
        // Set as child.
        transform.parent = cell.transform;
        
        // Reset locals.
        transform.localPosition = Vector3.zero;
        
        // Turn walls in the right direction.
        transform.localRotation = direction.ToRotation();
    }

    /// <summary>
    /// Player entered a cell.
    /// </summary>
    public virtual void OnPlayerEntered()
    {
    }

    /// <summary>
    /// Player exited a cell.
    /// </summary>
    public virtual void OnPlayerExited()
    {
    }
}