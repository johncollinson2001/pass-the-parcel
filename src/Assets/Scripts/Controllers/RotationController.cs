using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour 
{
    private const int BASE_MOVEMENT = 10;

    public int _rotationSpeed = 20;
    public bool _rotating = false;
    public RotationDirection _rotationDirection = RotationDirection.Clockwise;

    #region Mono Behaviours

    void Update()
    {
        // Check to see if the object should be rotating
        if(_rotating)
        {
            // Calculate the value which we will transform the rotation axis by
            int zRotation = BASE_MOVEMENT * _rotationSpeed;

            // Modify the movement to a negative if the rotation should be clockwise
            // so it turns the right way
            if(_rotationDirection == RotationDirection.Clockwise)
            {
                zRotation = zRotation * -1;
            }

            // Rotate the object
            transform.Rotate(new Vector3(0, 0, zRotation) * Time.deltaTime);
        }
    }

    #endregion

    #region Public Methods

    // Starts the rotation
    public void StartRotation(RotationDirection rotationDirection)
    {
        _rotationDirection = rotationDirection;
        _rotating = true;
    }

    // Stops the rotation
    public void StopRotation()
    {
        _rotating = false;
    }

    #endregion
}
