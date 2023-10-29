using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{
    public class RenderWareWorld
    {
        /// 1 x TextureDictionary.
        /// 1 x World.

        public Dictionary<UInt32, Vector3> atomicPositions = new Dictionary<UInt32, Vector3>();

        public RenderWareWorldFile renderWareWorldFile = null;

        /// <summary>
        /// 2 sections always. This class doesn't render anything. It contains texture data and PlaneSection/AtomicSection metadata for the sector meshes.
        /// </summary>
        public RenderWareWorld(UInt32 uiFileHash_)
        {
            renderWareWorldFile = new RenderWareWorldFile(RockstarArchiveManager.GetWadArchiveFile(uiFileHash_));

            UnityTextureManager.LoadTexturesFromRenderWareSections(renderWareWorldFile.renderWareStreamSections.ToArray());

            foreach (RenderWareSection section in renderWareWorldFile.renderWareStreamSections)
            {
                if (section is World world)
                {
                    if (world.firstWorldChunk is AtomicSection atomicSection)
                    {
                        AtomicToScene(atomicSection);
                    }
                    else if (world.firstWorldChunk is PlaneSection planeSection)
                    {
                        PlaneToScene(planeSection);
                    }
                }
            }
        }

        private void PlaneToScene(PlaneSection planeSection)
        {
            if (planeSection.leftSection is AtomicSection a1)
            {
                AtomicToScene(a1);
            }
            else if (planeSection.leftSection is PlaneSection p1)
            {
                PlaneToScene(p1);
            }

            if (planeSection.rightSection is AtomicSection a2)
            {
                AtomicToScene(a2);
            }
            else if (planeSection.rightSection is PlaneSection p2)
            {
                PlaneToScene(p2);
            }
        }

        private void AtomicToScene(AtomicSection atomicSection)
        {
            foreach (RenderWareSection section in atomicSection.atomicExtension.extensionSectionList)
            {
                if (section is AtomicPosition atomicPosition)
                {
                    if (atomicPosition.uiAtomicID != 0xFFFFFFFF && atomicPositions.ContainsKey(atomicPosition.uiAtomicID) == false)
                    {
                        atomicPositions.Add(atomicPosition.uiAtomicID, new Vector3(atomicPosition.fPositionX, atomicPosition.fPositionY, atomicPosition.fPositionZ));
                    }
                }
            }
        }
    }
}
