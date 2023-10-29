using System;

namespace TheWarriors
{
    public class GeometryListStructure : RenderWareSection
    {
        public Int32 iGeometryCount;

        public GeometryListStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iGeometryCount = reader.ReadInt32();

            return this;
        }
    }
}
