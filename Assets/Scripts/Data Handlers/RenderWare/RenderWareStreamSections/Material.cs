using System;

namespace TheWarriors
{
    public class Material : RenderWareSection
    {
        public MaterialStructure materialStructure;

        public Texture texture = null;

        public Extension materialExtension;

        public Material Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Material;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected MaterialStructure at position " + (reader.Position() - 4));
            }

            materialStructure = new MaterialStructure().Read(reader);

            // NOTE: This is a boolean, not a texture count.
            if (materialStructure.iIsTextured != 0)
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Texture)
                {
                    throw new Exception("*** Error: Expected RWTexture at position " + (reader.Position() - 4));
                }

                texture = new Texture().Read(reader);
            }

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
            {
                throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
            }

            materialExtension = new Extension().Read(reader);

            return this;
        }
    }
}
