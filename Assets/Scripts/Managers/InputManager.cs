using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager _gameManager; 
    public GameObject _workerLeft;
    public GameObject _workerRight;

    #region Mono Behaviours

    void Update()
    {
        HandleInputForWorkerLeft();
        HandleInputForWorkerRight();
        HandleInputToRestartGame();
        //HandleInputToSpeedGameUp();
    }    

    #endregion

    #region Private Methods

    // Handles the input that will restart the game
    void HandleInputToRestartGame()
    {
        // Check the game is not active
        if(!_gameManager.IsActiveGame)
        {
            // Look for user pressing the space bar key
            if (Input.GetKeyDown(Controls.gameRestart))
            {
                // Start a new game
                _gameManager.StartNewGame();
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