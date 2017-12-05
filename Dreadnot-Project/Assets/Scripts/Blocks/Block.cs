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

	public virtual void UpdateColors()
	{
		rend.materials[materialIDMain].color=myShip.shipColorMain;
	}

	public void OnTriggerStay(Collider other)
	{
		if(other.tag=="Damager" && other.transform.root!=myShip.transform)
		{
			myShip.Damage(other.GetComponent<Damager>().damage*Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag=="Bullet")
		{
			if(other.GetComponent<Bullet>().myShip!=myShip)
			{
				myShip.Damage(other.GetComponent<Bullet>().damage);
			}
		}
	}
}