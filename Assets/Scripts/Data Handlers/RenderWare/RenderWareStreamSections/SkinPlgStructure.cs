using System;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public class SkinPlgStructure : RenderWareSection
    {
        public Int32 iPlatform;

        public Int32 iBoneCount;

        public Int32 iUsedBonesCount;

        public Int32 iMaxWeightsPerVertex;

        public byte[] bUsedBoneIds;

        public Matrix4x4[] InverseVertexToBoneMatrix;

        public Int32 iBoneLimit;

        public Int32 iGroupCount;

        public Int32 iRemapCount;

        public byte[] MeshBoneRemapIndices;

        /*
        public List<byte> boneRemapIndices = new List<byte>();

        public List<BoneGroup> boneGroups = new List<BoneGroup>();

        public List<BoneRemap> boneRemaps = new List<BoneRemap>();

        public struct BoneGroup
        {
            public Byte firstBone;

            public Byte numBones;
        }

        public struct BoneRemap
        {
            public Byte boneIndex;

            public Byte indices;
        }
        */

        public SkinPlgStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.SkinPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            long lStartPos = reader.Position();

            Int32 iPlatform = reader.ReadInt32();

            if ((Platform)iPlatform == Platform.Playstation2ClumpNative)
            {
                ReadPS2SkinPlg(reader);
            }
            else
            {
                throw new InvalidDataException("*** Error: SkinPlgStructure.Read() - unsupported platform " + iPlatform + ".");
            }

            long lLastPos = reader.Position();

            return this;
        }

        public void ReadPS2SkinPlg(ArchiveFileBinaryReader reader)
        {
            iBoneCount = reader.ReadByte();
            iUsedBonesCount = reader.ReadByte();
            iMaxWeightsPerVertex = reader.ReadByte();
            reader.SeekCurrent(1); // NOTE: Padding

            bUsedBoneIds = new byte[iUsedBonesCount];
            bUsedBoneIds = reader.ReadBytes(iUsedBonesCount);

            /*
            bUsedBoneIds = new byte[iUsedBonesCount];

            for (Int32 iIterator = 0; iIterator < iUsedBonesCount; iIterator++)
            {
                bUsedBoneIds[iIterator] = reader.ReadByte();
            }
            */

            InverseVertexToBoneMatrix = new Matrix4x4[iBoneCount];

            for (Int32 iIterator = 0; iIterator < iBoneCount; iIterator++)
            {
                Matrix4x4 inverseVertexToBoneMatrix = new Matrix4x4();

                inverseVertexToBoneMatrix.m00 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m01 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m02 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m03 = reader.ReadSingle();

                inverseVertexToBoneMatrix.m10 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m11 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m12 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m13 = reader.ReadSingle();
                
                inverseVertexToBoneMatrix.m20 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m21 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m22 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m23 = reader.ReadSingle();

                inverseVertexToBoneMatrix.m30 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m31 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m32 = reader.ReadSingle();
                inverseVertexToBoneMatrix.m33 = reader.ReadSingle();

                InverseVertexToBoneMatrix[iIterator] = inverseVertexToBoneMatrix;
            }

            // TODO: Discover this 28 bytes???
            reader.SeekCurrent(28);

            /*
            UInt32 boneLimit = reader.ReadByte();

            UInt32 meshCount = reader.ReadByte();

            UInt32 RLE = reader.ReadByte();

            if (meshCount > 0)
            {
                MeshBoneRemapIndices = reader.ReadBytes((Int32)(iBoneCount + 2 * (RLE + meshCount)));
            }
            */

            /*
            iBoneLimit = reader.ReadByte();
            iGroupCount = reader.ReadByte();
            iRemapCount = reader.ReadByte(); // NOTE: iRLECount

            if (iGroupCount > 0)
            {
                // boneRemapIndices
                for (Int32 iIterator = 0; iIterator < iBoneLimit; iIterator++)
                {
                    boneRemapIndices.Add(reader.ReadByte());
                }

                // boneGroups
                for (Int32 iIterator = 0; iIterator < iGroupCount; iIterator++)
                {
                    boneRemapIndices.Add(reader.ReadByte());
                }

                // boneRemaps
                for (Int32 iIterator = 0; iIterator < iRemapCount; iIterator++)
                {
                    boneRemapIndices.Add(reader.ReadByte());
                }
            }
            */
        }
    }
}
