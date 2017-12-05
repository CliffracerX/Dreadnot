using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class ThrusterGroup
{
	public string name;
	public string axis;
	public List<Thruster> posThrust,negThrust;
}

[System.Serializable]
public class PowerSystem
{
	public string name;
	public float energyPerSec,energyStored,energyMax;
	public float passive;
	public List<AuxPowerSlot> slots;
}

[System.Serializable]
public class AuxPowerSlot
{
	public List<AuxModule> modules;
	public float mod;
	public List<int> groups;
	public enum ModuleType {Shield=0, Thrust=1, Weapon=2}
	public ModuleType mtype;
}

[System.Serializable]
public class ModuleSlot
{
	public List<Module> modules;
	public List<int> groups;
	public float modPower,modTime,modCooldown;
	public enum ModuleType {Autorepair=0, JumpDrive=1, Missile=2}
	public ModuleType mtype;
	public float cooldown,runtime;
	public bool active;
}

[System.Serializable]
public class WeaponPair
{
	public List<Weapon> weapons1,weapons2;
	public bool secondaryActive;
	public int ammo1,ammoMax1,ammo2,ammoMax2;
	public float reloadSpeed1,reloadSpeed2,reload1,reload2;
	public bool reloading1,reloading2;
	public float firerate1,firerate2,fire1,fire2;

	public void UpdateState()
	{
		fire1-=Time.deltaTime;
		fire2-=Time.deltaTime;
		if(reloading1)
		{
			reload1+=Time.deltaTime;
			if(reload1>=reloadSpeed1)
			{
				reloading1=false;
				ammo1=ammoMax1;
			}
		}
		if(reloading2)
		{
			reload2+=Time.deltaTime;
			if(reload2>=reloadSpeed2)
			{
				reloading2=false;
				ammo2=ammoMax2;
			}
		}
	}

	public void Reload()
	{
		if(secondaryActive)
		{
			reload2=0;
			reloading2=true;
		}
		else
		{
			reload1=0;
			reloading1=true;
		}
	}

	public void Fire(bool on)
	{
		if(secondaryActive)
		{
			if(on)
			{
				bool ammo = false;
				if(ammo2>0 || ammoMax2==0)
					ammo = true;
				//if(ammoMax2!=0 && this.fire2<0)
				{
					if(ammo && this.fire2<0)
					{
						foreach(Weapon w in weapons2)
						{
							w.Fire(true);
						}
						ammo2-=1;
					}
					else
					{
						foreach(Weapon w in weapons2)
						{
							w.Fire(false);
						}
					}
				}
				if(firerate2!=0 && this.fire2<0)
					fire2=firerate2;
			}
			else
			{
				foreach(Weapon w in weapons2)
				{
					w.Fire(false);
				}
			}
		}
		else
		{
			if(on)
			{
				bool ammo = false;
				if(ammo1>0 || ammoMax1==0)
					ammo = true;
				//if(ammoMax1!=0 && this.fire1<0)
				{
					if(ammo && this.fire1<0)
					{
						foreach(Weapon w in weapons1)
						{
							w.Fire(true);
						}
						ammo1-=1;
					}
					else
					{
						foreach(Weapon w in weapons1)
						{
							w.Fire(false);
						}
					}
				}
				if(firerate1!=0 && this.fire1<0)
					fire1=firerate1;
			}
			else
			{
				foreach(Weapon w in weapons1)
				{
					w.Fire(false);
				}
			}
		}
	}
}

[System.Serializable]
public class BridgeSeat
{
	public string name;
	public List<int> availableThrusts;
	public int weaponSet,powerSet;
	public Transform weaponTarget;
	public int[] moduleSlots;
}

[System.Serializable]
public class Crew:IComparable<Crew>
{
	public bool injured;
	public float recoveryTime;
	public enum Job {Engineer=0}
	public Job j;

	public int CompareTo(Crew other)
	{
		int num = 0;
		if(injured && !other.injured)
			num = 1;
		if(injured && other.injured)
			num = 0;
		if(!injured && !other.injured)
			num = 0;
		if(!injured && other.injured)
			num = -1;
		return num;
	}
}

