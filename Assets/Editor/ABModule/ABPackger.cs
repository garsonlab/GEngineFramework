/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: ABPackger
 * Date    : 2018/03/03
 * Version : v1.0
 * Describe: 缺少：场景打包，新旧资源对比bsdiff，资源下载
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GEngine;
using UnityEditor;
using UnityEngine;

public class ABPackger : Editor
{
    static string bundleSavePath = Application.dataPath.Replace("/Assets", "/AssetBundles");
    static string manifestPath = bundleSavePath + "/BundleManifest.manifest";
    static string shaderSet = "ShaderSet";



    [MenuItem("Bundle/CreateConfig")]
    static void CreateConfig()
    {
        if (File.Exists("Assets/assetbundle.asset"))
        {
            EditorUtility.DisplayDialog("提示", "已存在该配置，无需再创建", "OK");
            return;
        }
        BundleConfig config = CreateInstance<BundleConfig>();
        AssetDatabase.CreateAsset(config, "Assets/assetbundle.asset");
        AssetDatabase.Refresh();
    }


    [MenuItem("Bundle/Build")]
    static void Package()
    {
        #region Check
        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("警告", "资源正常编译，请完成后重试！", "OK");
            return;
        }

        BundleConfig config = AssetDatabase.LoadAssetAtPath<BundleConfig>("Assets/assetbundle.asset");
        if (config == null)
        {
            EditorUtility.DisplayDialog("警告", "未检测到对应的构建规则，请创建后重试！", "OK");
            return;
        }

        if (!Directory.Exists(bundleSavePath))
            Directory.CreateDirectory(bundleSavePath);
        #endregion


        Dictionary<string, BundleInfo> bundleInfos = new Dictionary<string, BundleInfo>();

