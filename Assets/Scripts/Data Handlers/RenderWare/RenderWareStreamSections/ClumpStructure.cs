using System;

namespace TheWarriors
{
    public class ClumpStructure : RenderWareSection
    {
        public Int32 iAtomicCount;

        public Int32 iLightCount;

        public Int32 iCameraCount;

        public ClumpStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iAtomicCount = reader.ReadInt32();
            iLightCount = reader.ReadInt32();
            iCameraCount = reader.ReadInt32();

            return this;
        }
    }
}
