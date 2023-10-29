using System;

namespace TheWarriors
{
    public class AtomicSection : RenderWareSection
    {
        public AtomicSectionStructure atomicSectionStructure;

        public Extension atomicExtension;

        public AtomicSection Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Atomic;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: AtomicSectionStructure
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected AtomicSectionStructure at position " + (reader.Position() - 4));
                }

                atomicSectionStructure = new AtomicSectionStructure().Read(reader);
            }

            // NOTE: Extension
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
                {
                    throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
                }

                atomicExtension = new Extension().Read(reader);
            }

            return this;
        }
    }
}
