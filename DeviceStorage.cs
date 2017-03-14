using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;

namespace MobileUtilities
{
    /*
        =======================================
        Exceptions:
        =======================================
    */

    public class WriteToFileException : System.Exception
    {
        public WriteToFileException() { }
        public WriteToFileException(string message) : base(message) { }
        public WriteToFileException(string message, Exception inner) : base(message, inner) { }
    }

    /*
        =======================================
        CLASS INTERFACE:
        =======================================
    */

    public interface IDeviceStorage
    {
        void Save (string filename, byte[] bytes);

        //byte[] Load (string filename);

        bool Exist (string filename);

        void Delete (string filename);

        string AbsoluteFileName(string filename);
    }

    /*
        =======================================
        CLASS IMPLEMENTATION:
        =======================================
    */

    public class DeviceStorage : MonoBehaviour, IDeviceStorage
    {
        /*
            PUBLIC
        */

        public void Save(string filename, byte[] bytes)
        {
            WriteToFile(filename, bytes);
        }

        public byte[] Load(string filename)
        {
            throw new NotImplementedException();
        }

        public bool Exist (string filename)
        {
            return File.Exists(AddApplicationRootPath(filename));
        }

        public void Delete(string filename)
        {
            DeleteFile(filename);
        }

        public string AbsoluteFileName(string filename)
        {
            return AddApplicationRootPath(filename);
        }

        /*
            PRIVATE
        */

        public string AddApplicationRootPath(string filename)
        {
            return System.IO.Path.Combine(Application.persistentDataPath, filename);
        }

        void WriteToFile(string filename, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(AddApplicationRootPath(filename), bytes);
            }
            catch (IOException e)
            {
                throw new WriteToFileException(string.Format("Failed to save your movie because an error occured when using the disk: {0}", e.Message), e);
            }
        }

        void DeleteFile(string filename)
        {
            #if UNITY_IOS

                File.Delete ("/private" + AddApplicationRootPath(filename));
            #else

                File.Delete(AddApplicationRootPath(filename));
            #endif
        }
    }
}
