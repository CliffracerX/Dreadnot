using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

[System.Serializable]
public class WeaponPreset
{
	public int ammo;
	public float firerate,reloadSpeed;
}

public class ShipSpawner : MonoBehaviour
{
	public TextAsset shipFilePlayer,shipFileTrainer;
	public GameObject shipCorePrefab;
	public GameObject[] blockPrefabs;
	public WeaponPreset[] weaponPresets;
	public ModuleSlot.ModuleType[] moduleTypes;
	public string[] blockNames,weaponNames,moduleNames;
	public Dictionary<string, GameObject> blocks;
	public Dictionary<string, WeaponPreset> weapons;
	public Dictionary<string, ModuleSlot.ModuleType> modules;
	public bool spawnPlayer = true;
	public bool spawnAI = false;
	public ShipScript myShip;
	public string shipPath = "Ships/NotDover.txt";

	void Start()
	{
		blocks = new Dictionary<string, GameObject>();
		weapons = new Dictionary<string, WeaponPreset>();
		modules = new Dictionary<string, ModuleSlot.ModuleType>();
		for(int i = 0; i<blockPrefabs.Length; i++)
		{
			blocks.Add(blockNames[i], blockPrefabs[i]);
		}
		for(int i = 0; i<weaponPresets.Length; i++)
		{
			weapons.Add(weaponNames[i], weaponPresets[i]);
		}
		for(int i = 0; i<moduleTypes.Length; i++)
		{
			modules.Add(moduleNames[i], moduleTypes[i]);
		}
		if(PlayerPrefs.HasKey("ShipPath"))
		{
			shipPath=PlayerPrefs.GetString("ShipPath");
		}
	}

	void OnGUI()
	{
		if(!myShip)
		{
			shipPath = GUI.TextField(new Rect(15, 15, Screen.width-30, 25), shipPath);
			if(GUI.Button(new Rect(15, 40, Screen.width-30, 25), "SPAWN SHIP"))
			{
				spawnPlayer=true;
				PlayerPrefs.SetString("ShipPath", shipPath);
			}
			if(GUI.Button(new Rect(15, Screen.height-40, Screen.width-30, 25), "BACK TO MENU"))
			{
				Application.LoadLevel("SceneMenu");
			}
		}
	}

	void Update()
	{
		if(spawnPlayer)
		{
			SpawnShip(shipFilePlayer);
			spawnPlayer=false;
		}
	}

	void SpawnShip(TextAsset file)
	{
		GameObject shipBase = (GameObject)Instantiate(shipCorePrefab, transform.position, transform.rotation);
		ShipScript ss = shipBase.GetComponent<ShipScript>();
		myShip=ss;
		Transform targ = shipBase.transform.FindChild("CameraPos/CamObj/WeaponsTarget");
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
		string[] posOff = strings[offset].Split(',');
		offset+=1;
		Vector3 poff = new Vector3(float.Parse(posOff[0]), float.Parse(posOff[1]), float.Parse(posOff[2]));
		int numSeats = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.seats = new BridgeSeat[numSeats];
		for(int i = 0; i<numSeats; i++)
		{
			ss.seats[i] = new BridgeSeat();
			ss.seats[i].name=strings[offset];
			ss.seats[i].weaponTarget=targ;
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
			string wepSetName = strings[offset];
			print(strings[offset]);
			offset+=1;
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
			wp.name=wepSetName;
			ss.wp.Add(wp);
		}
		int numPowerSys = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		ss.powerSys = new List<PowerSystem>();
		for(int i = 0; i<numPowerSys; i++)
		{
			string powName = strings[offset];
			print(strings[offset]);
			offset+=1;
			int numModules = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			PowerSystem ps = new PowerSystem();
			ps.name=powName;
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
			string modName = strings[offset];
			print(strings[offset]);
			offset+=1;
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
			float time = 1 + (timePower-10)*0.1f;
			float cool = 1 + (coolPower-20)*-0.025f;
			ModuleSlot ms = new ModuleSlot();
			ms.name=modName;
			ms.modPower=power;
			ms.modTime=time;
			ms.modCooldown=cool;
			ms.mtype=modules[modType];
			ms.modules = new List<Module>();
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
		targ.localPosition = new Vector3(0, 0, range);
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
			blockObj.transform.localPosition=pos+poff;
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
		}
		sr.Close();
	}
}