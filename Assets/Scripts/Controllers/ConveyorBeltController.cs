using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyorBeltController : MonoBehaviour
{
    private const float BASE_PARCEL_MOVEMENT = 0.01f;
    private bool _operational = true;
    private List<GameObject> _cogs = new List<GameObject>();
    private List<GameObject> _parcels = new List<GameObject>();

    public ScreenSide _travellingTo = ScreenSide.Left;
    public float _speed = 2;	
    
    private bool IsParcelOnBelt
    {
        get { return _parcels.Count > 0;  }
    }

    #region Mono Behaviours

    void Start () 
    {
	    // Set the cogs... Iterate over all children 
        foreach(Transform child in transform)
        {
            // If this child is a cog, add it to the member variable
            if(child.tag == Tags.conveyorCog)
            {
                _cogs.Add(child.gameObject);
            }
        }      
	}

    void FixedUpdate()
    {
        // Check to see if the conveyors running and there's parcels on the belt
        if(_operational && IsParcelOnBelt)
        {
            // Loop over parcels on the belt and move them
            foreach (GameObject parcel in _parcels)
            {
                MoveParcel(parcel);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Test to see if the collision was with a parcel
        if (collision.gameObject.tag == Tags.parcel)
        {
            // Add the parcel to the parcels list
            _parcels.Add(collision.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Test to see if the collision was with a parcel
        if (collision.gameObject.tag == Tags.parcel)
        {
            // Remove the parcel from the list
            _parcels.Remove(collision.gameObject);

			// Mark the parcel as dropped
			collision.gameObject.GetComponent<ParcelController>().HasBeenDropped = true;

			// Trigger an event to shout to the world that the parcel is falling
			EventManager.Instance.TriggerParcelDropped(this.gameObject, collision.gameObject);
        }
    }

    #endregion

    #region Public Methods

    // Starts the conveyor belt
    public void StartConveyorBelt()
    {
        // Iterate over the cogs
        foreach(GameObject cog in _cogs)
        {
            // Get the cogs rotator
			RotationController cogRotator = cog.GetComponent<RotationController>();

            // Work out which way we want the cog to rotate
            if(_travellingTo == ScreenSide.Left)
            {
                cogRotator.StartRotation(RotationDirection.CounterClockwise);
            }
            else
            {
                cogRotator.StartRotation(RotationDirection.Clockwise);
            }            
        }        

        // Set operational status member
        _operational = true;
    }

    // Stops the conveyor belt
    public void StopConveyorBelt()
    {
        // Iterate over the cogs
        foreach (GameObject cog in _cogs)
        {
            // Get the cogs rotator
			RotationController cogRotator = cog.GetComponent<RotationController>();

            // Stop the rotation
            cogRotator.StopRotation();
        }

        // Set operational status member
        _operational = false;
    }

    // Clears the parcels from the conveyor belt
    public void ClearParcels()
    {
        foreach(var parcel in _parcels)
        {
            Destroy(parcel);
        }

        _parcels.Clear();
    }

    #endregion

    #region Private Methods

    // Moves a parcel along the conveyor belt
    void MoveParcel(GameObject parcel)
    {
        // Calculate the value which we will transform the position by
        float xMovement = BASE_PARCEL_MOVEMENT * _speed;

        // Modify the movement to a negative value depending on the direction of travel
        if (_travellingTo == ScreenSide.Left)
        {
            xMovement = xMovement * -1;
        }

        // Move the object
        parcel.transform.position += new Vector3(xMovement, 0, 0);
    }

    #endregion
}
