using System;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public static class RockstarArchiveManager
    {
        private static String wadFileName = ConfigJsonManager.GetValueFromConfig("wadFileName");

        private static String dirFileName = ConfigJsonManager.GetValueFromConfig("dirFileName");

        private static String musicFileName = ConfigJsonManager.GetValueFromConfig("musicFileName"); // "C:\\WARRIORS\\IOP\\MUSIC.SND"

        private static String soundFileName = ConfigJsonManager.GetValueFromConfig("soundFileName"); // "C:\\WARRIORS\\IOP\\BFW.SND"

        private static Stream wadArchive;
        
        private static Stream musicArchive;
        
        private static Stream soundArchive;

        public static bool OpenArchives()
        {
            if (File.Exists(dirFileName) && File.Exists(wadFileName) && File.Exists(musicFileName) && File.Exists(soundFileName))
            {
                try
                {
                    wadArchive = File.OpenRead(wadFileName);
                    musicArchive = File.OpenRead(musicFileName);
                    soundArchive = File.OpenRead(soundFileName);
                }
                catch (FileNotFoundException Exception)
                {
                    if (Exception.Source != null)
                    {
                        throw;
                    }
                }
                catch (IOException Exception)
                {
                    if (Exception.Source != null)
                    {
                        throw;
                    }
                }

                return true;
            }

            return false;
        }

        public static void CloseArchiveFiles()
        {
            wadArchive.Dispose();
            musicArchive.Dispose();
            soundArchive.Dispose();
        }

        public static String GetDirFileName()
        {
            return dirFileName;
        }

        public static Stream GetWadStream()
        {
            return wadArchive;
        }

        public static long GetMusicArchiveLength()
        {
            return musicArchive.Length;
        }

        public static Stream GetWadArchiveFile(UInt32 uiFileHash_)
        {
            RockstarMetadataManager.WadMetadata fileDetails = RockstarMetadataManager.GetWadFileMetadata(uiFileHash_);

            if (fileDetails != null)
            {
                return new ArchiveFileStream(wadArchive, fileDetails.lFileOffset, fileDetails.lFileSize, fileDetails.uiFileHash);
            }

            Debug.Log("*** Error: RockstarArchiveManage.GetWadArchiveFile(" + String.Format("{0:X8}", uiFileHash_) + ") failed.");

            return null;
        }
    }
}