using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
	public string name;
	public List<AuxPowerSlot> slots;
}

[System.Serializable]
public class ProtoModset
{
	public string name;
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
	public string name,primWep,altWep;
}

[System.Serializable]
public class ProtoSeat
{
	public string name;
	public List<int> thrusts;
	public int wepSet;
	public int powSet;
	public int[] modSlots;

	/*public ProtoSeat(int numMods)
	{
		this.modSlots = new int[numMods];
		this.thrusts = new List<int>();
	}*/
}

[System.Serializable]
public class BlockSet
{
	public string name;
	public string[] blocksIn,blockDescs;
	public int[] blockIDs;
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
	public enum BlockType {Hull=0, Power=1, Thruster=2, Module=3, Weapon=4, APM=5}
	public BlockType[] types;
	public GameObject[] blockPreviews;
	public Dictionary<string, BlockType> blockToType;
	public Dictionary<string, GameObject> blockAps;
	public BlockSet[] bs;
	public string shipPath = "Ships/YourShipHere.txt";
	public Camera cam;
	public float sensitivity,speed;
	public string shipsName;
	public Color colMain,colPower,colThrust,colSpecial;
	public ModuleSlot.ModuleType[] moduleTypes;
	public string[] moduleNames;
	public Dictionary<string, ModuleSlot.ModuleType> modules;
	public Transform rotPivot,camTemp;
	public float wepRange;
	public Vector3 poff;
	public float timeThing;
	public int curBlockIC,curClass,curBlock;
	public int curPSys,curOpt1,curOptBon;
	public GUIStyle style;
	public Transform blockPrev,blockPrevPos;

	void Start()
	{
		blockAps = new Dictionary<string, GameObject>();
		for(int i = 0; i<blockAppearences.Length; i++)
		{
			blockAps.Add(blockNames[i], blockAppearences[i]);
		}
		modules = new Dictionary<string, ModuleSlot.ModuleType>();
		for(int i = 0; i<moduleTypes.Length; i++)
		{
			modules.Add(moduleNames[i], moduleTypes[i]);
		}
		blockToType = new Dictionary<string, BlockType>();
		for(int i = 0; i<blockNames.Length; i++)
		{
			blockToType.Add(blockNames[i], types[i]);
		}
		if(PlayerPrefs.HasKey("ShipPath"))
		{
			shipPath=PlayerPrefs.GetString("ShipPath");
		}
	}

	void SaveShip()
	{
		StreamWriter sw = new StreamWriter(Application.dataPath+"/"+shipPath);
		sw.WriteLine(shipsName);
		int cmR,cmG,cmB;
		cmR = (int)(255*colMain.r);
		cmG = (int)(255*colMain.g);
		cmB = (int)(255*colMain.b);
		sw.WriteLine(cmR+","+cmG+","+cmB);
		int cpR,cpG,cpB;
		cpR = (int)(255*colPower.r);
		cpG = (int)(255*colPower.g);
		cpB = (int)(255*colPower.b);
		sw.WriteLine(cpR+","+cpG+","+cpB);
		int ctR,ctG,ctB;
		ctR = (int)(255*colThrust.r);
		ctG = (int)(255*colThrust.g);
		ctB = (int)(255*colThrust.b);
		sw.WriteLine(ctR+","+ctG+","+ctB);
		int csR,csG,csB;
		csR = (int)(255*colSpecial.r);
		csG = (int)(255*colSpecial.g);
		csB = (int)(255*colSpecial.b);
		sw.WriteLine(csR+","+csG+","+csB);
		sw.WriteLine(poff.x+","+poff.y+","+poff.z);
		sw.WriteLine(seats.Count);
		for(int i = 0; i<seats.Count; i++)
		{
			sw.WriteLine(seats[i].name);
			sw.WriteLine(seats[i].thrusts.Count);
			foreach(int j in seats[i].thrusts)
			{
				sw.WriteLine(j);
			}
			sw.WriteLine(seats[i].wepSet);
			sw.WriteLine(seats[i].powSet);
			sw.WriteLine(seats[i].modSlots.Length);
			for(int j = 0; j<seats[i].modSlots.Length; j++)
			{
				sw.WriteLine(seats[i].modSlots[j]);
			}
		}
		sw.WriteLine(thrust.Count);
		for(int i = 0; i<thrust.Count; i++)
		{
			sw.WriteLine(thrust[i].name);
			sw.WriteLine(thrust[i].axis);
		}
		sw.WriteLine(weps.Count);
		for(int i = 0; i<weps.Count; i++)
		{
			sw.WriteLine(weps[i].name);
			sw.WriteLine(weps[i].primWep);
			sw.WriteLine(weps[i].altWep);
		}
		sw.WriteLine(pows.Count);
		for(int i = 0; i<pows.Count; i++)
		{
			sw.WriteLine(pows[i].name);
			sw.WriteLine(pows[i].slots.Count);
			for(int j = 0; j<pows[i].slots.Count; j++)
			{
				sw.WriteLine(pows[i].slots[j].mtype.ToString());
				sw.WriteLine(pows[i].slots[j].mod);
			}
		}
		sw.WriteLine(mods.Count);
		for(int i = 0; i<mods.Count; i++)
		{
			sw.WriteLine(mods[i].name);
			sw.WriteLine(mods[i].mtype.ToString());
			sw.WriteLine(mods[i].pow);
			sw.WriteLine(mods[i].tim);
			sw.WriteLine(mods[i].col);
		}
		sw.WriteLine(rotPivot.position.x);
		sw.WriteLine(rotPivot.position.y);
		sw.WriteLine(rotPivot.position.z);
		sw.WriteLine(-camTemp.transform.localPosition.z);
		sw.WriteLine(wepRange);
		sw.WriteLine(blocks.Count);
		for(int i = 0; i<blocks.Count; i++)
		{
			string thisBlock = blocks[i].blockType;
			thisBlock+="/"+blocks[i].position.x+","+blocks[i].position.y+","+blocks[i].position.z;
			thisBlock+="/"+blocks[i].rotation.x+","+blocks[i].rotation.y+","+blocks[i].rotation.z;
			thisBlock+="/"+blocks[i].pSys;
			thisBlock+="/"+blocks[i].optSlot;
			thisBlock+="/"+blocks[i].optBonus;
			sw.WriteLine(thisBlock);
		}
		sw.Close();
	}

