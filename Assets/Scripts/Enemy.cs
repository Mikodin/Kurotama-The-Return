using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
	public int health;
	public bool deathAnim;
	Animator anim;
	SpriteRenderer theGraphic;
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
		theGraphic = GetComponent<SpriteRenderer> ();
		anim.SetBool ("dead", false);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
			Destroy (gameObject);
			//gameObject.transform.Rote
		}
	}		




	public int GetHealth() {
		return health;
	}

	public void SetHealth(int h) {
		health = h;
	}
}

