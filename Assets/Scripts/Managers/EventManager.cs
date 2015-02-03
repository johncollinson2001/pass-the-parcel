using UnityEngine;
using System;
using System.Collections.Generic;

public class EventManager 
{
    private static EventManager _instance;

    public static EventManager Instance
    {
        get
        {
            // Instantiate instance if not created
            if(_instance == null)
            {
                _instance = new EventManager();                
            }

            return _instance;
        }
    }

    // Singleton constructor
    private EventManager() { }

    // Debug write event
    public event Action<GUIText> DebugWrite;
    public void TriggerDebugWrite(GUIText guiText)
    {
        if (DebugWrite != null)
        {
            DebugWrite(guiText);
        }
    }

    // Parcel falling event
    public event Action<GameObject, GameObject> ParcelDropped;
    public void TriggerParcelDropped(GameObject conveyorBelt, GameObject parcel)
    {
        if (ParcelDropped != null)
        {
            ParcelDropped(conveyorBelt, parcel);
        }
    }

    // Parcel loaded event
    public event Action ParcelLoaded;
    public void TriggerParcelLoaded()
    {
        if (ParcelLoaded != null)
        {
            ParcelLoaded();
        }
    }

    // Parcel broken event
    public event Action ParcelBroken;
    public void TriggerParcelBroken()
    {
        if (ParcelBroken != null)
        {
            ParcelBroken();
        }
    }

    // Parcel caught
    public event Action<GameObject> ParcelPassed;
    public void TriggerParcelPassed(GameObject parcel)
    {
        if (ParcelPassed != null)
        {
            ParcelPassed(parcel);
        }
    }

    // Game state changed event
    public event Action<GameState, GameState> GameStateChanged;
    public void TriggerGameStateChanged(GameState changedFrom, GameState changedTo)
    {
        if (GameStateChanged != null)
        {
            GameStateChanged(changedFrom, changedTo);
        }
    }
}