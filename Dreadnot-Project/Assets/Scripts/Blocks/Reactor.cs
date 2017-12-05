using UnityEngine;
using System.Collections;

public class Reactor : Block
{
	public float powerOut;
	public int materialIDPower;
	
	public override void UpdateColors()
	{
		base.UpdateColors();
		this.rend.materials[materialIDPower].color=myShip.shipColorPower;
	}
}