namespace TheWarriors
{
    public class AtomicScale : RenderWareSection
    {
        public float fVertexScale;

        public float fUVScale;

        public float fUnknownScale;

        public AtomicScale Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.RockstarGamesCustom1;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            fVertexScale = reader.ReadSingle();
            fUVScale = reader.ReadSingle();
            fUnknownScale = reader.ReadSingle();

            return this;
        }
    }
}
