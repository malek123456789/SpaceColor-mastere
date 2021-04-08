using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{

    #region == Private Variables ==

    [SerializeField]
    private Text TimeText;

    private float gameTimer = 0.0f;

    private float timer = 0.0f;

    #endregion

    private int playerTime = 0;
    public int PlayerTime { get { return playerTime; } }

    //Default timer value (120 seconds)
    public const int TIMING = 120;

    //Default time bonus value (10 seconds)
    public const float TIME_BONUS = 10.0f;

    // Singleton design pattern to get instance of class in PlayerCollider.cs
    public static TimeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // subscribe to a blue rock collected event and add the bonus time
    private void OnEnable()
    {
        // subscribe
        PlayerCollider.TimeCollectedEvent += HandleTimeCollectedEvent;
    }

    private void OnDisable()
    {
        // unsubscribe
        PlayerCollider.TimeCollectedEvent -= HandleTimeCollectedEvent;
    }

    //Start the player time and the time decounter
    public int CountDownTimer() {
        timer -= Time.deltaTime;
        gameTimer += Time.deltaTime;
        playerTime = (int)gameTimer % 1000;
        int result = ConvertInSeconds(timer);
        UpdateTimeField(result);
        return result;
    }

    //Convert milliseconds to seconds 
    private int ConvertInSeconds(float time) {
        int timeSeconds = (int) timer % 1000;
        return timeSeconds + TIMING;
    }

    // Add to Timer when event is fired.
    private void HandleTimeCollectedEvent(Collider2D collision)
    {
        // Adding ten seconds to the timer
        timer += TIME_BONUS;
    }

    public void UpdateTimeField(int time) {
        // Set the onscreen Time.
        // Check if score has been saved before.
        TimeText.text = time.ToString();
    }
}
