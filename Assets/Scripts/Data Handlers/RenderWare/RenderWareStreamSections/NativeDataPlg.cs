using System;

namespace TheWarriors
{
    public class NativeDataPlg : RenderWareSection
    {
        public NativeDataPlgStructure nativeDataPlgStructure;

        public NativeDataPlg Read(ArchiveFileBinaryReader reader, BinMeshPlg binMeshPlg_)
        {
            SectionID = RenderWareSectionID.NativeDataPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected NativeDataPlgStructure at position " + (reader.Position() - 4));
            }

            nativeDataPlgStructure = new NativeDataPlgStructure().Read(reader, binMeshPlg_);

            return this;
        }
    }
}
