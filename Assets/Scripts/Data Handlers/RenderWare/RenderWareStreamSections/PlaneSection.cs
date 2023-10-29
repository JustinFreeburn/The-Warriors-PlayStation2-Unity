using System;

namespace TheWarriors
{
    public class PlaneSection : RenderWareSection
    {
        public PlaneSectionStructure planeSectionStructure;

        public RenderWareSection leftSection;

        public RenderWareSection rightSection;

        public PlaneSection Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Atomic;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: PlaneSectionStructure
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected PlaneSectionStructure at position " + (reader.Position() - 4));
                }

                planeSectionStructure = new PlaneSectionStructure().Read(reader);
            }

            // NOTE: Left AtomicSection/PlaneSection
            {
                RenderWareSectionID leftSectionSection = (RenderWareSectionID)reader.ReadInt32();

                if (leftSectionSection == RenderWareSectionID.AtomicSection && planeSectionStructure.iLeftIsAtomic == 1)
                {
                    leftSection = new AtomicSection().Read(reader);
                }
                else if (leftSectionSection == RenderWareSectionID.PlaneSection && planeSectionStructure.iLeftIsAtomic == 0)
                {
                    leftSection = new PlaneSection().Read(reader);
                }
                else
                {
                    throw new Exception("*** Error: Expected AtomicSection/PlaneSection at position " + (reader.Position() - 4));
                }
            }

            // NOTE: Right AtomicSection/PlaneSection
            {
                RenderWareSectionID section = (RenderWareSectionID)reader.ReadInt32();

                if (section == RenderWareSectionID.AtomicSection && planeSectionStructure.iRightIsAtomic == 1)
                {
                    rightSection = new AtomicSection().Read(reader);
                }
                else if (section == RenderWareSectionID.PlaneSection && planeSectionStructure.iRightIsAtomic == 0)
                {
                    rightSection = new PlaneSection().Read(reader);
                }
                else
                {
                    throw new Exception("*** Error: Expected AtomicSection/PlaneSection at position " + (reader.Position() - 4));
                }
            }

            return this;
        }
    }
}
