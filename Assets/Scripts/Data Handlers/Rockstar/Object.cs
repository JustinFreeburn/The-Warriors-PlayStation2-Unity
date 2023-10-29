using System;
using UnityEngine;

namespace TheWarriors
{
    public class Object
    {
        /// 1 x TextureDictionary.
        /// ? x Atomic.

        private RockstarMetadataManager.ObjectMetadata objectMetadataFile = null;

        String sObjectName;

        /// <summary>
        /// 2+ sections minimum. This class renders Atomic sections. It contains no texture data (has a dummy TextureDictionary), but contains mesh data in the Atomic Geometry Extension.
        /// </summary>
        public Object(String sObjectName_)
        {
            objectMetadataFile = RockstarMetadataManager.GetObjectMetadata(sObjectName_);

            sObjectName = sObjectName_;
        }

        public GameObject CreateObjectObject()
        {
            if (objectMetadataFile == null)
            {
                return null;
            }

            //GameObject objectObject = new GameObject("object_" + sObjectName);
            GameObject objectObject = new GameObject(sObjectName);

            RenderWareStream renderWareTextureStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(objectMetadataFile.uiTextureHash)));
            RenderWareStream renderWareModelStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(objectMetadataFile.uiModelHash)));

            UnityTextureManager.LoadTexturesFromRenderWareSections(renderWareTextureStream.RenderWareStreamSections.ToArray());

            objectObject.transform.localPosition = Vector3.zero;
            objectObject.transform.localRotation = Quaternion.identity;

            foreach (RenderWareSection renderWareSection in renderWareModelStream.RenderWareStreamSections)
            {
                if (renderWareSection is Clump clump)
                {
                    // NOTE: Create the geometry
                    foreach (Geometry geometry in clump.geometryList.geometryList)
                    {
                        foreach (RenderWareSection geometryExtensionSection in geometry.geometryExtension.extensionSectionList)
                        {
                            if (geometryExtensionSection is NativeDataPlg nativeDataPlg)
                            {
                                for (Int32 iIterator = 0; iIterator < nativeDataPlg.nativeDataPlgStructure.materialSplits.Count; iIterator++)
                                {
                                    NativeDataPlgStructure.MaterialSplit materialSplit = nativeDataPlg.nativeDataPlgStructure.materialSplits[iIterator];

                                    //GameObject objectGeometryMaterialSplitObject = new GameObject("object_" + sObjectName + "_mesh_" + iIterator);
                                    GameObject objectGeometryMaterialSplitObject = new GameObject(sObjectName + "_mesh_" + iIterator);

                                    MeshRenderer meshRenderer = objectGeometryMaterialSplitObject.AddComponent<MeshRenderer>();
                                    MeshFilter meshFilter = objectGeometryMaterialSplitObject.AddComponent<MeshFilter>();

                                    NativeDataPlgStructure.MaterialSplit scaledMaterialSplit = nativeDataPlg.nativeDataPlgStructure.materialSplits[iIterator];

                                    // NOTE: Scale the vertex, UV and normal data.
                                    bool bAtomicScaleFound = false;

                                    foreach (Atomic atomic in clump.atomicList)
                                    {
                                        foreach (RenderWareSection atomicExtension in atomic.atomicExtension.extensionSectionList)
                                        {
                                            if (atomicExtension is AtomicScale atomicScale)
                                            {
                                                RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, atomicScale.fVertexScale, atomicScale.fUVScale, atomicScale.fUnknownScale);
                                                bAtomicScaleFound = true;

                                                break;
                                            }
                                        }
                                    }

                                    if (bAtomicScaleFound == false)
                                    {
                                        RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, -1, -1, -1);

                                        Debug.Log("Warning: Didn't find AtomicScale for object " + sObjectName + "!");
                                    }

                                    Mesh mesh = RenderWareModel.UnityModelFromMaterialSplit(materialSplit);

                                    meshFilter.mesh = mesh;

                                    Texture2D tex = UnityTextureManager.GetTextureFromDictionary(sObjectName);

                                    meshRenderer.material.mainTexture = tex;
                                    meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");

                                    objectGeometryMaterialSplitObject.transform.parent = objectObject.transform;
                                }
                            }
                        }
                    }
                }
            }

            // NOTE: This flips the model right way up
            objectObject.transform.localScale = new Vector3(-1f, 1f, 1f);

            return objectObject;
        }

        public GameObject CreateObjectObjectFromHash(UInt32 uiHash)
        {
            GameObject objectObject = new GameObject(String.Format("{0:X8}", uiHash));

            RenderWareStream renderWareModelStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(uiHash)));

            objectObject.transform.localPosition = Vector3.zero;
            objectObject.transform.localRotation = Quaternion.identity;

            foreach (RenderWareSection renderWareSection in renderWareModelStream.RenderWareStreamSections)
            {
                if (renderWareSection is Clump clump)
                {
                    // NOTE: Create the geometry
                    foreach (Geometry geometry in clump.geometryList.geometryList)
                    {
                        foreach (RenderWareSection geometryExtensionSection in geometry.geometryExtension.extensionSectionList)
                        {
                            if (geometryExtensionSection is NativeDataPlg nativeDataPlg)
                            {
                                for (Int32 iIterator = 0; iIterator < nativeDataPlg.nativeDataPlgStructure.materialSplits.Count; iIterator++)
                                {
                                    NativeDataPlgStructure.MaterialSplit materialSplit = nativeDataPlg.nativeDataPlgStructure.materialSplits[iIterator];

                                    GameObject objectGeometryMaterialSplitObject = new GameObject(String.Format("{0:X8}", uiHash) + "_mesh_" + iIterator);

                                    MeshRenderer meshRenderer = objectGeometryMaterialSplitObject.AddComponent<MeshRenderer>();
                                    MeshFilter meshFilter = objectGeometryMaterialSplitObject.AddComponent<MeshFilter>();

                                    NativeDataPlgStructure.MaterialSplit scaledMaterialSplit = nativeDataPlg.nativeDataPlgStructure.materialSplits[iIterator];

                                    // NOTE: Scale the vertex, UV and normal data.
                                    bool bAtomicScaleFound = false;

                                    foreach (Atomic atomic in clump.atomicList)
                                    {
                                        foreach (RenderWareSection atomicExtension in atomic.atomicExtension.extensionSectionList)
                                        {
                                            if (atomicExtension is AtomicScale atomicScale)
                                            {
                                                RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, atomicScale.fVertexScale, atomicScale.fUVScale, atomicScale.fUnknownScale);
                                                bAtomicScaleFound = true;

                                                break;
                                            }
                                        }
                                    }

                                    if (bAtomicScaleFound == false)
                                    {
                                        RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, -1, -1, -1);

                                        Debug.Log("Warning: Didn't find AtomicScale for object " + String.Format("{0:X8}", uiHash) + "!");
                                    }

                                    Mesh mesh = RenderWareModel.UnityModelFromMaterialSplit(materialSplit);

                                    meshFilter.mesh = mesh;

                                    Texture2D tex = UnityTextureManager.GetTextureFromDictionary(String.Format("{0:X8}", uiHash));

                                    meshRenderer.material.mainTexture = tex;
                                    meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");

                                    objectGeometryMaterialSplitObject.transform.parent = objectObject.transform;
                                }
                            }
                        }
                    }
                }
            }

            // NOTE: This flips the model right way up
            objectObject.transform.localScale = new Vector3(-1f, 1f, 1f);

            return objectObject;
        }
    }
}