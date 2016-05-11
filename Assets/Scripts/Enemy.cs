using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
	public int health;
	public bool deathAnim;

	Animator anim;
	SpriteRenderer theGraphic;

	public LayerMask PlayerLayer;

	public float SightDistance;
	public float AttackDistance;
	public bool CanLeap;
	public float LeapDistance;
	public int damage;
	public bool GraphicFlipped;

	public GameObject player;
	Controller2D controller;

	float moveSpeed = 6;
	Vector3 velocity;
	public Vector2 leap;
	public float maxJumpHeight = 4;
	public float timeToJumpApex = .4f;


	float velocityXSmoothing;
	float accelerationTimeGrounded = .1f;
	float accelerationTimeAirborne = .2f;

	bool attack = true;

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();
		theGraphic = GetComponent<SpriteRenderer> ();
		anim.SetBool ("dead", false);

		if (GraphicFlipped) {
			theGraphic.flipX = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Vector3 hitDir = swordTip.transform.position - swordHilt.transform.position;
		Vector3 rightDir = new Vector3 (180, 0, 0);
		Vector3 leftSight = new Vector3 (-180, 0, 0);
		RaycastHit2D attackRay;

		RaycastHit2D rightSideSight = Physics2D.Raycast (gameObject.transform.position, rightDir, SightDistance, PlayerLayer);
		RaycastHit2D leftSideSight = Physics2D.Raycast (gameObject.transform.position, leftSight, SightDistance, PlayerLayer);
		if (CanLeap) {
			attackRay = Physics2D.Raycast (gameObject.transform.position, leftSight, LeapDistance, PlayerLayer);

		} else {
			attackRay = Physics2D.Raycast (gameObject.transform.position, leftSight, AttackDistance, PlayerLayer);
		}
		Debug.DrawRay (gameObject.transform.position, leftSight);
		Debug.DrawRay (gameObject.transform.position, rightDir);

		//Debug.DrawRay (swordTip.transform.position, hitDir);
		//print (hit.collider.name);
		if (rightSideSight.collider != null || leftSideSight.collider != null) {
			print ("Player Seen!");

			if (rightSideSight.collider != null) {
				EnemyRun (1, ref velocity);
			}

			if (leftSideSight.collider != null) {
				if (attackRay.collider != null) {
					EnemyLeap (-1, ref velocity);
					Player p = attackRay.collider.gameObject.GetComponent<Player> ();
					AttackPlayer (p);
					attack = false;
				}
				EnemyRun (-1, ref velocity);
			}
		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
			if (controller.collisions.below) {
			}
		}
		controller.Move (velocity * Time.deltaTime, new Vector2 (1, 0));

	}

	void AttackPlayer (Player player)
	{
		if (attack) {
			if (CanLeap) {
				player.Damage (damage * 2);
				anim.SetBool ("attacking", true);

			} else {
				//player.Damage (damage);
				anim.SetBool ("attacking", true);

			}
			Kill ();
		}
		attack = true;
	}

	void EnemyRun (float direction, ref Vector3 velocity)
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

	void EnemyLeap (int direction, ref Vector3 velocity)
	{
		velocity.x = direction * leap.x;
		velocity.y = leap.y;
	}

	public void Damage (int dmg)
	{
		health -= dmg;
		anim.SetBool ("dieing", true);
		if (health <= 0) {
			Kill ();
		}
	}

	public void Kill ()
	{
		if (deathAnim) {
			anim.SetBool ("dead", true);
		} else {
			if (!CanLeap) {
				theGraphic.flipY = true;
			}
			StartCoroutine ("DelayedDestroyObject", .3f);
		}
	}

	IEnumerator DelayedDestroyObject (float time)
	{
		yield return new WaitForSeconds (time);
		Destroy (gameObject);

	}


	public int GetHealth ()
	{
		return health;
	}

	public void SetHealth (int h)
	{
		health = h;
	}
}

