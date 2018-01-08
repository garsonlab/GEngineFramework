/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: AMediator
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using Kernal.Patterns;
using UnityEngine;

public class AMediator : Mediator
{
    public override void OnRegister()
    {
        RegisterMessage(2, args =>
        {
            Debug.Log("#########");
        } );
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
