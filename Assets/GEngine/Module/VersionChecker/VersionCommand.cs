/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: VersionCommand
 * Date    : 2018/03/16
 * Version : v1.0
 * Describe: 非Bundle模式：直接跳过检测
 * Bundle模式：
 *      获取远程版本
 *          不同-》检测网络环境wifi
 *              大版本更新-》提示更新大版本-》android直接下载/ios跳转appstore
 *              文件更新-》下载
 *          相同-》进入游戏
 */

using System.Collections.Generic;
using System.IO;
using System.Text;
using GEngine.Core;
using GEngine.Managers;
using GEngine.Patterns;
using UnityEngine;

namespace GEngine.Modules
{
    public class VersionCommand : Command
    {
        public new const string NAME = "VersionCommand";
        private VersionMediator mediator;

        private const string m_remoteUrl = "http://127.0.0.1/";
        private string m_savePath;
        private string m_temPath;
        private string m_remoteVersion;
        private WebRequestAgent m_webAgent;
        private Queue<JsonNode> m_waitingList;
        private StreamWriter m_writer;
        private JsonNode m_root;

        public override void OnRegister()
        {
            m_savePath = Application.persistentDataPath + "/Bundles/";
            m_temPath = Application.persistentDataPath + "/TemFolder/";
            m_waitingList = new Queue<JsonNode>();

            RegisterMessage(GConst.VERSION_CHECK, CheckVeison);
            Facade.Instance.RegisterMediator(new VersionMediator());
            mediator = RetrieveMediator<VersionMediator>(VersionMediator.NAME);
        }

