using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region == Private Variables == 

    [SerializeField]
    private GameObject circle;

    [SerializeField]
    private GameObject sliderRight;

    [SerializeField]
    private GameObject sliderLeft;

    [SerializeField]
    private GameObject square;

    [SerializeField]
    private GameObject colourSwapper;

    [SerializeField]
    private GameObject scoreRock;

    [SerializeField]
    private GameObject timeRock;

    // Previous gameObject postion
    private Transform prevPosition;

    // parent gameObject container
    private GameObject parent;

    // player gameObject (Rocket gameObject)
    private GameObject player;

    // Number of the obstacles which the time bonus rock will be created
    private int numberObstacle;
    #endregion

    //Number of obstacles which after the blue time rock will be created
    public const int MAX_ROCK_TIME = 6;

    // Singleton design pattern to get instance of class in PlayerCollider.cs
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Use it for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        initGame();
    }

    public void initGame()
    {
        // Set up parent game object container
        parent = GameObject.Find("GameObjectContainer");

        // Set level switch to 0 to load circles, and create inital game objects
        PlayerPrefs.SetInt("LevelSwitch", 0);
        PlayerPrefs.SetInt("Obstacle", 0);

        //Initiliaze 
        numberObstacle = SetNewNumberObstacles();

        createGameObjects();

        // Set initial player colour
        ColourManager.Instance.setPlayerColour();
    }

    // Create all core game ojects (Circles, sliders, squares, Colour Swappers and rocks: time and score)
    public void createGameObjects()
    {
        // Get the type of level to load, 0 = circles, 1 = sliders and 2 = squares
        int levelSwitch = PlayerPrefs.GetInt("LevelSwitch");
        if (levelSwitch == 0)
        {
            Vector2 circlePos = new Vector2(0, 0);
            GameObject newCircle = null;
            // == CIRCLE ==            
            // If very first circle then set manually.
            if (prevPosition == null)
            {
                circlePos = new Vector2(0, 1.2f);
            }
            else
            {
                // Set the position for new circle using the previous location.
                // Add 5 to positon it correctly above last gameObject.
                circlePos = prevPosition.position;
                circlePos.x = 0;
                circlePos.y += 5f;
            }

            // Instantiate new circle using position and add to parent container.
            newCircle = Instantiate(circle, circlePos, Quaternion.identity);
            newCircle.transform.SetParent(parent.transform, false);

            // == ROCK  == 
            // Instantiate new score or time rock using same position and add to parent container. As rocks are in the center of circles
            CreateRockObject(circlePos);

            // Set prevPosition to the current slider, for next componant.
            prevPosition = newCircle.transform;

            // == COLOUR SWAPPER ==
            // Create a position for the new Colour Swapper using the same postion plus 2.75
            circlePos.y += 2.75f;

            Instantiate(colourSwapper, circlePos, Quaternion.identity).transform.SetParent(parent.transform, false);

            // Finally set the old position of the circle to the new position for following calls.
            // So that they spawn correctly in order.
            prevPosition = newCircle.transform;

            // Set randomy the level switch so it will load circles, sliders or squares on next call.
            PlayerPrefs.SetInt("LevelSwitch", UnityEngine.Random.Range(0, 3));

            // == DIFFICULTY == 
            // Increase next componant speed.
            IncreaseSpeedComponant(levelSwitch);
        }
        else if (levelSwitch == 1)
        {
            Vector2 sliderPos = new Vector2(0, 0);
            GameObject newSlider = null;

            // == SLIDER == 
            // Create randomly two or three sliders equaly spaced from each other
            int numbSlider = UnityEngine.Random.Range(2, 4);
            for (int i = 0; i < numbSlider; i++)
            {
                // Get the previous gameObject postion and set x so it starts off screen.
                sliderPos = prevPosition.position;
                if (i == 1)
                    sliderPos.x = 8.45f;
                else
                    sliderPos.x = 2.81f;

                // If first slider, set sligthly higher so it's not too close to the colour swapper.
                if (i == 0)
                    sliderPos.y += 4.5f;
                else
                    sliderPos.y += 3f;

                if (i == 1)
                    newSlider = Instantiate(sliderLeft, sliderPos, Quaternion.identity);
                else
                    newSlider = Instantiate(sliderRight, sliderPos, Quaternion.identity);

                newSlider.transform.SetParent(parent.transform, false);

                // == ROCK == 
                // Add a rock score to the parent in the center only for the first two sliders.
                if (i < 1)
                {
                    // Set back to 0 so rock is centered.
                    sliderPos.x = 0;
                    sliderPos.y += 1.5f;
                    CreateRockObject(sliderPos);
                }

                // Set prevPosition to the current slider, for next slider or next level.
                prevPosition = newSlider.transform;
            }

            // == COLOUR SWAPPER == 
            // Set the position rock to the center, and move up.
            sliderPos.x = 0;
            sliderPos.y += 3f;

            Instantiate(colourSwapper, sliderPos, Quaternion.identity).transform.SetParent(parent.transform, false);

            // Finally set the old position to the new position for following calls.
            // So that they spawn correctly in order.
            prevPosition = newSlider.transform;

            // Set level switch so it will load circles on next call.
            PlayerPrefs.SetInt("LevelSwitch", UnityEngine.Random.Range(0, 3));

            // == DIFFICULTY == 
            // Increase next componant speed.
            IncreaseSpeedComponant(levelSwitch);
        }
        else
        {
            Vector2 squarePos = new Vector2(0, 0);
            GameObject newSquare = null;
            // == SQUARE == 
            // Set the position for new Square using the previous location.
            // Add five to positon it correctly above last gameObject.
            squarePos = prevPosition.position;
            squarePos.x = 0;
            squarePos.y += 5.5f;

            // Instantiate new square using position and add to parent container.
            newSquare = Instantiate(square, squarePos, Quaternion.identity);
            newSquare.transform.SetParent(parent.transform, false);

            // == ROCK == 
            // Instantiate new time or score rock using same position and add to parent container. As rocks are in the center of squares
            CreateRockObject(squarePos);

            // Set prevPosition to the current slider, for next slider or next level.
            prevPosition = newSquare.transform;

            // == COLOUR SWAPPER ==
            // Create a position for the new Colour Swapper using the same postion plus 2.75
            squarePos.y += 2.75f;

            Instantiate(colourSwapper, squarePos, Quaternion.identity).transform.SetParent(parent.transform, false);

            // Finally set the old position of the square to the new position for following calls.
            // So that they spawn correctly in order.
            prevPosition = newSquare.transform;

            // Set level switch so it will load squares on next call.
            PlayerPrefs.SetInt("LevelSwitch", UnityEngine.Random.Range(0, 3));

            // == DIFFICULTY == 
            // Increase next componant speed.
            IncreaseSpeedComponant(levelSwitch);
        }
    }

    // == DIFFICULTY == 
    // Increase componant speed to increase difficulty slightly.
    private void IncreaseSpeedComponant(int levelSwitch)
    {

        //Increase sliders movement speed
        if (levelSwitch == 1)
        {
            DifficultyController.MovementSpeed += 0.001f;
        }
        else
        {
            // Increase circle and square speed rotation for next level, depending if rotation is clockwise or anticlockwise.
            if (DifficultyController.RotationSpeed < 0)
                DifficultyController.RotationSpeed -= 11.5f;
            else
                DifficultyController.RotationSpeed += 11.5f;
        }
    }

    private void CreateRockObject(Vector2 position)
    {
        //Get the obstacles counter value
        int obstacle = PlayerPrefs.GetInt("Obstacle");

        //Check if it is time to create a time bonus object. 
        if (obstacle < numberObstacle)
        {
            //Create a new score red rock object and increment the obstacles counter.
            Instantiate(scoreRock, position, Quaternion.identity).transform.SetParent(parent.transform, false);
            obstacle++;
        }
        else
        {
            //Create a new time blue rock object and reinitialise obstacles counter to zero.
            Instantiate(timeRock, position, Quaternion.identity).transform.SetParent(parent.transform, false);
            numberObstacle = SetNewNumberObstacles();
            obstacle = 0;
        }

        //Setting the new obstacles counter value.
        PlayerPrefs.SetInt("Obstacle", obstacle);
    }

    private int SetNewNumberObstacles()
    {
        return UnityEngine.Random.Range(MAX_ROCK_TIME, MAX_ROCK_TIME + 2);
    }

    public void RestartWithLastPosition()
    {
        //Get the last componant position.
        float prevComponantPosition = prevPosition.position.y;

        //Get the last player position.
        float playerPosition = player.gameObject.transform.localPosition.y;

        float newPosition = Math.Abs(prevComponantPosition - playerPosition) > 3 ? playerPosition - 1.8f : prevComponantPosition - 2.55f;
        
        //Setting the rocket body type to static in order to lock the player movement, after resarting game.
        player.gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        
        //Setting the new rocket position.
        player.transform.position = new Vector2(0, newPosition);
    }

    void OnApplicationQuit()
    {
        // Clean up
        PlayerPrefs.DeleteKey("LevelSwitch");
    }
}
