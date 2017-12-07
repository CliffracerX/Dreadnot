using UnityEngine;
using System.Collections;

public class PreviewBlock : MonoBehaviour
{
	public Renderer rend1,rend2;
	public int slotMain1,slotMain2,slotAlt1,slotAlt2;
	public enum AltColor {None=0, Main=1, Power=2, Thrust=3, Special=4}
	public bool colMain1,colMain2;
	public AltColor ac1,ac2;
	public AltColor lc1,lc2;
	public ShipBuilder myShip;
	public Light light1,light2;

	public void Update()
	{
		if(colMain1)
			rend1.materials[slotMain1].color=myShip.colMain;
		if(ac1==AltColor.Main)
		{
			rend1.materials[slotAlt1].color=myShip.colMain;
		}
		if(ac1==AltColor.Power)
		{
			rend1.materials[slotAlt1].color=myShip.colPower;
		}
		if(ac1==AltColor.Thrust)
		{
			rend1.materials[slotAlt1].color=myShip.colThrust;
		}
		if(ac1==AltColor.Special)
		{
			rend1.materials[slotAlt1].color=myShip.colSpecial;
		}
		if(rend2)
		{
			if(colMain2)
				rend2.materials[slotMain2].color=myShip.colMain;
			if(ac2==AltColor.Main)
			{
				rend2.materials[slotAlt2].color=myShip.colMain;
			}
			if(ac2==AltColor.Power)
			{
				rend2.materials[slotAlt2].color=myShip.colPower;
			}
			if(ac2==AltColor.Thrust)
			{
				rend2.materials[slotAlt2].color=myShip.colThrust;
			}
			if(ac2==AltColor.Special)
			{
				rend2.materials[slotAlt2].color=myShip.colSpecial;
			}
		}
		if(light1)
		{
			if(lc1==AltColor.Main)
			{
				light1.color=myShip.colMain;
			}
			if(lc1==AltColor.Power)
			{
				light1.color=myShip.colPower;
			}
			if(lc1==AltColor.Thrust)
			{
				light1.color=myShip.colThrust;
			}
			if(lc1==AltColor.Special)
			{
				light1.color=myShip.colSpecial;
			}
		}
		if(light2)
		{
			if(lc2==AltColor.Main)
			{
				light2.color=myShip.colMain;
			}
			if(lc2==AltColor.Power)
			{
				light2.color=myShip.colPower;
			}
			if(lc2==AltColor.Thrust)
			{
				light2.color=myShip.colThrust;
			}
			if(lc2==AltColor.Special)
			{
				light2.color=myShip.colSpecial;
			}
		}
	}
}