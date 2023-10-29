using System;
using System.Collections.Generic;

namespace TheWarriors
{
    public class Clump : RenderWareSection
    {
        public ClumpStructure clumpStructure;

        public FrameList frameList;

        public GeometryList geometryList;

        public List<Atomic> atomicList;

        public Extension clumpExtension;

        public Clump Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Clump;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: ClumpStructure
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected ClumpStructure at position " + (reader.Position() - 4));
                }

                clumpStructure = new ClumpStructure().Read(reader);
            }

            // NOTE: FrameList
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.FrameList)
                {
                    throw new Exception("*** Error: Expected FrameList at position " + (reader.Position() - 4));
                }

                frameList = new FrameList().Read(reader);
            }

            // NOTE: GeometryList
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.GeometryList)
                {
                    throw new Exception("*** Error: Expected GeometryList at position " + (reader.Position() - 4));
                }

                geometryList = new GeometryList().Read(reader, frameList);
            }

            // NOTE: Atomic
            {
                atomicList = new List<Atomic>();

                for (Int32 iIterator = 0; iIterator < clumpStructure.iAtomicCount; iIterator++)
                {
                    if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Atomic)
                    {
                        throw new Exception("*** Error: Expected Atomic at position " + (reader.Position() - 4));
                    }

                    atomicList.Add(new Atomic().Read(reader));
                }
            }

            // NOTE: Extension
            {
                long lStartSectionPosition = reader.Position();

                if ((RenderWareSectionID)reader.ReadInt32() == RenderWareSectionID.Extension)
                {
                    clumpExtension = new Extension().Read(reader);
                }
                else
                {
                    reader.SeekBeginning(lStartSectionPosition);
                }
            }

            return this;
        }
    }
}
