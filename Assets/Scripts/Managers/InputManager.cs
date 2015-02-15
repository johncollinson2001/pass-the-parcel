using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager _gameManager;
    public GameMenuManager _gameMenuManager; 
    public GameObject _workerLeft;
    public GameObject _workerRight;
    public GameObject _menuButton;
    public GameObject _gamePadLeft_Up;
    public GameObject _gamePadLeft_Down;
    public GameObject _gamePadRight_Up;
    public GameObject _gamePadRight_Down;

    #region Mono Behaviours

    void Update()
    {
        HandleInputForWorkerLeft();
        HandleInputForWorkerRight();
        HandleInputToRestartGame();
        HandleInputToOpenGameMenu();
        //HandleInputToSpeedGameUp();

        HandleGamePadUnpressed(_gamePadLeft_Down);
        HandleGamePadUnpressed(_gamePadLeft_Up);
        HandleGamePadUnpressed(_gamePadRight_Down);
        HandleGamePadUnpressed(_gamePadRight_Up);
    }    

    #endregion

    #region Public Methods

    // Handles a click on the quit button from the game menu
    public void GameMenu_StartNewGame_Click()
    {
        _gameMenuManager.StartNewGameClickHandler();
    }

    // Handles a click on the resume button of the game menu
    public void GameMenu_Resume_Click()
    {
        _gameMenuManager.ResumeButtonClickHandler();
    }

    #endregion

    #region Private Methods

    // Handles the game menu button click
    void HandleInputToOpenGameMenu()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == _menuButton)
            {                
                _gameMenuManager.InGameMenuButtonClickHandler();
            }
        }
    }

    // Handles the input that will restart the game
    void HandleInputToRestartGame()
    {
            // Look for user pressing the space bar key
            if (Input.GetKeyDown(Controls.gameRestart) && _gameManager.CurrentState == GameState.GameOver)
            {
                // Start a new game
                _gameMenuManager.RestartAfterGameOverClickHandler();
            }
    }

    // Applys input (if any) to the left worker
    void HandleInputForWorkerLeft()
    {
        // Check the worker is active
        if (_workerLeft.GetComponent<WorkerController>().Active)
        {
            // Look for the user pressing a key and handle the worker movement
            if (Input.GetKeyDown(Controls.workerLeft_UpKey) || GamePadPressed(_gamePadLeft_Up))
            {
                _workerLeft.GetComponent<WorkerController>().MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerLeft_DownKey) || GamePadPressed(_gamePadLeft_Down))
            {
                _workerLeft.GetComponent<WorkerController>().MoveWorkerDown();
            }
        }
    }

    // Applys input (if any) to the right worker
    void HandleInputForWorkerRight()
    {
        // Check the worker is active
        if (_workerRight.GetComponent<WorkerController>().Active)
        {
            // Look for the user pressing a key and handle the worker movement
            if (Input.GetKeyDown(Controls.workerRight_UpKey) || GamePadPressed(_gamePadRight_Up))
            {
                _workerRight.GetComponent<WorkerController>().MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerRight_DownKey) || GamePadPressed(_gamePadRight_Down))
            {
                _workerRight.GetComponent<WorkerController>().MoveWorkerDown();
            }
        }
    }

    bool GamePadPressed(GameObject gamePadButton)
    {
        if (Input.GetMouseButtonDown(0) && MouseOverGamePad(gamePadButton))
        {
            // Make the game pad button opaque
            Color currentColor = gamePadButton.GetComponent<SpriteRenderer>().color;
            gamePadButton.GetComponent<SpriteRenderer>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.75f);

            return true;
        }

        return false;
    }

    bool HandleGamePadUnpressed(GameObject gamePadButton)
    {
        if (Input.GetMouseButtonUp(0) && MouseOverGamePad(gamePadButton))
        {
            // Make the game pad button opaque
            Color currentColor = gamePadButton.GetComponent<SpriteRenderer>().color;
            gamePadButton.GetComponent<SpriteRenderer>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);

            return true;
        }

        return false;
    }

    // States if the mouse is over the game pad button passed in
    bool MouseOverGamePad(GameObject gamePadButton)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gamePadButton)
        {
            return true;
        }  
        else
        {
            return false;
        }
    }

    #endregion
}