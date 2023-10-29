using System;
using System.Numerics;

namespace TheWarriors
{
    public class AtomicSectionStructure : RenderWareSection
    {
        public Int32 iMaterialListWindowBase;

        public Int32 iTriangleCount;

        public Int32 iVerteciesCount;

        public Vector3 BoxMaximum;

        public Vector3 BoxMinimum;

        public Int32 iCollisionSectionPresent;

        public Int32 iUnknown;

        public AtomicSectionStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iMaterialListWindowBase = reader.ReadInt32();
            iTriangleCount = reader.ReadInt32();
            iVerteciesCount = reader.ReadInt32();

            BoxMaximum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            BoxMinimum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            iCollisionSectionPresent = reader.ReadInt32();
            iUnknown = reader.ReadInt32();

            return this;
        }
    }
}
