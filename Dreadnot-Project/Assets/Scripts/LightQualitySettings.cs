using UnityEngine;
using System.Collections;

public class LightQualitySettings : MonoBehaviour
{
	public Light l;
	public LightShadows[] ls;
	//public int[] res;

	void Update()
	{
		l.shadows = ls[QualitySettings.GetQualityLevel()];
		//l.shadow
		//<INSERT TABLEFLIP HERE>
		//THIS IS SUPPOSED TO HAVE SHADOW RESOLUTION TOO GODDAMNIT
	}
}