using System;

namespace TheWarriors
{
    public class GeometryStructure : RenderWareSection
    {
        public Int16 iGeometryFlags1;

        public Int16 iGeometryFlags2;

        public Int32 iTriangleCount;

        public Int32 iVertexCount;

        public Int32 iMorphTargetCount;

        public Int32 iTextureCoordinatesCount;

        public GeometryStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iGeometryFlags1 = reader.ReadInt16();
            iGeometryFlags2 = reader.ReadInt16();

            iTriangleCount = reader.ReadInt32();
            iVertexCount = reader.ReadInt32();

            // NOTE: As morphing is not used in The Warriors, this is always 1.
            iMorphTargetCount = reader.ReadInt32();

            //iTextureCoordinatesCount = (iGeometryFlags & 0x00FF0000) >> 16;

            // TODO: Read the following 24 bytes...
            reader.SeekCurrent(iSectionSize - 16);

            return this;
        }
    }
}
