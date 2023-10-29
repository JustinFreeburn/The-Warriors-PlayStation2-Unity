using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public static class RockstarMetadataManager
    {
        public class WadMetadata
        {
            public long lFileOffset;
            
            public long lFileSize;
            
            public UInt32 uiFileHash;
            
            public bool bModel;
            
            public Int32 iWADIndex;
            
            public UInt32 uiOriginalFileHash;
            
            public Int32 iModelType;
        }

        public class MusicMetadata
        {
            public int iFileEntry;
            
            public long lFileSize;
            
            public UInt32 uiUnknown1;
            
            public Int32 iFrequency;

            // NOTE: Sampling frequency?
            public UInt32 uiUnknown2;
            
            public Int32 iSampleBits;
            
            public long lFileOffset;

            // NOTE: 0-1 one channel (mono), 2 two channels (stereo)
            public Int32 iChannels;
            
            public Int32 iInterleave;
            
            public Int32 iInterleavesPerChannel;
            
            public UInt32 uiUnknown4;
            
            public UInt32 uiFileHash;
            
            public String sFileName;
        }

        public class AudioMetadata
        {
            public int iFileEntry;
            
            public long lFileSize;
            
            public long lFileOffset;
            
            public UInt32 uiFileHash;
            
            public String sFileName;
            
            public UInt32 uiUnknown1;
            
            public UInt32 uiUnknown2;
            
            public UInt32 uiUnknown3;
            
            public UInt32 uiUnknown4;
        }

        public class CharacterMetadata
        {
            public UInt32 uiUnknownHash1;
            
            public UInt32 uiUnknownHash2;
            
            public UInt32 uiModelHash;
            
            public UInt32 uiTextureHash;
            
            public UInt32 uiUnknown1;
            
            public UInt32 uiUnknown2;
            
            public UInt32 uiUnknown3;
            
            public UInt32 uiUnknown4;
        }

        public class ObjectMetadata
        {
            public UInt32 uiUnknownHash1;
            
            public UInt32 uiUnknownHash2;
            
            public UInt32 uiModelHash;
            
            public UInt32 uiTextureHash;
            
            public UInt32 uiUnknown1;
            
            public UInt32 uiUnknown2;
            
            public UInt32 uiUnknown3;
            
            public UInt32 uiUnknown4;
            
            public UInt32 uiUnknown5;
        }

        public class DependencyMetadata
        {
            public UInt32 uiFileHash;
            
            public UInt32 uiDependency;
        }

        public static Dictionary<UInt32, WadMetadata> WadMetadataContainer = new Dictionary<UInt32, WadMetadata>();
        
        public static List<MusicMetadata> MusicMetadataContainer = new List<MusicMetadata>();
        
        public static Dictionary<String, AudioMetadata> AudioMetadataContainer = new Dictionary<String, AudioMetadata>();
        
        public static Dictionary<String, CharacterMetadata> CharacterMetadataContainer = new Dictionary<String, CharacterMetadata>();
        
        public static Dictionary<String, ObjectMetadata> ObjectMetadataContainer = new Dictionary<String, ObjectMetadata>();
        
        public static Dictionary<UInt32, DependencyMetadata> DependencyMetadataContainer = new Dictionary<UInt32, DependencyMetadata>();

        public static bool CreateMetadata()
        {
            using (Stream DirStreamFile = File.OpenRead(RockstarArchiveManager.GetDirFileName()))
            {
                using (ArchiveFileBinaryReader DirFileReader = new ArchiveFileBinaryReader(DirStreamFile))
                {
                    ArchiveFileBinaryReader WadFileReader = new ArchiveFileBinaryReader(RockstarArchiveManager.GetWadStream());

                    Int32 iMetadataCount = DirFileReader.ReadInt32();
                    DirFileReader.SeekCurrent(12);

                    for (Int32 iIterator = 0; iIterator < iMetadataCount; iIterator++)
                    {
                        WadMetadata Entry = new WadMetadata();
                        UInt32[] uiPeekBytes = new UInt32[4];

                        Entry.lFileOffset = DirFileReader.ReadUInt32();
                        Entry.lFileSize = DirFileReader.ReadUInt32();
                        Entry.uiFileHash = DirFileReader.ReadUInt32();

                        Entry.iWADIndex = iIterator;
                        Entry.uiOriginalFileHash = Entry.uiFileHash;
                        Entry.iModelType = -1;

                        WadFileReader.SeekBeginning(Entry.lFileOffset);

                        uiPeekBytes[0] = WadFileReader.ReadUInt32();
                        WadFileReader.SeekCurrent(8);
                        uiPeekBytes[1] = WadFileReader.ReadUInt32();
                        uiPeekBytes[2] = WadFileReader.ReadUInt32();
                        WadFileReader.SeekCurrent(12);
                        uiPeekBytes[3] = WadFileReader.ReadUInt32();

                        if ((uiPeekBytes[0] == 0x01 || uiPeekBytes[0] == 0x02) && (uiPeekBytes[2] == 0x47) && (uiPeekBytes[3] == 0x10) ||
                            (uiPeekBytes[0] == 0x01 || uiPeekBytes[0] == 0x02) && (uiPeekBytes[2] == 0x2A) && (uiPeekBytes[3] == 0x16))
                        {
                            Entry.uiFileHash = uiPeekBytes[1];
                        }

                        // NOTE: Lets find some unused DFF's...
                        if ((uiPeekBytes[0] == 0x01 || uiPeekBytes[0] == 0x02) && (uiPeekBytes[2] == 0x47) && (uiPeekBytes[3] == 0x10))
                        {
                            if (uiPeekBytes[0] == 0x01)
                            {
                                Entry.iModelType = 1;
                            }
                            else if (uiPeekBytes[0] == 0x02)
                            {
                                Entry.iModelType = 2;
                            }
                            Entry.bModel = true;
                        }
                        else
                        {
                            Entry.bModel = false;
                        }

                        WadMetadataContainer.Add(Entry.uiFileHash, Entry);
                    }
                }
            }

            if (WadMetadataContainer.Count <= 0)
            {
                Debug.Log("*** Error: No archive metadata read in.");

                return false;
            }

            RenderWareStreamFile MetadataFile = new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(0xE87E18D5));

            if (MetadataFile == null)
            {
                Debug.Log("*** Error: Failed to get metadata file from archive.");

                return false;
            }

            if (GenerateMusicMetadata(MetadataFile) == false)
            {
                Debug.Log("*** Error: Failed to GenerateMusicMetadata().");

                return false;
            }

            if (GenerateSoundMetadata(MetadataFile) == false)
            {
                Debug.Log("*** Error: Failed to GenerateSoundMetadata().");

                return false;
            }

            if (GenerateCharacterMetadata(MetadataFile) == false)
            {
                Debug.Log("*** Error: Failed to GenerateCharacterMetadata().");

                return false;
            }

            if (GenerateObjectMetadata(MetadataFile) == false)
            {
                Debug.Log("*** Error: Failed to GenerateObjectMetadata().");

                return false;
            }

            if (GenerateDependencyMetadata(MetadataFile) == false)
            {
                Debug.Log("*** Error: Failed to GenerateDependencyMetadata().");

                return false;
            }

            return true;
        }

        private static bool GenerateMusicMetadata(RenderWareStreamFile StreamFile_)
        {
            Stream MusicMetadataFile = StreamFile_.GetStreamFile(0x00000031, 0xFADA6479);

            if (MusicMetadataFile == null)
            {
                return false;
            }

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(MusicMetadataFile))
            {
                Int32 iMetadataCount = Reader.ReadInt32();

                for (Int32 iIterator = 0; iIterator < iMetadataCount; iIterator++)
                {
                    MusicMetadata Entry = new MusicMetadata();

                    Entry.iFileEntry = iIterator;

                    // TODO: Discover which of these unknown values is the file size. Once discovered, remove RockstarArchiveManager.GetMusicArchiveLength()
                    Entry.uiUnknown1 = Reader.ReadUInt32();
                    Entry.iFrequency = Reader.ReadInt32();
                    Entry.uiUnknown2 = Reader.ReadUInt32();
                    Entry.iSampleBits = Reader.ReadInt32();
                    Entry.lFileOffset = Reader.ReadUInt32();
                    Entry.iChannels = Reader.ReadInt32();
                    Entry.iInterleave = Reader.ReadInt32();
                    Entry.iInterleavesPerChannel = Reader.ReadInt32();
                    Entry.uiUnknown4 = Reader.ReadUInt32();
                    Entry.uiFileHash = Reader.ReadUInt32();
                    Entry.sFileName = new String(Reader.ReadChars(64)).Replace("\0", "");

                    if (iIterator > 0)
                    {
                        MusicMetadataContainer[iIterator - 1].lFileSize = (Entry.lFileOffset - MusicMetadataContainer[iIterator - 1].lFileOffset);
                    }

                    if (iIterator == (iMetadataCount - 1))
                    {
                        Entry.lFileSize = RockstarArchiveManager.GetMusicArchiveLength() - Entry.lFileOffset;
                    }

                    MusicMetadataContainer.Add(Entry);
                }
            }

            return true;
        }

        private static bool GenerateSoundMetadata(RenderWareStreamFile StreamFile_)
        {
            Stream SoundMetadataFile = StreamFile_.GetStreamFile(0x00000029, 0xB536E6E3);

            if (SoundMetadataFile == null)
            {
                return false;
            }

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(SoundMetadataFile))
            {
                Int32 iMetadataCount = Reader.ReadInt32();

                for (Int32 iIterator = 0; iIterator < iMetadataCount; iIterator++)
                {
                    AudioMetadata Entry = new AudioMetadata();

                    Entry.iFileEntry = iIterator;
                    Entry.lFileSize = -1;
                    Entry.lFileOffset = -1;
                    Entry.uiFileHash = 0x00000000;
                    Entry.sFileName = Reader.ReadChars(16).ToString();
                    Entry.uiUnknown1 = Reader.ReadUInt32();
                    Entry.uiUnknown2 = Reader.ReadUInt32();
                    Entry.uiUnknown3 = Reader.ReadUInt32();
                    Entry.uiUnknown4 = Reader.ReadUInt32();

                    //AudioList.Add(Entry.sFileName, Entry);
                }
            }

            return true;
        }

        private static bool GenerateCharacterMetadata(RenderWareStreamFile StreamFile_)
        {
            Stream CharacterMetadataFile = StreamFile_.GetStreamFile(0x00000044, 0x00000000);

            if (CharacterMetadataFile == null)
            {
                return false;
            }

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(CharacterMetadataFile))
            {
                Int32 iMetadataCount = Reader.ReadInt32();

                Reader.SeekCurrent(12);

                for (Int32 iIterator = 0; iIterator < iMetadataCount; iIterator++)
                {
                    CharacterMetadata Entry = new CharacterMetadata();

                    Entry.uiUnknownHash1 = Reader.ReadUInt32();
                    Entry.uiUnknownHash2 = Reader.ReadUInt32();
                    Entry.uiModelHash = Reader.ReadUInt32();
                    Entry.uiTextureHash = Reader.ReadUInt32();
                    Entry.uiUnknown1 = Reader.ReadUInt32();
                    Entry.uiUnknown2 = Reader.ReadUInt32();
                    Entry.uiUnknown3 = Reader.ReadUInt32();
                    Entry.uiUnknown4 = Reader.ReadUInt32();

                    String TextureName = UnityTextureManager.GetTextureNameFromFileNameHash(Entry.uiTextureHash);

                    // TODO: investigate why there are duplicates? textures should be unique... looks like R* has varying models but use the same textures?
                    if (CharacterMetadataContainer.ContainsKey(TextureName))
                    {
                        int iDuplicateIterator = 0;
                        String sDuplicateTextureName = String.Format(TextureName + "_dupe_" + iDuplicateIterator);

                        foreach (var item in RockstarMetadataManager.CharacterMetadataContainer)
                        {
                            if (item.Key == String.Format(TextureName + "_dupe_" + iDuplicateIterator))
                            {
                                iDuplicateIterator++;
                                sDuplicateTextureName = String.Format(TextureName + "_dupe_" + iDuplicateIterator);
                            }
                        }

                        TextureName = sDuplicateTextureName;
                    }

                    CharacterMetadataContainer.Add(TextureName, Entry);
                }
            }

            return true;
        }

        private static bool GenerateObjectMetadata(RenderWareStreamFile StreamFile_)
        {
            Stream ObjectMetadataFile = StreamFile_.GetStreamFile(0x00000046, 0x00000000);

            if (ObjectMetadataFile == null)
            {
                return false;
            }

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(ObjectMetadataFile))
            {
                Int32 iMetadataCount = Reader.ReadInt32();

                Reader.SeekCurrent(12);

                for (Int32 iIterator = 0; iIterator < iMetadataCount; iIterator++)
                {
                    ObjectMetadata Entry = new ObjectMetadata();

                    Entry.uiUnknownHash1 = Reader.ReadUInt32();
                    Entry.uiUnknownHash2 = Reader.ReadUInt32();
                    Entry.uiModelHash = Reader.ReadUInt32();
                    Entry.uiTextureHash = Reader.ReadUInt32();
                    Entry.uiUnknown1 = Reader.ReadUInt32();
                    Entry.uiUnknown2 = Reader.ReadUInt32();
                    Entry.uiUnknown3 = Reader.ReadUInt32();
                    Entry.uiUnknown4 = Reader.ReadUInt32();
                    Entry.uiUnknown5 = Reader.ReadUInt32();

                    String TextureName = UnityTextureManager.GetTextureNameFromFileNameHash(Entry.uiTextureHash);

                    if (ObjectMetadataContainer.ContainsKey(TextureName))
                    {
                        int iDuplicateIterator = 0;
                        String sDuplicateTextureName = String.Format(TextureName + "_dupe_" + iDuplicateIterator);

                        foreach (var item in RockstarMetadataManager.ObjectMetadataContainer)
                        {
                            if (item.Key == String.Format(TextureName + "_dupe_" + iDuplicateIterator))
                            {
                                iDuplicateIterator++;
                                sDuplicateTextureName = String.Format(TextureName + "_dupe_" + iDuplicateIterator);
                            }
                        }

                        TextureName = sDuplicateTextureName;
                    }

                    ObjectMetadataContainer.Add(TextureName, Entry);
                }
            }

            return true;
        }

        private static bool GenerateDependencyMetadata(RenderWareStreamFile StreamFile_)
        {
            /*
            Stream DependencyMetadataFile = StreamFile_.GetStreamFile(0x00000046, 0x00000000);

            if (DependencyMetadataFile == null)
            {
                return false;
            }

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(DependencyMetadataFile))
            {
                Int32 iMetadataCount = Reader.ReadInt32();

                Reader.SeekCurrent(12);

                for (Int32 iIterator = 0; iIterator < iMetadataCount; iIterator++)
                {
                    DependencyMetadata Entry = new DependencyMetadata()
                    {
                        uiFileHash = Reader.ReadUInt32(),
                        uiDependency = Reader.ReadUInt32()
                    };

                    // NOTE: Search DependencyMetadataContainer for the .lev internal hash.
                    DependencyMetadataContainer.Add(Entry.uiFileHash, Entry);
                }
            }
            */

            return true;
        }

        public static WadMetadata GetWadFileMetadata(UInt32 uiFileHash_)
        {
            if (WadMetadataContainer.ContainsKey(uiFileHash_) == true)
            {
                return new WadMetadata
                {
                    lFileOffset = WadMetadataContainer[uiFileHash_].lFileOffset,
                    lFileSize = WadMetadataContainer[uiFileHash_].lFileSize,
                    uiFileHash = WadMetadataContainer[uiFileHash_].uiFileHash
                };
            }

            return null;
        }

        public static CharacterMetadata GetCharacterMetadata(String sFileName_)
        {
            if (CharacterMetadataContainer.ContainsKey(sFileName_) == true)
            {
                return CharacterMetadataContainer[sFileName_];
            }

            return null;
        }

        public static ObjectMetadata GetObjectMetadata(String sFileName_)
        {
            if (ObjectMetadataContainer.ContainsKey(sFileName_) == true)
            {
                return ObjectMetadataContainer[sFileName_];
            }

            return null;
        }
    }
}