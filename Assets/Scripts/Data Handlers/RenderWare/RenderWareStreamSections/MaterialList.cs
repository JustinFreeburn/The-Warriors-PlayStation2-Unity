using System;
using System.Collections.Generic;

namespace TheWarriors
{
    public class MaterialList : RenderWareSection
    {
        public MaterialListStructure materialListStructure;

        public List<Material> materialList;

        public MaterialList Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.MaterialList;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected MaterialListStructure at position " + (reader.Position() - 4));
            }

            materialListStructure = new MaterialListStructure().Read(reader);

            materialList = new List<Material>();

            for (Int32 iIterator = 0; iIterator < materialListStructure.iMaterialCount; iIterator++)
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Material)
                {
                    throw new Exception("*** Error: Expected Material at position " + (reader.Position() - 4));
                }

                materialList.Add(new Material().Read(reader));
            }

            return this;
        }
    }
}
