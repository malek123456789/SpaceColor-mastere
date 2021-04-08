using UnityEngine;

public class ColourManager : MonoBehaviour
{
    [SerializeField]
    public Sprite rocketBlue;

    [SerializeField]
    public Sprite rocketYellow;

    [SerializeField]
    public Sprite rocketGreen;

    [SerializeField]
    public Sprite rocketRed;

    [SerializeField]
    private GameObject flameBlue;

    [SerializeField]
    private GameObject flameYellow;

    [SerializeField]
    private GameObject flameGreen;

    [SerializeField]
    private GameObject flameRed;

    #region == Private Variables == 

    // Four colours used to change the current player colour.
    private Sprite[] sprites = new Sprite[4];

    // Array used to set the tag for the player to collide with obstacles.
    private string[] colourOptions = new string[4] { "GreenTag", "YellowTag", "BlueTag", "RedTag" };

    [SerializeField]
    private SpriteRenderer sr;



    #endregion

    // Singleton design pattern to get instance of class in PlayerCollider.cs
    public static ColourManager Instance { get; private set; }
    void Init()
    {
        //Filling sprites array with the four tags one tage per colour.
        sprites.SetValue(rocketGreen, 0);

        sprites.SetValue(rocketYellow, 1);

        sprites.SetValue(rocketBlue, 2);

        sprites.SetValue(rocketRed, 3);

    }

    public void Update()
    {
        if (sr.sprite == sprites[0])
        {
            flameGreen.SetActive(true);
            flameYellow.SetActive(false);
            flameBlue.SetActive(false);
            flameRed.SetActive(false);
        }
        else if (sr.sprite == sprites[1])
        {
            flameGreen.SetActive(false);
            flameYellow.SetActive(true);
            flameBlue.SetActive(false);
            flameRed.SetActive(false);
        }
        else if (sr.sprite == sprites[2])
        {
            flameGreen.SetActive(false);
            flameYellow.SetActive(false);
            flameBlue.SetActive(true);
            flameRed.SetActive(false);
        }
        else if (sr.sprite == sprites[3])
        {
            flameGreen.SetActive(false);
            flameYellow.SetActive(false);
            flameBlue.SetActive(false);
            flameRed.SetActive(true);
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Init();
            Instance = this;
        }
    }

    public void setPlayerColour()
    {
        // Get a random index between 1 and 4
        int index = Random.Range(0, sprites.Length);

        // Check if the new random colour is the current player colour, if so call again until it's different (Recursion).
        // This is used when swapping colours so the player always gets a new random colour.
        if (sprites[index] == sr.sprite)
        {
            setPlayerColour();
        }
        else
        {
            // Set the colour to one of the determined colours in Unity
            sr.sprite = sprites[index];

            // Set the tag of the SpriteRenderer to a colour in the string array.
            // This will be retrieved in the PlayerCollider script.
            sr.tag = colourOptions[index];
        }

    }

}
