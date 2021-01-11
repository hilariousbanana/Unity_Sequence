using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustRotate : MonoBehaviour {

public bool canRotate=true;
public float speed=10;
	public Vector3 rotateDir;
 
	void Update ()
	{
		if(canRotate)
		  transform.Rotate(speed*rotateDir*Time.deltaTime);
	}
}
