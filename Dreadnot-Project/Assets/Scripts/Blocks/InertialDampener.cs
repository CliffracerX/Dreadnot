using UnityEngine;
using System.Collections;

public class InertialDampener : Block
{
	public float drag;
	public float aDrag;
	public float dampenSpeed;
	public int materialIDSpecial;
	
	public override void UpdateColors()
	{
		base.UpdateColors();
		this.rend.materials[materialIDSpecial].color=myShip.shipColorSpecial;
	}
}