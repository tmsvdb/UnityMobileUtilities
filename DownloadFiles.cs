using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                throw new DownloadFailedException(string.Format("Failed to download file '{0}' because an error occurred: {1}", www.error));

            onCompleteCallback(www);
        }

        private IEnumerator ShowProgress(WWW www, Action<float> onProgressUpdate)
        {
            while (!www.isDone)
            {
                onProgressUpdate(www.progress);
                yield return new WaitForSeconds(.1f);
            }
        }
	}
}
