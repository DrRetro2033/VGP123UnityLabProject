using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Animation;

//ensures that these components are attached to the gameobject
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public bool TestMode = false;

    //Components
    Rigidbody2D rb;
    SpriteRenderer sr;
    BoxCollider2D collider;
    Animator ani_hander;
    BoxCollider2D box_collider;

	public float speed = 5.0f;
    private const float jump_force = 10.0f;
	private const float gravity_scale = 2.0f;
    private bool can_jump = false;
	private bool mouth_full_of_air = false;
	// Start is called before the first frame update
	void Start()
    {
        //Component references grabbed through script
        rb = GetComponent<Rigidbody2D>();
        box_collider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
		ani_hander = GetComponent<Animator>();
		collider = GetComponent<BoxCollider2D>();
	}

    // Update is called once per frame
    void Update()
    {
		Debug.DrawRay(collider.bounds.center, Vector2.down * (collider.bounds.extents.y + 0.06f));
		float xInput = Input.GetAxis("Horizontal");
		if (xInput != 0) {
			float force = speed;
			if (mouth_full_of_air)
			{
				force /= 2;
			}
			rb.AddForce(new Vector2(xInput * force, 0));
		}
		if (IsGrounded())
		{
			ani_hander.SetBool("is_grounded", true);
		}
		else
		{
			ani_hander.SetBool("is_grounded", false);
		}
		Breath();
		if (mouth_full_of_air)
		{
			rb.gravityScale = gravity_scale / 2;
		}
		else
		{
			rb.gravityScale = gravity_scale;
		}
		AnimatorStateInfo state = ani_hander.GetCurrentAnimatorStateInfo(0);
		if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || mouth_full_of_air) && (!state.IsName("HeavyKirby") &&d !state.IsName("inhale_on_ground")))
        {
			ani_hander.SetTrigger("jump");
			rb.velocity = new Vector2(rb.velocity.x, 0);
			float force = jump_force;
			if (mouth_full_of_air)
			{
				force /= 2;
			}
			rb.AddForce(new Vector2(0, force),ForceMode2D.Impulse);
        }
		rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, speed), rb.velocity.y);
		if (xInput != 0) { sr.flipX = (xInput < 0); }
		ani_hander.SetFloat("x_velocity", xInput);
	}

	private void Breath()
	{
		if (Input.GetAxis("Vertical") > 0 && !IsGrounded() && !mouth_full_of_air)
		{
			mouth_full_of_air = true;
			ani_hander.ResetTrigger("exhale");
			ani_hander.SetTrigger("inhale");
		}
		else if (Input.GetAxis("Vertical") < 0 && IsGrounded() && !mouth_full_of_air)
		{
			mouth_full_of_air = true;
			ani_hander.ResetTrigger("exhale");
			ani_hander.SetTrigger("inhale");
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) && mouth_full_of_air)
		{
			mouth_full_of_air = false;
			ani_hander.ResetTrigger("inhale");
			ani_hander.SetTrigger("exhale");
		}
	}

	private bool IsGrounded()
    {
		Physics2D.queriesStartInColliders = false;
		bool isGrounded;
		RaycastHit2D hit = Physics2D.Raycast(collider.bounds.center, Vector2.down, collider.bounds.extents.y + 0.1f);
		
		if (hit.collider != null)
		{
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}
		return isGrounded;
	}
}
