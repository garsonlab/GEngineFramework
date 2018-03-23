/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * --------------------------------------------------
 * FileName: AssetModification
 * Date    : 2018/1/2
 * Version : v1.0
 * Describe: 
 */

using System;
using System.IO;
using System.Text;
using UnityEditor;

public class AssetModification : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset(string path)
    {
        // 只修改C#脚本
        path = path.Replace(".meta", "");
        
        if (path.EndsWith(".cs"))
        {
           ModifyCSharpScript(path);
        }
    }


    static void ModifyCSharpScript(string path)
    {
        string header =  "/*\n"
                        + " * GEngine Framework for Unity By Garson(https://github.com/garsonlab)\n"
                        + " * -------------------------------------------------------------------\n"
                        + " * FileName: #SCRIPTNAME#\n"
                        + " * Date    : #DATE#\n"
                        + " * Version : v1.0\n"
                        + " * Describe: \n"
                        + " */\n";
        header = header.Replace("#SCRIPTNAME#", Path.GetFileNameWithoutExtension(path));
        header = header.Replace("#DATE#", DateTime.Now.ToString("yyyy/MM/dd"));
        header += File.ReadAllText(path);
        File.WriteAllText(path, header, new UTF8Encoding(false));
        AssetDatabase.Refresh();
    }

}
