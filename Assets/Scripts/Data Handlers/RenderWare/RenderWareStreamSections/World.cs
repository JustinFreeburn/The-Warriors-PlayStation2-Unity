using System;

namespace TheWarriors
{
    public class World : RenderWareSection
    {
        public WorldStructure worldStructure;
        
        public MaterialList materialList;
        
        public RenderWareSection firstWorldChunk;
        
        public Extension extension;

        public World Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.World;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: WorldStructure
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Struct)
                {
                    throw new Exception("*** Error: Expected WorldStructure at position " + (reader.Position() - 4));
                }

                worldStructure = new WorldStructure().Read(reader);
            }

            // NOTE: MaterialList
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.MaterialList)
                {
                    throw new Exception("*** Error: Expected MaterialList at position " + (reader.Position() - 4));
                }

                materialList = new MaterialList().Read(reader);
            }

            // NOTE: PlaneSection/AtomicSection
            if (worldStructure.iAtomicSectionCount == 1 && worldStructure.iPlaneSectionCount == 0)
            {
                RenderWareSectionID section = (RenderWareSectionID)reader.ReadInt32();

                if (section == RenderWareSectionID.AtomicSection)
                {
                    // NOTE: There is no FrameList or GeometryStructure in a World section.
                    firstWorldChunk = new AtomicSection().Read(reader);
                }
                else
                {
                    throw new Exception("*** Error: Expected AtomicSection at position " + (reader.Position() - 4));
                }
            }
            else
            {
                // NOTE: AtomicSection/PlaneSection
                RenderWareSectionID section = (RenderWareSectionID)reader.ReadInt32();

                if (section == RenderWareSectionID.AtomicSection)
                {
                    firstWorldChunk = new AtomicSection().Read(reader);
                }
                else if (section == RenderWareSectionID.PlaneSection)
                {
                    firstWorldChunk = new PlaneSection().Read(reader);
                }
                else
                {
                    throw new Exception("*** Error: Expected AtomicSection/PlaneSection at position " + (reader.Position() - 4));
                }
            }

            // NOTE: Extension
            {
                if ((RenderWareSectionID)reader.ReadInt32() != RenderWareSectionID.Extension)
                {
                    throw new Exception("*** Error: Expected Extension at position " + (reader.Position() - 4));
                }

                // NOTE: This final Extension is causing the program to crash. There is 0 bytes in the Extension section here.
                long lStartPosition = reader.Position();

                UInt32 uiExtensionSize = reader.ReadUInt32();

                reader.SeekBeginning(lStartPosition);

                if (uiExtensionSize == 0)
                {
                    reader.SeekCurrent(8);
                }
                else
                {
                    //throw new Exception("*** Error: Unexpected Extension size at position " + (reader.Position() - 4));

                    // NOTE: File 0x9F0A3DC5 (8014 - world file) has GeometryicPVS PLG in the extension section!
                    extension = new Extension().Read(reader);
                }
            }

            return this;
        }
    }
}