public class ShipScript : MonoBehaviour
{
	public Rigidbody shipBody;
	public BridgeSeat[] seats;
	public int curSeat = 0;
	public List<ThrusterGroup> tgs;
	public List<WeaponPair> wp;
	public List<PowerSystem> powerSys;
	public List<Crew> crewmembers;
	public List<ModuleSlot> slots;
	public List<Block> blocks;
	public float health,maxHealth;
	public float dampenSpeed = 0;
	public Color shipColorMain,shipColorPower,shipColorThrust,shipColorSpecial;
	public Texture2D barBG,barFG,barOverlay;
	public Texture2D hullNote,powerNote,warningNote;
	public Texture2D wepBarBG,wepBarFG,wepBarWarning,wepBarReload;
	public Texture2D crewTex,crewTexEmpty;
	public float repairDelay,timeSinceDamage;
	public float repairSpeed;
	public bool repairing;
	public float shipDamageMod,resistance;
	public int injurySlots;
	public GUIStyle mainStyle,ammoStyle;
	public Texture2D[] abilityIcons;
	public Texture2D abilityIconOverlay;

	public void Damage(float amount)
	{
		float a = amount - (amount * resistance);
		if(a<0)
			a=0;
		health-=a;
		if(repairing)
		{
			repairing=false;
			foreach(Crew c in crewmembers)
			{
				if(UnityEngine.Random.Range(0, 10/(shipDamageMod/a))<=0.1f)
				{
					c.injured=true;
					c.recoveryTime=0;
				}
			}
		}
		timeSinceDamage=0;
	}

	void OnTriggerStay(Collider other)
	{
		//print("TRIGGER");
		if(other.tag=="Damager")
		{
			Damage(other.GetComponent<Damager>().damage*Time.deltaTime);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.collider.tag!="Bullet")
		{
			float x = Math.Abs(other.relativeVelocity.x);
			float y = Math.Abs(other.relativeVelocity.y);
			float z = Math.Abs(other.relativeVelocity.z);
			Damage(x+y+z);
		}
	}

