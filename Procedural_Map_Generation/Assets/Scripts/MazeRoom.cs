using System.Collections.Generic;
using UnityEngine;

public class MazeRoom : ScriptableObject 
{
    // Declare variables.
    public int IndexOfSettings;
    public MazeRoomSettings Settings;
    private readonly List<MazeCell> _mazeCells = new List<MazeCell>();
	
    /// <summary>
    /// Add maze cell to the room.
    /// </summary>
    /// <param name="mazeCell">Surface of the maze.</param>
    public void Add(MazeCell mazeCell) 
    {
        mazeCell.Room = this;
        _mazeCells.Add(mazeCell);
    }
    
    /// <summary>
    /// Get rid of assimilated room instances.
    /// </summary>
    /// <param name="room">Assimilated room.</param>
    public void Assimilate(MazeRoom room)
    {
        foreach (MazeCell c in room._mazeCells)
        {
            Add(c);
        }
    }
    
    /// <summary>
    /// Hide only the room in which the player is currently located.
    /// </summary>
    public void Hide()
    {
        foreach (MazeCell c in _mazeCells)
        {
            c.Hide();
        }
    }
	
    /// <summary>
    /// Show only the room in which the player is currently located.
    /// </summary>
    public void Show()
    {
        foreach (MazeCell c in _mazeCells)
        {
            c.Show();
        }
    }
}