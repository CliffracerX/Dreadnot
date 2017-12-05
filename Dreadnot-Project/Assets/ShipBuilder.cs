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
				Destroy(b);
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