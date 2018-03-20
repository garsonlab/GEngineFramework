/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: VersionChecker
 * Date    : 2018/03/16
 * Version : v1.0
 * Describe: 
 */

using UnityEngine;

namespace GEngine.Modules
{
    public class VersionChecker : MonoBehaviour
    {


        void Start()
        {
            if (!Config.BundleMode)
            {
                Debug.Log("Not Bundle Mode Then Enter Game Directly.");
                Debug.Log("Launcher ...");
                return;
            }
            else
            {
                
            }
        }


    }
}