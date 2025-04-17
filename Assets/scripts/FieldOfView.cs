using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	public float viewRadius;
	public float viewAngle;

	public Vector3 DirFromAngle(float angleInDegrees)
	{
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	void Start()
	{
	//	Debug.Log("posicao principal: " + this.transform.position);
		
	}

	void Update()
    {
		viewRadius = this.GetComponent<CapsuleCollider>().radius;

	}

}