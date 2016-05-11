using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CnControls;
using UnityEditor.SceneManagement;


[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(MobileInput))]
[RequireComponent (typeof(PlayerAttack))]
public class Player : MonoBehaviour
{
	public int health;

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	public Vector2 leap;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	float repeatFlickTime = .25f;
	float timeToRepeatFlickTime;

	public bool playerRunning;

	bool touchSupported;

	Controller2D controller;
	MobileInput phone;
	PlayerAttack attack;
	SpriteRenderer theGraphic;
	Animator anim;

	private int textstage;
	//if textstage == x and isShowing == false and texttimer = 0 or isShowing == true and texttimer != 0
	private bool isShowing;
	private float texttimer;
	public Text dialogue;

	public Text healthText;


	Sprite theSprite;

	void Start ()
	{
		//touchSupported = Input.touchSupported;
		playerRunning = false;
		touchSupported = false;
		controller = GetComponent<Controller2D> ();
		phone = GetComponent<MobileInput> ();
		attack = GetComponent<PlayerAttack> ();
		anim = GetComponent<Animator> ();

		anim.SetBool ("running", false);
		// anim.SetBool ("jumping", false);
		anim.SetBool ("highAttack", false);
		anim.SetBool ("lowAttack", false);
		anim.SetBool ("midAttack", false);

		theGraphic = GetComponent<SpriteRenderer> ();
		theSprite = theGraphic.sprite;

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
		print ("Screen Size" + Screen.width + " " + Screen.height);

		textstage = 0;
		isShowing = false;
		texttimer = 0;
		dialogue.text = " ";
		healthText.text = "Health: " +health.ToString ();
	}

	void Update ()
	{
		Vector2 input = new Vector2 ();
		foreach (Touch touch in Input.touches) {
			if (phone.leftPane.InBounds (touch)) {
				input = new Vector2 (CnInputManager.GetAxis ("Horizontal"), CnInputManager.GetAxis ("Vertical"));
			}
		}

		if (input.x == 0) {
			anim.SetBool ("running", false);
		}

		PlayerRun (input.x, ref velocity);

		int wallDirX = (controller.collisions.left) ? -1 : 1;
		bool wallSliding = false;

		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirX && input.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				timeToWallUnstick = wallStickTime;
			}
		}
		 
		if (Input.GetKeyDown (KeyCode.Space) || phone.GetLeftGestures () == "up" || phone.GetLeftGestures () == "upleft" || phone.GetLeftGestures () == "upright") {

			if (phone.GetLeftGestures () == "upright") {
				Leap (1, ref velocity);
			}
			if (phone.GetLeftGestures () == "upleft") {
				Leap (-1, ref velocity);
			}

			if (wallSliding) {
				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				} else if (input.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				} else {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			if (controller.collisions.below) {
				velocity.y = maxJumpVelocity;
			}
		}

		if (Input.GetKeyUp (KeyCode.Space) || phone.GetLeftGestures () == "release") {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

		if (velocity.x < 0) {
			theGraphic.flipX = true;
		} else if (velocity.x > 0) {
			theGraphic.flipX = false;
		}

		Jump (ref velocity);			
		controller.Move (velocity * Time.deltaTime, input);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
			if (controller.collisions.below) {
			}
		}
			
		if (phone.GetRightGestures () == "upleft" || phone.GetRightGestures () == "upright") {
			//print ("true all day");
			attack.setAttack (true);
			if (phone.GetRightGestures () == "upleft") {
				attack.highAttack (-1);
				anim.SetBool ("highAttack", true);
			}

			if (phone.GetRightGestures () == "upright") {
				attack.highAttack (1);
				anim.SetBool ("highAttack", true);
			}
				
		} else if (phone.GetRightGestures () == "left" || phone.GetRightGestures () == "right") {
			//print ("true all day");
			attack.setAttack (true);
			if (phone.GetRightGestures () == "left") {
				attack.midAttack (-1);
				anim.SetBool ("midAttack", true);

			}

			if (phone.GetRightGestures () == "right") {
				attack.midAttack (1);
				anim.SetBool ("midAttack", true);
			}

		} else if (phone.GetRightGestures () == "downleft" || phone.GetRightGestures () == "downright") {
			//print ("true all day");
			attack.setAttack (true);
			if (phone.GetRightGestures () == "downleft") {
				attack.lowAttack (-1);
				anim.SetBool ("lowAttack", true);
			}

			if (phone.GetRightGestures () == "downright") {
				attack.lowAttack (1);
				anim.SetBool ("lowAttack", true);
			}
		}


		if (phone.GetRightGestures () == "release" || phone.GetRightGestures () == "") {
			attack.setAttack (false);
			anim.SetBool ("highAttack", false);
			anim.SetBool ("midAttack", false);
			anim.SetBool ("lowAttack", false);
		}

		if (phone.GetLeftGestures () == "release" || phone.GetLeftGestures () == "") {

		}

		// print ("Timer is " + texttimer + ", Stage is " + textstage + ", xpos is " + this.transform.position.x);

		if (texttimer != 0) {
			if (texttimer > 0) {
				texttimer = texttimer - Time.deltaTime; 
			} else {
				texttimer = 0;
			}
		}

		if (texttimer == 0 && isShowing == true) {
			isShowing = false;
			dialogue.text = " ";
		}


		if ((textstage == 0 && isShowing == false && texttimer == 0 && this.transform.position.x > 2) || (textstage == 0 && isShowing == true && texttimer != 0 && this.transform.position.x > 2)) {
			textstage = 1;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Hi! I am working properly.";
		}

		if ((textstage == 1 && isShowing == false && texttimer == 0 && this.transform.position.x > 8) || (textstage == 1 && isShowing == true && texttimer != 0 && this.transform.position.x > 8)) {
			textstage = 2;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "I am continuing to work properly.";
		}

		if ((textstage == 2 && isShowing == false && texttimer == 0 && this.transform.position.x > 15) || (textstage == 2 && isShowing == true && texttimer != 0 && this.transform.position.x > 15)) {
			textstage = 3;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Kuro is speaking this line.";
		}
	} //Update
		
	void PlayerRun (float direction, ref Vector3 velocity)
	{
		if (direction == 1 || direction == -1) {
			//playerRunning = true;
			anim.SetBool ("running", true);
		} else {
			//playerRunning = false;
		}
		float targetVelocityX = direction * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, 
			(controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
	}

	void Jump (ref Vector3 velocity)
	{
		if (velocity.y > 0) {
			// anim.SetBool ("jumping", true);
		} else {
			// anim.SetBool ("jumping", false);
		}

		velocity.y += gravity * Time.deltaTime;
	}

	void Leap (int direction, ref Vector3 velocity)
	{
		if (velocity.y > 0) {
			//anim.SetBool ("leap", true);
		} else {
			//anim.SetBool ("leap", false);
		}
		velocity.x = direction * leap.x;
		velocity.y = leap.y;
	}

	public void Damage(int  dmg) {
		health -= dmg;
		healthText.text = "Health: " +health.ToString ();

		if (health <= 0) {
			EditorSceneManager.LoadScene ("Kurotama");
		}
	}
}
