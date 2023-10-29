using System;

namespace TheWarriors
{
    public class Texture : RenderWareSection
    {
        TextureStructure textureStructure;

        Extension textureExtension;

        public String sDiffuseTextureName;

        public String sAlphaTextureName;

        public Texture Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.TextureNative;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected TextureStructure at position " + (reader.Position() - 4));
            }

            textureStructure = new TextureStructure().Read(reader);

            Int32 iStringLength = 0;

            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.String)
                {
                    throw new Exception("*** Error: Expected String at position " + (reader.Position() - 4));
                }

                iStringLength = reader.ReadInt32();
                reader.SeekCurrent(4);

                sDiffuseTextureName = new String(reader.ReadChars(iStringLength));
                sDiffuseTextureName = sDiffuseTextureName.Replace("\0", "");
            }

            iStringLength = 0;

            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.String)
                {
                    throw new Exception("*** Error: Expected String at position " + (reader.Position() - 4));
                }

                iStringLength = reader.ReadInt32();
                reader.SeekCurrent(4);

                sAlphaTextureName = new String(reader.ReadChars(iStringLength));
                sAlphaTextureName = sAlphaTextureName.Replace("\0", "");
            }

            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
                {
                    throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
                }

                textureExtension = new Extension().Read(reader);
            }

            return this;
        }
    }
}
