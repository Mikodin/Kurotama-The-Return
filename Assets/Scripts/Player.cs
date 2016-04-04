using UnityEngine;
using System.Collections;
using CnControls;


[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(MobileInput))]
[RequireComponent(typeof(PlayerAttack))]
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
	PlayerAttack attack;
	SpriteRenderer theGraphic;
	Animator anim;

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


		theGraphic = GetComponent<SpriteRenderer> ();
		theSprite = theGraphic.sprite;

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
		print ("Screen Size" + Screen.width + " " + Screen.height);
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

			if (phone.GetGestures () == "upright") {
				Leap (1, ref velocity);
			}
			if (phone.GetGestures () == "upleft") {
				Leap (-1,ref velocity);
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

		if (Input.GetKeyUp (KeyCode.Space) || phone.GetGestures () == "release") {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
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
			}
		}

		if(phone.GetGestures () == "upleft" || phone.GetGestures () == "upright") {
			//print ("true all day");
			attack.setAttack(true);
			attack.highAttack ();
		}

		if(phone.GetGestures () == "left" || phone.GetGestures () == "right") {
			//print ("true all day");
			attack.setAttack(true);
			attack.midAttack ();
		}

		if(phone.GetGestures () == "downleft" || phone.GetGestures () == "downright") {
			//print ("true all day");
			attack.setAttack(true);
			attack.lowAttack ();
		}


		if(Input.GetKeyDown(KeyCode.R)) {
			//print ("true all day");
			attack.setAttack(true);
			attack.highAttack ();
		}
		if (Input.GetKeyUp (KeyCode.R)) {
			attack.setAttack (false);
			//print ("false all day");
		}

		if(Input.GetKeyDown(KeyCode.F)) {
			//print ("true all day");
			attack.setAttack(true);
			attack.midAttack ();
		}
		if (Input.GetKeyUp (KeyCode.F)) {
			attack.setAttack (false);
			//print ("false all day");
		}

		if(Input.GetKeyDown(KeyCode.C)) {
			//print ("true all day");
			attack.setAttack(true);
			attack.lowAttack ();
		}
		if (Input.GetKeyUp (KeyCode.C)) {
			attack.setAttack (false);
			//print ("false all day");
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
