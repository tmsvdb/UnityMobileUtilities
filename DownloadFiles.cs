using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using kernel;

using System;

namespace MobileUtilities
{
    /*
        =======================================
        Exceptions:
        =======================================
    */

    public class DownloadFailedException : System.Exception
    {
        public DownloadFailedException() { }
        public DownloadFailedException(string message) : base(message) { }
        public DownloadFailedException(string message, Exception inner) : base(message, inner) { }
    }

    /*
       =======================================
       CLASS INTERFACE:
       =======================================
   */

    public interface IDownloadFiles
    {
        void Download(string url, Action<WWW> onCompleteCallback);
        void Download(string url, Action<WWW> onCompleteCallback, Action<float> onProgressUpdate);
    }

    /*
        =======================================
        CLASS IMPLEMENTATION:
        =======================================
    */

    public class DownloadFiles : MonoBehaviour, IDownloadFiles
    {
        private IDebug debugger;

        void Start ()
        {
            debugger = GetComponent<Debugger>();
        }

        /*
            PUBLIC
        */

        public void Download (string url, Action<WWW> onCompleteCallback)
        {
            StartCoroutine(DownloadFile(url, onCompleteCallback));
        }

        public void Download(string url, Action<WWW> onCompleteCallback, Action<float> onProgressUpdateCallback)
        {
            StartCoroutine(DownloadFile(url, onCompleteCallback, onProgressUpdateCallback));
        }


        /*
            PRIVATE
        */

        IEnumerator DownloadFile(string url, Action<WWW> onCompleteCallback, Action<float> onProgressUpdateCallback = null)
        {
            WWW www = new WWW(url);

            if (onProgressUpdateCallback != null)
                StartCoroutine(ShowProgress(www, onProgressUpdateCallback));

            while (!www.isDone)
                yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                string errorMSG = string.Format("[DownloadFile] ERROR :: Failed to download file from url {0} because an error occurred: {1}", url, www.error);
                debugger.log(errorMSG);
                throw new DownloadFailedException(errorMSG);
            }

            onCompleteCallback(www);
        }

        private IEnumerator ShowProgress(WWW www, Action<float> onProgressUpdate)
        {
            while (!www.isDone)
            {
                debugger.log("[DownloadFile] progress: " + Mathf.Round(www.progress*100)+"%");
                onProgressUpdate(www.progress);
                yield return new WaitForSeconds(.1f);
            }
        }
	}
}
