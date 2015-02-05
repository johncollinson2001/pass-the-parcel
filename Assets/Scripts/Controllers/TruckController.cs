using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE - It was considered having the truck controller contain logic for loading parcels loaded and exhibiting state
/// such as capacity etc, but it was deemed unnecessary at this stage as it can be simply managed elsewhere and all the truck
/// needs to do right now is drive away and come back again
/// </summary>
public class TruckController : MonoBehaviour 
{
    private bool _deliveryInProgress;
    private bool _returningToDepot;
    private float _originalX;
    private float _originalY;

    public float _deliverToX;
    public float _deliverToY;
    public int _animationSpeed;

	#region Mono Behaviours

    void Awake()
    {
        _originalX = transform.position.x;
        _originalY = transform.position.y;
    }

    void Update()
    {
        if(_deliveryInProgress)
        {
            // Check if the truck has reached its destination
            if (transform.position.x == _deliverToX && transform.position.y == _deliverToY)
            {
                ReturnToDepot();
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    new Vector3(_deliverToX, _deliverToY), 
                    _animationSpeed * Time.deltaTime
                );
            }
        }
        else if(_returningToDepot)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                new Vector3(_originalX, _originalY), 
                (_animationSpeed / Defaults.Truck.reverseSpeedModifier) * Time.deltaTime
            );
        }
    }

    #endregion

    #region Public Methods

    public void DeliverParcels()
    {
        // Starting the truck takes a few seconds...
        StartCoroutine(DriveAway());        
    }

    public void ReturnToDepot()
    {
        _deliveryInProgress = false;
        _returningToDepot = true;
    }

    #endregion

    #region Private Methods

    IEnumerator DriveAway()
    {
        yield return new WaitForSeconds(Defaults.Truck.secondsToStartTruck);

        _returningToDepot = false;
        _deliveryInProgress = true;
    }

    #endregion
}
