namespace TheWarriors
{
    public class UserDataPlg : RenderWareSection
    {
        public UserDataPlg Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Atomic;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            reader.SeekCurrent(iSectionSize);

            return this;
        }
    }
}
