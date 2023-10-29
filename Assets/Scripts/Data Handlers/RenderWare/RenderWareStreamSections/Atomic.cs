using System;

namespace TheWarriors
{
    public class Atomic : RenderWareSection
    {
        public AtomicStructure atomicStructure;

        public Extension atomicExtension;

        public Geometry geometry = null;

        public Atomic Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Atomic;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: AtomicStructure
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected AtomicStructure at position " + (reader.Position() - 4));
                }

                atomicStructure = new AtomicStructure().Read(reader);
            }

            // NOTE: Geometry/Extension
            {
                RenderWareSectionID section = (RenderWareSectionID)reader.ReadInt32();

                if (section == RenderWareSectionID.Geometry)
                {
                    geometry = new Geometry().Read(reader);

                    if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
                    {
                        throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
                    }

                    atomicExtension = new Extension().Read(reader);
                }
                else if (section == RenderWareSectionID.Extension)
                {
                    atomicExtension = new Extension().Read(reader);
                }
                else
                {
                    throw new Exception("*** Error: Expected Geometry/Extension at position " + (reader.Position() - 4));
                }
            }

            return this;
        }
    }
}