	void OnGUI()
	{
		shipPath = GUI.TextField(new Rect(15, 15, Screen.width-30, 25), shipPath, style);
		if(GUI.Button(new Rect(15, 40, 300, 25), "LOAD SHIP", style))
		{
			ClearShip();
			LoadShip();
			PlayerPrefs.SetString("ShipPath", shipPath);
		}
		if(GUI.Button(new Rect(Screen.width-315, 40, 300, 25), "SAVE SHIP", style))
		{
			SaveShip();
			PlayerPrefs.SetString("ShipPath", shipPath);
		}
		if(GUI.Button(new Rect(Screen.width/2-150, 40, 300, 25), "CLEAR SHIP", style))
		{
			ClearShip();
		}
		if(GUI.Button(new Rect(Screen.width/2-150, Screen.height-40, 300, 25), "RETURN TO MENU", style))
		{
			Application.LoadLevel("SceneMenu");
		}
		GUI.Label(new Rect(Screen.width/2-300, 65, 600, 100), "Fire1 to add/remove blocks w/ captured mouse.\nMousewheel to change selected block.  Ctrl/CMD+Wheel for block set.\n1 to change power systems.\n2 to cycle through groups.\n3 for bonus block options.\nQ/E, R/F, & Z/X to rotate blocks.", style);
		if(types[curBlock]==BlockType.Hull)
		{
			GUI.Label(new Rect(15, 65, 300, 50), "BLOCK: "+blockNames[curBlock]+"\n"+bs[curClass].blockDescs[curBlockIC], style);
		}
		if(types[curBlock]==BlockType.Power)
		{
			GUI.Label(new Rect(15, 65, 300, 50), "BLOCK: "+blockNames[curBlock]+", POWER: "+pows[curPSys].name+"\n"+bs[curClass].blockDescs[curBlockIC], style);
		}
		if(types[curBlock]==BlockType.Thruster)
		{
			GUI.Label(new Rect(15, 65, 300, 50), "BLOCK: "+blockNames[curBlock]+", POWER: "+pows[curPSys].name+", GROUP: "+thrust[curOpt1].name+", POSITIVE: "+(curOptBon==1)+"\n"+bs[curClass].blockDescs[curBlockIC], style);
		}
		if(types[curBlock]==BlockType.Module)
		{
			GUI.Label(new Rect(15, 65, 300, 50), "BLOCK: "+blockNames[curBlock]+", POWER: "+pows[curPSys].name+", GROUP: "+mods[curOpt1].name+"\n"+bs[curClass].blockDescs[curBlockIC], style);
		}
		if(types[curBlock]==BlockType.Weapon)
		{
			GUI.Label(new Rect(15, 65, 300, 50), "BLOCK: "+blockNames[curBlock]+", POWER: "+pows[curPSys].name+", GROUP: "+weps[curOpt1].name+", PRIMARY: "+(curOptBon==0)+"\n"+bs[curClass].blockDescs[curBlockIC], style);
		}
		if(types[curBlock]==BlockType.APM)
		{
			GUI.Label(new Rect(15, 65, 300, 50), "BLOCK: "+blockNames[curBlock]+", POWER: "+pows[curPSys].name+", GROUP: "+pows[curPSys].slots[curOpt1].mtype.ToString()+"\n"+bs[curClass].blockDescs[curBlockIC], style);
		}
		winRect = GUI.Window(0, winRect, DrawWindow, "", style);
	}

