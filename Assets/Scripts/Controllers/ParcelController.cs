using UnityEngine;
using System.Collections;

public class ParcelController : MonoBehaviour 
{
	private int _platformLayer;
	private int _workerLayer;
	private int _parcelLayer;	

	public bool HasBeenDropped { get; set; } 
	public bool IsBroken { get; set; }

	#region Mono Behaviours

	void Start() 
	{
		_platformLayer = LayerMask.NameToLayer (Layers.platform);
		_workerLayer = LayerMask.NameToLayer (Layers.worker);
		_parcelLayer = LayerMask.NameToLayer (Layers.parcel);
	}

	void FixedUpdate() 
	{
		Physics2D.IgnoreLayerCollision (_parcelLayer, _platformLayer);
        Physics2D.IgnoreLayerCollision(_parcelLayer, _workerLayer);
        Physics2D.IgnoreLayerCollision(_parcelLayer, _parcelLayer);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// Test to see if the collision is with something other than the conveyor belt
        // and has not already been marked as dropped
		if (collision.gameObject.tag != Tags.conveyorBelt && !IsBroken) 
		{
			// Set the parcels state as broken
			IsBroken = true;

			// Raise an event to let others know that a parcel has been dropped
			EventManager.Instance.TriggerParcelBroken();            
		}
	}
	
	#endregion
}
