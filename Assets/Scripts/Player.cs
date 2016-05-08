using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.UI;

[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(MobileInput))]
public class Player : MonoBehaviour
{

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
	SpriteRenderer theGraphic;
	Animator anim;

	Sprite theSprite;

	private int textstage; //if textstage == x and isShowing == false and texttimer = 0 or isShowing == true and texttimer != 0
	private bool isShowing;
	private float texttimer;
	public Text dialogue;

	void Start ()
	{


		//touchSupported = Input.touchSupported;
		playerRunning = false;
		touchSupported = false;
		controller = GetComponent<Controller2D> ();
		phone = GetComponent<MobileInput> ();
		anim = GetComponent<Animator> ();

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
	}

	void Update ()
	{

		Vector2 input = new Vector2 (CnInputManager.GetAxis ("Horizontal"), CnInputManager.GetAxis ("Vertical"));

		int wallDirX = (controller.collisions.left) ? -1 : 1;

		PlayerRun (input.x, ref velocity);

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
		 
		if (Input.GetKeyDown (KeyCode.Space) || phone.GetGestures () == "up" || phone.GetGestures () == "upleft" || phone.GetGestures () == "upright") {
			print("In player: " +phone.GetGestures());
			
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

		if (Input.GetKeyUp (KeyCode.Space) || phone.GetGestures () == "release") {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

		if (phone.GetGestures () == "upright") {
			Leap (1, ref velocity);
		}
		if (phone.GetGestures () == "upleft") {
			Leap (-1,ref velocity);
		}
		if (velocity.x < 0) {
			theGraphic.flipX = true;
		} else {
			theGraphic.flipX = false;
		}

		Jump (ref velocity);			
		controller.Move (velocity * Time.deltaTime, input);
		
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
			if (controller.collisions.below) {
				//print ("On ground");
			}
		}


		print ("Timer is " + texttimer + ", Stage is " + textstage + ", xpos is " + this.transform.position.x);

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


	}

	void PlayerRun(float direction, ref Vector3 velocity) {
		if (direction == 1 || direction == -1) {
			//playerRunning = true;
			anim.SetBool ("running", true);
		} else {
			//playerRunning = false;
			anim.SetBool ("running", false);

		}

		float targetVelocityX = direction * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

	}

	void Jump(ref Vector3 velocity) {
		if (velocity.y > 0) {
			anim.SetBool ("falling", false);
			anim.SetBool ("jumping", true);
		} else {
			anim.SetBool ("jumping", false);
			anim.SetBool ("falling", true);
		}

		velocity.y += gravity * Time.deltaTime;
	}

	void Leap(int direction, ref Vector3 velocity) {
		if (velocity.y > 0) {
			anim.SetBool ("leap", true);
		} else {
			anim.SetBool ("leap", false);
		}
		velocity.x = direction * leap.x;
		velocity.y = leap.y;
	}
}
