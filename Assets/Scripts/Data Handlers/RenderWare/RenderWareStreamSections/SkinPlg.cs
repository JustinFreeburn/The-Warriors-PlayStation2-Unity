using System;

namespace TheWarriors
{
    public class SkinPlg : RenderWareSection
    {
        public SkinPlgStructure nativeDataPlgStructure;

        public SkinPlg Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.SkinPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected SkinPlgStructure at position " + (reader.Position() - 4));
            }

            nativeDataPlgStructure = new SkinPlgStructure().Read(reader);

            return this;
        }
    }
}
