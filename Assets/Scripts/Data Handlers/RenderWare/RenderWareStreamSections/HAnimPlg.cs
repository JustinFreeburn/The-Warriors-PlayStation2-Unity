using System;
using System.Collections.Generic;

namespace TheWarriors
{
    public struct Bone
    {
        public Int32 iId;

        public Int32 iIndex;

        public Int32 iFlags;
    }

    public class HAnimPlg : RenderWareSection
    {
        public List<Bone> Bones;

        public Int32 iConstant;

        public Int32 iId;

        public Int32 iBoneCount;

        // NOTE: Only read if iBoneCount > 0
        public Int32 iFlags;

        public Int32 iKeyFrameSize;

        public HAnimPlg Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.HAnimPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iConstant = reader.ReadInt32(); // NOTE: Version?
            iId = reader.ReadInt32();
            iBoneCount = reader.ReadInt32();

            if ((iBoneCount > 0) /*&& (iId == 0)*/)
            {
                Bones = new List<Bone>();

                iFlags = reader.ReadInt32();

                // NOTE: Size of data (in bytes) needed for one animaton frame (in GTA this parameter is equal to 36, in The Warriors it is also equal to 36)
                iKeyFrameSize = reader.ReadInt32();

                for (Int32 iIterator = 0; iIterator < iBoneCount; iIterator++)
                {
                    Bone bone = new Bone();

                    bone.iId = reader.ReadInt32();
                    bone.iIndex = reader.ReadInt32();
                    
                    // 0 NONE
                    // 1 SUBHIERARCHY - hierarchy inherits from another hierarchy (Suspect the skybox, sky and shadow are 1's. Shows in debug log as 1s.)
                    // 2 NOMATRICES - hierarchy doesn't use local matrices for bones
                    // 4096 UPDATEMODELLINGMATRICES - update local matrices for bones
                    // 8192 UPDATELTMS - recalculate global matrices for bones ... update global matrices
                    // 16384 LOCALSPACEMATRICES - hierarchy computes matrices in the local space
                    bone.iFlags = reader.ReadInt32();

                    Bones.Add(bone);
                }
            }

            return this;
        }
    }
}
