using UnityEngine;

public class Player : MonoBehaviour 
{
    // Declare variables.
    private MazeCell _currentMazeCell;
    private MazeDirection _currentDirection;

    /// <summary>
    /// Movement.
    /// </summary>
    private void Update() 
    {
        // Player movement and rotation.
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            Move(_currentDirection);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            Move(_currentDirection.GetNextClockwise());
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            Move(_currentDirection.GetOpposite());
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            Move(_currentDirection.GetNextCounterclockwise());
        }
        else if (Input.GetKeyDown(KeyCode.Q)) 
        {
            Look(_currentDirection.GetNextCounterclockwise());
        }
        else if (Input.GetKeyDown(KeyCode.E)) 
        {
            Look(_currentDirection.GetNextClockwise());
        }
    }

    /// <summary>
    /// Tell it what cell it player in.
    /// </summary>
    /// <param name="mazeCell">Current maze cell.</param>
    public void SetLocation(MazeCell mazeCell) 
    {
        if (_currentMazeCell != null) 
        {
            _currentMazeCell.OnPlayerExited();
        }
        
        _currentMazeCell = mazeCell;
        transform.localPosition = mazeCell.transform.localPosition;
        _currentMazeCell.OnPlayerEntered();
    }

    /// <summary>
    /// Change player rotation.
    /// </summary>
    /// <param name="direction">Direction from player.</param>
    private void Look(MazeDirection direction) 
    {
        transform.localRotation = direction.ToRotation();
        _currentDirection = direction;
    }

    /// <summary>
    /// Movement should only happen if the edge we would cross is a passage, otherwise we're blocked.
    /// </summary>
    /// <param name="direction">Direction from player.</param>
    private void Move(MazeDirection direction) 
    {
        // Declare variables.
        MazeCellEdge edge = _currentMazeCell.GetEdge(direction);
        
        if (edge is MazePassage) 
        {
            SetLocation(edge.OtherCell);
        }
    }
}