using UnityEngine;
using System;
using System.Collections.Generic;

public class DebugWriter : MonoBehaviour 
{
    public bool _enabled;
    public GUIText _debugText;

    #region Mono Behaviours

    void Awake()
    {
        _debugText.text = string.Empty;
    }

    void Update()
    {        
        if (_enabled)
        {
            // Empty the string on each frame
            _debugText.text = string.Empty;

            // Raise event for subscribers to write to the debugging text
            EventManager.Instance.TriggerDebugWrite(_debugText);
        }
    }

    #endregion
}