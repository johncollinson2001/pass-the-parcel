using UnityEngine;
using System.Collections;

public class ParcelController : MonoBehaviour 
{
    private bool _flashing;
    private bool _isHighlighted;
    private Color _originalColor;
	private int _platformLayer;
	private int _workerLayer;
	private int _parcelLayer;
    private GameObject _conveyorBelt;

    public ParcelState State { get; set; }

	#region Mono Behaviours

	void Awake() 
	{
        _flashing = false;
        _originalColor = GetComponent<SpriteRenderer>().color;

        _platformLayer = LayerMask.NameToLayer(Layers.platform);
		_workerLayer = LayerMask.NameToLayer (Layers.worker);
		_parcelLayer = LayerMask.NameToLayer (Layers.parcel);

        State = ParcelState.Travelling;
	}

	void FixedUpdate() 
	{
        // Parcel should ignore collisions with everything but conveyors and the ground
		Physics2D.IgnoreLayerCollision (_parcelLayer, _platformLayer);
        Physics2D.IgnoreLayerCollision(_parcelLayer, _workerLayer);
        Physics2D.IgnoreLayerCollision(_parcelLayer, _parcelLayer);

        // Handle flashing sequences
        //
        // Stop the flashing if...
        if (_flashing && ( // the flashing flag is set to true and...
            State != ParcelState.AboutToDrop // the parcel is not about to drop or...
            || !_conveyorBelt.GetComponent<ConveyorBeltController>().Operational // the conveyor belt is not operational or...
            || _conveyorBelt.GetComponent<ConveyorBeltController>().IsWorkerWaitingToReceiveParcel() // there is a worker waiting to receive the parcel
        ))
        {
            StopFlashing();
        }
        else if (!_flashing && State == ParcelState.AboutToDrop)
        {
            StartFlashing();
        }
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Test to see if the collision is with something other than the conveyor belt
        // and has not already been marked as dropped
		if (collision.gameObject.tag != Tags.conveyorBelt && State == ParcelState.Dropped) 
		{
			// Set the parcels state as broken
            State = ParcelState.Broken;

			// Raise an event to let others know that a parcel has been dropped
			EventManager.Instance.TriggerParcelBroken();            
		}

        // Keep a record of the conveyor belt the parcel is travelling on
        if (collision.gameObject.tag == Tags.conveyorBelt)
        {
            _conveyorBelt = collision.gameObject;
        }
	}

    #endregion

    #region Public Methods

    // Freezes the parcel in the scene
    public void Freeze()
    {
        if (State != ParcelState.Broken)
        {
            rigidbody2D.isKinematic = true;
        }
    }

    // Unfreezes the parcel in the scene
    public void Unfreeze()
    {
        if (State != ParcelState.Broken)
        {
            rigidbody2D.isKinematic = false;
        }
    }

    #endregion

    #region Private Methods
    
    // Makes the parcel flash
    void StartFlashing()
    {
        _flashing = true;
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        while(_flashing)
        {            
            // Manage the flash on/off
            if (!_isHighlighted)
            {
                GetComponent<SpriteRenderer>().color = Constants.Parcel.flashHighlightColor;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = _originalColor;
            }
            
            // Reverse the highlighted flag
            _isHighlighted = !_isHighlighted;            

            yield return new WaitForSeconds(Constants.Parcel.flashSpeed);
        }

        // Clears the highlight from the parcel
        ClearHighlight();
    }

    // Stops the parcel flashing
    void StopFlashing()
    {
        _flashing = false;

        // Stop all coroutines immediately so the flashing stops instantly
        StopAllCoroutines();

        // Clear the highlight so the parcel is the original color
        ClearHighlight();
    }

    // Clears the highlight from the parcel
    void ClearHighlight()
    {
        if (_isHighlighted)
        {
            GetComponent<SpriteRenderer>().color = _originalColor;
            _isHighlighted = false;
        }
    }

    #endregion
}
