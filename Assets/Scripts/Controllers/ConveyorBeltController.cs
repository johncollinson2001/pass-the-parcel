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

    void Awake() 
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
        if (collision.gameObject.tag == Tags.parcel && !_parcels.Exists(p => p == collision.gameObject))
        {
            // Add the parcel to the parcels list
            _parcels.Add(collision.gameObject);
        }
    }


    // investigate if you can get a final on collision exit - when the whole object has left the conveyor
    // or could try to set the rotation/physics force on the parcel when its passed so it doesnt tip over?

    void OnCollisionExit2D(Collision2D collision)
    {
        // Test to see if the collision was with a parcel
        if (collision.gameObject.tag == Tags.parcel && _parcels.Exists(p => p == collision.gameObject))
        {
            // Remove the parcel from the list
            _parcels.Remove(collision.gameObject);

            // Drop the parcel
            collision.gameObject.GetComponent<ParcelController>().State = ParcelState.Dropped;

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

        // Set the state of the parcel depending on where we've moved it to
        SetParcelState(parcel);
    }

    void SetParcelState(GameObject parcel)
    {
        float conveyorPositionX = transform.position.x;
        float conveyorWidth = collider2D.bounds.size.x; // Work to the collider incase of offset
        float parcelWidth = parcel.renderer.bounds.size.x;

        // The calculations will be different depending on which way the conveyor is travelling
        if (_travellingTo == ScreenSide.Left)
        {
            // Work out the position which the parcel must of moved beyond if we're to set the state
            float aboutToDropX =
                (conveyorPositionX - (conveyorWidth / 2))
                + (parcelWidth / 2)
                + Constants.ConveyorBelt.aboutToDropParcelBuffer;

            if (parcel.GetComponent<ParcelController>().State != ParcelState.Dropped
                && parcel.transform.position.x < aboutToDropX)
            {
                parcel.GetComponent<ParcelController>().State = ParcelState.AboutToDrop;
            }
        }
        else
        {
            // Work out the position which the parcel must of moved beyond if we're to set the state
            float aboutToDropX =
                (conveyorPositionX + (conveyorWidth / 2))
                - (parcelWidth / 2)
                - Constants.ConveyorBelt.aboutToDropParcelBuffer;

            if (parcel.GetComponent<ParcelController>().State != ParcelState.Dropped
                && parcel.transform.position.x > aboutToDropX)
            {
                parcel.GetComponent<ParcelController>().State = ParcelState.AboutToDrop;
            }
        }
    }

    #endregion
}
