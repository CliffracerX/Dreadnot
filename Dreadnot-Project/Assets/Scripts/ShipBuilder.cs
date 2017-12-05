using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class BBlock
{
	public string blockType;
	public Vector3 position,rotation;
	public int pSys,optSlot,optBonus;
	public GameObject repreObj;
}

[System.Serializable]
public class ProtoPowersys
{
	public List<AuxPowerSlot> slots;
}

[System.Serializable]
public class ProtoModset
{
	public ModuleSlot.ModuleType mtype;
	public int pow,tim,col;
}

[System.Serializable]
public class ProtoThrust
{
	public string name,axis;
}

[System.Serializable]
public class ProtoWep
{
	public string primWep,altWep;
}

[System.Serializable]
public class ProtoSeat
{
	public string name;
	public List<int> thrusts;
	public int wepSet;
	public int powSet;
	public int[] modSlots;
}

public class ShipBuilder : MonoBehaviour
{
	public List<BBlock> blocks;
	public List<ProtoPowersys> pows;
	public List<ProtoModset> mods;
	public List<ProtoThrust> thrust;
	public List<ProtoWep> weps;
	public List<ProtoSeat> seats;
	public GameObject[] blockAppearences;
	public string[] blockNames;
	public Dictionary<string, GameObject> blockAps;
	public string shipPath = "Ships/YourShipHere.txt";
	public Camera cam;
	public float sensitivity,speed;

	void Start()
	{
		blockAps = new Dictionary<string, GameObject>();
		for(int i = 0; i<blockAppearences.Length; i++)
		{
			blockAps.Add(blockNames[i], blockAppearences[i]);
		}
	}

