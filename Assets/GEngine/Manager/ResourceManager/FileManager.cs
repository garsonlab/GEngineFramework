/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: FileManager
 * Date    : 2018/02/27
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GEngine.Managers
{
    /// <summary>
    /// 文件管理器，只接受资源管理器初始化调用
    /// </summary>
    public class FileManager
    {
        private static string[] units = new[] { "B", "KB", "MB", "GB", "TB", "PB" };
        private static string persistentDataPath;
        private static string streamingAssetsPath;
        private static JsonNode m_bundles;
        //private static int[] m_version;
        //public static int[] version { get { return m_version; } }
        private static string m_version;
        public static string version { get { return m_version; } }
        public static JsonNode bundles { get { return m_bundles;} }

        static FileManager()
        {
            persistentDataPath = Application.persistentDataPath + "/AssetBundles/";
            streamingAssetsPath = Application.streamingAssetsPath + "/AssetBundles/";
            m_bundles = JsonNode.NewTable();
            //m_version = new []{0};
            m_version = "0";
        }
        /// <summary>
        /// 初始化Manifest
        /// </summary>
        /// <param name="menifestBytes"></param>
        public static void LoadMenifest(byte[] menifestBytes)
        {
            JsonNode m_menifest = new JsonParser().Load(menifestBytes);
            if (m_menifest.IsTable())
            {
                m_bundles = m_menifest["BundleManifest"];
                m_version = m_menifest["Version"].ToString();
                //StringUtil.Parse(ver, out m_version, ".");
                
            }
            else
            {
                m_bundles = JsonNode.NewTable();
                //m_version = new[] { 0 };
                m_version = "0";
            }
        }

        /// <summary>
        /// 根据Asset路径返回是否存在该bundle及bundle路径
        /// </summary>
        /// <param name="assetPath">eg.Asset/Resources/...</param>
        /// <param name="bundlPath"></param>
        /// <returns></returns>
        public static bool GetBundlePath(string assetPath, ref string bundlPath)
        {
            bool isBundle = false;
            JsonNode node = m_bundles[assetPath];
            if (node.IsAny())
            {
                isBundle = true;
                bundlPath = assetPath;
            }
            else
            {
                string name = Path.GetFileName(assetPath);
                assetPath = assetPath.Replace("/" + name, "");
                node = m_bundles[assetPath];
                if (node.IsAny())
                {
                    isBundle = true;
                    bundlPath = assetPath;
                }
            }
            return isBundle;
        }



        /// <summary>
        /// 根据资源路径获取实际路径，优先persistent
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetBundleRealPath(string path)
        {
            if (File.Exists(persistentDataPath + path))
                return persistentDataPath + path;

            return streamingAssetsPath + path;
        }


        /// <summary>
        /// 获取Resources文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns>空表示不在Resource目录下</returns>
        public static bool GetResourcePath(string path, ref string resPath)
        {
            if (path.Contains("Resources/"))
            {
                resPath = path.Split(new[] {"Resources/"}, StringSplitOptions.None)[1];
                resPath = resPath.Replace(Path.GetExtension(path), "");
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取依赖文件
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static List<JsonNode> GetDependencies(string assetPath)
        {
            JsonNode node = m_bundles[assetPath];
            if (node.IsTable())
            {
                List<JsonNode> deps = (List<JsonNode>)node["Dependencies"];
                return deps;
            }
            return new List<JsonNode>();
        }
        /// <summary>
        /// 获取Md5值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string MD5(string name)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
            return ToHexString(bytes);
        }
        /// <summary>
        /// 获取Md5值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string MD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Bytes = md5.ComputeHash(bytes);
            return ToHexString(md5Bytes);
        }

        /// <summary>
        /// 获取文件Hash值
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Hash(string filePath, out long length)
        {
            FileStream fs = new FileStream(filePath,
                   FileMode.Open,
                   FileAccess.Read,
                   FileShare.Read);
            length = fs.Length;
            HashAlgorithm ha = HashAlgorithm.Create();
            byte[] bytes = ha.ComputeHash(fs);
            fs.Close();
            return ToHexString(bytes);
        }
        /// <summary>
        /// 获取文件Hash值
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Hash(byte[] bytes, out long length)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            byte[] hashBytes = ha.ComputeHash(bytes);
            length = bytes.Length;
            return ToHexString(hashBytes);
        }
        /// <summary>
        /// 转换16进制
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString().ToLower();
            }
            return hexString;
        }
        /// <summary>
        /// 转换字节单位
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ByteConvert(long length)
        {
            double size = length;
            int i = 0;
            while (size >= 1024)
            {
                size /= 1024.0d;
                i++;
                if (i >= units.Length)
                    break;
            }
            return Math.Round(size, 2) + units[i];
        }
        /// <summary>
        /// 检测并创建目录
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }
        /// <summary>
        /// 删除目录下所有文件
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ClearFiles(string folderPath)
        {
            CreateDirectory(folderPath);
            string[] files = Directory.GetFiles(folderPath);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }

    }
}