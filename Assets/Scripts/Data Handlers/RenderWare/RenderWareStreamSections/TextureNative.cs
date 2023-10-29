using System;
using UnityEngine;

namespace TheWarriors
{
    public class TextureNative : RenderWareSection
    {
        public TextureNativeStructure textureNativeStructure;

        public Extension textureNativeExtension;

        public TextureNative Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.TextureNative;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
            {
                throw new Exception("*** Error: Expected TextureNativeStructure at position " + (reader.Position() - 4));
            }

            textureNativeStructure = new TextureNativeStructure().Read(reader);

            long lReaderOldPosition = reader.Position();

            if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
            {
                //throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
                Debug.Log("Warning: : Expected Extension at position " + (reader.Position() - 4));

                reader.SeekCurrent(lReaderOldPosition);
            }
            else
            {
                textureNativeExtension = new Extension().Read(reader);
            }

            return this;
        }
    }
}
