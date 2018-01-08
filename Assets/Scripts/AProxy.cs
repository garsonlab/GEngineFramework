/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: AProxy
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using Kernal.Patterns;
using UnityEngine;

public class AProxy : Proxy
{
    public override void OnRegister()
    {
        RegisterMessage(3, args =>
        {Debug.Log("DDDDDDDDDD");
        });
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
