using UnityEngine;

namespace TheWarriors
{
    public class RenderWareAnimation : MonoBehaviour
    {
        /*
        public class Clip
        {
            public readonly string Name;
            
            public readonly Int32 BoneCount;
        
            public readonly Int32 FrameLength;
        
            public readonly Int32 Unknown;
        
            public readonly Bone[] Bones;
        
            public readonly int EndTime;

            public Clip(BinaryReader reader)
            {
                Name = reader.ReadString(24);
                BoneCount = reader.ReadInt32();
                FrameLength = reader.ReadInt32();
                Unknown = reader.ReadInt32();

                Bones = new Bone[BoneCount];

                for (int i = 0; i < BoneCount; ++i)
                {
                    Bones[i] = new Bone(reader);
                }

                if (BoneCount > 0)
                {
                    EndTime = Bones.Max(x => x.EndTime);
                }
            }
        }

        public class Bone
        {
            public readonly string Name;
            
            public readonly Int32 FrameType;
        
            public readonly Int32 FrameCount;
        
            public readonly Int32 BoneId;
        
            public readonly Frame[] Frames;
        
            public readonly int EndTime;

            public Bone(BinaryReader reader)
            {
                Name = reader.ReadString(24);
                FrameType = reader.ReadInt32();
                FrameCount = reader.ReadInt32();
                BoneId = reader.ReadInt32();

                Frames = new Frame[FrameCount];

                for (int i = 0; i < FrameCount; ++i)
                {
                    Frames[i] = new Frame(reader, FrameType == 4);
                }

                if (FrameCount > 0)
                {
                    EndTime = Frames[FrameCount - 1].Time;
                }
            }
        }

        public class Frame
        {
            public readonly Vector3 Translation;

            public readonly Quaternion Rotation;

            public readonly Int16 Time;

            public Frame(BinaryReader reader, bool root)
            {
                Rotation = new Quaternion(reader, QuaternionCompression.Animation);

                Time = reader.ReadInt16();

                if (root)
                {
                    Translation = new Vector3(reader, VectorCompression.Animation);
                }
            }
        }
        */
    }
}
