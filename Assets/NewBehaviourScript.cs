/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: NewBehaviourScript
 * Date    : 2018/03/09
 * Version : v1.0
 * Describe: 
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GEngine.Managers;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private WebRequestAgent agent;
    void Start()
    {
        string folder = Application.persistentDataPath;
        folder += "/TestDown";
        Debug.Log(folder);

        string url = "http://192.168.1.34/CMake32.zip";
        string save = folder + "/aaads.temp";

        agent = new WebRequestAgent();
        agent.onDownloadSuccess = () => Debug.Log("Success");
        agent.onDownloadFailed = () => Debug.Log("Failed");
        long len = agent.GetLength(url);
        Debug.Log("Len::  " + len);
        StartCoroutine(agent.Download(url, save, len));

    }

    void Update()
    {
        if(agent != null)
            Debug.Log(string.Format("Downloaded {0} / {1} , ({2})", agent.DownloadedStr, agent.TotalStr, agent.Progress));
    }

}
