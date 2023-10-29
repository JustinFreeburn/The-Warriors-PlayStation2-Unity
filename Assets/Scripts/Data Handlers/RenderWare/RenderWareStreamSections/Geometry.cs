using System;

namespace TheWarriors
{
    public class Geometry : RenderWareSection
    {
        public GeometryStructure geometryStructure;

        public MaterialList materialList;

        public Extension geometryExtension;

        public Geometry Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Geometry;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: GeometryStructure
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected GeometryStructure at position " + (reader.Position() - 4));
                }

                geometryStructure = new GeometryStructure().Read(reader);
            }

            // NOTE: MaterialList
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.MaterialList)
                {
                    throw new Exception("*** Error: Expected MaterialList at position " + (reader.Position() - 4));
                }

                materialList = new MaterialList().Read(reader);
            }

            // NOTE: Extension
            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
            {
                throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
            }

            geometryExtension = new Extension().Read(reader);

            return this;
        }
    }
}