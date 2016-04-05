using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public int health;
	Animator anim;
	SpriteRenderer theGraphic;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
		theGraphic = GetComponent<SpriteRenderer> ();
		anim.SetBool ("dead", false);
		anim.SetBool ("dieing", false);
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
		anim.SetBool ("dead", true);
	}

	public int GetHealth() {
		return health;
	}

	public void SetHealth(int h) {
		health = h;
	}
}

