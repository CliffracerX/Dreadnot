using UnityEngine;
using System.Collections;

public class Module : Block
{
	public float power1;
	public Transform transform1;
	public GameObject go;
	public int moduleSlot;
	public float cooldown,time;
	public int colorSlot;

	public override void UpdateColors()
	{
		base.UpdateColors();
		this.rend.materials[colorSlot].color=myShip.shipColorSpecial;
	}
}