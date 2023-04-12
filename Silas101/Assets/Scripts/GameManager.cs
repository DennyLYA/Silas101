using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{   
    // Stack
    public GameObject   stack;
    public Transform    stackSpawner1;
    public Transform    stackSpawner2;

    // UI
    public TextMeshProUGUI scoreText;
    public GameObject      mainMenu;
    public GameObject      optionsMenu;


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
        if (Input.GetMouseButtonDown(0))
        {
            // Click to start the Game
            if (!_hasGameStarted)
            {
                _hasGameStarted = true;

                // Close the main menu
                mainMenu.SetActive(false);

                // Enable score text
                scoreText.enabled = true;

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

                    // spawn the next stack
                    SpawnStack();

                    // Move the position of the camera upwards 
                    // each time a new stack is spawned.
                    cam.GetComponent<CameraBehaviour>().MovePosition();
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
        // change stack spawner
        if (selectedSpawner == stackSpawner1)
        {
            selectedSpawner = stackSpawner2;
        }
        else
        {
            selectedSpawner = stackSpawner1;
        }

        // spawn stack
        currentStack = Instantiate(stack, selectedSpawner);

        // Move the stack once it spawns
        //currentStack.transform.parent = null;
        currentStack.GetComponent<Stack>().MoveStack = true;

        MoveSpawners();
    }
    
    // Move the spawners
    void MoveSpawners()
    {
        stackSpawner1.transform.position += Vector3.up;
        stackSpawner2.transform.position += Vector3.up;
    }

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
}
