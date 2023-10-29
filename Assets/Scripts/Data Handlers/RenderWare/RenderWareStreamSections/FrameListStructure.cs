using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{
    public struct Frame
    {
        public Vector3 MatrixRight;

        public Vector3 MatrixForward;

        public Vector3 MatrixUp;

        public Vector3 Position;

        public Int32 iParent;

        public Int16 iUnknown1;

        public Int16 iUnknown2;
    }

    public class FrameListStructure : RenderWareSection
    {
        public List<Frame> Frames;

        public Int32 iFrameCount;

        public FrameListStructure Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Struct;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            iFrameCount = reader.ReadInt32();

            Frames = new List<Frame>();

            for (Int32 iIterator = 0; iIterator < iFrameCount; iIterator++)
            {
                Frame frame = new Frame();

                frame.MatrixRight.x = reader.ReadSingle();
                frame.MatrixRight.y = reader.ReadSingle();
                frame.MatrixRight.z = reader.ReadSingle();

                frame.MatrixUp.x = reader.ReadSingle();
                frame.MatrixUp.y = reader.ReadSingle();
                frame.MatrixUp.z = reader.ReadSingle();

                frame.MatrixForward.x = reader.ReadSingle();
                frame.MatrixForward.y = reader.ReadSingle();
                frame.MatrixForward.z = reader.ReadSingle();

                frame.Position.x = reader.ReadSingle();
                frame.Position.y = reader.ReadSingle();
                frame.Position.z = reader.ReadSingle();

                frame.iParent = reader.ReadInt32();

                frame.iUnknown1 = reader.ReadInt16();
                frame.iUnknown2 = reader.ReadInt16();

                Frames.Add(frame);
            }

            return this;
        }
    }
}
