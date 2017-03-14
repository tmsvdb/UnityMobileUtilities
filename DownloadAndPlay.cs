using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


/*
    =======================================
    DOWNLOAD AND PLAY EVENTS:
    =======================================
*/
public class DownloadAndPlayEvents
{
    // EVENTS:
    // ---------------------

    /* 
        DownloadComplete is fired if the download of the file was successful

            @WWW: the WWW parameter contains the download object
    */
    public Action<WWW> DownloadComplete;

    /*
        DownloadComplete is fired if the movie has finished playing
    */
    public Action MovieFinished;

    /*
        DownLoadProgress is fired every update during the download process

            @float: the float parameter contains the progress of the download, this is a value between zero and one; 
    */
    public Action<float> DownLoadProgress;

    // ERRORS:
    // ---------------------

    /*
        WriteFailed is fired if the downloaded fil was not saved correctly to the local storage location
    */
    public Action WriteFailed;

    /*
        DownloadFailed is fired if the movie could not be downloaded from the given url location

            @string: the string parameter contains the error message discribing what went wrong
    */
    public Action<string> DownloadFailed;
}


/*
    =======================================
    DOWNLOAD AND PLAY INTERFACE:
    =======================================
*/
public interface IDownloadAndPlay
{
    /*
        AutomatedDownloadAndPlay is used to go through the whole process of checking if there is a movie,
        if not: downloading it and storing it on the device. And auto playing the movie afterwards.

            @param downloadLocation: 
            the url location where the video can be downloaded from.

            @param filename: 
            the name of the video to download.

            @param events: 
            an package (typeof DownloadAndPlayEvents) containing all events that could be fired during this process.
            To listen to these events you still have to subscribe to the different events within this object!

    */
    void AutomatedDownloadAndPlay(string downloadLocation, string filename, DownloadAndPlayEvents events = null);

    /*
        Download a movie from a url location

            @param downloadLocation: 
            the url location where the video can be downloaded from.

            @param filename: 
            the name of the video to download and save (on this running device)

            @param DownloadComplete:
            DownloadComplete is a callback methode that is fired if the movie is downloaded succesful
                Action<WWW>: the WWW parameter contains the unity web class that holds the download data

            @param DownloadFailed (optional):
            DownloadFailed is a callback methode that is fired if the movie could not be downloaded
                Action<string>: the string parameter contains the error message discribing what went wrong

            @param DownLoadProgress (optional):
            DownLoadProgress is a callback methode that is fired every update during the download process
                Action<float>: the float parameter contains the progress of the download, this is a value between zero and one;
    */
    void DownloadMovie(string downloadLocation, string filename, Action<WWW> DownloadComplete, Action<float> DownLoadProgress = null);

    /*
        Write an list of bytes to a file located at the divices persistentDataPath with a specified filename

            @param filename: 
            the name of the video we want to save

            @param bytes: 
            the list of bytes that represents the video we want to save
    */
    void WriteToFile(string filename, byte[] bytes);

    /*
        Delete a file located at the divices persistentDataPath with a specified filename

            @param filename: 
            the name of the video we want to save
    */
    void DeleteFile(string filename);

    /*
        Play a movie that has been saved on this device

            @param filename: 
            the name of the video we want to play

            @param MovieFinished: 
            MovieFinished is a callback methode that is fired if the movie hasstoped playing
    */
    void PlayMovie(string filename, Action MovieFinished);
}


/*
    =======================================
    DOWNLOAD AND PLAY CLASS IMPLEMENTATION:
    =======================================
*/
public class DownloadAndPlay : MonoBehaviour, IDownloadAndPlay
{
    private MobileUtilities.IMoviePlayer moviePlayer;
    private MobileUtilities.IDeviceStorage deviceStorage;
    private MobileUtilities.IDownloadFiles downloadFiles;

    void Start()
    {
        moviePlayer = gameObject.AddComponent<MobileUtilities.MoviePlayer>();
        deviceStorage = gameObject.AddComponent<MobileUtilities.DeviceStorage>();
        downloadFiles = gameObject.AddComponent<MobileUtilities.DownloadFiles>();

        AutomatedDownloadAndPlay("https://blobs.upscore.nl/v0/", "c9e273fa-39ec-4076-a10c-ed366c71ea16");
    }

    /*
        ----------------------------
        PUBLIC IMPLEMENTATION:
        ----------------------------
    */

    public void AutomatedDownloadAndPlay(string downloadLocation, string filename, DownloadAndPlayEvents events = null)
    {
        Debug.Log("[DownloadAndPlay] Automated DownloadAndPlay: " + downloadLocation + filename);

        if (events == null) events = new DownloadAndPlayEvents();

        LoadMovieAndPlay(downloadLocation, filename, events, false);
    }

    public void DownloadMovie(string downloadLocation, string filename, Action<WWW> DownloadComplete, Action<float> DownLoadProgress = null)
    {
        Debug.Log("[DownloadAndPlay] Download Movie: " + downloadLocation + filename);
        downloadFiles.Download(downloadLocation + filename, DownloadComplete, DownLoadProgress);
    }

    public void WriteToFile(string filename, byte[] bytes)
    {
        Debug.Log("[DownloadAndPlay] Write To File: " + filename);
        deviceStorage.Save(filename, bytes);
    }

    public void PlayMovie(string filename, Action MovieFinished)
    {
        Debug.Log("[DownloadAndPlay] Play Movie: " + deviceStorage.AbsoluteFileName(filename));
        moviePlayer.Play(deviceStorage.AbsoluteFileName(filename), MovieFinished);
    }

    public void DeleteFile(string filename)
    {
        Debug.Log("[DownloadAndPlay] Delete File: " + filename);
        deviceStorage.Delete(filename);
    }

    /*
        ----------------------------
        LOCAL IMPLEMENTATION:
        ----------------------------
    */

    void LoadMovieAndPlay(string downloadLocation, string filename, DownloadAndPlayEvents events, bool isRecursive = false)
    {
        if (!deviceStorage.Exist(filename))
        {
            if (!isRecursive)
            {
                DownloadMovie
                (
                    downloadLocation,
                    filename,
                    (WWW www) =>
                    {
                        if (events.DownloadComplete != null)
                            events.DownloadComplete(www);

                        Debug.Log("[DownloadAndPlay] Download Complete");

                        if (!string.IsNullOrEmpty(www.error))
                            Debug.Log("[DownloadAndPlay] Download ERROR: " + www.error);

                        WriteToFile(filename, www.bytes);
                        LoadMovieAndPlay(downloadLocation, filename, events, true);
                    },
                    (float p) =>
                    {
                        Debug.Log("[DownloadAndPlay] Progress: " + p);
                        if (events.DownLoadProgress != null)
                            events.DownLoadProgress(p);
                    }
                   
                    
                );
            }
            else
            {
                if (events.WriteFailed != null) events.WriteFailed();
                Debug.Log("[DownloadAndPlay] WriteFailed");
            }
        }
        else
        {
            PlayMovie(filename, () => 
            {
                Debug.Log("[DownloadAndPlay] Movie Finished");
                if (events.MovieFinished != null) events.MovieFinished();
            });
        }
    }
}