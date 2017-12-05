using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform lockT;
	public float sensitivity;
	public float freeTime=5;

	void Start()
	{
		transform.parent=null;
	}

	void Update()
	{
		if(lockT)
		{
			transform.position=lockT.position;
			float rx = transform.rotation.eulerAngles.x;
			rx-=Input.GetAxis("Mouse Y")*sensitivity;
			if(rx>360)
				rx=-360;
			if(rx<-360)
				rx=360;
			float ry = transform.rotation.eulerAngles.y;
			ry+=Input.GetAxis("Mouse X")*sensitivity;
			if(ry>360)
				ry=-360;
			if(ry<-360)
				ry=360;
			Quaternion q = Quaternion.identity;
			q.eulerAngles = new Vector3(rx, ry, 0);
			transform.rotation = q;
		}
		else
		{
			freeTime-=Time.deltaTime;
			if(freeTime<=0)
			{
				Destroy(transform.root.gameObject);
			}
		}
	}
}