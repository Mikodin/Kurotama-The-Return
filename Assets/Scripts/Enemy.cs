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
	public GameObject player;
	Controller2D controller;
	float moveSpeed = 6;
	Vector3 velocity;
	float velocityXSmoothing;
	float accelerationTimeGrounded = .1f;
	float accelerationTimeAirborne = .2f;

	// Use this for initialization
	void Start ()
	{
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();
		theGraphic = GetComponent<SpriteRenderer> ();
		anim.SetBool ("dead", false);
		theGraphic.flipX = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Vector3 hitDir = swordTip.transform.position - swordHilt.transform.position;
		Vector3 rightDir = new Vector3(180,0,0);
		Vector3 leftSight = new Vector3(-180,0,0);
		RaycastHit2D rightSideSight = Physics2D.Raycast (gameObject.transform.position, rightDir,SightDistance, PlayerLayer);
		RaycastHit2D leftSideSight = Physics2D.Raycast (gameObject.transform.position, leftSight,SightDistance, PlayerLayer);
		Debug.DrawRay (gameObject.transform.position, leftSight);
		Debug.DrawRay (gameObject.transform.position, rightDir);

		//Debug.DrawRay (swordTip.transform.position, hitDir);
		//print (hit.collider.name);
		if (rightSideSight.collider != null || leftSideSight.collider != null) {
			print ("Hit");
			if (rightSideSight.collider != null) {
				EnemyRun (1, ref velocity);
			}
			if (leftSideSight.collider != null) {
				EnemyRun (-1, ref velocity);
			}

			// Player e = hit.collider.gameObject.GetComponent<Player>();
			//e.Damage(100);
			//setAttack(false);
		}
		controller.Move (velocity * Time.deltaTime, new Vector2(1,0));

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

	public void Damage(int dmg) {
		health -= dmg;
		anim.SetBool ("dieing", true);
		if (health <= 0) {
			Kill();
		}
	}

	public void Kill() {
		if (deathAnim) {
			anim.SetBool ("dead", true);
		} else {
			theGraphic.flipY = true;
			StartCoroutine ("DelayedDestroyObject", .3f);
		}
	}		

	IEnumerator DelayedDestroyObject (float time)
	{
		yield return new WaitForSeconds (time);
		Destroy (gameObject);

	}


	public int GetHealth() {
		return health;
	}

	public void SetHealth(int h) {
		health = h;
	}
}

