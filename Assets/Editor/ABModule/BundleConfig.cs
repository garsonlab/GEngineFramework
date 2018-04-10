/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: BundleConfig
 * Date    : 2018/03/03
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BundleFilter
{
    public string assetPath = "Assets"; //打包资源路径
    public bool isFolderPack = false; //文件夹整体打包，包含所有后缀
    public bool isChildFolder = false; //是子文件夹

    public string extension = ".prefab"; //打包后缀名
    public bool isSingle = true; //单个打包，否则该路径下打整体
    public bool isDepend = false; //是否依赖打包
    public bool isDependSplit = false; //依赖打包时公共资源是否单个打包
    public string dependName = String.Empty; //依赖非单个打包时所在包名

    public bool isExpand = true; //是否展开
}

public class BundleConfig : ScriptableObject
{
    public List<BundleFilter> filters = new List<BundleFilter>(); 
}


[CustomEditor(typeof(BundleConfig))]
public class BundleConfigEditor : Editor
{
    private BundleConfig configs;

    private string note = @"Bundle 配置：当前共{0}条配置
AssetPath：打包路径，从Assets下开始
IsFolderPack: 文件夹整体打包，包含所有的后缀
isChildFolder：该文件下子文件夹，在IsFolderPack开启后有用

Extension：打包后缀，如“.prefab”，全部小写，多个使用“|”分割
IsSingle：是否单个打包，是则每个符合条件的后缀打一个bundle，否则该路径下所有打一个bundle
IsDepend：是否依赖打包，是则把依赖提出打包
IsDependSplit：是否依赖单独打包，在IsDepend开启前提下，把提出的依赖每个单独打成一个bundle
DependName：依赖包名，在IsDepend开启前提下，若IsDependSplit未开启，则把提出的依赖全部打进该名的包中";
    private Vector2 scrollView;
    private int delNo;


    public override void OnInspectorGUI()
    {
        if (configs == null)
            configs = (BundleConfig)target;

        scrollView = GUILayout.BeginScrollView(scrollView);
        EditorGUILayout.HelpBox(string.Format(note, configs.filters.Count), MessageType.Info);
        GUILayout.Space(10);

        GUILayout.BeginVertical();
        #region 添加、清空
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.color = Color.green;
        if (GUILayout.Button("添加", GUILayout.Width(80)))
        {
            BundleFilter filter = new BundleFilter();
            configs.filters.Add(filter);
        }

        GUI.color = Color.red;
        if (GUILayout.Button("清空", GUILayout.Width(80)))
        {
            if (EditorUtility.DisplayDialog("", "确认要删除所有配置？", "确认", "取消"))
            {
                configs.filters.Clear();
            }
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10);

        int count = configs.filters.Count;
        for (int i = 0; i < count; i++)
        {
            GUI.backgroundColor = Color.gray;

            BundleFilter filter = configs.filters[i];
            if (GUILayout.Button("AssetPath: " + filter.assetPath + "(" + (filter.isExpand ? "OL Minus" : "OL Plus") + ")"))
            {
                filter.isExpand = !filter.isExpand;
            }

            if (filter.isExpand)
            {
                GUILayout.BeginHorizontal();
                filter.assetPath = EditorGUILayout.TextField("Asset Path:", filter.assetPath);
                //GUILayout.FlexibleSpace();
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    string p = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
                    if (!string.IsNullOrEmpty(p))
                    {
                        if (p.Contains("Assets/"))
                        {
                            string[] ps = p.Split(new[] { "Assets/" }, StringSplitOptions.None);
                            filter.assetPath = "Assets/" + ps[1];
                        }
                    }
                }

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    delNo = i + 1;
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                filter.isFolderPack = EditorGUILayout.Toggle("IsFolderPack:", filter.isFolderPack);
                if (filter.isFolderPack)
                {
                    filter.isChildFolder = EditorGUILayout.Toggle("IsChildFolder:", filter.isChildFolder);
                    if(filter.isChildFolder)
                        GUILayout.Label("<!--当前把Asset Path下每个子文件夹打成一个Bundle.-->");
                    else
                        GUILayout.Label("<!--当前把Asset Path下所有后缀资源打成一个Bundle.-->");
                }
                else
                {
                    GUILayout.Label("<!-------当前按照后缀打包-------->");
                    filter.extension = EditorGUILayout.TextField("Extension:", filter.extension);
                    filter.isSingle = EditorGUILayout.Toggle("IsSingle:", filter.isSingle);
                    filter.isDepend = EditorGUILayout.Toggle("IsDepend:", filter.isDepend);

                    if (filter.isDepend)
                    {
                        filter.isDependSplit = EditorGUILayout.Toggle("IsDependSplit:", filter.isDependSplit);
                        if (!filter.isDependSplit)
                        {
                            filter.dependName = EditorGUILayout.TextField("DependName", filter.dependName);
                        }
                    }
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(5);
        }

        if (delNo > 0 && delNo <= configs.filters.Count)
        {
            configs.filters.RemoveAt(delNo - 1);
        }
        delNo = 0;

        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}