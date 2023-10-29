using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheWarriors
{

    public class Character
    {
        /// 1 x TextureDictionary.
        /// ? x Atomic.

        private RockstarMetadataManager.CharacterMetadata characterMetadataFile = null;

        String sCharacterName;

        /// <summary>
        /// 2+ sections minimum. This class renders Atomic sections. It contains no texture data (has a dummy TextureDictionary), but contains mesh data in the Atomic Geometry Extension.
        /// </summary>
        public Character(String sCharacterName_)
        {
            characterMetadataFile = RockstarMetadataManager.GetCharacterMetadata(sCharacterName_);

            sCharacterName = sCharacterName_;
        }

        public GameObject CreateCharacterObject()
        {
            if (characterMetadataFile == null)
            {
                return null;
            }

            //GameObject characterObject = new GameObject("character_" + sCharacterName);
            GameObject characterObject = new GameObject(sCharacterName);

            RenderWareStream renderWareTextureStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(characterMetadataFile.uiTextureHash)));
            RenderWareStream renderWareModelStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(characterMetadataFile.uiModelHash)));

            UnityTextureManager.LoadTexturesFromRenderWareSections(renderWareTextureStream.RenderWareStreamSections.ToArray());

            characterObject.transform.localPosition = Vector3.zero;
            characterObject.transform.localRotation = Quaternion.identity;

            foreach (RenderWareSection renderWareSection in renderWareModelStream.RenderWareStreamSections)
            {
                if (renderWareSection is Clump clump)
                {
                    // NOTE: Create the skeleton
                    List<Transform> bones = new List<Transform>();
                    Matrix4x4[] bindPoses = new Matrix4x4[clump.frameList.frameListStructure.iFrameCount];

                    for (Int32 iIterator = 0; iIterator < clump.frameList.frameListStructure.iFrameCount; iIterator++)
                    {
                        Frame frame = clump.frameList.frameListStructure.Frames[iIterator];
                        Transform bone = new GameObject("bone_" + iIterator).transform;

                        if (frame.iParent == -1 || bones[frame.iParent] == null)
                        {
                            bone.parent = characterObject.transform;
                        }
                        else
                        {
                            bone.parent = bones[frame.iParent].transform;
                        }

                        bone.localPosition = frame.Position;
                        bone.localRotation = Quaternion.LookRotation(frame.MatrixForward, frame.MatrixUp);

                        bindPoses[iIterator] = bone.worldToLocalMatrix * characterObject.transform.localToWorldMatrix;

                        bones.Add(bone);

                        if (iIterator == 0)
                        {
                            bone.gameObject.AddComponent<DrawBones>();
                        }
                    }

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

                                    //GameObject characterGeometryMaterialSplitObject = new GameObject("character_" + sCharacterName + "_mesh_" + iIterator);
                                    GameObject characterGeometryMaterialSplitObject = new GameObject(sCharacterName + "_mesh_" + iIterator);

                                    SkinnedMeshRenderer skinnedMeshRenderer = characterGeometryMaterialSplitObject.AddComponent<SkinnedMeshRenderer>();

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

                                        Debug.Log("Warning: Didn't find AtomicScale for character " + sCharacterName + "!");
                                    }

                                    BoneWeight[] weights = new BoneWeight[materialSplit.vertexBoneIndices.Length];

                                    for (Int32 iNestedIterator = materialSplit.vertexBoneIndices.Length - 1; iNestedIterator > -1; iNestedIterator--)
                                    {
                                        weights[iNestedIterator].weight0 = materialSplit.vertexBoneWeights[iNestedIterator][0];
                                        weights[iNestedIterator].weight1 = materialSplit.vertexBoneWeights[iNestedIterator][1];
                                        weights[iNestedIterator].weight2 = materialSplit.vertexBoneWeights[iNestedIterator][2];
                                        weights[iNestedIterator].weight3 = materialSplit.vertexBoneWeights[iNestedIterator][3];

                                        weights[iNestedIterator].boneIndex0 = (int)materialSplit.vertexBoneIndices[iNestedIterator][0];
                                        weights[iNestedIterator].boneIndex1 = (int)materialSplit.vertexBoneIndices[iNestedIterator][1];
                                        weights[iNestedIterator].boneIndex2 = (int)materialSplit.vertexBoneIndices[iNestedIterator][2];
                                        weights[iNestedIterator].boneIndex3 = (int)materialSplit.vertexBoneIndices[iNestedIterator][3];
                                    }

                                    Mesh mesh = RenderWareModel.UnityModelFromMaterialSplit(materialSplit);

                                    mesh.bindposes = bindPoses;
                                    mesh.boneWeights = weights;
                                    
                                    skinnedMeshRenderer.sharedMesh = mesh;
                                    skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;

                                    skinnedMeshRenderer.bones = bones.ToArray();
                                    skinnedMeshRenderer.rootBone = bones[0];

                                    skinnedMeshRenderer.material.mainTexture = UnityTextureManager.GetTextureFromDictionary(sCharacterName);
                                    skinnedMeshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");

                                    characterGeometryMaterialSplitObject.transform.parent = characterObject.transform;
                                }
                            }
                        }
                    }
                }
            }

            // NOTE: This flips the model right way up
            characterObject.transform.localScale = new Vector3(-1f, 1f, 1f);

            return characterObject;
        }

        public GameObject CreateCharacterObjectFromHash(UInt32 uiHash)
        {
            GameObject characterObject = new GameObject(String.Format("{0:X8}", uiHash));

            RenderWareStream renderWareModelStream = new RenderWareStream(new RenderWareStreamFile(RockstarArchiveManager.GetWadArchiveFile(uiHash)));

            characterObject.transform.localPosition = Vector3.zero;
            characterObject.transform.localRotation = Quaternion.identity;

            foreach (RenderWareSection renderWareSection in renderWareModelStream.RenderWareStreamSections)
            {
                if (renderWareSection is Clump clump)
                {
                    // NOTE: Create the skeleton
                    List<Transform> bones = new List<Transform>();
                    Matrix4x4[] bindPoses = new Matrix4x4[clump.frameList.frameListStructure.iFrameCount];

                    for (Int32 iIterator = 0; iIterator < clump.frameList.frameListStructure.iFrameCount; iIterator++)
                    {
                        Frame frame = clump.frameList.frameListStructure.Frames[iIterator];
                        Transform bone = new GameObject("bone_" + iIterator).transform;

                        if (frame.iParent == -1 || bones[frame.iParent] == null)
                        {
                            bone.parent = characterObject.transform;
                        }
                        else
                        {
                            bone.parent = bones[frame.iParent].transform;
                        }

                        bone.localPosition = frame.Position;
                        bone.localRotation = Quaternion.LookRotation(frame.MatrixForward, frame.MatrixUp);

                        bindPoses[iIterator] = bone.worldToLocalMatrix * characterObject.transform.localToWorldMatrix;

                        bones.Add(bone);

                        if (iIterator == 0)
                        {
                            bone.gameObject.AddComponent<DrawBones>();
                        }
                    }

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

                                    GameObject characterGeometryMaterialSplitObject = new GameObject(String.Format("{0:X8}", uiHash) + "_mesh_" + iIterator);

                                    SkinnedMeshRenderer skinnedMeshRenderer = characterGeometryMaterialSplitObject.AddComponent<SkinnedMeshRenderer>();

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

                                        Debug.Log("Warning: Didn't find AtomicScale for character " + String.Format("{0:X8}", uiHash) + "!");
                                    }

                                    BoneWeight[] weights = new BoneWeight[materialSplit.vertexBoneIndices.Length];

                                    for (Int32 iNestedIterator = materialSplit.vertexBoneIndices.Length - 1; iNestedIterator > -1; iNestedIterator--)
                                    {
                                        weights[iNestedIterator].weight0 = materialSplit.vertexBoneWeights[iNestedIterator][0];
                                        weights[iNestedIterator].weight1 = materialSplit.vertexBoneWeights[iNestedIterator][1];
                                        weights[iNestedIterator].weight2 = materialSplit.vertexBoneWeights[iNestedIterator][2];
                                        weights[iNestedIterator].weight3 = materialSplit.vertexBoneWeights[iNestedIterator][3];

                                        weights[iNestedIterator].boneIndex0 = (int)materialSplit.vertexBoneIndices[iNestedIterator][0];
                                        weights[iNestedIterator].boneIndex1 = (int)materialSplit.vertexBoneIndices[iNestedIterator][1];
                                        weights[iNestedIterator].boneIndex2 = (int)materialSplit.vertexBoneIndices[iNestedIterator][2];
                                        weights[iNestedIterator].boneIndex3 = (int)materialSplit.vertexBoneIndices[iNestedIterator][3];
                                    }

                                    Mesh mesh = RenderWareModel.UnityModelFromMaterialSplit(materialSplit);

                                    mesh.bindposes = bindPoses;
                                    mesh.boneWeights = weights;

                                    skinnedMeshRenderer.sharedMesh = mesh;
                                    skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;

                                    skinnedMeshRenderer.bones = bones.ToArray();
                                    skinnedMeshRenderer.rootBone = bones[0];

                                    skinnedMeshRenderer.material.shader = Shader.Find("Universal Render Pipeline/Unlit");

                                    characterGeometryMaterialSplitObject.transform.parent = characterObject.transform;
                                }
                            }
                        }
                    }
                }
            }

            // NOTE: This flips the model right way up
            characterObject.transform.localScale = new Vector3(-1f, 1f, 1f);

            return characterObject;
        }
    }
}