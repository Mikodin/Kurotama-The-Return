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

	public Vector2 highSwordTipOffset;
	public Vector2 midSwordTipOffset;
	public Vector2 lowSwordTipOffset;

	// Use this for initialization
	void Start ()
	{
		attacking = false;
		print ("SwordHilt at: " + swordHilt.transform.position + " SwordTip at: " + swordTip.transform.position);
	}
	
	// Update is called once per frame
	void Update ()
	{
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

	private void setSwordHilt (int dir)
	{
		Vector3 temp = swordHilt.transform.position;
		if (dir == 1) {
			temp.x = owner.transform.position.x + .84f;
			temp.y = owner.transform.position.y - .43f;
			swordHilt.transform.position = temp;
		} else if (dir == -1) {
			temp.x = owner.transform.position.x - .84f;
			temp.y = owner.transform.position.y - .43f;
			swordHilt.transform.position = temp;
		}
	}

	private void setSwordTip (int dir, float offsetX, float offsetY)
	{
		Vector3 temp = swordHilt.transform.position;
		temp = swordTip.transform.position;
		temp.x = owner.transform.position.x + offsetX;
		temp.y = owner.transform.position.y + offsetY;
		swordTip.transform.position = temp;
	}

	public void highAttack ()
	{
		if (theSprite.flipX == false) {
			setSwordHilt (1);
			setSwordTip (1, 1.89f, .92f);
		} else {
			setSwordHilt (-1);
			setSwordTip (1, -1.89f, .92f);

		}
	}

	public void midAttack ()
	{
		Vector3 temp = swordHilt.transform.position;
		if (theSprite.flipX == false) {
			setSwordHilt (1);
			setSwordTip (1, 1.89f, -.43f);
		} else {
			setSwordHilt (-1);
			setSwordTip (1, -1.89f, -.43f);
		}
	}

	public void lowAttack ()
	{
		Vector3 temp = swordHilt.transform.position;
		if (theSprite.flipX == false) {
			setSwordHilt (1);
			setSwordTip (1, 1.89f, -.92f);
		} else {
			setSwordHilt (-1);
			setSwordTip (1, -1.89f, -.92f);
		}
	}

	public void setAttack (bool attack)
	{
		attacking = attack;
	}
}

