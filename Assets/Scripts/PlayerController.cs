using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static UnityEngine.Random;

public enum FOOD
{
	NONE,
	SOME,
	SWORD,
};

//ensures that these components are attached to the gameobject
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
	//Objects
	public GameObject SuckEffector;
	public GameObject MouthArea;
	public GameObject ProjectilePrefab;
	public GameObject Camera;
	public GameObject GUI;
	public Transform proj_offset;
	[Serializable]
	public struct AudioClips
	{
		public string name;
		public AudioClip sound;
	}
	[Serializable]
	public struct AbilityGainAnimation
	{
		public string name;
		public AnimationClip clip;
	}
	public AudioClips[] Sounds;
	public AbilityGainAnimation[] Abilitiy_Gain_Animations;
	//Components
	Rigidbody2D rb;
    SpriteRenderer sr;
    PolygonCollider2D collider;
    Animator ani_hander;
	AudioSource audio_source;
	public float speed = 5.0f;
	public int max_health = 6;
	public int lives = 3;
	int health = 6;
	private const float max_speed = 500.0f;
    private const float jump_force = 13.0f;
	private FOOD food_in_mouth = FOOD.NONE;
	private const float gravity_scale = 3.0f;
    private bool can_jump = false;
	private bool was_in_air = false;
	private bool mouth_full = false;
	private float last_known_vertical_input = 0.0f;
	// Start is called before the first frame update

	AudioClip GetSound(string name) {
		for(int i = 0; i < Sounds.Length; i++) { 
			AudioClips y = Sounds[i];
			if (y.name == name) {
				return y.sound;
			}
		}
		return null;
	}

	AnimationClip GetGainAnimation(string name)
	{
		for (int i = 0; i < Abilitiy_Gain_Animations.Length; i++)
		{
			AbilityGainAnimation y = Abilitiy_Gain_Animations[i];
			if (y.name == name)
			{
				return y.clip;
			}
		}
		return null;
	}
	void Start()
    {
        //Component references grabbed through script
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
		ani_hander = GetComponent<Animator>();
		collider = GetComponent<PolygonCollider2D>();
		SuckEffector.GetComponent<PointEffector2D>().enabled = false;
		MouthArea.GetComponent<CapsuleCollider2D>().enabled = false;
		audio_source = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update()
    {
		AnimatorStateInfo state = ani_hander.GetCurrentAnimatorStateInfo(0);
		if (state.IsName("damage") || state.IsTag("Swallow")) {
			return;
		}
		if (state.IsTag("Gain")) {
			rb.velocity = new Vector2(0, 0);
			Time.timeScale = 0;
			return;
		}
		Debug.DrawRay(collider.bounds.center, Vector2.down * (collider.bounds.extents.y + 0.06f));
		float xInput = Input.GetAxis("Horizontal");
		if (xInput != 0) {
			float force = speed*Time.deltaTime;
			if (mouth_full && food_in_mouth != FOOD.NONE)
			{
				force /= 2;
			}
			rb.AddForce(new Vector2(xInput * force, 0));
		}
		if (!state.IsName("inhale"))
		{
			if (IsGrounded()) {
				ani_hander.SetBool("is_grounded", true);
				if (was_in_air && rb.velocity.y <= Physics.gravity.y) {
					audio_source.PlayOneShot(GetSound("land_sfx"));
				}
				was_in_air = false;
			}
			else {
				ani_hander.SetBool("is_grounded", false);
				was_in_air = true;
			}
		}
		if (mouth_full && Input.GetKeyDown(KeyCode.S)){
			ani_hander.SetLayerWeight(0, 0.0f);
			ani_hander.SetLayerWeight(ani_hander.GetLayerIndex("Sword"), 1.0f);
			ani_hander.SetTrigger("swallow");
			mouth_full = false;
		}
		Breath();
		if (mouth_full)
		{
			rb.gravityScale = gravity_scale / 2;
		}
		else
		{
			rb.gravityScale = gravity_scale;
		}
		if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || mouth_full) && (!state.IsName("HeavyKirby") && !state.IsName("inhale_on_ground")))
        {
			ani_hander.SetTrigger("jump");
			rb.velocity = new Vector2(rb.velocity.x, 0);
			float force = jump_force;
			if (mouth_full)
			{
				force /= 2;
				audio_source.PlayOneShot(GetSound("jump_in_air_sfx"));
			}
			else {
				audio_source.PlayOneShot(GetSound("jump_sfx"));
			}
			rb.AddForce(new Vector2(0, force),ForceMode2D.Impulse);
        }
		rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x/2, -max_speed, max_speed), rb.velocity.y);
		if (xInput != 0) { sr.flipX = (xInput < 0); }
		ani_hander.SetFloat("x_velocity", xInput);
		if (sr.flipX)
		{
			SuckEffector.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
			SuckEffector.transform.localPosition = new Vector3(-Mathf.Abs(SuckEffector.transform.localPosition.x), SuckEffector.transform.localPosition.y, SuckEffector.transform.localPosition.z);
			MouthArea.transform.localPosition = new Vector3(-Mathf.Abs(MouthArea.transform.localPosition.x), MouthArea.transform.localPosition.y, MouthArea.transform.localPosition.z);
		}
		else
		{
			SuckEffector.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			SuckEffector.transform.localPosition = new Vector3(Mathf.Abs(SuckEffector.transform.localPosition.x), SuckEffector.transform.localPosition.y, SuckEffector.transform.localPosition.z);
			MouthArea.transform.localPosition = new Vector3(Mathf.Abs(MouthArea.transform.localPosition.x), MouthArea.transform.localPosition.y, MouthArea.transform.localPosition.z);
		}
	}

	public bool CaughtEnemy(GameObject enemy){
		audio_source.Stop();
		audio_source.PlayOneShot(GetSound("inhale_enemy_sfx"));
		food_in_mouth = enemy.GetComponent<EnemyBehavior>().type_of_food;
		Destroy(enemy);
		print("Yum Yum!");
		mouth_full = true;
		MouthArea.GetComponent<CapsuleCollider2D>().enabled = false;
		SuckEffector.GetComponent<PointEffector2D>().enabled = false;
		ani_hander.SetTrigger("caught");
		return true;
	}

	public void Damage(GameObject obj) {
		AnimatorStateInfo state = ani_hander.GetCurrentAnimatorStateInfo(0);
		if (!state.IsName("damage"))
		{
			health -= 1;
		}
		if (health <= 0) {
				
		}
		GUI.GetComponent<GameUI>().on_player_damaged(health);
		ani_hander.ResetTrigger("exhale");
		ani_hander.ResetTrigger("inhale");
		ani_hander.ResetTrigger("damaged");
		ani_hander.SetTrigger("damaged");
		rb.velocity = new Vector2(0, 0);
		Vector2 pos = obj.GetComponent<Transform>().position;
		Vector2 dir = (this.transform.position - obj.GetComponent<Transform>().position).normalized;
		float force = jump_force;
		dir = new Vector2(dir.x*2, dir.y);
		rb.AddForce(dir*(force), ForceMode2D.Impulse);
		mouth_full = false;
	}
	private void Breath()
	{
		if (IsGrounded())
		{
			last_known_vertical_input = 0.0f;
		}
		if (Input.GetAxis("Vertical") > 0 && last_known_vertical_input == 0.0f && !IsGrounded() && !mouth_full)
		{
			mouth_full = true;
			food_in_mouth = FOOD.NONE;
			ani_hander.ResetTrigger("exhale");
			ani_hander.ResetTrigger("inhale");
			ani_hander.SetTrigger("inhale");
		}
		else if (Input.GetKeyDown(KeyCode.KeypadEnter) && !mouth_full && IsGrounded())
		{
			audio_source.PlayOneShot(GetSound("inhale_sfx"));
			ani_hander.ResetTrigger("exhale");
			ani_hander.SetTrigger("inhale");
			SuckEffector.GetComponent<PointEffector2D>().enabled = true;
			MouthArea.GetComponent<CapsuleCollider2D>().enabled = true;
		}
		else if (Input.GetKeyUp(KeyCode.KeypadEnter))
		{
			audio_source.Stop();
			if (!mouth_full) {
				ani_hander.ResetTrigger("inhale");
				ani_hander.ResetTrigger("exhale");
				ani_hander.Play("Idle", 0);
			}
			SuckEffector.GetComponent<PointEffector2D>().enabled = false;
			MouthArea.GetComponent<CapsuleCollider2D>().enabled = false;
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) && mouth_full)
		{
			ani_hander.ResetTrigger("inhale");
			if(food_in_mouth != FOOD.NONE)
			{
				audio_source.PlayOneShot(GetSound("exhale_star_sfx"));
				spitOutStar();
			}
			else {
				audio_source.PlayOneShot(GetSound("exhale_air_sfx"));
			}
			mouth_full = false;
			ani_hander.SetTrigger("exhale");
		}
		last_known_vertical_input = Input.GetAxis("Vertical");
	}

	private void spitOutStar()
	{
		Instantiate(ProjectilePrefab, proj_offset.position, SuckEffector.transform.rotation);
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