	public Rect winRect;

	public Vector2 scrollP;
	public Texture2D colorSwatch;
	public int tempCMR,tempCMG,tempCMB;
	public int tempCPR,tempCPG,tempCPB;
	public int tempCTR,tempCTG,tempCTB;
	public int tempCSR,tempCSG,tempCSB;
	public int group;

	void DrawWindow(int id)
	{
		//GUI.DragWindow();
		scrollP = GUILayout.BeginScrollView(scrollP, style);
		GUILayout.Space(25);
		GUILayout.Label("-=SHIP NAME=-", style, GUILayout.Width(220));
		shipsName = GUILayout.TextField(shipsName, style, GUILayout.Width(220));
		GUILayout.Label("-=PRIMARY SHIP COLOR (RGB 0-255)=-", style, GUILayout.Width(220));
		tempCMR = int.Parse(GUILayout.TextField(""+tempCMR, style, GUILayout.Width(220)));
		tempCMG = int.Parse(GUILayout.TextField(""+tempCMG, style, GUILayout.Width(220)));
		tempCMB = int.Parse(GUILayout.TextField(""+tempCMB, style, GUILayout.Width(220)));
		GUILayout.Label("-=SHIP POWER COLOR (RGB 0-255)=-", style, GUILayout.Width(220));
		tempCPR = int.Parse(GUILayout.TextField(""+tempCPR, style, GUILayout.Width(220)));
		tempCPG = int.Parse(GUILayout.TextField(""+tempCPG, style, GUILayout.Width(220)));
		tempCPB = int.Parse(GUILayout.TextField(""+tempCPB, style, GUILayout.Width(220)));
		GUILayout.Label("-=SHIP THRUST COLOR (RGB 0-255)=-", style, GUILayout.Width(220));
		tempCTR = int.Parse(GUILayout.TextField(""+tempCTR, style, GUILayout.Width(220)));
		tempCTG = int.Parse(GUILayout.TextField(""+tempCTG, style, GUILayout.Width(220)));
		tempCTB = int.Parse(GUILayout.TextField(""+tempCTB, style, GUILayout.Width(220)));
		GUILayout.Label("-=SHIP SPECIAL COLOR (RGB 0-255)=-", style, GUILayout.Width(220));
		tempCSR = int.Parse(GUILayout.TextField(""+tempCSR, style, GUILayout.Width(220)));
		tempCSG = int.Parse(GUILayout.TextField(""+tempCSG, style, GUILayout.Width(220)));
		tempCSB = int.Parse(GUILayout.TextField(""+tempCSB, style, GUILayout.Width(220)));
		//if(GUILayout.Button("APPLY COLOR", style, GUILayout.Width(220)))
		//{
			colMain = new Color(tempCMR/255f, tempCMG/255f, tempCMB/255f);
			colPower = new Color(tempCPR/255f, tempCPG/255f, tempCPB/255f);
			colThrust = new Color(tempCTR/255f, tempCTG/255f, tempCTB/255f);
			colSpecial = new Color(tempCSR/255f, tempCSG/255f, tempCSB/255f);
		//}
		//if(GUILayout.Button("LOAD COLOR", style, GUILayout.Width(220)))
		//{
			tempCMR = (int)(colMain.r*255);
			tempCMG = (int)(colMain.g*255);
			tempCMB = (int)(colMain.b*255);
			tempCPR = (int)(colPower.r*255);
			tempCPG = (int)(colPower.g*255);
			tempCPB = (int)(colPower.b*255);
			tempCTR = (int)(colThrust.r*255);
			tempCTG = (int)(colThrust.g*255);
			tempCTB = (int)(colThrust.b*255);
			tempCSR = (int)(colSpecial.r*255);
			tempCSG = (int)(colSpecial.g*255);
			tempCSB = (int)(colSpecial.b*255);
		//}
		GUILayout.Label("-=BRIDGE SEATING=-", style, GUILayout.Width(220));
		if(GUILayout.Button("ADD SEAT", style, GUILayout.Width(220)))
		{
			ProtoSeat ps = new ProtoSeat();
			ps.name="";
			ps.thrusts = new List<int>();
			ps.powSet=0;
			ps.wepSet=0;
			ps.modSlots = new int[4];
			seats.Add(ps);
		}
		for(int i = 0; i<seats.Count; i++)
		{
			GUILayout.Label("Seat: "+seats[i].name+"/#"+i, style, GUILayout.Width(220));
			seats[i].name = GUILayout.TextField(seats[i].name, style, GUILayout.Width(220));
			GUILayout.Label("Seat thruster group IDs", style, GUILayout.Width(220));
			for(int j = 0; j<thrust.Count; j++)
			{
				if(GUILayout.Button("ADD GROUP "+thrust[j].name, style, GUILayout.Width(220)))
				{
					seats[i].thrusts.Add(j);
				}
			}
			for(int j = 0; j<seats[i].thrusts.Count; j++)
			{
				if(GUILayout.Button("REMOVE GROUP: "+thrust[seats[i].thrusts[j]].name, style, GUILayout.Width(220)))
				{
					seats[i].thrusts.RemoveAt(j);
				}
			}
			GUILayout.Label("Seat weapon group", style, GUILayout.Width(220));
			for(int j = 0; j<weps.Count; j++)
			{
				if(seats[i].wepSet==j)
				{
					GUI.color=colSpecial;
				}
				else
				{
					GUI.color=Color.white;
				}
				if(GUILayout.Button("SET GROUP "+weps[j].name, style, GUILayout.Width(220)))
				{
					seats[i].wepSet=j;
				}
				GUI.color=Color.white;
			}
			GUILayout.Label("Seat power system", style, GUILayout.Width(220));
			for(int j = 0; j<pows.Count; j++)
			{
				if(seats[i].powSet==j)
				{
					GUI.color=colPower;
				}
				else
				{
					GUI.color=Color.white;
				}
				if(GUILayout.Button("SET GROUP "+pows[j].name, style, GUILayout.Width(220)))
				{
					seats[i].wepSet=j;
				}
				GUI.color=Color.white;
			}
			for(int m = 0; m<seats[i].modSlots.Length; m++)
			{
				GUILayout.Label("Seat module slot #"+m, style, GUILayout.Width(220));
				for(int j = 0; j<mods.Count; j++)
				{
					if(seats[i].modSlots[m]==j)
					{
						GUI.color=colPower;
					}
					else
					{
						GUI.color=Color.white;
					}
					if(GUILayout.Button("SET GROUP "+mods[j].name, style, GUILayout.Width(220)))
					{
						seats[i].modSlots[m]=j;
					}
					GUI.color=Color.white;
				}
			}
			if(GUILayout.Button("!!WARNING: REMOVE SEAT!!", style, GUILayout.Width(220)))
			{
				seats.RemoveAt(i);
			}
		}
		GUILayout.Label("-=THRUSTER GROUPS=-", style, GUILayout.Width(220));
		if(GUILayout.Button("ADD GROUP", style, GUILayout.Width(220)))
		{
			ProtoThrust pt = new ProtoThrust();
			pt.name="";
			pt.axis="";
			thrust.Add(pt);
		}
		for(int i = 0; i<thrust.Count; i++)
		{
			GUILayout.Label("Thruster Group: "+thrust[i].name+"/#"+i, style, GUILayout.Width(220));
			thrust[i].name = GUILayout.TextField(thrust[i].name, style, GUILayout.Width(220));
			GUILayout.Label("Thruster Axis: "+thrust[i].axis, style, GUILayout.Width(220));
			thrust[i].axis = GUILayout.TextField(thrust[i].axis, style, GUILayout.Width(220));
			GUILayout.Label("REMOVE ALL REFERENCES TO THRUSTERGROUP BEFORE USE!", style, GUILayout.Width(220));
			if(GUILayout.Button("!!WARNING: REMOVE THRUSTER GROUP!!", style, GUILayout.Width(220)))
			{
				thrust.RemoveAt(i);
			}
		}
		GUILayout.Label("-=WEAPON GROUPS=-", style, GUILayout.Width(220));
		GUILayout.Label("WEAPON TYPES AVAILABLE RIGHT NOW: LightGatling, LightBeam", style, GUILayout.Width(220));
		GUILayout.Label("LightGatling: 240RPM 20-round-clip light gatling weapons.  Mid damage, mid range.", style, GUILayout.Width(220));
		GUILayout.Label("LightBeam: Constant DPS infinite ammo beam weapons.  Mid damage, short range.", style, GUILayout.Width(220));
		if(GUILayout.Button("ADD GROUP", style, GUILayout.Width(220)))
		{
			ProtoWep pw = new ProtoWep();
			pw.name="";
			pw.primWep="";
			pw.altWep="";
			weps.Add(pw);
		}
		for(int i = 0; i<weps.Count; i++)
		{
			GUILayout.Label("Weapon Group: "+weps[i].name+"/#"+i, style, GUILayout.Width(220));
			weps[i].name = GUILayout.TextField(weps[i].name, style, GUILayout.Width(220));
			GUILayout.Label("Primary Weapon: "+weps[i].primWep, style, GUILayout.Width(220));
			weps[i].primWep = GUILayout.TextField(weps[i].primWep, style, GUILayout.Width(220));
			GUILayout.Label("Secondary Weapon: "+weps[i].altWep, style, GUILayout.Width(220));
			weps[i].altWep = GUILayout.TextField(weps[i].altWep, style, GUILayout.Width(220));
			GUILayout.Label("REMOVE ALL REFERENCES TO WEAPONGROUP BEFORE USE!", style, GUILayout.Width(220));
			if(GUILayout.Button("!!WARNING: REMOVE WEAPON GROUP!!", style, GUILayout.Width(220)))
			{
				weps.RemoveAt(i);
			}
		}
		GUILayout.Label("-=POWER SYSTEMS=-", style, GUILayout.Width(220));
		GUILayout.Label("APMS AVAILABLE RIGHT NOW: Shield, Thrust, Weapon", style, GUILayout.Width(220));
		GUILayout.Label("Shield: Absorbs damage while active.", style, GUILayout.Width(220));
		GUILayout.Label("Thrust: Boosts thruster strength.", style, GUILayout.Width(220));
		GUILayout.Label("Weapon: Increases weapon damage, firerate, and reload speed.", style, GUILayout.Width(220));
		if(GUILayout.Button("ADD GROUP", style, GUILayout.Width(220)))
		{
			ProtoPowersys pp = new ProtoPowersys();
			pp.name="";
			pp.slots = new List<AuxPowerSlot>();
			pows.Add(pp);
		}
		for(int i = 0; i<pows.Count; i++)
		{
			GUILayout.Label("Power System: "+pows[i].name+"/#"+i, style, GUILayout.Width(220));
			pows[i].name = GUILayout.TextField(pows[i].name, style, GUILayout.Width(220));
			GUILayout.Label("APMS: "+pows[i].slots.Count, style, GUILayout.Width(220));
			if(GUILayout.Button("ADD SHIELD APM", style, GUILayout.Width(220)))
			{
				AuxPowerSlot aps = new AuxPowerSlot();
				aps.mtype = AuxPowerSlot.ModuleType.Shield;
				pows[i].slots.Add(aps);
			}
			if(GUILayout.Button("ADD THRUST APM", style, GUILayout.Width(220)))
			{
				AuxPowerSlot aps = new AuxPowerSlot();
				aps.mtype = AuxPowerSlot.ModuleType.Thrust;
				pows[i].slots.Add(aps);
			}
			if(GUILayout.Button("ADD WEAPON APM", style, GUILayout.Width(220)))
			{
				AuxPowerSlot aps = new AuxPowerSlot();
				aps.mtype = AuxPowerSlot.ModuleType.Weapon;
				pows[i].slots.Add(aps);
			}
			for(int j = 0; j<pows[i].slots.Count; j++)
			{
				GUILayout.Label("APM #"+j+" TYPE: "+pows[i].slots[j].mtype.ToString()+", MOD: "+pows[i].slots[j].mod.ToString("P2")+"%", style, GUILayout.Width(220));
				pows[i].slots[j].mod = GUILayout.HorizontalSlider(pows[i].slots[j].mod, 0, 1, GUILayout.Width(220));
				if(GUILayout.Button("!!WARNING: REMOVE APM!!", style, GUILayout.Width(220)))
				{
					pows[i].slots.RemoveAt(j);
				}
			}
			if(GUILayout.Button("!!WARNING: REMOVE POWER GROUP!!", style, GUILayout.Width(220)))
			{
				pows.RemoveAt(i);
			}
		}
		GUILayout.Label("-=MODULES=-", style, GUILayout.Width(220));
		if(GUILayout.Button("ADD JUMP DRIVE MODULE", style, GUILayout.Width(220)))
		{
			ProtoModset pm = new ProtoModset();
			pm.mtype=ModuleSlot.ModuleType.JumpDrive;
			pm.name="";
			pm.pow=20;
			pm.tim=10;
			pm.col=20;
			mods.Add(pm);
		}
		if(GUILayout.Button("ADD MISSILE MODULE", style, GUILayout.Width(220)))
		{
			ProtoModset pm = new ProtoModset();
			pm.mtype=ModuleSlot.ModuleType.Missile;
			pm.name="";
			pm.pow=20;
			pm.tim=10;
			pm.col=20;
			mods.Add(pm);
		}
		if(GUILayout.Button("ADD AUTOREPAIR MODULE", style, GUILayout.Width(220)))
		{
			ProtoModset pm = new ProtoModset();
			pm.mtype=ModuleSlot.ModuleType.AutoRepair;
			pm.name="";
			pm.pow=20;
			pm.tim=10;
			pm.col=20;
			mods.Add(pm);
		}
		for(int i = 0; i<mods.Count; i++)
		{
			GUILayout.Label("Module Slot: "+mods[i].name+"/#"+i, style, GUILayout.Width(220));
			mods[i].name = GUILayout.TextField(mods[i].name, style, GUILayout.Width(220));
			int tempP = int.Parse(GUILayout.TextField(""+mods[i].pow, style, GUILayout.Width(220)));
			int tempT = int.Parse(GUILayout.TextField(""+mods[i].tim, style, GUILayout.Width(220)));
			int tempC = int.Parse(GUILayout.TextField(""+mods[i].col, style, GUILayout.Width(220)));
			if(tempP+tempT+tempC<=50)
			{
				mods[i].pow=tempP;
				mods[i].tim=tempT;
				mods[i].col=tempC;
			}
			if(GUILayout.Button("!!WARNING: REMOVE MODSLOT!!", style, GUILayout.Width(220)))
			{
				pows.RemoveAt(i);
			}
		}
		GUILayout.Label("-=CAMERA SETUP (X/Y/Z/RANGE)=-", style, GUILayout.Width(220));
		float tempX = float.Parse(GUILayout.TextField(""+rotPivot.transform.position.x, style, GUILayout.Width(220)));
		float tempY = float.Parse(GUILayout.TextField(""+rotPivot.transform.position.y, style, GUILayout.Width(220)));
		float tempZ = float.Parse(GUILayout.TextField(""+rotPivot.transform.position.z, style, GUILayout.Width(220)));
		float tempR = float.Parse(GUILayout.TextField(""+(-camTemp.transform.localPosition.z), style, GUILayout.Width(220)));
		rotPivot.transform.position = new Vector3(tempX, tempY, tempZ);
		camTemp.transform.localPosition = new Vector3(0, 0, -tempR);
		GUILayout.Label("-=OPTIMAL WEAPONS RANGE=-", style, GUILayout.Width(220));
		wepRange = float.Parse(GUILayout.TextField(""+wepRange, style, GUILayout.Width(220)));
		GUILayout.EndScrollView();
		GUI.DragWindow();
	}
	
