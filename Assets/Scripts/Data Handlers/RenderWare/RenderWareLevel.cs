using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public class RenderWareLevel
    {
        /// LEV files are RWS files... Use the RenderWareStreamFile and RenderWareStream classes to parse a LEV file.
        /// 1 x 0x53 ???
        /// 1 x TextureDictionary
        /// 1 x Clump
        /// 1 x TextureDictionary
        /// 1 x Clump
        /// 1 x TextureDictionary
        /// 1 x Clump
        /// 1 x TextureDictionary
        /// 1 x World (very different World section to the other known World section)
        /// 1 x 0x40 ???
        /// 1 x 0x52 ???
        /// 1 x 0x07 Collision mesh vertices
        /// 1 x 0x04 Collision mesh indicies
        /// 1 x 0x05 ???
        /// 1 x 0x06 ???
        /// 1 x 0x03 ???
        /// 1 x 0x17 ???
        /// 1 x 0x51 ???

        public RenderWareStreamFile renderWareStreamFile = null;

        public RenderWareStream renderWareStream = null;

        private List<String> textureDiffuseNames;

        /// <summary>
        /// 18 files always. It is a RenderWareStreamFile. It contains the sky, skybox and terrain models and textures. It possibly contains the collision mesh in the World section???
        /// </summary>
        public RenderWareLevel(UInt32 uiFileHash_)
        {
            renderWareStreamFile = new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(uiFileHash_));

            if (renderWareStreamFile != null)
            {
                renderWareStream = new RenderWareStream(renderWareStreamFile);

                if (renderWareStream != null)
                {
                    UnityTextureManager.LoadTexturesFromRenderWareSections(renderWareStream.RenderWareStreamSections.ToArray());

                    textureDiffuseNames = new List<String>();

                    foreach (RenderWareSection renderWareSection in renderWareStream.RenderWareStreamSections)
                    {
                        if (renderWareSection is TextureDictionary textureDictionary)
                        {
                            foreach (TextureNative textureNative in textureDictionary.textureNativeList)
                            {
                                textureDiffuseNames.Add(textureNative.textureNativeStructure.sTextureName);
                            }
                        }
                    }
                }
            }
        }

        public GameObject CreateCollisionObject()
        {
            if (renderWareStreamFile == null)
            {
                return null;
            }

            Stream LevelCollisionMeshFile = renderWareStreamFile.GetStreamFileWithHeader(0x00000007, 0x00000000);
            Stream LevelCollisionMeshIndicesFile = renderWareStreamFile.GetStreamFileWithHeader(0x00000004, 0x00000000);

            if (LevelCollisionMeshFile == null || LevelCollisionMeshIndicesFile == null)
            {
                return null;
            }

            Vector3[] Vertices = null;
            List<Int32> Triangles = null;

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(LevelCollisionMeshFile))
            {
                if (Reader.ReadUInt32() != 0x00000007)
                {
                    return null;
                }

                UInt32 uiFileSize = Reader.ReadUInt32();
                Reader.SeekCurrent(8);

                Vertices = new Vector3[uiFileSize / 16];

                for (Int32 iIterator = 0; iIterator < (uiFileSize / 16); iIterator++)
                {
                    Vertices[iIterator].x = Reader.ReadSingle();
                    Vertices[iIterator].y = Reader.ReadSingle();
                    Vertices[iIterator].z = Reader.ReadSingle();
                    Reader.SeekCurrent(4);
                }
            }

            using (ArchiveFileBinaryReader Reader = new ArchiveFileBinaryReader(LevelCollisionMeshIndicesFile))
            {
                if (Reader.ReadUInt32() != 0x00000004)
                {
                    return null;
                }

                UInt32 uiFileSize = Reader.ReadUInt32() - 2;
                Reader.SeekCurrent(8);

                Triangles = new List<Int32>();

                for (Int32 iIterator = 0; iIterator < uiFileSize / 10; iIterator++)
                {
                    Triangles.Add(Reader.ReadInt16());
                    Triangles.Add(Reader.ReadInt16());
                    Triangles.Add(Reader.ReadInt16());
                    Reader.SeekCurrent(4);
                }
            }

            GameObject collisionMesh = new GameObject("collision_mesh");

            MeshFilter meshFilter = collisionMesh.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = collisionMesh.AddComponent<MeshRenderer>();

            meshFilter.mesh = new Mesh()
            {
                vertices = Vertices,
                triangles = Triangles.ToArray()
            };

            meshRenderer.material.mainTexture = null;
            meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
            meshRenderer.enabled = false;

            collisionMesh.transform.localScale = new Vector3(-1f, -1f, -1f);
            collisionMesh.transform.eulerAngles = new Vector3(90f, collisionMesh.transform.eulerAngles.y, collisionMesh.transform.eulerAngles.z);

            collisionMesh.AddComponent<MeshCollider>();

            return collisionMesh;
        }

        public GameObject CreateGameObjectFromIndex(String sLabel, Int32 iIndex)
        {
            if (renderWareStream == null)
            {
                return null;
            }

            if (iIndex < 0 || iIndex > textureDiffuseNames.Count)
            {
                Debug.Log("*** Error: RenderWareLevel.CreateGameObjectFromIndex(" + iIndex + ") failed. Outside list bounds!");

                return null;
            }

            Int32 iClumpCount = 0;

            GameObject skyBoxMesh = new GameObject(sLabel);

            foreach (RenderWareSection renderWareSection in renderWareStream.RenderWareStreamSections)
            {
                if (renderWareSection is Clump clump)
                {
                    if (iClumpCount == iIndex)
                    {
                        foreach (Geometry geometry in clump.geometryList.geometryList)
                        {
                            foreach (RenderWareSection geometryExtensionSection in geometry.geometryExtension.extensionSectionList)
                            {
                                if (geometryExtensionSection is NativeDataPlg nativeDataPlg)
                                {
                                    for (Int32 iIterator = 0; iIterator < nativeDataPlg.nativeDataPlgStructure.materialSplits.Count; iIterator++)
                                    {
                                        GameObject meshObject = new GameObject("mesh_" + iIterator);

                                        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                                        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

                                        NativeDataPlgStructure.MaterialSplit scaledMaterialSplit = nativeDataPlg.nativeDataPlgStructure.materialSplits[iIterator];

                                        // NOTE: Scale the vertex, UV and normal data.
                                        bool bAtomicScaleFound = false;

                                        foreach (RenderWareSection extension in clump.atomicList[clump.atomicList.Count-1].atomicExtension.extensionSectionList)
                                        {
                                            if (extension is AtomicScale atomicScale)
                                            {
                                                RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, atomicScale.fVertexScale, atomicScale.fUVScale, atomicScale.fUnknownScale);

                                                bAtomicScaleFound = true;

                                                break;
                                            }
                                        }

                                        if (bAtomicScaleFound == false)
                                        {
                                            //RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, -1, -1, -1);

                                            Debug.Log("Warning: Didn't find AtomicScale for level!");
                                        }

                                        meshFilter.mesh = RenderWareModel.UnityModelFromMaterialSplit(scaledMaterialSplit);

                                        meshRenderer.material.mainTexture = UnityTextureManager.GetTextureFromDictionary(textureDiffuseNames[iIndex]);
                                        meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");

                                        meshFilter.mesh.RecalculateNormals();

                                        meshObject.transform.parent = skyBoxMesh.transform;
                                    }
                                }
                            }
                        }

                        for (Int32 iIterator = 0; iIterator < clump.frameList.frameListStructure.iFrameCount; iIterator++)
                        {
                            Frame frame = clump.frameList.frameListStructure.Frames[iIterator];

                            if (frame.iParent == -1)
                            {
                                //skyBoxMesh.transform.localPosition = frame.Position;
                                skyBoxMesh.transform.localRotation = Quaternion.LookRotation(frame.MatrixForward, frame.MatrixUp);
                            }
                        }

                        break;
                    }

                    iClumpCount++;
                }
            }

            return skyBoxMesh;
        }
    }
}