using UnityEngine;
using System.Collections;

public class Thruster : Block
{
	public float force;
	public Transform burn;
	public Light burnLight;
	public float range;
	public int materialIDThrust;
	public Renderer burnRend;
	public AudioSource thrustSource;
	public float time,volume;
	public bool posNeg;
	public int thrustGroup;

	void Update()
	{
		time+=Time.deltaTime;
		thrustSource.volume=Mathf.Lerp(0, volume, time);
	}
	
	public override void UpdateColors()
	{
		base.UpdateColors();
		this.rend.materials[materialIDThrust].color=myShip.shipColorThrust;
		this.burnRend.material.color=myShip.shipColorThrust;
		this.burnLight.color=myShip.shipColorThrust;
	}
}