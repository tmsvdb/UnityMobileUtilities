using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /*
            PUBLIC
        */

        public void Play(string fullPath, Action MovieFinished)
        {
            StartCoroutine(Playback(fullPath, MovieFinished));
        }

        /*
            PRIVATE
        */

        IEnumerator Playback(string path, Action finishedCallback = null)
        {
            Handheld.PlayFullScreenMovie("file://" + path, Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (finishedCallback != null)
                finishedCallback();
        }
    }
}
