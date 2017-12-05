using UnityEngine;
using System.Collections;

public class Weapon : Block
{
	public Transform head;
	public GameObject shot;
	public float exitVelocity,spread;
	public int numShots = 1;
	public Vector3 minRot,maxRot;
	public bool active = false;
	public LineRenderer lr;
	public Renderer headRenderer;
	public int headColorSlot;
	public enum FireType {Shot=0, Ray=1}
	public FireType ft;
	public AudioSource fireSource;
	public Transform fireSpawn;
	public int weaponGroupID;
	public bool oneTwo;

	Quaternion Spread(Quaternion qq)
	{
		Quaternion q = new Quaternion(qq.x, qq.y, qq.z, qq.w);
		q.eulerAngles += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
		return q;
	}

	public void Fire(bool on)
	{
		if(ft==FireType.Shot)
		{
			if(on)
			{
				for(int i = 0; i<numShots; i++)
				{
					GameObject bullet = (GameObject)Instantiate(shot, fireSpawn.position, Spread(fireSpawn.rotation));
					bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*exitVelocity);
				}
				fireSource.Play();
			}
		}
		else if(ft==FireType.Ray)
		{
			if(on)
			{
				shot.SetActive(true);
				if(fireSource.isPlaying==false)
					fireSource.Play();
			}
			else
			{
				shot.SetActive(false);
				if(fireSource.isPlaying)
					fireSource.Stop();
			}
		}
	}

	public override void UpdateColors()
	{
		base.UpdateColors();
		lr.SetColors(myShip.shipColorPower, Color.clear);
		if(headRenderer)
		{
			headRenderer.materials[headColorSlot].color=myShip.shipColorSpecial;
		}
	}

	void Update()
	{
		lr.enabled=active;
	}

	public void LookAt(Transform t)
	{
		head.LookAt(t);
		Vector3 rot = head.localRotation.eulerAngles;
		rot.x = Utilities.ClampAngle(rot.x, minRot.x, maxRot.x);
		rot.y = Utilities.ClampAngle(rot.y, minRot.y, maxRot.y);
		rot.z = Utilities.ClampAngle(rot.z, minRot.z, maxRot.z);
		Quaternion q = new Quaternion();
		q.eulerAngles = rot;
		head.localRotation=q;
	}
}