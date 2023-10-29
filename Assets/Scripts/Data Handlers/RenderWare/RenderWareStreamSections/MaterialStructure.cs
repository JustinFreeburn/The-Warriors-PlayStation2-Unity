using System;

namespace TheWarriors
{
    public class MaterialStructure : RenderWareSection
    {
        public Int32 iInteger1;

        public byte bColourR;

        public byte bColourG;

        public byte bColourB;

        public byte bColourA;

        public Int32 iInteger2;

        public Int32 iIsTextured;

        public float fAmbient;

        public float fSpecular;

        public float fDiffuse;

        public MaterialStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iInteger1 = reader.ReadInt32();

            bColourR = reader.ReadByte();
            bColourG = reader.ReadByte();
            bColourB = reader.ReadByte();
            bColourA = reader.ReadByte();

            iInteger2 = reader.ReadInt32();

            iIsTextured = reader.ReadInt32();

            fAmbient = reader.ReadSingle();
            fSpecular = reader.ReadSingle();
            fDiffuse = reader.ReadSingle();

            return this;
        }
    }
}
