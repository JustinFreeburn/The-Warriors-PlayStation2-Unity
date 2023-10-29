using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public class RenderWareStreamFile
    {
        public class RenderWareStreamArchiveEntry
        {
            public UInt32 uiFileID;

            public long lFileSize;

            public UInt32 uiFileHash;

            public long lFileOffset;
        }

        public List<RenderWareStreamArchiveEntry> ArchiveEntries;

        public Stream renderWareStreamFile = null;

        public Int32 iFileCount;

        public long lFileSize;

        public UInt32 uiFileHash;

        /// <summary>
        /// Reads the RWS file and stores a RenderWareStreamArchiveEntry list in the class instance
        /// </summary>
        public RenderWareStreamFile(Stream archiveFileStream_)
        {
            if (archiveFileStream_ == null)
            {
                return;
            }

            ArchiveEntries = new List<RenderWareStreamArchiveEntry>();

            renderWareStreamFile = archiveFileStream_;

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(renderWareStreamFile))
            {
                iFileCount = Reader.ReadInt32();
                lFileSize = Reader.ReadInt32();
                Reader.SeekCurrent(4);
                uiFileHash = Reader.ReadUInt32();

                for (UInt32 uiIterator = 0; uiIterator < iFileCount; uiIterator++)
                {
                    RenderWareStreamArchiveEntry Entry = new RenderWareStreamArchiveEntry();

                    Entry.uiFileID = Reader.ReadUInt32();
                    Entry.lFileSize = Reader.ReadInt32();
                    Reader.SeekCurrent(4);
                    Entry.uiFileHash = Reader.ReadUInt32();
                    Entry.lFileOffset = Reader.Position();

                    ArchiveEntries.Add(Entry);

                    Reader.SeekCurrent(Entry.lFileSize);
                }
            }
        }

        public Stream GetStreamFile(UInt32 uiFileId_, UInt32 uiFileHash_)
        {
            for (Int32 iIterator = 0; iIterator < ArchiveEntries.Count; iIterator++)
            {
                if ((ArchiveEntries[iIterator].uiFileID == uiFileId_) && (ArchiveEntries[iIterator].uiFileHash == uiFileHash_))
                {
                    return new ArchiveFileStream(renderWareStreamFile, ArchiveEntries[iIterator].lFileOffset, ArchiveEntries[iIterator].lFileSize, ArchiveEntries[iIterator].uiFileHash);
                }
            }

            Debug.Log("*** Error: RenderWareStreamFile.GetStreamFile(" + String.Format("{0:X8}, {0:X8}", uiFileId_, uiFileHash_) + ") failed.");

            return null;
        }

        public Stream GetStreamFileWithHeader(UInt32 uiFileId_, UInt32 uiFileHash_)
        {
            for (Int32 iIterator = 0; iIterator < ArchiveEntries.Count; iIterator++)
            {
                if ((ArchiveEntries[iIterator].uiFileID == uiFileId_) && (ArchiveEntries[iIterator].uiFileHash == uiFileHash_))
                {
                    return new ArchiveFileStream(renderWareStreamFile, ArchiveEntries[iIterator].lFileOffset - 16, ArchiveEntries[iIterator].lFileSize + 16, ArchiveEntries[iIterator].uiFileHash);
                }
            }

            Debug.Log("*** Error: RenderWareStreamFile.GetStreamFileWithHeader(" + String.Format("{0:X8}, {0:X8}", uiFileId_, uiFileHash_) + ") failed.");

            return null;
        }

        public Stream GetStreamFile(Int32 iIndex_)
        {
            if (iIndex_ >= 0 && iIndex_ < ArchiveEntries.Count)
            {
                return new ArchiveFileStream(renderWareStreamFile, ArchiveEntries[iIndex_].lFileOffset, ArchiveEntries[iIndex_].lFileSize, ArchiveEntries[iIndex_].uiFileHash);
            }

            Debug.Log("*** Error: RenderWareStreamFile.GetStreamFile(" + iIndex_ + ") failed.");

            return null;
        }

        /// <summary>
        /// Returns an array of Stream by file type from the RenderWareStreamFile.
        /// </summary>
        public Stream[] GetFilesByType(UInt32 uiFileId_)
        {
            List<Stream> streamsByType = new List<Stream>();

            for (Int32 iIterator = 0; iIterator < ArchiveEntries.Count; iIterator++)
            {
                if (ArchiveEntries[iIterator].uiFileID == uiFileId_)
                {
                    streamsByType.Add(new ArchiveFileStream(renderWareStreamFile, ArchiveEntries[iIterator].lFileOffset, ArchiveEntries[iIterator].lFileSize, ArchiveEntries[iIterator].uiFileHash));
                }
            }

            return streamsByType.ToArray();
        }

        /// <summary>
        /// Returns true if the RenderWareStream contains the file type.
        /// </summary>
        public bool ContainsFileType(UInt32 uiFileId_)
        {
            foreach (RenderWareStreamArchiveEntry entry in ArchiveEntries)
            {
                if (entry.uiFileID == uiFileId_)
                {
                    return true;
                }
            }

            return false;
        }
    }
}