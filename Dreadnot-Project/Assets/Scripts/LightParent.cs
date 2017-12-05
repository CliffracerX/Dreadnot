using UnityEngine;
using System.Collections;

public class LightParent : MonoBehaviour
{
	public Light parent,self;

	void Start()
	{
		self = this.GetComponent<Light>();
	}

	void Update()
	{
		self.range=parent.range;
		self.intensity=parent.intensity;
		self.color=parent.color;
	}
}