	void OnGUI()
	{
		PowerSystem ps = powerSys[seats[curSeat].powerSet];
		GUI.color=Color.white;
		GUI.DrawTexture(new Rect((Screen.width*0.1f), Screen.height/2-256, 32, 512), barBG);
		GUI.DrawTexture(new Rect((Screen.width*0.1f), Screen.height/2-256+512, 32, 32), hullNote);
		if((health*1.0f)/maxHealth<=0.5f)
		{
			GUI.DrawTexture(new Rect((Screen.width*0.1f), Screen.height/2-256-32, 32, 32), warningNote);
		}
		GUI.DrawTexture(new Rect((Screen.width*0.9f)-32, Screen.height/2-256, 32, 512), barBG);
		GUI.DrawTexture(new Rect((Screen.width*0.9f)-32, Screen.height/2-256+512, 32, 32), powerNote);
		if((ps.energyStored*1.0f)/ps.energyMax<=0.5f)
		{
			GUI.DrawTexture(new Rect((Screen.width*0.9f)-32, Screen.height/2-256-32, 32, 32), warningNote);
		}
		GUI.color=shipColorMain;
		int hbarH = (int)(510.0f*((health*1.0f)/maxHealth));
		GUI.DrawTexture(new Rect((Screen.width*0.1f)+1, Screen.height/2-256+(510-hbarH)+1, 30, hbarH), barFG);
		GUI.color=Color.white;
		GUI.Label(new Rect((Screen.width*0.1f)-100, Screen.height/2-10, 100, 20), ((int)health)+"/"+((int)maxHealth), mainStyle);
		GUI.color=shipColorPower;
		int pbarH = (int)(510.0f*((((int)ps.energyStored)*1.0f)/ps.energyMax));
		GUI.DrawTexture(new Rect((Screen.width*0.9f)-32+1, Screen.height/2-256+(510-pbarH)+1, 30, pbarH), barFG);
		GUI.color=Color.white;
		GUI.Label(new Rect((Screen.width*0.9f), Screen.height/2-10, 100, 20), ((int)ps.energyStored)+"/"+((int)ps.energyMax), mainStyle);
		GUI.color=Color.white;
		GUI.DrawTexture(new Rect((Screen.width*0.1f), Screen.height/2-256, 32, 512), barOverlay);
		GUI.DrawTexture(new Rect((Screen.width*0.9f)-32, Screen.height/2-256, 32, 512), barOverlay);
		WeaponPair thisWep = wp[seats[curSeat].weaponSet];
		GUI.DrawTexture(new Rect((Screen.width*0.1f)+32, Screen.height/2-64, 16, 128), wepBarBG);
		if((thisWep.ammo1*1.0f)/thisWep.ammoMax1<=0.5f)
		{
			GUI.DrawTexture(new Rect((Screen.width*0.1f)+32, Screen.height/2-64-16, 16, 16), wepBarWarning);
		}
		if(thisWep.reloading1)
		{
			GUI.DrawTexture(new Rect((Screen.width*0.1f)+32, Screen.height/2+64, 16, 16), wepBarReload);
		}
		GUI.DrawTexture(new Rect((Screen.width*0.9f)-32-16, Screen.height/2-64, 16, 128), wepBarBG);
		if((thisWep.ammo2*1.0f)/thisWep.ammoMax2<=0.5f)
		{
			GUI.DrawTexture(new Rect((Screen.width*0.9f)-32-16, Screen.height/2-64-16, 16, 16), wepBarWarning);
		}
		if(thisWep.reloading2)
		{
			GUI.DrawTexture(new Rect((Screen.width*0.9f)-32-16, Screen.height/2+64, 16, 16), wepBarReload);
		}
		if(!thisWep.secondaryActive)
		{
			GUI.color=shipColorSpecial;
		}
		else
		{
			GUI.color=shipColorThrust;
		}
		int abar1H = (int)(126.0f*((thisWep.ammo1*1.0f)/thisWep.ammoMax1));
		if(thisWep.reloading1)
		{
			abar1H = (int)(126.0f*(thisWep.reload1/thisWep.reloadSpeed1));
		}
		float cm1 = 1;
		string t1 = "∞";
		if(thisWep.ammoMax1!=0)
		{
			cm1=((thisWep.ammo1+(thisWep.ammoMax1/2))/thisWep.ammoMax1);
			t1=thisWep.ammo1+"/"+thisWep.ammoMax1;
		}
		else
		{
			abar1H=126;
		}
		GUI.DrawTexture(new Rect((Screen.width*0.1f)+32+1, Screen.height/2-64+1+(126-abar1H), 14, abar1H), wepBarFG);
		GUI.color=Color.white;
		GUI.Label(new Rect((Screen.width*0.1f)+32+16, Screen.height/2-5, 50, 10), t1, ammoStyle);
		if(thisWep.secondaryActive)
		{
			GUI.color=shipColorSpecial;
		}
		else
		{
			GUI.color=shipColorThrust;
		}
		int abar2H = (int)(126.0f*((thisWep.ammo2*1.0f)/thisWep.ammoMax2));
		if(thisWep.reloading2)
		{
			abar2H = (int)(126.0f*(thisWep.reload2/thisWep.reloadSpeed2));
		}
		float cm2 = 1;
		string t2 = "∞";
		if(thisWep.ammoMax2!=0)
		{
			cm2=((thisWep.ammo2+(thisWep.ammoMax2/2))/thisWep.ammoMax2);
			t2=thisWep.ammo2+"/"+thisWep.ammoMax2;
		}
		else
		{
			abar2H=126;
		}
		GUI.DrawTexture(new Rect((Screen.width*0.9f)-32-16+1, Screen.height/2-64+1+(126-abar2H), 14, abar2H), wepBarFG);
		GUI.color=Color.white;
		GUI.Label(new Rect((Screen.width*0.9f)-32-16-50, Screen.height/2-5, 50, 10), t2, ammoStyle);
		crewmembers.Sort();
		List<Crew> crewList = crewmembers;
		int width = crewList.Count*crewTex.width;
		for(int i = 0; i<crewList.Count; i++)
		{
			if(crewList[i].injured)
			{
				GUI.color=Color.white;
				GUI.DrawTexture(new Rect((Screen.width/2)-(width/2)+(crewTex.width*i), Screen.height*0.1f, crewTex.width, crewTex.height), crewTexEmpty);
			}
			else
			{
				if(repairing)
				{
					GUI.color=shipColorPower;
				}
				else
				{
					GUI.color=shipColorMain;
				}
				GUI.DrawTexture(new Rect((Screen.width/2)-(width/2)+(crewTex.width*i), Screen.height*0.1f, crewTex.width, crewTex.height), crewTex);
			}
		}
		int[] ms = seats[curSeat].moduleSlots;
		foreach(int i in ms)
		{
			GUI.color=Color.white;
			GUI.DrawTexture(new Rect((Screen.width/2)-(abilityIcons[(int)slots[i].mtype].width*2)+(abilityIcons[(int)slots[i].mtype].width*i), Screen.height*0.9f, abilityIcons[(int)slots[i].mtype].width, abilityIcons[(int)slots[i].mtype].height), abilityIcons[(int)slots[i].mtype]);
			if(slots[i].cooldown>0)
			{
				GUI.color=shipColorPower;
				int h1 = (int)(abilityIconOverlay.height*1.0f*(1-(slots[i].cooldown/(slots[i].modules[0].cooldown*slots[i].modCooldown))));
				GUI.DrawTexture(new Rect((Screen.width/2)-(abilityIcons[(int)slots[i].mtype].width*2)+(abilityIcons[(int)slots[i].mtype].width*i)+1, Screen.height*0.9f+1+(abilityIconOverlay.height-h1), abilityIconOverlay.width, h1), abilityIconOverlay);
			}
			if(slots[i].runtime>0)
			{
				GUI.color=shipColorSpecial;
				int h2 = (int)(abilityIconOverlay.height*1.0f*(1-(slots[i].runtime/(slots[i].modules[0].time*slots[i].modTime))));
				GUI.DrawTexture(new Rect((Screen.width/2)-(abilityIcons[(int)slots[i].mtype].width*2)+(abilityIcons[(int)slots[i].mtype].width*i)+1, Screen.height*0.9f+1+h2, abilityIconOverlay.width, (abilityIconOverlay.height-h2)), abilityIconOverlay);
			}
		}
	}

