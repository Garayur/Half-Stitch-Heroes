using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	//Attach to a rigidbody
	protected BaseController owner;
	protected float damage;

	void OnCollisionEnter(Collision other){
		Debug.Log ("Collide with: " + other.gameObject);
		if(other.gameObject.tag == "Player" || other.gameObject.tag == "Mob" ){
			if(other.gameObject.GetComponent<BaseController>() != owner)
				other.gameObject.GetComponent<BaseController> ().TakeDamage (owner, gameObject.transform.position, gameObject.GetComponent<Rigidbody>().velocity, damage, AttackEffect.None);
		}
			
		if(other.gameObject.GetComponent<BaseController>() != owner || other.gameObject.GetComponent<BaseController>() == null)
			Destroy (this.gameObject);
	}

	public virtual void Initialize(BaseController owner, float damage){
		this.owner = owner;
		this.damage = damage;
	}
}
