namespace TheWarriors
{
    public class LevelCollisionMesh : RenderWareSection
    {
        public LevelCollisionMesh Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.CollisionPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            return null;
        }
    }
}