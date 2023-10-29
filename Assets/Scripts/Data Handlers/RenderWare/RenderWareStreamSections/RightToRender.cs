using System;

namespace TheWarriors
{
    public class RightToRender : RenderWareSection
    {
        public UInt32 uiPluginIdentifier;

        public UInt32 uiExtraData;

        public RightToRender Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.RightToRender;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            uiPluginIdentifier = reader.ReadUInt32();
            uiExtraData = reader.ReadUInt32();

            return this;
        }
    }
}
