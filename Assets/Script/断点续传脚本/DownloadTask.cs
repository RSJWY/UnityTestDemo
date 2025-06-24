using System;
using System.Net;
using UnityEngine.Networking;

namespace Script.断点续传脚本
{
    public class DownloadTask
    {
        
        public string url;
        public string savePath;
        /// <summary>
        /// Unity的下载器
        /// </summary>
        public UnityWebRequest request;
        /// <summary>
        /// 文件总大小
        /// </summary>
        public long totalBytes;
        /// <summary>
        /// 已下载大小
        /// </summary>
        public long downloadedBytes;
        /// <summary>
        /// 下载的百分比
        /// </summary>
        public long progress=>downloadedBytes/totalBytes;
        /// <summary>
        /// 已下载长度
        /// </summary>
        public long downloadProgress;
        public bool isPaused;
        public bool isDone;
        public bool isError;
        /// <summary>
        /// 错误日志
        /// </summary>
        public string errorMessage;
        /// <summary>
        /// 下载进度率刷新
        /// </summary>
        public Action<DownloadTask> onProgress;
        /// <summary>
        /// 完成下载
        /// </summary>
        public Action<DownloadTask> onCompleted;
    }
}