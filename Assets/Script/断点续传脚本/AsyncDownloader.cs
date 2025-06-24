using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Script.断点续传脚本
{

    public class AsyncDownloader : MonoBehaviour
    {
       
        private static AsyncDownloader _instance;
        public static AsyncDownloader Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("AsyncDownloader");
                    _instance = obj.AddComponent<AsyncDownloader>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        private List<DownloadTask> activeTasks = new List<DownloadTask>();
        private const int bufferSize = 4096;

        public DownloadTask DownloadFile(string url, string savePath, 
            Action<DownloadTask> onProgress = null, 
            Action<DownloadTask> onCompleted = null)
        {
            // 检查是否已有相同任务
            var existingTask = activeTasks.Find(t => t.url == url && t.savePath == savePath);
            if (existingTask != null)
            {
                if (existingTask.isPaused)
                {
                    ResumeDownload(existingTask);
                }
                return existingTask;
            }

            var task = new DownloadTask()
            {
                url = url,
                savePath = savePath,
                onProgress = onProgress,
                onCompleted = onCompleted,
                isPaused = false,
                isDone = false
            };

            activeTasks.Add(task);
            StartCoroutine(DownloadCoroutine(task));
            return task;
        }

        /// <summary>
        /// 异步执行的下载逻辑函数
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        private IEnumerator DownloadCoroutine(DownloadTask task)
        {
            // 检查已下载文件大小（断点续传），基于已下载文件大小信息
            FileInfo fileInfo = new FileInfo(task.savePath);
            //判断是否需要徐
            bool resumeDownload = fileInfo.Exists && fileInfo.Length > 0;
        
            // 创建下载请求
            if (resumeDownload)
            {
                //如果断点续传，则需要设置Rangeq请求头，设置请求的数据范围
                task.request = UnityWebRequest.Get(task.url);
                task.request.SetRequestHeader("Range", $"bytes={fileInfo.Length}-");
            }
            else
            {
                //直接下载
                task.request = UnityWebRequest.Get(task.url);
            }

            // 设置下载处理器
            task.request.downloadHandler = new DownloadHandlerFile(task.savePath, resumeDownload);

            // 开始下载
            task.request.SendWebRequest();

            // 更新进度
            // 在本循环内，每一帧检查是否请求暂停
            // 其实暂停就行进行了取消
            while (!task.request.isDone)
            {
                if (task.isPaused)
                {
                    task.request.Abort();
                    yield break;
                }

                if (task.onProgress != null)
                {
                    task.onProgress.Invoke(task);
                }
                yield return null;
            }

            // 处理完成状态
            if (task.request.result == UnityWebRequest.Result.Success)
            {
                task.isDone = true;
            }
            else
            {
                task.isError = true;
                task.errorMessage = task.request.error;
            }

            // 触发完成回调
            if (task.onCompleted != null)
            {
                task.onCompleted.Invoke(task);
            }

            // 清理
            activeTasks.Remove(task);
            task.request.Dispose();
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        /// <param name="task">下载任务</param>
        public void PauseDownload(DownloadTask task)
        {
            if (task != null && !task.isPaused && !task.isDone)
            {
                task.isPaused = true;
                if (task.request != null)
                {
                    task.request.Abort();
                }
            }
        }

        /// <summary>
        /// 重启下载
        /// </summary>
        /// <param name="task"></param>
        public void ResumeDownload(DownloadTask task)
        {
            if (task != null && task.isPaused && !task.isDone)
            {
                task.isPaused = false;
                StartCoroutine(DownloadCoroutine(task));
            }
        }

        /// <summary>
        /// 取消下砸
        /// </summary>
        /// <param name="task"></param>
        public void CancelDownload(DownloadTask task)
        {
            if (task != null && !task.isDone)
            {
                task.isPaused = true;
                if (task.request != null)
                {
                    task.request.Abort();
                }
                activeTasks.Remove(task);
            }
        }

        /// <summary>
        /// 根据任务获取下载进度
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public float GetProgress(DownloadTask task)
        {
            if (task == null || task.totalBytes <= 0) return 0;
            return (float)task.downloadedBytes / task.totalBytes;
        }
    }
}