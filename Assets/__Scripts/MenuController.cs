using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    
    #region == Private Variables == 
    private static bool isGamePaused = false;

    [SerializeField]
    private GameObject pauseMenuUI;

    [SerializeField]
    private GameObject gameOverMenuUI;

    [SerializeField]
    private GameObject tutorialMenuUI;

    [SerializeField]
    private Text currentScoreText;

    [SerializeField]
    private Text highScoreText;

    [SerializeField]
    private Text timePlayerText;

    [SerializeField]
    private Toggle soundToggle;

    // Singleton design pattern to get instance of class in PlayerCollider.cs
    public static MenuController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start ()
    {
        // Add listener for when the state of the Toggle changes, to take action
        // Code adapted from: https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.Toggle-onValueChanged.html
        soundToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(soundToggle);
        });

        // Check if sound option has been saved before.
        // E.g if not the first time playing the game, or sound has never been turned off.
        // If so then change toggle switch to false if sound is off.
        // If not then set playerPref to true, for again.
        if (PlayerPrefs.HasKey("Sound"))
        {
            bool toggle = Convert.ToBoolean(PlayerPrefs.GetString("Sound"));

            if (!toggle)
            {
                soundToggle.isOn = false;
            }
        }
        else
        {
            PlayerPrefs.SetString("Sound", soundToggle.isOn.ToString());
        }
        
        //Initiate a boolean flag to false, to define if the player was already revived or not
        PlayerPrefs.SetString("Revive", Boolean.FalseString);

        //Initiate this boolean with false value, to indicate the game is started
        isGamePaused = false;
    }

    // Update is called once per frame
    void Update () {
        // Get esc key input from keyboard, to pause game from keyboard entry
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        //Start time counter if the game is started.
        if (!isGamePaused)
        {
            LaunchTimeCounter();
        }
    }

    // Resumes game if called
    public void ResumeGame()
    {
        // Turn off the menu UI
        pauseMenuUI.SetActive(false);

        // Start the game running again
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    // Pauses game if called
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void LaunchTimeCounter()
    {
        if (TimeController.Instance != null) { 
            //Check if the timer decreased to zero value, then game is over.
            if (TimeController.Instance.CountDownTimer() == 0)
            {
                GameOverDisplay();
            }
        }
    }

    #region == recim-end == 
    //////////////////////////recim-end/// ///////////////////// 
    /////Variable contenant le score : recimScore 
    /////Variable contenant si le joueur a revive ou non  : recimRevive
    /////Variable contenant le temps que le joueur a joué pour sa partie : recimTime
    public void GameOverDisplay()
    {
        gameOverMenuUI.SetActive(true);
        isGamePaused = true;

        // Check if score has been saved before.
        if (PlayerPrefs.HasKey("Score"))
        {
            currentScoreText.text = ScoreController.Instance.PlayerScore.ToString();
            highScoreText.text = PlayerPrefs.GetInt("Score").ToString();
        }
        else
        {
            PlayerPrefs.SetInt("Score", 0);
        }

        // Save final player time.
        int timeplayer = TimeController.Instance.PlayerTime;
        timePlayerText.text = timeplayer.ToString();
        PlayerPrefs.SetInt("Time", timeplayer);

        //Rec'im variables :
        bool recimRevive = Convert.ToBoolean(PlayerPrefs.GetString("Revive"));
        int recimScore = PlayerPrefs.GetInt("Score");
        int recimTime = PlayerPrefs.GetInt("Time");
    }
    #endregion

    // Displays tutorial screen
    public void DisplayTutorial()
    {
        tutorialMenuUI.SetActive(true);
    }

    // Closes tutorial screen
    public void CloseTutorial()
    {
        tutorialMenuUI.SetActive(false);
    }

    //Output the new state of the Toggle into Text
    void ToggleValueChanged(Toggle change)
    {
        PlayerPrefs.SetString("Sound", soundToggle.isOn.ToString());
    }
    #endregion

    #region == recim-revive == 
    /////////////////////// ///recim-revive//////////////////////// 
    public void ReplayGame()
    {
        bool revive = Convert.ToBoolean(PlayerPrefs.GetString("Revive"));

        //Check if the player was already restarted the game
        if (!revive)
        {
            //Override this part with adding Rec'im video script into "isVideoWatched" method, to allow the restart from the last position.
            //Check if the player watched the marketing Rec'im video
            if (isVideoWatched()) 
            {
                isGamePaused = false;
                gameOverMenuUI.SetActive(false);

                //Setting the revive var to true
                PlayerPrefs.SetString("Revive", Boolean.TrueString);

                //Reatart the game, with a new rocket position.
                GameController.Instance.RestartWithLastPosition();
            }
        }
        else 
        {
            //Otherwise, the player will restart from the beggining.
            ResetGame();
        }

        Time.timeScale = 1f;
        isGamePaused = false;
    }

    //To override with Rec'im video script
    private Boolean isVideoWatched() {
        return true;
    }

    #endregion
    // Resets game if called 
    public void ResetGame()
    {
        // Clean up
        PlayerPrefs.DeleteKey("LevelSwitch");

        // Reset movement and rotation speed of cirlces, sliders and squares on reset.
        DifficultyController dc = new DifficultyController();
        dc.ResetMovementSpeed();
        dc.ResetRotationSpeed();

        // Reload current scene abd pause game.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Navigate the player back to the home menu
    public void GoToHomeMenu()
    {
        // Reset movement and rotation speed of cirlces, sliders and squres on reset.
        new DifficultyController().ResetRotationSpeed();

        SceneManager.LoadScene("HomeMenu", LoadSceneMode.Single);
        Time.timeScale = 1f;
        isGamePaused = true;
    }

    #region == recim-start == 
    /////////////////////// ///recim-start/// ///////////////////// 
    // Load the intial game
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }
    #endregion

    // Quit the game.
    public void QuitGame()
    {
        Application.Quit();
    }
}