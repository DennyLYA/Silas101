using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public float speed;
    public float minBoundary, maxBoundary;

    private bool _moveStack         = false;
    private bool _changeDirection   = false;

    private Color    _stackColor;
    private Renderer _stackRenderer;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeStackColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveStack)
        {
            if (!_changeDirection)
            {
                // Keep moving in one direction
                transform.position += transform.parent.forward * speed * Time.deltaTime;

                // Check if stack has gone out of boundary
                if (transform.localPosition.z >= maxBoundary)
                {
                    // Change Direction
                    _changeDirection = true;
                }
            }
            else
            {
                // Keep moving in the other direction once the boundary is hit
                transform.position += -transform.parent.forward * speed * Time.deltaTime;

                // Check if stack has gone out of boundary
                if (transform.localPosition.z <= minBoundary)
                {
                    // Change Direction
                    _changeDirection = false;
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

}
