using System;
using System.Collections.Generic;
using System.IO;

namespace TheWarriors
{
    public class RenderWareSectorFile
    {
        /// (HEADER???)
        /// 0x01000000	4b	count (always one)
        /// 0x00000000	4b	size (always zero)
        /// 0x00000000	4b	empty/padding (always zero)
        /// 0xXXXXXXXX	4b	file name/hash
        /// 
        /// (SECTION)
        /// 0x16000000	4b	texture dictionary
        /// 0x1C000000	4b	size (always 28)
        /// 0x0A00021C	4b	version (may be others, yet to confirm)
        /// ... standard renderware section ...
        /// (only 4 bytes in the structure section. not just a dummy section! 8730 contains real texture data)
        /// 
        /// 0xXX000000	4b	count
        /// 
        /// for (count)
        /// {
	    ///     0xXX000000	4b	id/index???
        /// 
	    ///     (SECTION)
	    ///     0x14000000	4b	atomic
	    ///     0xXXXXXXXX	4b	size
	    ///     0x0A00021C	4b	version
	    ///     ... standard renderware section ...
        /// }

        public Dictionary<UInt32, RenderWareSection> renderWareStreamAtomicSections;

        public List<RenderWareSection> renderWareStreamTextureDictionarySections;

        public Int32 iFileCount;

        public long lFileSize;

        public UInt32 uiFileHash;

        public Int32 iAtomicCount;

        public RenderWareSectorFile(Stream StreamFile_)
        {
            renderWareStreamAtomicSections = new Dictionary<UInt32, RenderWareSection>();
            renderWareStreamTextureDictionarySections = new List<RenderWareSection>();

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(StreamFile_))
            {
                iFileCount = Reader.ReadInt32();
                lFileSize = Reader.ReadInt32();
                Reader.SeekCurrent(4);
                uiFileHash = Reader.ReadUInt32();

                if (iFileCount != 1)
                {
                    throw new Exception("*** Error: Expected iFileCount to be 1.");
                }

                if ((RenderWareSectionID)Reader.ReadInt32() != RenderWareSectionID.TextureDictionary)
                {
                    throw new Exception("*** Error: Expected TextureDictionary at position " + (Reader.Position() - 4));
                }

                // NOTE: This is not always a dummy TextureDictionary. 8730 contains real TextureDictionary data.
                renderWareStreamTextureDictionarySections.Add(new TextureDictionary().Read(Reader));

                UnityTextureManager.LoadTexturesFromRenderWareSections(renderWareStreamTextureDictionarySections.ToArray());

                iAtomicCount = Reader.ReadInt32();

                for (Int32 iIterator = 0; iIterator < iAtomicCount; iIterator++)
                {
                    UInt32 uiSectorID = Reader.ReadUInt32();

                    if ((RenderWareSectionID)Reader.ReadInt32() != RenderWareSectionID.Atomic)
                    {
                        throw new Exception("*** Error: Expected Atomic at position " + (Reader.Position() - 4));
                    }

                    Atomic atomic = new Atomic().Read(Reader);

                    renderWareStreamAtomicSections.Add(uiSectorID, atomic);
                }
            }
        }
    }
}