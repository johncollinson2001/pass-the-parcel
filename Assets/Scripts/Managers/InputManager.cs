using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager _gameManager; 
    public GameObject _workerLeft;
    public GameObject _workerRight;
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
        _gameManager.StartNewGame();
    }

    // Handles a click on the resume button of the game menu
    public void GameMenu_Resume_Click()
    {
        _gameManager.CloseGameMenu();
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
                _gameManager.OpenGameMenu();
            }
        }
    }

    // Handles the input that will restart the game
    void HandleInputToRestartGame()
    {
        // Check the game is in game over state
        if(_gameManager.CurrentState == GameState.GameOver)
        {
            // Look for user pressing the space bar key
            if (Input.GetKeyDown(Controls.gameRestart))
            {
                // Start a new game
                _gameManager.RestartAfterGameOver();
            }
        }
    }

    // Applys input (if any) to the left worker
    void HandleInputForWorkerLeft()
    {
        // Check the worker is active
        if (_workerLeft.GetComponent<WorkerController>().Active)
        {
            // Look for the user pressing a key and handle the worker movement
            if (Input.GetKeyDown(Controls.workerLeft_UpKey))
            {
                _workerLeft.GetComponent<WorkerController>().MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerLeft_DownKey))
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
            if (Input.GetKeyDown(Controls.workerRight_UpKey))
            {
                _workerRight.GetComponent<WorkerController>().MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerRight_DownKey))
            {
                _workerRight.GetComponent<WorkerController>().MoveWorkerDown();
            }
        }
    }

    #endregion
}