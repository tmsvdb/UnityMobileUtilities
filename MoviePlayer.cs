using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using kernel;

using System;

namespace MobileUtilities
{
    /*
       =======================================
       CLASS INTERFACE:
       =======================================
   */

    public interface IMoviePlayer
    {
        void Play (string fullPath, Action MovieFinished);
    }

    /*
        =======================================
        CLASS IMPLEMENTATION:
        =======================================
    */
    public class MoviePlayer : MonoBehaviour, IMoviePlayer
    {
        IDebug debugger;

        void Start ()
        {
            debugger = GetComponent<Debugger>();
        }

        /*
            PUBLIC
        */

        public void Play(string fullPath, Action MovieFinished)
        {
            debugger.log("[MoviePlayer] play => 'file://" + fullPath + "'");

            StartCoroutine(Playback(fullPath, MovieFinished));
        }

        /*
            PRIVATE
        */

        IEnumerator Playback(string path, Action finishedCallback = null)
        {
            Handheld.PlayFullScreenMovie("file://" + path, Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (finishedCallback != null)
                finishedCallback();
        }
    }
}
