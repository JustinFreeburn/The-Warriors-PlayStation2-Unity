using System;

namespace TheWarriors
{
    public class AtomicStructure : RenderWareSection
    {
        public Int32 iFrameIndex;

        public Int32 iGeometryIndex;

        public Int32 iUnknown1;

        public Int32 iUnknown2;

        public AtomicStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iFrameIndex = reader.ReadInt32();
            iGeometryIndex = reader.ReadInt32();
            iUnknown1 = reader.ReadInt32();
            iUnknown2 = reader.ReadInt32();

            return this;
        }
    }
}
