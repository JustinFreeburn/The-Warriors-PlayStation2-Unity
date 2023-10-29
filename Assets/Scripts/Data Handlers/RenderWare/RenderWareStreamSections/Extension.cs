using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{
    public class Extension : RenderWareSection
    {
        public List<RenderWareSection> extensionSectionList;

        public Extension Read(ArchiveFileBinaryReader reader)
        {
            SectionID = RenderWareSectionID.Extension;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            // NOTE: skip padding for last Extension... but only if there is data left to skip!
            if (iSectionSize == 0)
            {
                // TODO: Check that the SeekCurrent amount doesn't exceed the files length!
                //if ((reader.Position() % 16) != 0)
                //{
                //    reader.SeekCurrent((16 - (reader.Position() % 16)));
                //}

                return this;
            }

            long lSectionEnd = reader.Position() + iSectionSize;

            extensionSectionList = new List<RenderWareSection>();

            while (reader.Position() < lSectionEnd)
            {
                RenderWareSectionID currentSection = (RenderWareSectionID)reader.ReadInt32();

                if (currentSection == RenderWareSectionID.CollisionPLG)
                {
                    extensionSectionList.Add(new CollisionPlg().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.HAnimPLG)
                {
                    extensionSectionList.Add(new HAnimPlg().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.BinMeshPLG)
                {
                    extensionSectionList.Add(new BinMeshPlg().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.NativeDataPLG)
                {
                    BinMeshPlg binMeshPlg = null;
                    Int32 iBinMeshPlgCount = 0;

                    for (Int32 iIterator = 0; iIterator < extensionSectionList.Count; iIterator++)
                    {
                        if (extensionSectionList[iIterator] is BinMeshPlg)
                        {
                            binMeshPlg = (BinMeshPlg)extensionSectionList[iIterator];

                            iBinMeshPlgCount++;
                        }
                    }

                    if (binMeshPlg != null)
                    {
                        extensionSectionList.Add(new NativeDataPlg().Read(reader, binMeshPlg));
                    }

                    if (iBinMeshPlgCount == 0)
                    {
                        Debug.Log("*** Error: NativeDataPlg().Read failed. No BinMeshPlg found in extensionSectionList!");
                    }
                    else if (iBinMeshPlgCount > 1)
                    {
                        Debug.Log("*** Error: " + iBinMeshPlgCount + " BinMeshPlg's found in extensionSectionList! Expected 1 only. Used last instance.");
                    }
                }
                else if (currentSection == RenderWareSectionID.SkinPLG)
                {
                    extensionSectionList.Add(new SkinPlg().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.UserDataPLG)
                {
                    extensionSectionList.Add(new UserDataPlg().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.SkyMipmapVal)
                {
                    extensionSectionList.Add(new Generic().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.RightToRender)
                {
                    extensionSectionList.Add(new RightToRender().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.MaterialEffectsPLG)
                {
                    extensionSectionList.Add(new Generic().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.RockstarGamesCustom1)
                {
                    // NOTE: RockstarGames custom section - scale!
                    extensionSectionList.Add(new AtomicScale().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.RockstarGamesCustom2)
                {
                    // NOTE: RockstarGames custom section - position!
                    extensionSectionList.Add(new AtomicPosition().Read(reader));
                }
                else if (currentSection == RenderWareSectionID.GeometricPVSPlg)
                {
                    // TODO: Discover this section.
                    extensionSectionList.Add(new Generic().Read(reader));
                }
                else
                {
                    Debug.Log("*** Error: Unknown extension type \"" + currentSection.ToString() + "\" at " + (reader.Position() - 4) + "!");

                    extensionSectionList.Add(new Generic().Read(reader));
                }
            }

            return this;
        }
    }
}