	void Start()
	{
		int h = 0;
		float m = 0;
		float drag = 0;
		float aDrag = 0;
		foreach(Block b in transform.GetComponentsInChildren<Block>())
		{
			h+=b.hullIntegMod;
			m+=b.mass;
			b.myShip=this;
			b.UpdateColors();
			powerSys[b.powerSys].passive+=b.passivePower;
			shipDamageMod+=b.damageMod;
			repairSpeed+=b.shipRepairMod;
			repairDelay+=b.repairDelayMod;
			injurySlots+=b.injuryMod;
			for(int i = 0; i<b.crewMod; i++)
			{
				crewmembers.Add(new Crew());
			}
		}
		foreach(InertialDampener id in transform.GetComponentsInChildren<InertialDampener>())
		{
			drag+=id.drag;
			aDrag+=id.aDrag;
			dampenSpeed+=id.dampenSpeed;
		}
		foreach(Thruster t in transform.GetComponentsInChildren<Thruster>())
		{
			if(t.posNeg)
			{
				tgs[t.thrustGroup].posThrust.Add(t);
			}
			else
			{
				tgs[t.thrustGroup].negThrust.Add(t);
			}
		}
		foreach(Weapon w in transform.GetComponentsInChildren<Weapon>())
		{
			if(w.oneTwo)
			{
				wp[w.weaponGroupID].weapons2.Add(w);
			}
			else
			{
				wp[w.weaponGroupID].weapons1.Add(w);
			}
		}
		foreach(Reactor r in transform.GetComponentsInChildren<Reactor>())
		{
			powerSys[r.powerSys].energyPerSec+=r.powerOut;
		}
		foreach(Battery b in transform.GetComponentsInChildren<Battery>())
		{
			powerSys[b.powerSys].energyMax+=b.powerStored;
			powerSys[b.powerSys].energyStored+=b.powerStored;
		}
		foreach(AuxModule am in transform.GetComponentsInChildren<AuxModule>())
		{

		}
		foreach(Module mod in transform.GetComponentsInChildren<Module>())
		{
			slots[mod.moduleSlot].modules.Add(mod);
		}
		health=maxHealth=h;
		shipBody.mass=m;
		shipBody.drag=drag;
		shipBody.angularDrag=aDrag;
	}

