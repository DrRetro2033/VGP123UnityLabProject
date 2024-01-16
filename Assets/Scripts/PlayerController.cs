using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ensures that these components are attached to the gameobject
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public bool TestMode = false;

    //Components
    Rigidbody2D rb;
    SpriteRenderer sr;


    public float speed = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Component references grabbed through script
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); 

/*        if (speed <= 0)
        {
            speed = 7.0f;
            if (TestMode) Debug.Log("Speed has been set to a default value of 7.0f " + gameObject.name);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }
}
