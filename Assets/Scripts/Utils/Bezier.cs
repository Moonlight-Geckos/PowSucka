using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
	Vector3 p0, p1, p2;
	public Bezier(Vector3 p0, Vector3 p1, Vector3 p2) 
	{
		this.p0 = p0;
		this.p1 = p1;
		this.p2 = p2;
	}
	public Vector3 GetPoint(float t)
	{
		return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
	}
}