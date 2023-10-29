using System;

namespace TheWarriors
{
    public class TextureDictionaryStructure : RenderWareSection
    {
        public Int16 iTextureCount;

        public Int16 iUnknown;

        public TextureDictionaryStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iTextureCount = reader.ReadInt16();
            iUnknown = reader.ReadInt16();

            return this;
        }
    }
}
