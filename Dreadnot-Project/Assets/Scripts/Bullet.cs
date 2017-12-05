using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public float lifetime;
	public float damage;
	public ShipScript myShip;

	void Update()
	{
		lifetime-=Time.deltaTime;
		if(lifetime<=0)
		{
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		lifetime=0;
	}
}