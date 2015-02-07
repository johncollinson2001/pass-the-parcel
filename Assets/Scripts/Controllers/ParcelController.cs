using UnityEngine;
using System.Collections;

public class ParcelController : MonoBehaviour 
{
    private bool _flashing;
    private Color _originalColor;
	private int _platformLayer;
	private int _workerLayer;
	private int _parcelLayer;	

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
		Physics2D.IgnoreLayerCollision (_parcelLayer, _platformLayer);
        Physics2D.IgnoreLayerCollision(_parcelLayer, _workerLayer);
        Physics2D.IgnoreLayerCollision(_parcelLayer, _parcelLayer);

        if (!_flashing && State == ParcelState.AboutToDrop)
        {
            StartFlashing();
        }
        else if (_flashing && State != ParcelState.AboutToDrop)
        {
            StopFlashing();
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
        bool isHighlighted = false;

        while(_flashing)
        {
            // Manage the flash on/off
            if (!isHighlighted)
            {
                GetComponent<SpriteRenderer>().color = Constants.Parcel.flashHighlightColor;                
            }
            else
            {
                GetComponent<SpriteRenderer>().color = _originalColor;
            }

            isHighlighted = !isHighlighted;

            yield return new WaitForSeconds(Constants.Parcel.flashSpeed);
        }

        // Return the parcel to its original color if it's still highlighted
        if(isHighlighted)
        {
            GetComponent<SpriteRenderer>().color = _originalColor;
        }
    }

    // Stops the parcel flashing
    void StopFlashing()
    {
        _flashing = false;
    }

    #endregion
}
