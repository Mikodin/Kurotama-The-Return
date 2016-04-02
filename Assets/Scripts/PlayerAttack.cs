using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
	public float damage;
	public LayerMask EnemyLayer;
	public GameObject swordHilt;
	public GameObject swordTip;

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
		if (attacking) {
			Vector3 hitDir = swordTip.transform.position - swordHilt.transform.position;

			float hitDistance = hitDir.magnitude;

			//print ("Hit direction: " + hitDir + " hit Distance: " + hitDistance);
			hitDir.Normalize();

			RaycastHit2D hit = Physics2D.Raycast(swordHilt.transform.position, hitDir, hitDistance, EnemyLayer);
			print (hit.ToString());
			Debug.DrawRay (swordHilt.transform.position, hitDir);
			Debug.DrawRay (swordTip.transform.position, hitDir);

			if (hit.collider != null)
			{
				print ("Hit ma niggaaa");
				//Enemy e = hit.collider.gameObject.GetComponent<Enemy>();
				//e.DamageEntity(Damage);
			}
		}
	}

	public void setAttack(bool attack) {
		attacking = attack;
	}
}

