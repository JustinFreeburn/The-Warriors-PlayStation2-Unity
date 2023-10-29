using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheWarriors
{
    public class RenderWareSector
    {
        /// 1 x TextureDictionary.
        /// ? x Atomic.

        private RenderWareSectorFile renderWareSectorFile;

        /// <summary>
        /// 2+ sections minimum. This class renders Atomic sections. It contains no texture data (has a dummy TextureDictionary), but contains mesh data in the Atomic Geometry Extension.
        /// </summary>
        public RenderWareSector(UInt32 uiFileHash_)
        {
            renderWareSectorFile = new RenderWareSectorFile(RockstarArchiveManager.GetWadArchiveFile(uiFileHash_));
        }

        public GameObject CreateSectorObject(Dictionary<UInt32, Vector3> sectorModelPositionList, bool bTransparent)
        {
            if (renderWareSectorFile == null)
            {
                return null;
            }

            GameObject sectorObject = new GameObject("sector_" + String.Format("{0:X8}", renderWareSectorFile.uiFileHash));

            foreach (KeyValuePair<UInt32, RenderWareSection> keyValuePair in renderWareSectorFile.renderWareStreamAtomicSections)
            {
                if (keyValuePair.Value is Atomic atomic)
                {
                    if (atomic.geometry != null)
                    {
                        MaterialList materialList = atomic.geometry.materialList;

                        GameObject atomicObject = new GameObject("atomic_" + keyValuePair.Key);

                        foreach (RenderWareSection atomicGeometryRenderWareSection in atomic.geometry.geometryExtension.extensionSectionList)
                        {
                            if (atomicGeometryRenderWareSection is NativeDataPlg nativeDataPlg)
                            {
                                for (Int32 iIterator = 0; iIterator < nativeDataPlg.nativeDataPlgStructure.materialSplits.Count; iIterator++)
                                {
                                    GameObject atomicMeshObject = new GameObject("atomic_" + keyValuePair.Key + "_mesh_" + iIterator);

                                    MeshFilter meshFilter = atomicMeshObject.AddComponent<MeshFilter>();
                                    MeshRenderer meshRenderer = atomicMeshObject.AddComponent<MeshRenderer>();

                                    NativeDataPlgStructure.MaterialSplit scaledMaterialSplit = nativeDataPlg.nativeDataPlgStructure.materialSplits[iIterator];

                                    // NOTE: Scale the vertex, UV and normal data.
                                    bool bAtomicScaleFound = false;

                                    foreach (RenderWareSection extension in atomic.atomicExtension.extensionSectionList)
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
                                        RenderWareModel.ScaleMaterialSplit(ref scaledMaterialSplit, -1, -1, -1);

                                        Debug.Log("Warning: Didn't find AtomicScale for sector!");
                                    }

                                    meshFilter.mesh = RenderWareModel.UnityModelFromMaterialSplit(scaledMaterialSplit);

                                    if (materialList.materialList[scaledMaterialSplit.iMaterialIndex].texture != null)
                                    {
                                        Texture2D texture = UnityTextureManager.GetTextureFromDictionary(materialList.materialList[scaledMaterialSplit.iMaterialIndex].texture.sDiffuseTextureName);

                                        if (texture == null)
                                        {
                                            Debug.Log("Warning: Failed to find texture \"" + materialList.materialList[scaledMaterialSplit.iMaterialIndex].texture.sDiffuseTextureName + "\"");
                                        }

                                        meshRenderer.material.mainTexture = texture;
                                    }

                                    if (scaledMaterialSplit.bHasUVData == true)
                                    {
                                        meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                                        //meshRenderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                                        string[] keywords = meshRenderer.material.shaderKeywords;

                                        if (Array.IndexOf(keywords, "TRANSPARENT") == -1)
                                        {
                                            meshRenderer.material.EnableKeyword("TRANSPARENT");

                                            meshRenderer.material.SetFloat("_Surface", 1);
                                            meshRenderer.material.renderQueue = (int)RenderQueue.Transparent;
                                        }

                                        meshRenderer.material.SetPass(0);

                                        meshRenderer.material.SetFloat("_Mode", 2);
                                        meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                        meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                        meshRenderer.material.SetInt("_ZWrite", 0);
                                        meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
                                        meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                                        meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                        meshRenderer.material.renderQueue = 3000;
                                    }
                                    else
                                    {
                                        meshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                                    }

                                    if (scaledMaterialSplit.bHasRGBAData == true)
                                    {
                                        Color[] colors = new Color[scaledMaterialSplit.RGBA.Length];

                                        for (Int32 iNestedIterator = 0; iNestedIterator < scaledMaterialSplit.RGBA.Length; iNestedIterator++)
                                        {
                                            colors[iNestedIterator].r = scaledMaterialSplit.RGBA[iNestedIterator][0];
                                            colors[iNestedIterator].g = scaledMaterialSplit.RGBA[iNestedIterator][1];
                                            colors[iNestedIterator].b = scaledMaterialSplit.RGBA[iNestedIterator][2];
                                            colors[iNestedIterator].a = scaledMaterialSplit.RGBA[iNestedIterator][3];
                                        }

                                        meshFilter.mesh.colors = colors;
                                    }

                                    //meshFilter.mesh.RecalculateNormals();

                                    atomicMeshObject.transform.SetParent(atomicObject.transform);
                                }
                            }
                        }

                        if (sectorModelPositionList.ContainsKey(keyValuePair.Key) == true)
                        {
                            atomicObject.transform.position = sectorModelPositionList[keyValuePair.Key];
                        }
                        
                        atomicObject.transform.parent = sectorObject.transform;
                    }
                }
            }

            sectorObject.transform.localScale = new Vector3(-1f, 1f, 1f);

            return sectorObject;
        }
    }
}