/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: Test
 * Date    : 2018/01/04
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    AppFacade.Instance.Start();
	}


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AppFacade.Instance.SendMessage(2);
        }
        if (Input.GetMouseButtonDown(1))
        {
            AppFacade.Instance.SendMessage(3);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AppFacade.Instance.RemoveMediator<AMediator>(AMediator.NAME);
        }
    }
}
