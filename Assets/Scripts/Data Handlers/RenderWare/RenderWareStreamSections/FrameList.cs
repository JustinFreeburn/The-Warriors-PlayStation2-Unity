using System;
using System.Collections.Generic;

namespace TheWarriors
{
    public class FrameList : RenderWareSection
    {
        public FrameListStructure frameListStructure;

        public List<Extension> extensionList;

        public FrameList Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.FrameList;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected FrameListStructure at position " + (reader.Position() - 4));
            }

            frameListStructure = new FrameListStructure().Read(reader);

            extensionList = new List<Extension>();

            // NOTE: extension, extension???
            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
            {
                throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
            }

            // NOTE: skip size and version...
            reader.SeekCurrent(8);

            // NOTE: frameListStructure.Frames.Count is bone count + 1.
            // NOTE: the first HAnimPlg is for assembling the bones via a hierarchy.
            for (Int32 iIterator = 1; iIterator < frameListStructure.Frames.Count; iIterator++)
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
                {
                    throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
                }

                extensionList.Add(new Extension().Read(reader));
            }

            return this;
        }
    }
}
