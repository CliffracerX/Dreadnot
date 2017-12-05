using UnityEngine;
using System.Collections;

public class Utilities
{
	/// <summary>
	/// Angle-clamper for Weapons.
	/// FROM: https://forum.unity.com/goto/post?id=2244265#post-2244265
	/// </summary>
	/// <returns>The angle.</returns>
	/// <param name="angle">Angle.</param>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Max.</param>
	public static float ClampAngle(float angle, float min, float max)
	{
		if (min < 0 && max > 0 && (angle > max || angle < min))
		{
			angle -= 360;
			if (angle > max || angle < min)
			{
				if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
				else return max;
			}
		}
		else if(min > 0 && (angle > max || angle < min))
		{
			angle += 360;
			if (angle > max || angle < min)
			{
				if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
				else return max;
			}
		}
		
		if (angle < min) return min;
		else if (angle > max) return max;
		else return angle;
	}
}
