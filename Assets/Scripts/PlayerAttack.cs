using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
	public float damage;
	public LayerMask EnemyLayer;
	public GameObject swordHilt;
	public GameObject swordTip;
	public GameObject owner;
	public SpriteRenderer theSprite;

	private bool attacking;

	// Use this for initialization
	void Start ()
	{
		attacking = false;
		print ("SwordHilt at: " + swordHilt.transform.position + " SwordTip at: " + swordTip.transform.position);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		Vector3 temp = swordHilt.transform.position;
		if (theSprite.flipX == false) {
			temp.x = owner.transform.position.x + .84f;
			temp.y = owner.transform.position.y - .43f;
			swordHilt.transform.position = temp;

			temp = swordTip.transform.position;
			temp.x = owner.transform.position.x + 1.89f;
			temp.y = owner.transform.position.y + .92f;
			swordTip.transform.position = temp;
		} else {
			temp.x = owner.transform.position.x - .84f;
			temp.y = owner.transform.position.y - .43f;
			swordHilt.transform.position = temp;

			temp = swordTip.transform.position;
			temp.x = ((owner.transform.position.x - 1.89f));
			temp.y = owner.transform.position.y + .92f;
			swordTip.transform.position = temp;
		}

		if (attacking) {
			Vector3 hitDir = swordTip.transform.position - swordHilt.transform.position;
			float hitDistance = hitDir.magnitude;

			hitDir.Normalize ();
			RaycastHit2D hit = Physics2D.Raycast (swordHilt.transform.position, hitDir, hitDistance, EnemyLayer);
			Debug.DrawRay (swordHilt.transform.position, hitDir);
			Debug.DrawRay (swordTip.transform.position, hitDir);
			//print (hit.collider.name);
			if (hit.collider != null) {
				print ("Hit");
				//Enemy e = hit.collider.gameObject.GetComponent<Enemy>();
				//e.DamageEntity(Damage);
			}
		}
	}

	public void setAttack (bool attack)
	{
		attacking = attack;
	}
}

