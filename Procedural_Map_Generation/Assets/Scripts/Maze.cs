using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour 
{
    // Declare variables.
    [SerializeField] private float _generationStepDelay;
    [SerializeField] private IntVector2 _size;
    private MazeCell[,] _mazeCells;
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private MazePassage _mazePassagePrefab;
    [SerializeField] private MazeDoor _mazeDoorPrefab;
    [SerializeField] [Range(0.0f, 1.0f)] private float _mazeDoorProbability;
    [SerializeField] private MazeWall[] _mazeWallPrefabs;
    [SerializeField] private MazeRoomSettings[] _mazeRoomSettings;
    private readonly List<MazeRoom> _rooms = new List<MazeRoom>();
    
    /// <summary>
    /// Constructing the maze contents.
    /// </summary>
    public IEnumerator Generate() 
    {
        // Declare variables.
        WaitForSeconds delay = new WaitForSeconds(_generationStepDelay);
        
        // Set the maximum spread of the maze cells.
        _mazeCells = new MazeCell[_size.X, _size.Z];
        
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        
        // Create maze cells.
        while (activeCells.Count > 0)
        {
            // Slow down the generation process.
            yield return delay;
            
            DoNextGenerationStep(activeCells);
        }
        
        // Hide all rooms when it's done generating.
        foreach (MazeRoom r in _rooms)
        {
            r.Hide();
        }
    }
    
    /// <summary>
    /// We instantiate a new cell, put it in the array and give it a descriptive name.
    /// </summary>
    /// <param name="coordinates">X and Z.</param>
    /// <returns>A new created cell.</returns>
    private MazeCell CreateMazeCell(IntVector2 coordinates) 
    {
        // Declare variables.
        MazeCell newMazeCell = Instantiate(_mazeCellPrefab);
        
        // Customize Maze to use IntVector2 when creating cells and for their size instead of using two separate
        // integers.
        _mazeCells[coordinates.X, coordinates.Z] = newMazeCell;
        newMazeCell.Coordinates = coordinates;
        
        // Add properties.
        newMazeCell.name = "Maze Cell " + coordinates.X + ", " + coordinates.Z;
        newMazeCell.transform.parent = transform;
        
        // Set the point of new maze cell.
        newMazeCell.transform.localPosition = new Vector3(coordinates.X - _size.X * 0.5f + 0.5f, 0f,
            coordinates.Z - _size.Z * 0.5f + 0.5f);

        return newMazeCell;
    }
    
    /// <summary>
    /// Generates some coordinates
    /// </summary>
    public IntVector2 RandomCoordinates 
    {
        get { return new IntVector2(Random.Range(0, _size.X), Random.Range(0, _size.Z)); }
    }

    /// <summary>
    /// Checks if some coordinates fall into the maze.
    /// </summary>
    /// <param name="coordinate">Current coordinate with x and z.</param>
    /// <returns>Whether it is within the size.</returns>
    private bool ContainsCoordinates(IntVector2 coordinate) 
    {
        return (coordinate.X >= 0 && coordinate.X < _size.X && coordinate.Z >= 0 && coordinate.Z < _size.Z);
    }
    
    /// <summary>
    /// Retrieve the cell of the maze at some coordinates.
    /// </summary>
    /// <param name="coordinates">Some coordinates.</param>
    /// <returns>Cell of the mazes at some coordinates.</returns>
    public MazeCell GetMazeCell(IntVector2 coordinates) 
    {
        return _mazeCells[coordinates.X, coordinates.Z];
    }
    
    /// <summary>
    /// Set a random start point.
    /// </summary>
    /// <param name="activeCells">Active cells.</param>
    private void DoFirstGenerationStep(ICollection<MazeCell> activeCells) 
    {
        // Declare variables.
        MazeCell newMazeCell = CreateMazeCell(RandomCoordinates);
        
        // Create a new room with first cell.
        newMazeCell.Initialize(CreateRoom(-1));
        activeCells.Add(newMazeCell);
    }

    /// <summary>
    /// Calls the current cell and checks if the move is possible, otherwise the cell will be removed from the list.
    /// </summary>
    /// <param name="activeCells">Active cells.</param>
    private void DoNextGenerationStep(IList<MazeCell> activeCells) 
    {
        // Declare variables.
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];
        
        // Remove a cell from the active list only when all edges have been initialized.
        if (currentCell.IsFullyInitialized) 
        {
            activeCells.RemoveAt(currentIndex);
            
            return;
        }
        
        // Just pick a random direction that has not yet been initialized for the current cell.
        MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.Coordinates + direction.ToIntVector2();
        
        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbour = GetMazeCell(coordinates);
            
            // If we're still inside the maze, we need to check if the current cell's neighbour doesn't exist yet.
            if (neighbour == null)
            {
                // If so, we create it and place a passage in between them..
                neighbour = CreateMazeCell(coordinates);
                CreatePassage(currentCell, neighbour, direction);
                activeCells.Add(neighbour);
            }
            // Join together adjacent rooms if they share the same settings.
            else if (currentCell.Room.IndexOfSettings == neighbour.Room.IndexOfSettings) 
            {
                // Calls this method when two cells share a room, instead of placing a wall.
                CreatePassageInSameRoom(currentCell, neighbour, direction);
            }
            else
            {
                // But if the neighbor already exists, we separate them with a wall.
                CreateWall(currentCell, neighbour, direction);
            }
        }
        else 
        {
            // When we would go out of the maze, we add a wall.
            CreateWall(currentCell, null, direction);
        }
    }
    
    /// <summary>
    /// Simply instantiate their respective prefabs and initialize them, once for both cells.
    /// </summary>
    /// <param name="mazeCell">Current cell point.</param>
    /// <param name="otherMazeCell">Neighbour cell point.</param>
    /// <param name="mazeDirection">Direction of the passage.</param>
    private void CreatePassage(MazeCell mazeCell, MazeCell otherMazeCell, MazeDirection mazeDirection) 
    {
        // Place a door or a passageway.
        MazePassage prefab = Random.value < _mazeDoorProbability ? _mazeDoorPrefab : _mazePassagePrefab;
        MazePassage passage = Instantiate(prefab);
        
        passage.Initialize(mazeCell, otherMazeCell, mazeDirection);
        passage = Instantiate(prefab);
        
        // Check whether a door has been placed.
        // If so, the other cell is the first of a new room.
        // If not, it belongs to the same room as the previous cell.
        otherMazeCell.Initialize(passage is MazeDoor ? CreateRoom(mazeCell.Room.IndexOfSettings) : mazeCell.Room);
        
        passage.Initialize(otherMazeCell, mazeCell, mazeDirection.GetOpposite());
    }

    /// <summary>
    /// Simply instantiate their respective prefabs and initialize them, once for both cells.
    /// </summary>
    /// <param name="mazeCell">Current cell point.</param>
    /// <param name="otherMazeCell">Neighbour cell point.</param>
    /// <param name="mazeDirection">Direction of the passage.</param>
    private void CreateWall(MazeCell mazeCell, MazeCell otherMazeCell, MazeDirection mazeDirection) 
    {
        // Declare variables.
        MazeWall wall = Instantiate(_mazeWallPrefabs[Random.Range(0, _mazeWallPrefabs.Length)]);
        
        wall.Initialize(mazeCell, otherMazeCell, mazeDirection);

        if (otherMazeCell == null)
        {
            return;
        }

        wall = Instantiate(_mazeWallPrefabs[Random.Range(0, _mazeWallPrefabs.Length)]);
        wall.Initialize(otherMazeCell, mazeCell, mazeDirection.GetOpposite());
    }
    
    /// <summary>
    /// Create a new room for the first cell and each time we spawn a door.
    /// </summary>
    /// <param name="indexToExclude">Exclude cell by index.</param>
    /// <returns>Colored room.</returns>
    private MazeRoom CreateRoom(int indexToExclude) 
    {
        // Declare variables.
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        
        newRoom.IndexOfSettings = Random.Range(0, _mazeRoomSettings.Length);
        
        // Check whether we picked the same index as the room we came from.
        if (newRoom.IndexOfSettings == indexToExclude) 
        {
            // If so, we'll just add one to the index and wrap around.
            newRoom.IndexOfSettings = (newRoom.IndexOfSettings + 1) % _mazeRoomSettings.Length;
        }
        
        newRoom.Settings = _mazeRoomSettings[newRoom.IndexOfSettings];
        _rooms.Add(newRoom);
        
        return newRoom;
    }
    
    /// <summary>
    /// Creates a passage between two cells, with no chance of a door.
    /// </summary>
    /// <param name="mazeCell">Current cell point.</param>
    /// <param name="otherMazeCell">Neighbour cell point.</param>
    /// <param name="direction">Direction of the passage.</param>
    private void CreatePassageInSameRoom (MazeCell mazeCell, MazeCell otherMazeCell, MazeDirection direction) 
    {
        // Declare variables.
        MazePassage passage = Instantiate(_mazePassagePrefab);
        
        // Initializing.
        passage.Initialize(mazeCell, otherMazeCell, direction);
        passage = Instantiate(_mazePassagePrefab);
        passage.Initialize(otherMazeCell, mazeCell, direction.GetOpposite());

        // Then we have Maze check whether it's connecting different rooms, in which case it assimilates and removes the
        // other room.
        if (mazeCell.Room == otherMazeCell.Room)
        {
            return;
        }

        MazeRoom roomToAssimilate = otherMazeCell.Room;
            
        mazeCell.Room.Assimilate(roomToAssimilate);
        _rooms.Remove(roomToAssimilate);
            
        Destroy(roomToAssimilate);
    }
}