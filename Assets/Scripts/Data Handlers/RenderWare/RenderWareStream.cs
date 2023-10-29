using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{
    public class RenderWareStream
    {
        public List<RenderWareSection> RenderWareStreamSections = new List<RenderWareSection>();

        /// <summary>
        /// Stores a RenderWareSection list of all files that were identified and parsed.
        /// </summary>
        public RenderWareStream(RenderWareStreamFile renderWareStreamFile_)
        {
            if (renderWareStreamFile_.renderWareStreamFile == null)
            {
                return;
            }

            for (Int32 iIterator = 0; iIterator < renderWareStreamFile_.ArchiveEntries.Count; iIterator++)
            {
                switch ((RenderWareStreamFileID)renderWareStreamFile_.ArchiveEntries[iIterator].uiFileID)
                {
                    case (RenderWareStreamFileID.Texture):
                    case (RenderWareStreamFileID.Mesh):
                    case (RenderWareStreamFileID.World):
                        {
                            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(renderWareStreamFile_.GetStreamFile(iIterator)))
                            {
                                // NOTE: Ensure there is at least 12 bytes to be read.
                                while (Reader.Position() < (Reader.Length() - 12))
                                {
                                    RenderWareSectionID CurrentSection = (RenderWareSectionID)Reader.ReadInt32();

                                    if (CurrentSection == RenderWareSectionID.Clump)
                                    {
                                        RenderWareStreamSections.Add(new Clump().Read(Reader));
                                    }
                                    else if (CurrentSection == RenderWareSectionID.TextureDictionary)
                                    {
                                        RenderWareStreamSections.Add(new TextureDictionary().Read(Reader));
                                    }
                                    else if (CurrentSection == RenderWareSectionID.World)
                                    {
                                        RenderWareStreamSections.Add(new World().Read(Reader));
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        public RenderWareSection GetSection(Int32 iIndex_)
        {
            if (iIndex_ >= 0 && iIndex_ < RenderWareStreamSections.Count)
            {
                return RenderWareStreamSections[iIndex_];
            }

            Debug.Log("*** Error: RenderWareStream.GetStreamFile(" + iIndex_ + ") failed.");

            return null;
        }

        /// <summary>
        /// Returns an array of RenderWareSection by SectionID from the RenderWareStream.
        /// </summary>
        public RenderWareSection[] GetFilesByType(UInt32 uiSectionId_)
        {
            List<RenderWareSection> renderWareSections = new List<RenderWareSection>();

            for (Int32 iIterator = 0; iIterator < RenderWareStreamSections.Count; iIterator++)
            {
                if (RenderWareStreamSections[iIterator].SectionID == (RenderWareSectionID)uiSectionId_)
                {
                    renderWareSections.Add(renderWareSections[iIterator]);
                }
            }

            return renderWareSections.ToArray();
        }
    }
}