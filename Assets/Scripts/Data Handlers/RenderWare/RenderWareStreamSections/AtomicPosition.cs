using System;

namespace TheWarriors
{
    public class AtomicPosition : RenderWareSection
    {
        public UInt32 uiAtomicID;

        public UInt32 uiUnknown;

        public float fPositionX;

        public float fPositionY;

        public float fPositionZ;

        public AtomicPosition Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.RockstarGamesCustom2;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            uiAtomicID = reader.ReadUInt32();
            uiUnknown = reader.ReadUInt32();
            fPositionX = reader.ReadSingle();
            fPositionY = reader.ReadSingle();
            fPositionZ = reader.ReadSingle();

            return this;
        }
    }
}
