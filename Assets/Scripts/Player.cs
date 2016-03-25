using UnityEngine;
using System.Collections;

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

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	float repeatFlickTime = .25f;
	float timeToRepeatFlickTime;

	bool touchSupported;

	Controller2D controller;
	MobileInput phone;

	void Start ()
	{
		//touchSupported = Input.touchSupported;
		touchSupported = true;
		controller = GetComponent<Controller2D> ();
		phone = GetComponent<MobileInput> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
	}

	void Update ()
	{
		Vector2 input = new Vector2 (0, 0);
		if (touchSupported == true) {
			if (phone.GetGestures () == "right")
				input = new Vector2 (1, 0);
			if (phone.GetGestures () == "left")
				input = new Vector2 (-1, 0);
		} else {
			input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		}

		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

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
					Leap (ref velocity, 1);
			}

			if (Input.GetKeyDown (KeyCode.RightArrow) && Input.GetKeyDown (KeyCode.UpArrow)) {
				Leap (ref velocity, -1);
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


		Jump (ref velocity);			
		controller.Move (velocity * Time.deltaTime, input);
		
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}

	void Jump(ref Vector3 velocity) {
		velocity.y += gravity * Time.deltaTime;
	}

	void Leap(ref Vector3 velocity, int direction) {
		int dir = direction * -1;
		velocity.y += (gravity * Time.deltaTime) * 5;
		velocity.x += (gravity * Time.deltaTime) * (dir *5);
	}
}