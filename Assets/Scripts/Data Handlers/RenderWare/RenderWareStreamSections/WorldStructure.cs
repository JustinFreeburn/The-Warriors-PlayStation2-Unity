using System;
using System.Numerics;

namespace TheWarriors
{
    public class WorldStructure : RenderWareSection
    {
        public Int32 iRootIsWorldSector;
        
        public Vector3 InverseOrigin;
        
        public Int32 iTriangleCount;
        
        public Int32 iVerticesCount;
        
        public Int32 iPlaneSectionCount;
        
        public Int32 iAtomicSectionCount;
        
        public Int32 iCollisionSectionSize;
        
        public WorldFlag WorldFlags;
        
        public Vector3 BoxMaximum;
        
        public Vector3 BoxMinimum;

        public WorldStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iRootIsWorldSector = reader.ReadInt32();

            InverseOrigin.X = reader.ReadSingle();
            InverseOrigin.Y = reader.ReadSingle();
            InverseOrigin.Z = reader.ReadSingle();

            iTriangleCount = reader.ReadInt32();
            iVerticesCount = reader.ReadInt32();
            iPlaneSectionCount = reader.ReadInt32();
            iAtomicSectionCount = reader.ReadInt32();
            iCollisionSectionSize = reader.ReadInt32();

            WorldFlags = (WorldFlag)reader.ReadUInt32();

            BoxMaximum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            BoxMinimum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return this;
        }
    }
}
