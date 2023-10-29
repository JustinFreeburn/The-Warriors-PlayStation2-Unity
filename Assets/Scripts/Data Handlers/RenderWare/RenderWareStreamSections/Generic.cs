namespace TheWarriors
{
    public class Generic : RenderWareSection
    {
        public Generic Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.None;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            reader.SeekCurrent(iSectionSize);

            return this;
        }
    }
}
