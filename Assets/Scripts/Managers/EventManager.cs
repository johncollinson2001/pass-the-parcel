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
    public event Action<GameObject> ParcelDropped;
    public void TriggerParcelDropped(GameObject parcel)
    {
        if (ParcelDropped != null)
        {
            ParcelDropped(parcel);
        }
    }

    // Parcel dropping soon event
    public event Action<GameObject> ParcelAboutToDrop;
    public void TriggerParcelAboutToDrop(GameObject parcel)
    {
        if (ParcelAboutToDrop != null)
        {
            ParcelAboutToDrop(parcel);
        }
    }

    // Parcel loaded event
    public event Action<GameObject> ParcelLoaded;
    public void TriggerParcelLoaded(GameObject parcel)
    {
        if (ParcelLoaded != null)
        {
            ParcelLoaded(parcel);
        }
    }

    // Parcel broken event
    public event Action<GameObject> ParcelBroken;
    public void TriggerParcelBroken(GameObject parcel)
    {
        if (ParcelBroken != null)
        {
            ParcelBroken(parcel);
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

    // Game over changed event
    public event Action GameOver;
    public void TriggerGameOver()
    {
        if (GameOver != null)
        {
            GameOver();
        }
    }

    // Life lost changed event
    public event Action LifeLost;
    public void TriggerLifeLost()
    {
        if (LifeLost != null)
        {
            LifeLost();
        }
    }

    // Level Up event
    public event Action<LevelModel> LevelUp;
    public void TriggerLevelUp(LevelModel nextLevel)
    {
        if (LevelUp != null)
        {
            LevelUp(nextLevel);
        }
    }
}