        private void CheckVeison(MessageArgs messageArgs)
        {
            string remoteVersion = "1.1";//获取服务器版本：
            string curVersion = FileManager.version;
            m_remoteVersion = remoteVersion;
            if (remoteVersion.Equals(curVersion))
            {
                //当前版本和服务器版本一致
                Debug.Log("Enter Game");
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    mediator.ShowNetBreak(CheckVeison);
                }
                else
                {
                    string rvm = StringUtil.Split(remoteVersion, '.')[0];
                    string cvm = StringUtil.Split(curVersion, '.')[0];

                    if (!rvm.Equals(cvm))
                    {
                        //大版本不同，更新整包
                        mediator.ShowNewVersion(DownloadNewVersion);
                    }
                    else
                    {
                        //小版本不同，更新资源
                        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) //wifi或局域网
                        {
                            DownloadNewResources();
                        }
                        else
                        {
                            mediator.ShowNewResources(DownloadNewResources);
                        }
                    }
                }
            }
        }

        //更新整包
        private void DownloadNewVersion()
        {
            
        }

        //更新资源
        private void DownloadNewResources()
        {
            FileManager.CreateDirectory(m_temPath);
            FileLoadAgent agent = new FileLoadAgent();

            Dictionary<string, DownState> downloaded = new Dictionary<string, DownState>();
            if (File.Exists(m_temPath + "download.list"))
            {
                ParseDownloadList(downloaded, agent);
            }

            JsonNode root = null;
            if (File.Exists(m_temPath + Config.BundleManifest))
            {
                root = new JsonParser().Load(agent.Load(m_temPath+Config.BundleManifest));
                if (root.IsTable())
                {
                    string ver = root["Version"].ToString();
                    if (ver.Equals(m_remoteVersion))
                    { }
                    else
                    {
                        root = null;
                    }
                }
                else
                    root = null;
            }

            if (root == null)
            {
                DownLoadManifest(() =>
                {
                    root = new JsonParser().Load(agent.Load(m_temPath + Config.BundleManifest));
                    CombineDownloadList(root, downloaded);
                });
            }
            else
            {
                CombineDownloadList(root, downloaded);
            }
        }

        class DownState
        {
            public string Hash;
            public bool IsDone;

            public DownState(string hash, string done)
            {
                Hash = hash;
                UpdateState(done);
            }

            public void UpdateState(string done)
            {
                if (string.IsNullOrEmpty(done))
                {
                    IsDone = false;
                }
                else
                {
                    IsDone = done == "1";
                }
            }
        }
        private void ParseDownloadList(Dictionary<string, DownState> downloaded, FileLoadAgent agent)
        {
            byte[] bytes = agent.Load(m_temPath + "download.list");
            string str = new UTF8Encoding().GetString(bytes);
            string[] lines = StringUtil.Split(str, '\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = StringUtil.Split(lines[i], ':');
                if (data.Length >= 2)
                {
                    if (downloaded.ContainsKey(data[0]))
                        downloaded[data[0]].UpdateState(data[2]);
                    else
                        downloaded.Add(data[0], new DownState(data[1], data.Length > 2 ? data[2] : ""));
                }
            }
        }

        private void DownLoadManifest(Callback_0 onSuccess)
        {
            if (m_webAgent == null)
            {
                m_webAgent = new WebRequestAgent();
            }
            if(File.Exists(m_temPath + Config.BundleManifest))
                File.Delete(m_temPath + Config.BundleManifest);

            long length = m_webAgent.GetLength(m_remoteUrl + Config.BundleManifest);
            m_webAgent.onDownloadSuccess = onSuccess;
            m_webAgent.onDownloadFailed = () =>
            {
                GLog.E("下载manifest失败，提示重试");
            };
            StartCoroutine(m_webAgent.Download(m_remoteUrl + Config.BundleManifest, m_temPath + Config.BundleManifest,
                length));
        }

        private void CombineDownloadList(JsonNode root, Dictionary<string, DownState> downloaded)
        {
            m_waitingList.Clear();
            m_root = root;
            JsonNode curVer = FileManager.bundles;
            m_writer = new StreamWriter(m_temPath + "download.list", false);
            var assets = (Dictionary<string, JsonNode>)root["BundleManifest"];
            var eor = assets.GetEnumerator();
            while (eor.MoveNext())
            {
                DownState state;
                if (downloaded.TryGetValue(eor.Current.Value["BundleName"].ToString(), out state))
                {
                    if (state.Hash == eor.Current.Value["Hash"].ToString())
                    {
                        if (state.IsDone)
                        {
                            m_writer.Write(eor.Current.Value["BundleName"].ToString()+":"+ state.Hash+":1\n");
                        }
                        else
                        {
                            m_writer.Write(eor.Current.Value["BundleName"].ToString()+":"+ state.Hash+"\n");
                            m_waitingList.Enqueue(eor.Current.Value);
                        }
                        continue;
                    }
                    File.Delete(m_temPath + eor.Current.Value["BundleName"].ToString());
                }

                if (curVer[eor.Current.Key].IsTable() && curVer[eor.Current.Key]["Hash"].ToString() == eor.Current.Value["Hash"].ToString())
                {
                    continue;
                }
                m_waitingList.Enqueue(eor.Current.Value);
            }
            eor.Dispose();

            if(m_webAgent == null)
                m_webAgent = new WebRequestAgent();
            m_webAgent.onDownloadSuccess = StartDown;
            m_webAgent.onDownloadFailed = () =>
            {
                GLog.E("Down　Error");
            };
            StartDown();
        }

        private void StartDown()
        {
            if (m_waitingList.Count <= 0)
            {
                Debug.Log("下载完毕");
                m_writer.Flush();
                m_writer.Close();
                m_writer.Dispose();
                OnDownOver();
                return;
            }

            var node = m_waitingList.Dequeue();
            string name = node["BundleName"].ToString();
            string len = node["Length"].ToString();
            m_writer.Write(name +":"+node["Hash"]);
            //mediator 进度变化
            m_webAgent.onDownloadSuccess = () =>
            {
                m_writer.Write(":1\n");
                StartDown();
            };
            StartCoroutine(m_webAgent.Download(m_remoteUrl + name, m_temPath + name, long.Parse(len)));
        }

        private void OnDownOver()
        {
            FileManager.CreateDirectory(m_savePath);
            if (File.Exists(m_savePath + Config.BundleManifest))
                File.Delete(m_savePath + Config.BundleManifest);
            File.Move(m_temPath+Config.BundleManifest, m_savePath+Config.BundleManifest);
            var assets = (Dictionary<string, JsonNode>)m_root["BundleManifest"];
            var eor = assets.GetEnumerator();
            while (eor.MoveNext())
            {
                string name = eor.Current.Value["BundleName"].ToString();
                if (File.Exists(m_temPath + name))
                {
                    if(File.Exists(m_savePath+name))
                        File.Delete(m_savePath+name);

                    File.Move(m_temPath+name, m_savePath+name);
                }
            }
            eor.Dispose();
            FileManager.ClearFiles(m_temPath);
        }
        

        public override void OnRemove()
        {
            Facade.Instance.RemoveMediator<VersionMediator>(VersionMediator.NAME);
        }
    }
}