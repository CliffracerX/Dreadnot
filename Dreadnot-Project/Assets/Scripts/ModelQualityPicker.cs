using UnityEngine;
using System.Collections;

public class ModelQualityPicker : MonoBehaviour
{
	public GameObject[] objsQuality;

	void Update()
	{
		for(int i = 0; i<objsQuality.Length; i++)
		{
			if(QualitySettings.GetQualityLevel()!=i)
				objsQuality[i].SetActive(false);
			else
				objsQuality[i].SetActive(true);
		}
	}
}