        foreach (var filter in config.filters)
        {
            #region 校验空值
            if (!Directory.Exists(filter.assetPath))
            {
                Debug.LogWarning("Cannot Find Path : " + filter.assetPath);
                continue;
            }
            #endregion

            #region 按文件夹打包
            if (filter.isFolderPack)
            {
                if (filter.isChildFolder)
                {
                    string[] folders = Directory.GetDirectories(filter.assetPath);
                    foreach (var folder in folders)
                    {
                        string folderPath = folder.Replace("\\", "/");
                        BundleInfo info = AddToSet(bundleInfos, folderPath, false);
                        string[] folderfiles = Directory.GetFiles(folderPath);
                        foreach (var folderfile in folderfiles)
                        {
                            string path = folderfile.Replace("\\", "/");
                            if(folderfile.ToLower().EndsWith(".meta"))
                                continue;
                            if(folderfile.ToLower().EndsWith(".cs"))
                                continue;
                            if (folderfile.ToLower().EndsWith(".shader"))
                            {
                                AddToShaderSet(bundleInfos, path);
                                continue;
                            }
                            info.AddAsset(path);
                        }
                    }
                }
                else
                {
                    BundleInfo info = AddToSet(bundleInfos, filter.assetPath, false);
                    string[] folderfiles = Directory.GetFiles(filter.assetPath);
                    foreach (var folderfile in folderfiles)
                    {
                        string path = folderfile.Replace("\\", "/");
                        if (folderfile.ToLower().EndsWith(".meta"))
                            continue;
                        if (folderfile.ToLower().EndsWith(".cs"))
                            continue;
                        if (folderfile.ToLower().EndsWith(".shader"))
                        {
                            AddToShaderSet(bundleInfos, path);
                            continue;
                        }
                        info.AddAsset(path);
                    }
                }
                continue;
            }
            #endregion

            #region 按后缀打包
            #region 处理后缀
            if (string.IsNullOrEmpty(filter.extension))
            {
                Debug.LogWarning("Cannot Match Extension : " + filter.assetPath);
                continue;
            }

            string[] exs = filter.extension.Split('|');
            List<string> extends = new List<string>();
            for (int i = 0; i < exs.Length; i++)
            {
                string ex = exs[i].Trim().ToLower();
                if (!extends.Contains(ex))
                {
                    extends.Add(ex);
                }
            }
            #endregion

            #region 判断整体
            string wholeBundleName = filter.assetPath.Replace("\\", "/");
            BundleInfo whole;
            if (!bundleInfos.TryGetValue(wholeBundleName, out whole))
            {
                if (!filter.isSingle)
                {
                    whole = new BundleInfo(wholeBundleName, false);
                }
            }
            #endregion

            #region 单个及依赖
            string[] files = Directory.GetFiles(filter.assetPath);
            foreach (var file in files)
            {
                if (!extends.Contains(Path.GetExtension(file).ToLower()))
                    continue;

                string path = file.Replace("\\", "/");

                if (path.ToLower().EndsWith(".shader"))//所有Shader单独处理，即使填写规则也全部放到一起
                {
                    AddToShaderSet(bundleInfos, path);
                    continue;
                }


                BundleInfo single = null;
                if (!filter.isSingle) //整体打包
                {
                    whole.AddAsset(path);
                }
                else
                {
                    single = AddToSet(bundleInfos, path);
                    single.AddAsset(path);
                }

                if (filter.isDepend)//依赖打包
                {
                    string[] deps = AssetDatabase.GetDependencies(path);
                    foreach (var dep in deps)
                    {
                        if (dep.Equals(path))
                            continue;
                        if (dep.EndsWith(".cs"))
                            continue;

                        var depPath = dep.Replace("\\", "/");
                        if (dep.ToLower().EndsWith(".shader"))
                        {
                            AddToShaderSet(bundleInfos, depPath);

                            AddDependency(whole, single, filter.isSingle, shaderSet);
                            continue;
                        }

                        if (filter.isDependSplit)
                        {
                            BundleInfo depSingle = AddToSet(bundleInfos, depPath);
                            depSingle.AddAsset(depPath);
                            AddDependency(whole, single, filter.isSingle, depPath);
                        }
                        else
                        {
                            string wholeName = string.IsNullOrEmpty(filter.dependName.Trim())
                                ? "CommonDependencies"
                                : filter.dependName;

                            BundleInfo info;
                            if (!bundleInfos.TryGetValue(wholeName, out info))
                            {
                                info = new BundleInfo(wholeName, false);
                                bundleInfos.Add(wholeName, info);
                            }

                            info.AddAsset(depPath);
                            AddDependency(whole, single, filter.isSingle, wholeName);
                        }
                    }
                }
            }
            #endregion
            #endregion
        }

        #region 删除旧资源
        if (File.Exists(manifestPath))
        {
            JsonNode oldRoot = new JsonParser().Load(File.ReadAllBytes(manifestPath));
            Dictionary<string, JsonNode> oldAssets = (Dictionary<string, JsonNode>)oldRoot["BundleManifest"];
            List<string> willDeletes = new List<string>();
            foreach (var oldAsset in oldAssets)
            {
                if(!bundleInfos.ContainsKey(oldAsset.Key))
                    willDeletes.Add(oldAsset.Key);
            }
            foreach (var willDelete in willDeletes)
            {
                Debug.Log("DELETE BUNDLE::  " + willDelete);
                string bundleName = (string)oldAssets[willDelete]["BundleName"];
                string bundlePath = bundleSavePath + "/" + bundleName;
                if (File.Exists(bundlePath))
                    File.Delete(bundlePath);
                if(File.Exists(bundlePath+".manifest"))
                    File.Delete(bundlePath+".manifest");
            }
        }
        #endregion

