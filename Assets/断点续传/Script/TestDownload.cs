using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace 断点续传.Script
{
    public class TestDownload : MonoBehaviour
    {
        public Button DownloadBtn;
        public Button PauseBtn;
        public Button ClearBtn;
        public TMP_InputField UrlInputField;
        public Slider ProgressSlider;
        public TextMeshProUGUI ProgressText;
        public TextMeshProUGUI Downloadinfo;
        
        DownloadTask _downloadTask;

        private void Awake()
        {
            DownloadBtn.onClick.AddListener(()=>
            {
                if (_downloadTask==null)
                {
                    if (UrlInputField.text==string.Empty)return;
                    _downloadTask=AsyncDownloader.Instance.DownloadFile
                    (UrlInputField.text,$"{Application.streamingAssetsPath}/TestDownload.mp4",
                        OnProgress,OnCompleted);
                    Downloadinfo.text = "下载开始";
                }
                if (_downloadTask.isPaused)
                {
                    AsyncDownloader.Instance.ResumeDownload(_downloadTask);
                    
                    Downloadinfo.text = "下载继续";
                }
            });
            PauseBtn.onClick.AddListener(() =>
            {
                if (_downloadTask==null)return;
                if (_downloadTask.isDone)return;
                if (!_downloadTask.isPaused)
                {
                    AsyncDownloader.Instance.PauseDownload(_downloadTask);
                    Downloadinfo.text = "重启下载";
                }
            });
            ClearBtn.onClick.AddListener(() =>
            {
                if (_downloadTask==null)return;
                if (_downloadTask.isDone)return;
                AsyncDownloader.Instance.CancelDownload(_downloadTask);
                Downloadinfo.text = "下载取消";
                ProgressSlider.value = 0;
                ProgressText.text = "0%";
            });
            
        }

        void OnProgress(DownloadTask task)
        {
            ProgressSlider.value = AsyncDownloader.GetProgress(task);
            ProgressText.text = $"{Mathf.RoundToInt(ProgressSlider.value*100)}%";
        }

        void OnCompleted(DownloadTask task)
        {
            ProgressText.text = "100%";
            ProgressSlider.value = 1;
            Downloadinfo.text = "下载完成";
            _downloadTask = null;
        }
    }
}
