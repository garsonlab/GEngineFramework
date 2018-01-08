/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: ACommand
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using Kernal.Patterns;
using UnityEngine;

public class ACommand : Command
{
    public new const string NAME = "ACommand";

    public ACommand() : base(NAME) { }

    public override void OnRegister()
    {
        RegisterMessage(1, OnStart);
    }

    private void OnStart(MessageArgs messageArgs)
    {
        Debug.Log("$$$$$$$$$$$$");
        AppFacade.Instance.RegisterProxy(new AProxy());

        AProxy proxy = RetrieveProxy<AProxy>(AProxy.NAME);
        Debug.Log("Retrive"+ proxy.ToString());
        AppFacade.Instance.RegisterMediator(new AMediator());
    }
}
