using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour 
{
	public GameObject _receiveFromConveyor;
	public GameObject _passToConveyor;
    public PlatformLevel _platformLevel;
    public ScreenSide _screenSide;
	public bool _loadsTruck;

    public bool HasWorker { get; private set; }

    #region Mono Behaviours

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Test to see if the collision was with a worker
        if (collision.gameObject.tag == Tags.worker)
        {
            HasWorker = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Test to see if the collision was with a worker
        if (collision.gameObject.tag == Tags.worker)
        {
            HasWorker = false;
        }
    }

    #endregion
}
