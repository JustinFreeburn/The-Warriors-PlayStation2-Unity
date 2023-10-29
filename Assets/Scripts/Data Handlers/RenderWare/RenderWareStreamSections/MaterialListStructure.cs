using System;

namespace TheWarriors
{
    public class MaterialListStructure : RenderWareSection
    {
        public Int32 iMaterialCount;

        public MaterialListStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iMaterialCount = reader.ReadInt32();

            for (Int32 iIterator = 0; iIterator < iMaterialCount; iIterator++)
            {
                reader.ReadInt32();
            }

            return this;
        }
    }
}
