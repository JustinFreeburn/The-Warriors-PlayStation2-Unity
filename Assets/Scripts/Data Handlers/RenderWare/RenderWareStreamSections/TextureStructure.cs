using System;

namespace TheWarriors
{
    public class TextureStructure : RenderWareSection
    {
        public byte bFilterMode;

        public byte bAddressMode;

        public UInt16 uiUseMipLevels;

        public TextureStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.TextureNative;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            bFilterMode = reader.ReadByte();
            bAddressMode = reader.ReadByte();

            uiUseMipLevels = reader.ReadUInt16();

            return this;
        }
    }
}
