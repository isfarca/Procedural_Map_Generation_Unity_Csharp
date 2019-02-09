using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    // Declare variables.
    [SerializeField] private Maze _mazePrefab;
    private Maze _mazeInstance;
    [SerializeField] private Player _playerPrefab;
    private Player _playerInstance;
    
    /// <summary>
    /// Initializing.
    /// </summary>
    private void Start() 
    {
        // Begin the game.
        StartCoroutine(BeginGame());
    }
	
    /// <summary>
    /// Restarting handling.
    /// </summary>
    private void Update() 
    {
        // When a player press space key, than restart the game.
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Create maze instance.
    /// </summary>
    private IEnumerator BeginGame()
    {
        SetMainCameraRect(0.0f, 0.0f, 1.0f, 1.0f, CameraClearFlags.Skybox);
        
        // Instantiate a maze.
        _mazeInstance = Instantiate(_mazePrefab);
        
        // Now let GameManager call Generate and the maze should appear when you enter play mode.
        yield return StartCoroutine(_mazeInstance.Generate());
        
        // Instantiate player.
        _playerInstance = Instantiate(_playerPrefab);
        _playerInstance.SetLocation(_mazeInstance.GetMazeCell(_mazeInstance.RandomCoordinates));

        SetMainCameraRect(0.0f, 0.0f, 0.5f, 0.5f, CameraClearFlags.Depth);
    }

    /// <summary>
    /// Destroy current instance and create new.
    /// </summary>
    private void RestartGame()
    {
        // Destroy the maze.
        StopAllCoroutines();
        Destroy(_mazeInstance.gameObject);
        
        if (_playerInstance != null) 
        {
            Destroy(_playerInstance.gameObject);
        }
        
        StartCoroutine(BeginGame());
    }

    /// <summary>
    /// Set the main camera rect position.
    /// </summary>
    /// <param name="x">Rect x of the main camera.</param>
    /// <param name="y">Rect y of the main camera.</param>
    /// <param name="width">Rect width of the main camera.</param>
    /// <param name="height">Rect height of the main camera.</param>
    /// <param name="clearFlags">Clear flags of the main camera.</param>
    private static void SetMainCameraRect(float x, float y, float width, float height, CameraClearFlags clearFlags)
    {
        if (Camera.main == null)
        {
            return;
        }

        Camera.main.clearFlags = clearFlags;
        Camera.main.rect = new Rect(x, y, width, height);
    }
}