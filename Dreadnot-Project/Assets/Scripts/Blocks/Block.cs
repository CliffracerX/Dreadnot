using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
	public int hullIntegMod;
	public float mass;
	public ShipScript myShip;
	public Renderer rend;
	public int materialIDMain;
	public float passivePower;
	public int powerSys;
	public int crewMod;
	public float repairDelayMod;
	public int shipRepairMod,injuryMod;
	public float damageMod;
	public float lifetime = 10;
	public bool destroyed = false;

	public virtual void Update()
	{
		if(destroyed)
		{
			lifetime-=Time.deltaTime;
			if(lifetime<=0)
			{
				Destroy(this.gameObject);
			}
		}
	}

	public virtual void OnShipDestroy()
	{
		Rigidbody r = this.gameObject.AddComponent<Rigidbody>();
		r.drag=0;
		r.angularDrag=0;
		r.mass=mass;
		r.isKinematic=false;
		r.useGravity=true;
		destroyed=true;
		transform.parent=null;
		r.velocity = myShip.shipBody.velocity;
		r.angularVelocity = myShip.shipBody.angularVelocity;
		r.AddExplosionForce(myShip.shipBody.mass*(5), myShip.transform.position+myShip.GetAvg(), myShip.shipBody.mass);
	}

	public virtual void UpdateColors()
	{
		rend.materials[materialIDMain].color=myShip.shipColorMain;
	}

	public void OnTriggerStay(Collider other)
	{
		if(myShip)
		{
			if(other.tag=="Damager" && other.transform.root!=myShip.transform)
			{
				myShip.Damage(other.GetComponent<Damager>().damage*Time.deltaTime);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag=="Bullet")
		{
			if(myShip)
			{
				if(other.GetComponent<Bullet>().myShip!=myShip)
				{
					myShip.Damage(other.GetComponent<Bullet>().damage);
				}
			}
		}
	}
}