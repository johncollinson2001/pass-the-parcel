using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundController : MonoBehaviour
{
    private int _backgroundIndex = 0;
    private SpriteRenderer _spriteRenderer;

    public Sprite[] _backgroundSprites;

    #region Mono Behaviours

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        EventManager.Instance.LevelUp += CheckIfBackgroundShouldBeChanged;
        EventManager.Instance.GameOver += Reset;
    }

    void OnDisable()
    {
        EventManager.Instance.LevelUp -= CheckIfBackgroundShouldBeChanged;
        EventManager.Instance.GameOver -= Reset;
    }

    #endregion

    #region Private Methods

    void Reset()
    {
        _backgroundIndex = 0;
        _spriteRenderer.sprite = _backgroundSprites[_backgroundIndex];
    }

    // Checks if the background should be changed and changes it
    void CheckIfBackgroundShouldBeChanged(LevelModel nextLevel)
    {
        if (nextLevel.LevelNumber % Constants.Background.LevelsPerBackgroundChange == 0)
        {
            ChangeBackground();
        }
    }

    // Changes the background
    void ChangeBackground()
    {
        // Get the next Background
        Sprite nextBackgroundSprite = GetNextBackgroundSprite();

        // Change the background of the sprite
        _spriteRenderer.sprite = nextBackgroundSprite;
    }

    // Gets the next background path on a sequence
    Sprite GetNextBackgroundSprite()
    {
        // Check if the index should go back to 0
        if (_backgroundIndex == _backgroundSprites.GetUpperBound(0))
        {
            _backgroundIndex = 0;
        }
        else
        {
            // Increment the background image index
            _backgroundIndex = _backgroundIndex + 1;
        }

        return _backgroundSprites[_backgroundIndex];
    }

    #endregion
}