        #region 打包
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        foreach (var bundleInfo in bundleInfos)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleInfo.Value.bundleName;
            build.assetNames = bundleInfo.Value.assets.ToArray();
            builds.Add(build);
        }
        BuildPipeline.BuildAssetBundles(bundleSavePath, builds.ToArray(),
               BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        #endregion

        #region 新规则构建
        JsonNode root = JsonNode.NewTable();
        JsonNode assets = JsonNode.NewTable();
        root["Version"] = JsonNode.NewString("1.0");
        root["BundleCount"] = JsonNode.NewInt(bundleInfos.Count);
        root["BundleManifest"] = assets;

        foreach (var bundleInfo in bundleInfos.Values)
        {
            JsonNode node = JsonNode.NewTable();
            //node["AssetPath"] = JsonNode.NewString(bundleInfo.assetPath);
            node["BundleName"] = JsonNode.NewString(bundleInfo.bundleName);
            //node["Single"] = JsonNode.NewBool(bundleInfo.isSingle);
            node["AssetCount"] = JsonNode.NewInt(bundleInfo.assets.Count);
            long length;
            node["Hash"] = JsonNode.NewString(bundleInfo.GetHash(bundleSavePath, out length));
            node["Length"] = JsonNode.NewNumber(length);

            JsonNode dependencies = JsonNode.NewArray();
            foreach (var dep in bundleInfo.dependencies)
            {
                dependencies.Add(JsonNode.NewString(dep));
            }
            node["Dependencies"] = dependencies;

            assets[bundleInfo.assetPath] = node;
        }
        if (File.Exists(manifestPath))
            File.Delete(manifestPath);
        File.WriteAllText(manifestPath, new JsonPrinter(true).String(root), new UTF8Encoding(false));
        AssetDatabase.Refresh();
        #endregion
    }

    static BundleInfo AddToSet(Dictionary<string, BundleInfo> bundleInfos, string path, bool isSingle = true)
    {
        BundleInfo info;
        if (!bundleInfos.TryGetValue(path, out info))
        {
            info = new BundleInfo(path, isSingle);
            bundleInfos.Add(path, info);
        }
        //foreach (var bundleInfo in bundleInfos)
        //{
        //    if (!bundleInfo.Value.isSingle)
        //    {
        //        bundleInfo.Value.RemoveAsset(path);//单个打包时，移除集合中打包
        //    }
        //}
        return info;
    }

    static void AddToShaderSet(Dictionary<string, BundleInfo> bundleInfos, string path)
    {
        BundleInfo info;
        if (!bundleInfos.TryGetValue(shaderSet, out info))
        {
            info = new BundleInfo(shaderSet, false);
            bundleInfos.Add(shaderSet, info);
        }

        info.AddAsset(path);
    }

    static void AddDependency(BundleInfo whole, BundleInfo single, bool isSingle, string dep)
    {
        if(isSingle)
            single.AddDependency(dep);
        else
            whole.AddDependency(dep);
    }

    [MenuItem("Bundle/Copy")]
    static void Copy()
    {
        string streamPath = Application.streamingAssetsPath + "/AssetBundles";
        if (!Directory.Exists(streamPath))
            Directory.CreateDirectory(streamPath);
        string[] files = Directory.GetFiles(streamPath);
        foreach (var file in files)
        {
            File.Delete(file);
        }
        files = Directory.GetFiles(bundleSavePath);
        foreach (var file in files)
        {
            string path = Path.GetFileName(file);
            path = streamPath + "/" + path;
            if(File.Exists(path))
                File.Delete(path);
            File.Copy(file, path);
        }
        AssetDatabase.Refresh();
    }
}

public class BundleInfo
{
    public string assetPath;
    public string bundleName;
    public bool isSingle;
    public List<string> assets = new List<string>();
    public List<string> dependencies = new List<string>();

    public BundleInfo(string assetPath, bool isSingle)
    {
        this.assetPath = assetPath;
        this.isSingle = isSingle;
        this.bundleName = GEngine.Managers.FileManager.MD5(assetPath);
    }

    public void AddAsset(string path)
    {
        if(!assets.Contains(path))
            assets.Add(path);
    }

    public void AddDependency(string dep)
    {
        if(!dependencies.Contains(dep))
            dependencies.Add(dep);
    }

    public string GetHash(string path, out long length)
    {
        return GEngine.Managers.FileManager.Hash(path + "/" + bundleName, out length);
    }
}