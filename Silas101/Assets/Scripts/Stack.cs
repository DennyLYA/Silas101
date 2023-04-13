using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public static GameObject lastStack;
    public static float speed = 5;
    

    // Stack boundaries value
    public float minZBoundary, maxZBoundary;
    public float minXBoundary, maxXBoundary;
    

    private bool _moveStack         = false;
    private bool _reverseMovement   = false;
    private bool _changeDirection   = false;


    private Renderer _stackRenderer;


    // Start is called before the first frame update
    void Start()
    {
        if (lastStack == null)
        {
            lastStack = this.gameObject;
        }

        RandomizeStackColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveStack)
        {
            // Moves in the z direction
            if (!_changeDirection)
            {
                if (!_reverseMovement)
                {
                    // Keep moving in one direction
                    transform.position += transform.forward * speed * Time.deltaTime;

                    // Check if stack has gone out of boundary
                    if (transform.localPosition.z >= maxZBoundary)
                    {
                        // reverse movement
                        _reverseMovement = true;
                    }
                }
                else // move in the reverse direction
                {
                    // Keep moving in the other direction once the boundary is hit
                    transform.position += -transform.forward * speed * Time.deltaTime;

                    // Check if stack has gone out of boundary
                    if (transform.localPosition.z <= minZBoundary)
                    {
                        // reverse movement
                        _reverseMovement = false;
                    }
                }
            }
            // moves in the x direction
            else if (_changeDirection)
            {
                if (!_reverseMovement)
                {
                    // Keep moving in one direction
                    transform.position += transform.right * speed * Time.deltaTime;

                    // Check if stack has gone out of boundary
                    if (transform.localPosition.x >= maxXBoundary)
                    {
                        // reverse movement
                        _reverseMovement = true;
                    }
                }
                else // move in the reverse direction
                {
                    // Keep moving in the other direction once the boundary is hit
                    transform.position += -transform.right * speed * Time.deltaTime;

                    // Check if stack has gone out of boundary
                    if (transform.localPosition.x <= minXBoundary)
                    {
                        // reverse movement
                        _reverseMovement = false;
                    }
                }
            }
        }
    }

    // This function randomizes the stack color when it spawns
    void RandomizeStackColor()
    {
        _stackRenderer = GetComponent<Renderer>();

        _stackRenderer.material.color = new Color(Random.value, Random.value, Random.value);
    }

    // This property sets and determine whether the stack is moving
    public bool MoveStack
    {
        get { return _moveStack; }
        set { _moveStack = value; }
    }

    public bool ChangeDirection
    {
        get { return _changeDirection; }
        set { _changeDirection = value; }
    }
}
