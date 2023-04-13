using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{   
    // Stack
    [Header("Stack")]
    public GameObject   stack;
    public GameObject   lastStack;
    public Transform    stackSpawner1;
    public Transform    stackSpawner2;
    public float        stackSpeedIncrementValue;


    // UI
    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI scoreCommendText;
    public GameObject      mainMenu;
    public GameObject      optionsMenu;
    public GameObject      gameOverMenu;


    // Score Commend Text - Colors
    [Header("Score Commend Color")]
    public Color poorCommendtxt_Color;
    public Color niceCommendtxt_Color;
    public Color greatCommendtxt_Color;



    private GameObject currentStack     = null;
    private Transform  selectedSpawner  = null;

    private Camera cam;


    private int  _score             = 0;
    private bool _hasGameStarted    = false;
    private bool _isGamePaused      = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Click to start the Game
        if (Input.GetMouseButtonDown(0))
        {            
            if (!_hasGameStarted)
            {
                _hasGameStarted = true;

                // Close the main menu
                mainMenu.SetActive(false);

                // Enable score text
                scoreText.gameObject.SetActive(true);
                scoreCommendText.gameObject.SetActive(true);

                // Reset the speed of the stacks
                Stack.speed = 5;

                // Spawn the first stack
                SpawnStack();
            }
            // Game has started
            else if (_hasGameStarted)
            {
                if (currentStack != null)
                {
                    // stop moving the previous stack
                    currentStack.GetComponent<Stack>().MoveStack = false;
                    currentStack.transform.parent = null;

                    // calculate hangover value
                    float hangOver = 0;
                    float stackSize = 0;

                    // calculate hangover & stack size value based on stack direction
                    if (!Stack.changeDirection)
                    {
                        hangOver = currentStack.transform.position.z - lastStack.transform.position.z;
                        stackSize = lastStack.transform.localScale.z;
                    }
                    else if (Stack.changeDirection)
                    {
                        hangOver = currentStack.transform.position.x - lastStack.transform.position.x;
                        stackSize = lastStack.transform.localScale.x;
                    }                    

                    // check if player has went over limit
                    if (Mathf.Abs(hangOver) >= stackSize)
                    {
                        GameOver();
                    }
                    else
                    {
                        // Check stack accuracy percentage
                        TriggerScoreCommendText(hangOver, stackSize);                    

                        // Increment the score
                        IncrementScore();

                        // Otherwise, continue the game
                        // and split the stack
                        if (!Stack.changeDirection)
                        {
                            SplitStackZ(hangOver);
                        }
                        else
                        {
                            SplitStackX(hangOver);
                        }
                            
                        // make the current stack
                        // as the new stack
                        lastStack = currentStack;

                        // spawn the next stack
                        SpawnStack();

                        // Move the position of the camera upwards 
                        // each time a new stack is spawned.
                        cam.GetComponent<CameraBehaviour>().MovePosition();
                    }                    
                }
            }
        }
        
        // Pause the Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    // Spawns a new stack at a spawner
    void SpawnStack()
    {
        // move the spawners up
        stackSpawner1.transform.position += Vector3.up;
        stackSpawner2.transform.position += Vector3.up;        

        // change stack spawner
        if (selectedSpawner == stackSpawner1)
        {
            selectedSpawner = stackSpawner2;

            // shift the spawner to be at the same Z-axis as the last stack
            selectedSpawner.transform.position = new Vector3(selectedSpawner.transform.position.x, selectedSpawner.transform.position.y, lastStack.transform.position.z);

            // Change direction of the stack to move in X-axis
            Stack.changeDirection = true;
        }
        else
        {
            selectedSpawner = stackSpawner1;

            // shift the spawner to be at the same X-axis as the last stack
            selectedSpawner.transform.position = new Vector3(lastStack.transform.position.x, selectedSpawner.transform.position.y, selectedSpawner.transform.position.z);            

            // Change direction of the stack to move in Z-axis
            Stack.changeDirection = false;
        }

        // spawn stack
        currentStack = Instantiate(stack, selectedSpawner.transform.position, Quaternion.identity);

        // make the stack size to be the same as the last stack size
        currentStack.transform.localScale = lastStack.transform.localScale;

        // Increment the speed of the stack
        Stack.speed += stackSpeedIncrementValue;

        // Move the stack once it spawns
        currentStack.GetComponent<Stack>().MoveStack = true;
    }

    void SplitStackZ(float hangOver)
    {
        float newStackSize = lastStack.transform.localScale.z - Mathf.Abs(hangOver);
        float cutoffBlockSize = currentStack.transform.localScale.z - newStackSize;

        // Change the scale of the current stack and move it
        currentStack.transform.localScale = new Vector3(currentStack.transform.localScale.x, 1, newStackSize);
        currentStack.transform.position = new Vector3(currentStack.transform.position.x, currentStack.transform.position.y, currentStack.transform.position.z - hangOver / 2);

        // Calculate and pass in the cutoff block position
        float cutOffBlockPositionZ = 0;

        // Determines the side where the cutOff block will spawn
        if (hangOver > 0)
        {
            cutOffBlockPositionZ = currentStack.transform.position.z + lastStack.transform.localScale.z / 2;
        }
        else if (hangOver < 0)
        {
            cutOffBlockPositionZ = currentStack.transform.position.z - lastStack.transform.localScale.z / 2;
        }

        // Spawn the cutoff block        
        SpawnDropCubeZ(cutOffBlockPositionZ, cutoffBlockSize);
    }

    void SpawnDropCubeZ(float cutOffBlockPositionZ, float cutOffBlockSize)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        cube.transform.localScale = new Vector3(currentStack.transform.localScale.x, 1, cutOffBlockSize);
        cube.transform.position   = new Vector3(currentStack.transform.position.x, currentStack.transform.position.y, cutOffBlockPositionZ);

        cube.AddComponent<Rigidbody>();
        cube.GetComponent<BoxCollider>().enabled = false;
    }

    void SplitStackX(float hangOver)
    {
        float newStackSize = lastStack.transform.localScale.x - Mathf.Abs(hangOver);
        float cutoffBlockSize = currentStack.transform.localScale.x - newStackSize;

        // Change the scale of the current stack and move it
        currentStack.transform.localScale = new Vector3(newStackSize, 1, currentStack.transform.localScale.z);
        currentStack.transform.position = new Vector3(currentStack.transform.position.x - hangOver / 2, currentStack.transform.position.y, currentStack.transform.position.z);

        // Calculate and pass in the cutoff block position
        float cutOffBlockPositionX = 0;

        // Determines the side where the cutOff block will spawn
        if (hangOver > 0)
        {
            cutOffBlockPositionX = currentStack.transform.position.x + lastStack.transform.localScale.x / 2;
        }
        else if (hangOver < 0)
        {
            cutOffBlockPositionX = currentStack.transform.position.x - lastStack.transform.localScale.x / 2;
        }

        // Spawn the cutoff block        
        SpawnDropCubeX(cutOffBlockPositionX, cutoffBlockSize);
    }

    void SpawnDropCubeX(float cutOffBlockPositionX, float cutOffBlockSize)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        cube.transform.localScale = new Vector3(cutOffBlockSize, 1, currentStack.transform.localScale.z);
        cube.transform.position = new Vector3(cutOffBlockPositionX, currentStack.transform.position.y, currentStack.transform.position.z);

        cube.AddComponent<Rigidbody>();
        cube.GetComponent<BoxCollider>().enabled = false;
    }

    void IncrementScore()
    {
        _score++;

        // Display the score UI
        scoreText.text = _score.ToString();

        // Trigger score animation
        scoreText.GetComponent<Animator>().SetTrigger("GainScore");
    }

    void TriggerScoreCommendText(float hangOver, float stackSize)
    {
        // Get the percentage value to check precision
        float percentageValue = (stackSize - Mathf.Abs(hangOver)) / stackSize;


        // Display the corresponding text based on precision value
        switch (percentageValue)
        {
            case <= 0.5f:

                scoreCommendText.text = "Uh-Oh...";
                scoreCommendText.color = poorCommendtxt_Color;
                break;

            case <= 0.95f:

                scoreCommendText.text = "Nice!";
                scoreCommendText.color = niceCommendtxt_Color;
                break;

            case <= 1f:

                scoreCommendText.text = "GREAT!!";
                scoreCommendText.color = greatCommendtxt_Color;
                break;
        }

        // trigger commend text animation
        scoreCommendText.GetComponent<Animator>().SetTrigger("GainScore");
        
    }

    // ======================================== Menus ======================================== //
    public void PauseGame()
    {
        optionsMenu.SetActive(true);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        optionsMenu.SetActive(false);

        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GameOver()
    {
        // Show game over screen
        gameOverMenu.SetActive(true);

        Time.timeScale = 0;

        // show the final score
        finalScoreText.text = scoreText.text;
    }
}
