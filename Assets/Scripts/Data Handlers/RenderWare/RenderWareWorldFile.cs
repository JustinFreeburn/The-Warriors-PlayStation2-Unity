using System;
using System.Collections.Generic;
using System.IO;

namespace TheWarriors
{
    public class RenderWareWorldFile
    {
        public List<RenderWareSection> renderWareStreamSections;

        public UInt32 uiWorldID;

        public RenderWareWorldFile(Stream StreamFile_)
        {
            renderWareStreamSections = new List<RenderWareSection>();

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(StreamFile_))
            {
                uiWorldID = Reader.ReadUInt32();

                if ((RenderWareSectionID)Reader.ReadInt32() != RenderWareSectionID.TextureDictionary)
                {
                    throw new Exception("*** Error: Expected TextureDictionary at position " + (Reader.Position() - 4));
                }

                renderWareStreamSections.Add(new TextureDictionary().Read(Reader));

                if ((RenderWareSectionID)Reader.ReadInt32() != RenderWareSectionID.World)
                {
                    throw new Exception("*** Error: Expected World at position " + (Reader.Position() - 4));
                }

                renderWareStreamSections.Add(new World().Read(Reader));
            }
        }
    }
}