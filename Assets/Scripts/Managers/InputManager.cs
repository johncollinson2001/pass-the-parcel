using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public WorkerController _workerLeft;
    public WorkerController _workerRight;
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
            if (Input.GetKeyDown(Controls.workerLeft_UpKey) || GamePadPressed(_gamePadLeft_Up))
            {
                _workerLeft.MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerLeft_DownKey) || GamePadPressed(_gamePadLeft_Down))
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
            if (Input.GetKeyDown(Controls.workerRight_UpKey) || GamePadPressed(_gamePadRight_Up))
            {
                _workerRight.MoveWorkerUp();
            }
            else if (Input.GetKeyDown(Controls.workerRight_DownKey) || GamePadPressed(_gamePadRight_Down))
            {
                _workerRight.MoveWorkerDown();
            }
        }
    }

    bool GamePadPressed(GameObject gamePadButton)
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOverGamePad(gamePadButton))
        {
            // Make the game pad button opaque
            SpriteRenderer spriteRenderer = gamePadButton.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.75f);

            return true;
        }

        return false;
    }

    bool HandleGamePadUnpressed(GameObject gamePadButton)
    {
        // Check the game is active and being played by a human
        if (GameManager.Instance.CurrentState == GameState.Active && GameManager.Instance.Player.IsHuman)
        {
            // Check the user has lifted the mouse button and over the game pad too
            if (Input.GetMouseButtonUp(0) && IsMouseOverGamePad(gamePadButton))
            {
                // Make the game pad button opaque
                SpriteRenderer spriteRenderer = gamePadButton.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);

                return true;
            }
        }

        return false;
    }

    // States if the mouse is over the game pad button passed in
    bool IsMouseOverGamePad(GameObject gamePadButton)
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