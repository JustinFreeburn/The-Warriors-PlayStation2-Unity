using System;
using System.Collections.Generic;

namespace TheWarriors
{
    public class BinMeshPlg : RenderWareSection
    {
        public class BinMesh
        {
            public Int32 iIndexCount;

            public Int32 iMaterialIndex;

            public List<Int32> VertexIndices;
        }

        public Int32 iFaceType;

        public Int32 iMaterialSplitCount;

        public Int32 iTotalFaceCount;

        public List<BinMesh> binMeshes;

        public BinMeshPlg Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.BinMeshPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if (iSectionSize == 12)
            {
                reader.SeekCurrent(iSectionSize);

                return this;
            }

            // 1 = triangle strip, ???
            iFaceType = reader.ReadInt32();
            iMaterialSplitCount = reader.ReadInt32();
            iTotalFaceCount = reader.ReadInt32();

            binMeshes = new List<BinMesh>();

            for (Int32 iIterator = 0; iIterator < iMaterialSplitCount; iIterator++)
            {
                BinMesh binMesh = new BinMesh();

                binMesh.iIndexCount = reader.ReadInt32();
                binMesh.iMaterialIndex = reader.ReadInt32();

                // NOTE: I don't believe this condition is met with sector files???
                if (iSectionSize != (12 + (8 * iMaterialSplitCount)))
                {
                    binMesh.VertexIndices = new List<Int32>();

                    for (Int32 iNestedIterator = 0; iNestedIterator < binMesh.iIndexCount; iNestedIterator++)
                    {
                        binMesh.VertexIndices.Add(reader.ReadInt32());
                    }
                }

                binMeshes.Add(binMesh);
            }

            return this;
        }
    }
}
