using UnityEngine;
using System.Collections;

public class Battery : Block
{
	public float powerStored;
	public int materialIDPower;

	public override void UpdateColors()
	{
		base.UpdateColors();
		this.rend.materials[materialIDPower].color=myShip.shipColorPower;
	}
}