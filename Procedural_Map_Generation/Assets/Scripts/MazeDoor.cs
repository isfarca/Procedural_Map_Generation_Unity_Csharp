using UnityEngine;

public class MazeDoor : MazePassage 
{
    // Declare variables.
    [SerializeField] private Transform _hinge;
    private static readonly Quaternion NormalRotation = Quaternion.Euler(0f, -90f, 0f),
        MirroredRotation = Quaternion.Euler(0f, 90f, 0f);
    private bool _isMirrored;
    
    /// <summary>
    /// Check if it was created as a second door.
    /// </summary>
    private MazeDoor OtherSideOfDoor 
    {
        get { return (MazeDoor)OtherCell.GetEdge(Direction.GetOpposite()); }
    }

    /// <inheritdoc cref="MazeCellEdge.Initialize"/>
    public override void Initialize(MazeCell primary, MazeCell other, MazeDirection direction) 
    {
        base.Initialize(primary, other, direction);

        // Have you not the other side of door, than return.
        if (OtherSideOfDoor == null)
        {
            return;
        }

        // Mirror door.
        _isMirrored = true;
        
        // Set local scale of hinge.
        _hinge.localScale = new Vector3(-1f, 1f, 1f);
        
        // Set local position x of hinge.
        Vector3 point = _hinge.localPosition;
        point.x = -point.x;
        _hinge.localPosition = point;
        
        // Set the material of all its direct children except for the hinge.
        for (int i = 0; i < transform.childCount; i++) 
        {
            Transform child = transform.GetChild(i);
            
            if (child != _hinge) 
            {
                child.GetComponent<Renderer>().material = Cell.Room.Settings.WallMaterial;
            }
        }
    }
    
    /// <inheritdoc cref="MazeCellEdge.OnPlayerEntered"/>
    public override void OnPlayerEntered() 
    {
        OtherSideOfDoor._hinge.localRotation = _hinge.localRotation = _isMirrored ? MirroredRotation : NormalRotation;
        OtherSideOfDoor.Cell.Room.Show();
    }
	
    /// <inheritdoc cref="MazeCellEdge.OnPlayerExited"/>
    public override void OnPlayerExited() 
    {
        OtherSideOfDoor._hinge.localRotation = _hinge.localRotation = Quaternion.identity;
        OtherSideOfDoor.Cell.Room.Hide();
    }
}