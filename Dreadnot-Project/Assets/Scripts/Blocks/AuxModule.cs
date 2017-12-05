using UnityEngine;
using System.Collections;

public class AuxModule : Block
{
	public float powerUseWhileOn;
	public enum BuffType {Shields=0, Thrusters=1}
	public int moduleSlot;
	public int colorSlot;
	
	public override void UpdateColors()
	{
		base.UpdateColors();
		this.rend.materials[colorSlot].color=myShip.shipColorPower;
	}
}