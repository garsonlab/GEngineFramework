/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: AppFacade
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using Kernal.Core;
using UnityEngine;

public class AppFacade : Facade 
{
    public new static AppFacade Instance
    {
        get
        {
            if(m_instance == null)
                m_instance = new AppFacade();
            return (AppFacade)m_instance;
        }
    }

    protected override void OnInitializeEnd()
    {
        RegisterCommand(new ACommand());
    }


    public void Start()
    {
        SendMessage(1);
    }
}
