using System;
using System.Collections.Generic;

namespace TheWarriors
{
    public class GeometryList : RenderWareSection
    {
        public GeometryListStructure geometryListStructure;

        public List<Geometry> geometryList;

        public GeometryList Read(ArchiveFileBinaryReader reader, FrameList frameList_)
        {
            SectionID = RenderWareSectionID.GeometryList;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected GeometryListStructure at position " + (reader.Position() - 4));
            }

            geometryListStructure = new GeometryListStructure().Read(reader);

            geometryList = new List<Geometry>();

            for (Int32 iIterator = 0; iIterator < geometryListStructure.iGeometryCount; iIterator++)
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Geometry)
                {
                    throw new Exception("*** Error: Expected Geometry at position " + (reader.Position() - 4));
                }

                geometryList.Add(new Geometry().Read(reader));
            }

            return this;
        }
    }
}
