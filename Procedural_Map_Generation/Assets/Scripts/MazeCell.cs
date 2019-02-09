using UnityEngine;

public class MazeCell : MonoBehaviour 
{
    // Declare variables.
    public IntVector2 Coordinates;
    private readonly MazeCellEdge[] _edges = new MazeCellEdge[MazeDirections.Count];
    private int _initializedEdgeCount;
    public MazeRoom Room;

    /// <summary>
    /// Get edges from array.
    /// </summary>
    /// <param name="direction">Current direction.</param>
    /// <returns>Vector point from edge with direction.</returns>
    public MazeCellEdge GetEdge(MazeDirection direction) 
    {
        return _edges[(int)direction];
    }

    /// <summary>
    /// Save edges in a array.
    /// </summary>
    /// <param name="direction">Current direction.</param>
    /// <param name="edge">Edge between two cells.</param>
    public void SetEdge(MazeDirection direction, MazeCellEdge edge) 
    {
        _edges[(int)direction] = edge;
        _initializedEdgeCount += 1;
    }

    /// <summary>
    /// Track of how often an edge has been set.
    /// </summary>
    public bool IsFullyInitialized 
    {
        get { return _initializedEdgeCount == MazeDirections.Count; }
    }
    
    /// <summary>
    /// Check, what direction does not exists.
    /// </summary>
    public MazeDirection RandomUninitializedDirection 
    {
        get 
        {
            // Randomly decide how many uninitialized directions we should skip.
            int skips = Random.Range(0, MazeDirections.Count - _initializedEdgeCount);
            
            // Then we loop through our edges array.
            for (int i = 0; i < MazeDirections.Count; i++) 
            {
                // Whenever we find a hole we check whether we are out of.
                if (_edges[i] != null) 
                    continue;
                
                // If so, this is our direction.
                if (skips == 0) 
                {
                    return (MazeDirection)i;
                }
                
                // Otherwise, we decrease our amount of skips by one.    
                skips -= 1;
            }
            
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions!");
        }
    }
    
    /// <summary>
    /// Initializing room.
    /// </summary>
    /// <param name="room">A room in maze.</param>
    public void Initialize(MazeRoom room) 
    {
        // Assigning the right materials.
        room.Add(this);
        
        // Insert material.
        transform.GetChild(0).GetComponent<Renderer>().material = room.Settings.FloorMaterial;
    }
    
    /// <summary>
    /// Player entered a cell.
    /// </summary>
    public void OnPlayerEntered()
    {
        Room.Show();
        
        foreach (MazeCellEdge edge in _edges)
        {
            edge.OnPlayerEntered();
        }
    }
	
    /// <summary>
    /// Player exited a cell.
    /// </summary>
    public void OnPlayerExited()
    {
        Room.Hide();
        
        foreach (MazeCellEdge edge in _edges)
        {
            edge.OnPlayerExited();
        }
    }
    
    /// <summary>
    /// Hide only the room in which the player is currently located.
    /// </summary>
    public void Hide() 
    {
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Show only the room in which the player is currently located.
    /// </summary>
    public void Show() 
    {
        gameObject.SetActive(true);
    }
}