	void LoadShip()
	{
		/*ShipScript ss = shipBase.GetComponent<ShipScript>();
		myShip=ss;
		int offset = 0;
		//string[] sts = shipFilePlayer.text.Split('\n');
		StreamReader sr = new StreamReader(Application.dataPath+"/"+shipPath);
		string[] sts = sr.ReadToEnd().Split('\n');
		List<string> stringss = new List<string>();
		foreach(string s in sts)
		{
			string result = Regex.Replace(s, @"\r\n?|\n", "");
			stringss.Add(result);
		}
		string[] strings = stringss.ToArray();
		string shipName = strings[offset];
		print(strings[offset]);
		offset+=1;
		shipBase.name=shipName;
		string[] colorPrimary = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cPrimary = new Color(float.Parse(colorPrimary[0])/255, float.Parse(colorPrimary[1])/255, float.Parse(colorPrimary[2])/255, 255);
		ss.shipColorMain=cPrimary;
		string[] colorPower = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cPower = new Color(float.Parse(colorPower[0])/255, float.Parse(colorPower[1])/255, float.Parse(colorPower[2])/255, 255);
		ss.shipColorPower=cPower;
		string[] colorThrust = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cThrust = new Color(float.Parse(colorThrust[0])/255, float.Parse(colorThrust[1])/255, float.Parse(colorThrust[2])/255, 255);
		ss.shipColorThrust=cThrust;
		string[] colorSpecial = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cSpecial = new Color(float.Parse(colorSpecial[0])/255, float.Parse(colorSpecial[1])/255, float.Parse(colorSpecial[2])/255, 255);
		ss.shipColorSpecial=cSpecial;
		int numSeats = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.seats = new BridgeSeat[numSeats];
		for(int i = 0; i<numSeats; i++)
		{
			ss.seats[i] = new BridgeSeat();
			ss.seats[i].name=strings[offset];
			print(strings[offset]);
			offset+=1;
			ss.seats[i].availableThrusts = new List<int>();
			int numThrustsSeat = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			for(int n = 0; n<numThrustsSeat; n++)
			{
				ss.seats[i].availableThrusts.Add(int.Parse(strings[offset]));
				print(strings[offset]);
				offset+=1;
			}
			ss.seats[i].weaponSet=int.Parse(strings[offset]);
			print("WEAPON SET: "+strings[offset]);
			offset+=1;
			ss.seats[i].powerSet=int.Parse(strings[offset]);
			print("POWER SET: "+strings[offset]);
			offset+=1;
			int numSeatModules = int.Parse(strings[offset]);
			print("NUM SEAT MODULES: "+strings[offset]);
			offset+=1;
			ss.seats[i].moduleSlots = new int[numSeatModules];
			for(int n = 0; n<numSeatModules; n++)
			{
				ss.seats[i].moduleSlots[n]=int.Parse(strings[offset]);
				print(strings[offset]);
				offset+=1;
			}
		}
		int numThrusts = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.tgs = new List<ThrusterGroup>();
		for(int i = 0; i<numThrusts; i++)
		{
			ThrusterGroup tg = new ThrusterGroup();
			tg.name=strings[offset];
			print(strings[offset]);
			offset+=1;
			tg.axis=strings[offset];
			print(strings[offset]);
			offset+=1;
			tg.negThrust = new List<Thruster>();
			tg.posThrust = new List<Thruster>();
			ss.tgs.Add(tg);
		}
		int numWeps = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.wp = new List<WeaponPair>();
		for(int i = 0; i<numWeps; i++)
		{
			string weaponPrimary = strings[offset];
			print(strings[offset]);
			offset+=1;
			string weaponSecondary = strings[offset];
			print(strings[offset]);
			offset+=1;
			WeaponPreset wep1 = weapons[weaponPrimary];
			WeaponPreset wep2 = weapons[weaponSecondary];
			WeaponPair wp = new WeaponPair();
			wp.weapons1=new List<Weapon>();
			wp.weapons2=new List<Weapon>();
			wp.ammo1=wep1.ammo;
			wp.ammoMax1=wep1.ammo;
			wp.ammo2=wep2.ammo;
			wp.ammoMax2=wep2.ammo;
			wp.firerate1=wep1.firerate;
			wp.reloadSpeed1=wep1.reloadSpeed;
			wp.firerate2=wep2.firerate;
			wp.reloadSpeed2=wep2.reloadSpeed;
			ss.wp.Add(wp);
		}
		int numPowerSys = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.powerSys = new List<PowerSystem>();
		for(int i = 0; i<numPowerSys; i++)
		{
			int numModules = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			PowerSystem ps = new PowerSystem();
			ps.slots = new List<AuxPowerSlot>();
			for(int n = 0; n<numModules; n++)
			{
				string modType = strings[offset];
				print(strings[offset]);
				offset+=1;
				float mod = float.Parse(strings[offset]);
				print(strings[offset]);
				offset+=1;
				AuxPowerSlot aps = new AuxPowerSlot();
				if(modType=="Shield")
				{
					aps.mtype = AuxPowerSlot.ModuleType.Shield;
				}
				else if(modType=="Thrust")
				{
					aps.mtype = AuxPowerSlot.ModuleType.Thrust;
				}
				else if(modType=="Weapon")
				{
					aps.mtype = AuxPowerSlot.ModuleType.Weapon;
				}
				aps.mod=mod;
				ps.slots.Add(aps);
			}
			ss.powerSys.Add(ps);
		}
		int numModulesReal = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.slots = new List<ModuleSlot>();
		for(int i = 0; i<numModulesReal; i++)
		{
			string modType = strings[offset];
			print(strings[offset]);
			offset+=1;
			int powerPower = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			int timePower = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			int coolPower = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			float power = 1 + (powerPower-20)*0.05f;
			float time = 1 + (powerPower-10)*0.1f;
			float cool = 1 + (powerPower-20)*-0.025f;
			ModuleSlot ms = new ModuleSlot();
			ms.modPower=power;
			ms.modTime=time;
			ms.modCooldown=cool;
			ms.mtype=modules[modType];
			ss.slots.Add(ms);
		}
		float offsetX = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		float offsetY = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		float offsetZ = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		shipBase.transform.GetChild(0).localPosition = new Vector3(offsetX, offsetY, offsetZ);
		float camrange = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		shipBase.transform.FindChild("CameraPos/CamObj/Main Camera").localPosition = new Vector3(0, 0, -camrange);
		float range = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		shipBase.transform.FindChild("CameraPos/CamObj/WeaponsTarget").localPosition = new Vector3(0, 0, range);
		int numBlocks = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.blocks = new List<Block>();
		for(int i = 0; i<numBlocks; i++)
		{
			string[] bb = strings[offset].Split('/');
			print(strings[offset]);
			offset+=1;
			string blockType = bb[0];
			string[] position = bb[1].Split(',');
			string[] rotation = bb[2].Split(',');
			Vector3 pos = new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2]));
			Vector3 ea = new Vector3(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2]));
			Quaternion rot = new Quaternion();
			rot.eulerAngles = ea;
			GameObject blockObj = (GameObject)Instantiate(blocks[blockType], Vector3.zero, Quaternion.identity);
			blockObj.transform.parent=shipBase.transform;
			blockObj.transform.localPosition=pos;
			blockObj.transform.localRotation=rot;
			Block b = blockObj.GetComponent<Block>();
			b.powerSys=int.Parse(bb[3]);
			if(b is Weapon)
			{
				Weapon w = (Weapon)b;
				w.weaponGroupID=int.Parse(bb[4]);
				w.oneTwo=int.Parse(bb[5])==1;
			}
			if(b is Module)
			{
				Module m = (Module)b;
				m.moduleSlot=int.Parse(bb[4]);
			}
			if(b is AuxModule)
			{
				AuxModule am = (AuxModule)b;
				am.moduleSlot=int.Parse(bb[4]);
			}
			if(b is Thruster)
			{
				Thruster t = (Thruster)b;
				t.thrustGroup=int.Parse(bb[4]);
				t.posNeg=int.Parse(bb[5])==1;
			}
		}*/
	}

