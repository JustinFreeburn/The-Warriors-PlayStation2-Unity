using System;

namespace TheWarriors
{
    public class PlaneSectionStructure : RenderWareSection
    {
        public Int32 iType;

        public float fValue;

        public Int32 iLeftIsAtomic;

        public Int32 iRightIsAtomic;

        public float fLeftValue;

        public float fRightValue;

        public PlaneSectionStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iType = reader.ReadInt32();
            fValue = reader.ReadSingle();

            iLeftIsAtomic = reader.ReadInt32();
            iRightIsAtomic = reader.ReadInt32();

            fLeftValue = reader.ReadSingle();
            fRightValue = reader.ReadSingle();

            return this;
        }
    }
}