	void Update()
	{
		Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up);
		shipBody.AddTorque(new Vector3(q.x, q.y, q.z)*dampenSpeed);
		timeSinceDamage+=Time.deltaTime;
		if(health<maxHealth && timeSinceDamage>=repairDelay)
		{
			repairing=true;
		}
		if(Input.GetButtonUp("WeaponSwap"))
		{
			wp[seats[curSeat].weaponSet].secondaryActive=!wp[seats[curSeat].weaponSet].secondaryActive;
		}
		if(Input.GetButtonUp("Module1"))
		{
			ModuleSlot mod = slots[seats[curSeat].moduleSlots[0]];
			if(mod.cooldown<=0)
			{
				mod.cooldown=mod.modules[0].cooldown*mod.modCooldown;
				mod.runtime=mod.modules[0].time*mod.modTime;
			}
		}
		if(Input.GetButtonUp("Module2"))
		{
			ModuleSlot mod = slots[seats[curSeat].moduleSlots[1]];
			if(mod.cooldown<=0)
			{
				mod.cooldown=mod.modules[0].cooldown*mod.modCooldown;
				mod.runtime=mod.modules[0].time*mod.modTime;
			}
		}
		if(Input.GetButtonUp("Module3"))
		{
			ModuleSlot mod = slots[seats[curSeat].moduleSlots[2]];
			if(mod.cooldown<=0)
			{
				mod.cooldown=mod.modules[0].cooldown*mod.modCooldown;
				mod.runtime=mod.modules[0].time*mod.modTime;
			}
		}
		if(Input.GetButtonUp("Module4"))
		{
			ModuleSlot mod = slots[seats[curSeat].moduleSlots[3]];
			if(mod.cooldown<=0)
			{
				mod.cooldown=mod.modules[0].cooldown*mod.modCooldown;
				mod.runtime=mod.modules[0].time*mod.modTime;
			}
		}
		foreach(ModuleSlot ms in slots)
		{
			ms.cooldown-=Time.deltaTime;
			if(ms.runtime>0)
			{
				ms.runtime-=Time.deltaTime;
				if(ms.mtype==ModuleSlot.ModuleType.Autorepair)
				{
					health+=Time.deltaTime*ms.modules[0].power1*ms.modPower;
				}
				else if(ms.mtype==ModuleSlot.ModuleType.JumpDrive)
				{
					if(ms.runtime<=0)
					{
						transform.Translate(ms.modules[0].power1*ms.modules[0].transform.forward, Space.World);
					}
				}
				else if(ms.mtype==ModuleSlot.ModuleType.Missile)
				{
					ms.runtime=-1;
					GameObject bullet = (GameObject)Instantiate(ms.modules[0].go, ms.modules[0].transform1.position, ms.modules[0].transform1.rotation);
					bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*ms.modules[0].time*ms.modTime);
				}
			}
		}
		int heals = injurySlots;
		foreach(Crew c in crewmembers)
		{
			if(c.j==Crew.Job.Engineer && c.injured==false && repairing)
			{
				health+=repairSpeed*Time.deltaTime;
			}
			if(c.injured && heals>0)
			{
				heals-=1;
				c.recoveryTime+=Time.deltaTime;
				if(c.recoveryTime>30)
				{
					c.recoveryTime=0;
					c.injured=false;
				}
			}
		}
		if(health<0)
			health=0;
		if(health>maxHealth)
		{
			health=maxHealth;
			repairing=false;
		}
		foreach(PowerSystem ps in powerSys)
		{
			ps.energyStored+=(ps.energyPerSec-ps.passive)*Time.deltaTime;
			if(ps.energyStored>ps.energyMax)
				ps.energyStored=ps.energyMax;
			if(ps.energyStored<0)
				ps.energyStored=0;
		}
		foreach(WeaponPair w in wp)
		{
			w.UpdateState();
		}
		if(Input.GetButton("Fire1"))
		{
			Cursor.lockState=CursorLockMode.Locked;
			Cursor.visible=false;
			wp[seats[curSeat].weaponSet].Fire(true);
		}
		if(Input.GetButtonUp("Fire1"))
		{
			wp[seats[curSeat].weaponSet].Fire(false);
		}
		if(Input.GetButtonUp("Reload"))
		{
			wp[seats[curSeat].weaponSet].Reload();
		}
		if(Input.GetButton("Fire2"))
		{
			Cursor.lockState=CursorLockMode.Locked;
			Cursor.visible=false;
		}
		if(Input.GetButtonUp("Pause"))
		{
			Cursor.lockState=CursorLockMode.None;
			Cursor.visible=true;
		}
		foreach(ThrusterGroup tg in tgs)
		{
			float tgx = Input.GetAxis(tg.axis);
			if(tgx>0)
			{
				for(int i = 0; i<tg.posThrust.Count; i++)
				{
					shipBody.AddForceAtPosition(tg.posThrust[i].transform.forward*tg.posThrust[i].force*Time.deltaTime, tg.posThrust[i].transform.position);
					tg.posThrust[i].burn.localScale = new Vector3(1, Mathf.Abs(tgx)+.1f, 1);
					tg.posThrust[i].burnLight.range = tg.posThrust[i].range*Mathf.Abs(tgx+.1f);
					tg.posThrust[i].time=Mathf.Abs(tgx);
				}
			}
			else
			{
				for(int i = 0; i<tg.posThrust.Count; i++)
				{
					tg.posThrust[i].burn.localScale = new Vector3(1, 0.1f, 1);
					tg.posThrust[i].burnLight.range = tg.posThrust[i].range*0.1f;
					tg.posThrust[i].time=0;
				}
			}
			if(tgx<0)
			{
				for(int i = 0; i<tg.negThrust.Count; i++)
				{
					shipBody.AddForceAtPosition(tg.negThrust[i].transform.forward*tg.negThrust[i].force*Time.deltaTime, tg.negThrust[i].transform.position);
					tg.negThrust[i].burn.localScale = new Vector3(1, Mathf.Abs(tgx), 1);
					tg.negThrust[i].burnLight.range = tg.negThrust[i].range*Mathf.Abs(tgx+.1f);
					tg.negThrust[i].time=Mathf.Abs(tgx);
				}
			}
			else
			{
				for(int i = 0; i<tg.negThrust.Count; i++)
				{
					tg.negThrust[i].burn.localScale = new Vector3(1, 0.1f, 1);
					tg.negThrust[i].burnLight.range = tg.negThrust[i].range*0.1f;
					tg.negThrust[i].time=0;
				}
			}
		}
		WeaponPair wpp = wp[seats[curSeat].weaponSet];
		if(wpp.secondaryActive)
		{
			foreach(Weapon w in wpp.weapons2)
			{
				w.LookAt(seats[curSeat].weaponTarget);
				w.active=true;
			}
			foreach(Weapon w in wpp.weapons1)
			{
				w.active=false;
			}
		}
		else
		{
			foreach(Weapon w in wpp.weapons1)
			{
				w.LookAt(seats[curSeat].weaponTarget);
				w.active=true;
			}
			foreach(Weapon w in wpp.weapons2)
			{
				w.active=false;
			}
		}
	}
}