	void AddBlock(string blockType, Vector3 pos, Vector3 rot, int pSys, int optSlot, int optBonus)
	{
		Quaternion q = new Quaternion();
		q.eulerAngles=rot;
		GameObject go = (GameObject)Instantiate(blockAps[blockType], pos, q);
		BBlock bb = new BBlock();
		bb.blockType=blockType;
		bb.position=pos;
		bb.rotation=rot;
		bb.pSys=pSys;
		bb.optSlot=optSlot;
		bb.optBonus=optBonus;
		bb.repreObj=go;
	}

	void RemoveBlock(Vector3 pos)
	{
		foreach(BBlock b in blocks)
		{
			if(b.position==pos)
			{
				Destroy(b.repreObj);
				blocks.Remove(b);
			}
		}
	}

	void Update()
	{
		float rx = cam.transform.rotation.eulerAngles.x;
		rx-=Input.GetAxis("Mouse Y")*sensitivity;
		if(rx>360)
			rx=-360;
		if(rx<-360)
			rx=360;
		float ry = cam.transform.rotation.eulerAngles.y;
		ry+=Input.GetAxis("Mouse X")*sensitivity;
		if(ry>360)
			ry=-360;
		if(ry<-360)
			ry=360;
		Quaternion q = Quaternion.identity;
		q.eulerAngles = new Vector3(rx, ry, 0);
		cam.transform.rotation = q;
		cam.transform.Translate(Input.GetAxis("Horizontal")*Time.deltaTime*speed, Input.GetAxis("Jump")*Time.deltaTime*speed, Input.GetAxis("Vertical")*Time.deltaTime*speed);
		if(Input.GetButton("Fire1"))
		{
			Cursor.lockState=CursorLockMode.Locked;
			Cursor.visible=false;
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
	}

	void ClearShip()
	{
		foreach(BBlock b in blocks)
		{
			Destroy(b.repreObj);
		}
		blocks.Clear();
		pows.Clear();
		mods.Clear();
		thrust.Clear();
		weps.Clear();
		seats.Clear();
	}
}