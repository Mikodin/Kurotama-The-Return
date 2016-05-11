using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CnControls;
//using UnityEditor.SceneManagement;


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


		if ((textstage == 0 && isShowing == false && texttimer == 0 && this.transform.position.x > -22) || (textstage == 0 && isShowing == true && texttimer != 0 && this.transform.position.x > -22)) {
			textstage = 1;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "What's going on?..Who are you";
		}

		if ((textstage == 1 && isShowing == false && texttimer == 0 && this.transform.position.x > -21) || (textstage == 1 && isShowing == true && texttimer != 0 && this.transform.position.x > -21)) {
			textstage = 2;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "It's been so long...and this body feels so..strange.  I can't just rush into this";
		}

		if ((textstage == 2 && isShowing == false && texttimer == 0 && this.transform.position.x > -20) || (textstage == 2 && isShowing == true && texttimer != 0 && this.transform.position.x > -20)) {
			textstage = 3;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Rush into what? That's my body you're-";
		}

		if ((textstage == 3 && isShowing == false && texttimer == 0 && this.transform.position.x > -19) || (textstage == 3 && isShowing == true && texttimer != 0 && this.transform.position.x > -19)) {
			textstage = 4;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "These odd sculptures should make for good practice!";
		}

		if ((textstage == 4 && isShowing == false && texttimer == 0 && this.transform.position.x > -18) || (textstage == 4 && isShowing == true && texttimer != 0 && this.transform.position.x > -18)) {
			textstage = 5;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "What, you mean my mannequins?";
		}

		if ((textstage == 5 && isShowing == false && texttimer == 0 && this.transform.position.x > -17) || (textstage == 5 && isShowing == true && texttimer != 0 && this.transform.position.x > -17)) {
			textstage = 6;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Walk up to it and swipe your right thumb forward, the angle determines the kind of normal attack";
		}

		if ((textstage == 6 && isShowing == false && texttimer == 0 && this.transform.position.x > -9.3) || (textstage == 6 && isShowing == true && texttimer != 0 && this.transform.position.x > 9.3)) {
			textstage = 7;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Normal attack?  What's NORMAL about this? What did you just do!";
		}

		if ((textstage == 7 && isShowing == false && texttimer == 0 && this.transform.position.x > -8.5) || (textstage == 7 && isShowing == true && texttimer != 0 && this.transform.position.x > -8.5)) {
			textstage = 8;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 6;
			dialogue.text = "Accurate enough...though this body lacks any strength." + "\n" + "Go to the next one and swipe your right thumb up and to the right to decapitate it!";
		}

		if ((textstage == 8 && isShowing == false && texttimer == 0 && this.transform.position.x > 0) || (textstage == 8 && isShowing == true && texttimer != 0 && this.transform.position.x > -0)) {
			textstage = 9;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Nice! Very good job!" +"\n" +"I can get used to this, and you should be used to me by now";
		}

		if ((textstage == 9 && isShowing == false && texttimer == 0 && this.transform.position.x > 1) || (textstage == 9 && isShowing == true && texttimer != 0 && this.transform.position.x > 1)) {
			textstage = 10;
			dialogue.color = Color.white;
			isShowing = true;
			texttimer = 4;
			dialogue.text = "Hey! Just who are you anyway!?";
		}

		if ((textstage == 10 && isShowing == false && texttimer == 0 && this.transform.position.x > 2) || (textstage == 10 && isShowing == true && texttimer != 0 && this.transform.position.x > 2)) {
			textstage = 11;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 6;
			dialogue.text = "Oh, how rude of me!  My apologies, I am too focused on saving the world!" + "\n" +"I am Kurotama, the best Samuraii the world has ever seen" +"\n" +"We are about to become very good friends";
		}
		if ((textstage == 11 && isShowing == false && texttimer == 0 && this.transform.position.x > 10) || (textstage == 10 && isShowing == true && texttimer != 0 && this.transform.position.x > 10)) {
			textstage = 12;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 6;
			dialogue.text = "Lets see if this puny body can wall jump!" + "\n" +"With your left thumb swipe up and to the right" +"\n" +"When you hit the wall swipe up and to the left";
		}
		if ((textstage == 12 && isShowing == false && texttimer == 0 && this.transform.position.x > 80) || (textstage == 10 && isShowing == true && texttimer != 0 && this.transform.position.x > 80)) {
			textstage = 13;
			dialogue.color = Color.red;
			isShowing = true;
			texttimer = 6;
			dialogue.text = "One step closer to saving the world!";
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
			//EditorSceneManager.LoadScene ("Kurotama");
		}
	}
}
