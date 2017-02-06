using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public WorkerController _workerLeft;
    public WorkerController _workerRight;
    public GameObject _menuButton;

    #region Mono Behaviours

    void Update()
    {
        HandleInputForWorkerLeft();
        HandleInputForWorkerRight();
        HandleInputToRestartGame();
        HandleInputToOpenGameMenu();
        //HandleInputToSpeedGameUp();
    }    

    #endregion

    #region Public Methods

    // Handles a click on the quit button from the game menu
    public void GameMenu_StartNewGame_Click()
    {
        MenuManager.Instance.StartNewGame();
    }

    // Handles a click on the resume button of the game menu
    public void GameMenu_Resume_Click()
    {
        MenuManager.Instance.ResumeGame();
    }

    #endregion

    #region Private Methods

    // Handles the game menu button click
    void HandleInputToOpenGameMenu()
    {
        // Check for active game being played by a human
        if (GameManager.Instance.CurrentState == GameState.Active && GameManager.Instance.Player.IsHuman)
        {
            // Check for a mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                // See if the click was on the menu button
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject == _menuButton)
                {
                    MenuManager.Instance.OpenMenuDuringGame();
                }
            }
        }
    }

    // Handles the input that will restart the game
    void HandleInputToRestartGame()
    {
        // Look for user pressing the space bar key
        if (Input.GetKeyDown(Controls.gameRestart) && GameManager.Instance.CurrentState == GameState.GameOver)
        {
            // Open the menu
            MenuManager.Instance.OpenMenuWhenGameOver();
        }
    }

    // Applys input (if any) to the left worker
    void HandleInputForWorkerLeft()
    {
        // Check the game is currently being played by a human and the worker is active
        if (GameManager.Instance.CurrentState == GameState.Active && GameManager.Instance.Player.IsHuman && _workerLeft.Active)
        {
            // Look for the user pressing a key and handle the worker movement
            if (Input.GetKeyDown(Controls.workerLeft_UpKey))
            {
                _workerLeft.MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerLeft_DownKey))
            {
                _workerLeft.MoveWorkerDown();
            }
        }
    }

    // Applys input (if any) to the right worker
    void HandleInputForWorkerRight()
    {
        // Check the game is currently being played by a human and the worker is active
        if (GameManager.Instance.CurrentState == GameState.Active && GameManager.Instance.Player.IsHuman && _workerRight.Active)
        {
            // Look for the user pressing a key and handle the worker movement
            if (Input.GetKeyDown(Controls.workerRight_UpKey))
            {
                _workerRight.MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerRight_DownKey))
            {
                _workerRight.MoveWorkerDown();
            }
        }
    }

    #endregion
}