	void LoadShip()
	{
		/*GameObject shipBase = (GameObject)Instantiate(shipCorePrefab, transform.position, transform.rotation);
		ShipScript ss = shipBase.GetComponent<ShipScript>();
		myShip=ss;
		Transform targ = shipBase.transform.FindChild("CameraPos/CamObj/WeaponsTarget");*/
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
		shipsName=shipName;
		string[] colorPrimary = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cPrimary = new Color(float.Parse(colorPrimary[0])/255, float.Parse(colorPrimary[1])/255, float.Parse(colorPrimary[2])/255, 255);
		colMain=cPrimary;
		string[] colorPower = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cPower = new Color(float.Parse(colorPower[0])/255, float.Parse(colorPower[1])/255, float.Parse(colorPower[2])/255, 255);
		colPower=cPower;
		string[] colorThrust = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cThrust = new Color(float.Parse(colorThrust[0])/255, float.Parse(colorThrust[1])/255, float.Parse(colorThrust[2])/255, 255);
		colThrust=cThrust;
		string[] colorSpecial = strings[offset].Split(',');
		print(strings[offset]);
		offset+=1;
		Color cSpecial = new Color(float.Parse(colorSpecial[0])/255, float.Parse(colorSpecial[1])/255, float.Parse(colorSpecial[2])/255, 255);
		colSpecial=cSpecial;
		string[] posOff = strings[offset].Split(',');
		offset+=1;
		poff = new Vector3(float.Parse(posOff[0]), float.Parse(posOff[1]), float.Parse(posOff[2]));
		int numSeats = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		seats = new List<ProtoSeat>();
		for(int i = 0; i<numSeats; i++)
		{
			ProtoSeat seatie = new ProtoSeat();
			seatie.name=strings[offset];
			print(strings[offset]);
			offset+=1;
			seatie.thrusts = new List<int>();
			int numThrustsSeat = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			for(int n = 0; n<numThrustsSeat; n++)
			{
				seatie.thrusts.Add(int.Parse(strings[offset]));
				print(strings[offset]);
				offset+=1;
			}
			seatie.wepSet=int.Parse(strings[offset]);
			print("WEAPON SET: "+strings[offset]);
			offset+=1;
			seatie.powSet=int.Parse(strings[offset]);
			print("POWER SET: "+strings[offset]);
			offset+=1;
			int numSeatModules = int.Parse(strings[offset]);
			print("NUM SEAT MODULES: "+strings[offset]);
			offset+=1;
			seatie.modSlots = new int[numSeatModules];
			for(int n = 0; n<numSeatModules; n++)
			{
				seatie.modSlots[n]=int.Parse(strings[offset]);
				print(strings[offset]);
				offset+=1;
			}
			seats.Add(seatie);
		}
		int numThrusts = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		thrust = new List<ProtoThrust>();
		for(int i = 0; i<numThrusts; i++)
		{
			ProtoThrust tg = new ProtoThrust();
			tg.name=strings[offset];
			print(strings[offset]);
			offset+=1;
			tg.axis=strings[offset];
			print(strings[offset]);
			offset+=1;
			thrust.Add(tg);
		}
		int numWeps = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		weps = new List<ProtoWep>();
		for(int i = 0; i<numWeps; i++)
		{
			string weaponName = strings[offset];
			print(strings[offset]);
			offset+=1;
			string weaponPrimary = strings[offset];
			print(strings[offset]);
			offset+=1;
			string weaponSecondary = strings[offset];
			print(strings[offset]);
			offset+=1;
			ProtoWep pw = new ProtoWep();
			pw.name=weaponName;
			pw.primWep=weaponPrimary;
			pw.altWep=weaponSecondary;
			weps.Add(pw);
		}
		int numPowerSys = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		pows = new List<ProtoPowersys>();
		for(int i = 0; i<numPowerSys; i++)
		{
			string powName = strings[offset];
			print(strings[offset]);
			offset+=1;
			int numModules = int.Parse(strings[offset]);
			print(strings[offset]);
			offset+=1;
			ProtoPowersys ps = new ProtoPowersys();
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
			pows.Add(ps);
		}
		int numModulesReal = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		mods = new List<ProtoModset>();
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
			ProtoModset ms = new ProtoModset();
			ms.name=modName;
			ms.pow=powerPower;
			ms.tim=timePower;
			ms.col=coolPower;
			ms.mtype=modules[modType];
			mods.Add(ms);
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
		rotPivot.position = new Vector3(offsetX, offsetY, offsetZ);
		float camrange = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		camTemp.localPosition = new Vector3(0, 0, -camrange);
		float range = float.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		wepRange = range;
		int numBlocks = int.Parse(strings[offset]);
		print(strings[offset]);
		offset+=1;
		blocks = new List<BBlock>();
		for(int i = 0; i<numBlocks; i++)
		{
			string[] bb = strings[offset].Split('/');
			print(strings[offset]);
			offset+=1;
			string blockType = bb[0];
			string[] position = bb[1].Split(',');
			string[] rotation = bb[2].Split(',');
			Vector3 pos = new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2]));
			Vector3 ea = new Vector3(Utilities.NearestRound(float.Parse(rotation[0]), 90), Utilities.NearestRound(float.Parse(rotation[1]), 90), Utilities.NearestRound(float.Parse(rotation[2]), 90));
			/*GameObject blockObj = (GameObject)Instantiate(blocks[blockType], Vector3.zero, Quaternion.identity);
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
			}*/
			AddBlock(blockType, pos, ea, int.Parse(bb[3]), int.Parse(bb[4]), int.Parse(bb[5]));
		}
		tempCMR = (int)(colMain.r*255);
		tempCMG = (int)(colMain.g*255);
		tempCMB = (int)(colMain.b*255);
		tempCPR = (int)(colPower.r*255);
		tempCPG = (int)(colPower.g*255);
		tempCPB = (int)(colPower.b*255);
		tempCTR = (int)(colThrust.r*255);
		tempCTG = (int)(colThrust.g*255);
		tempCTB = (int)(colThrust.b*255);
		tempCSR = (int)(colSpecial.r*255);
		tempCSG = (int)(colSpecial.g*255);
		tempCSB = (int)(colSpecial.b*255);
		sr.Close();
	}

	void AddBlock(string blockType, Vector3 pos, Vector3 rot, int pSys, int optSlot, int optBonus)
	{
		Quaternion q = new Quaternion();
		q.eulerAngles=rot;
		GameObject go = (GameObject)Instantiate(blockAps[blockType], pos+poff, q);
		BBlock bb = new BBlock();
		bb.blockType=blockType;
		bb.position=pos;
		bb.rotation=rot;
		bb.pSys=pSys;
		bb.optSlot=optSlot;
		bb.optBonus=optBonus;
		bb.repreObj=go;
		blocks.Add(bb);
		go.GetComponent<PreviewBlock>().myShip=this;
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

	bool GetBlockAt(Vector3 pos)
	{
		foreach(BBlock b in blocks)
		{
			if(b.position==pos)
			{
				return true;
			}
		}
		return false;
	}

	public int gridSize = 2;

	void Update()
	{
		if(!Cursor.visible)
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
			if(Input.GetAxis("Mouse ScrollWheel")>0 && timeThing<=0)
			{
				if(Input.GetAxis("Mod")>0)
				{
					curClass+=1;
					if(curClass>=bs.Length)
						curClass=0;
					curBlockIC=0;
					curPSys=0;
					curOpt1=0;
					curOptBon=0;
				}
				else if(Input.GetAxis("Mod")==0)
				{
					curBlockIC+=1;
					if(curBlockIC>=bs[curClass].blocksIn.Length)
						curBlockIC=0;
				}
				else
				{
					
				}
				timeThing=0.1f;
			}
			if(Input.GetAxis("Mouse ScrollWheel")<0 && timeThing<=0)
			{
				if(Input.GetAxis("Mod")>0)
				{
					curClass-=1;
					if(curClass<0)
						curClass=bs.Length-1;
					curBlockIC=0;
					curPSys=0;
					curOpt1=0;
					curOptBon=0;
				}
				else if(Input.GetAxis("Mod")==0)
				{
					curBlockIC-=1;
					if(curBlockIC<0)
						curBlockIC=bs[curClass].blocksIn.Length-1;
				}
				else
				{
					
				}
				timeThing=0.1f;
			}
			Vector3 ea = blockPrev.transform.eulerAngles;
			if(Input.GetButtonUp("Axis1"))
			{
				if(Input.GetAxis("Axis1")<0)
				{
					ea.y-=90;
				}
				else
				{
					ea.y+=90;
				}
				if(Input.GetAxis("Mod")>0)
				{
					ea.y=0;
				}
			}
			if(Input.GetButtonUp("Axis2"))
			{
				if(Input.GetAxis("Axis2")<0)
				{
					ea.x-=90;
				}
				else
				{
					ea.x+=90;
				}
				if(Input.GetAxis("Mod")>0)
				{
					ea.x=0;
				}
			}
			if(Input.GetButtonUp("Axis3"))
			{
				if(Input.GetAxis("Axis3")<0)
				{
					ea.z-=90;
				}
				else
				{
					ea.z+=90;
				}
				if(Input.GetAxis("Mod")>0)
				{
					ea.z=0;
				}
			}
			ea = new Vector3(Utilities.NearestRound(ea.x, 90), Utilities.NearestRound(ea.y, 90), Utilities.NearestRound(ea.z, 90));
			if(Input.GetButtonUp("Module1"))
			{
				if(types[curBlock]!=BlockType.Hull)
				{
					curPSys+=1;
					if(curPSys>=pows.Count)
						curPSys=0;
				}
			}
			if(Input.GetButtonUp("Module2"))
			{
				if(types[curBlock]==BlockType.Thruster)
				{
					curOpt1+=1;
					if(curOpt1>=thrust.Count)
						curOpt1=0;
				}
				if(types[curBlock]==BlockType.Module)
				{
					curOpt1+=1;
					if(curOpt1>=mods.Count)
						curOpt1=0;
				}
				if(types[curBlock]==BlockType.Weapon)
				{
					curOpt1+=1;
					if(curOpt1>=weps.Count)
						curOpt1=0;
				}
				if(types[curBlock]==BlockType.APM)
				{
					curOpt1+=1;
					if(curOpt1>=pows[curPSys].slots.Count)
						curOpt1=0;
				}
			}
			if(Input.GetButtonUp("Module3"))
			{
				if(types[curBlock]==BlockType.Thruster)
				{
					if(curOptBon==0)
						curOptBon=1;
					else
						curOptBon=0;
				}
				if(types[curBlock]==BlockType.Weapon)
				{
					if(curOptBon==0)
						curOptBon=1;
					else
						curOptBon=0;
				}
			}
			blockPrev.eulerAngles = ea;
		}
		if(Input.GetButtonUp("Fire1"))
		{
			if(Cursor.visible==false)
			{
				if(GetBlockAt(blockPrev.position-poff))
				{
					RemoveBlock(blockPrev.position-poff);
				}
				else
				{
					AddBlock(blockNames[curBlock], blockPrev.position-poff, blockPrev.eulerAngles, curPSys, curOpt1, curOptBon);
				}
			}
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
		timeThing-=Time.deltaTime;
		foreach(GameObject go in blockPreviews)
		{
			go.SetActive(false);
		}
		curBlock = bs[curClass].blockIDs[curBlockIC];
		blockPreviews[curBlock].SetActive(true);
		blockPreviews[curBlock].GetComponent<PreviewBlock>().Update();
		Vector3 nPos = blockPrevPos.position;
		nPos.x = Mathf.Round(nPos.x/gridSize)*gridSize;
		nPos.y = Mathf.Round(nPos.y/gridSize)*gridSize;
		nPos.z = Mathf.Round(nPos.z/gridSize)*gridSize;
		blockPrev.position = nPos;
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
		wepRange=0;
		camTemp.transform.localPosition = new Vector3(0, 0, -10);
		rotPivot.transform.position = new Vector3(0, 0, 0);
		shipsName="";
		colMain=Color.green;
		colPower=Color.blue;
		colThrust=Color.red;
		colSpecial=Color.yellow;
	}
}