namespace TheWarriors
{
    public class CollisionPlg : RenderWareSection
    {
        public CollisionPlg Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Atomic;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            reader.SeekCurrent(iSectionSize);

            return this;
        }
    }
}
