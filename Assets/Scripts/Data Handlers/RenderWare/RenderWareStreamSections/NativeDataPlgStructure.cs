using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public class NativeDataPlgStructure : RenderWareSection
    {
        public class MaterialSplit
        {
            public bool bHasVertexData = false;

            public bool bHasUVData = false;

            public bool bHasUV2Data = false;

            public bool bHasRGBAData = false;

            public bool bHasNormalData = false;

            public bool bHasBoneWeightsIndicesData = false;

            public Int32 iMaterialIndex;

            public String sDiffuseTextureName;

            public Vector3[] Vertices;

            public List<Int32> Triangles;

            public Vector2[] UV;

            public Vector2[] UV2;

            public float[][] RGBA;

            public Vector3[] Normals;

            public float[][] vertexBoneWeights;

            public Int32[][] vertexBoneIndices;
        }

        public List<MaterialSplit> materialSplits;

        public BinMeshPlg binMeshPlg;

        public NativeDataPlgStructure Read(ArchiveFileBinaryReader reader, BinMeshPlg binMeshPlg_)
        {
            SectionID = RenderWareSectionID.NativeDataPLG;
            iSectionSize = reader.ReadInt32();
            iRenderWareVersion = reader.ReadInt32();

            if (iSectionSize == 4)
            {
                reader.SeekCurrent(iSectionSize);

                return this;
            }

            if (binMeshPlg_ == null)
            {
                reader.SeekCurrent(iSectionSize);

                Debug.Log("*** Error: Null value passed to NativeDataPlgStructure.Read(). NativeDataPlgStructure.Read() skipping...");

                return this;
            }

            binMeshPlg = binMeshPlg_;

            UInt32 uiPlatform = reader.ReadUInt32();

            if ((Platform)uiPlatform == Platform.Playstation2ClumpNative)
            {
                ReadPS2NativeDataPlg(reader);
            }
            else
            {
                throw new InvalidDataException("*** Error: NativeDataPlgStructure.Read() - unsupported platform " + uiPlatform + ".");
            }

            return this;
        }

        public void ReadPS2NativeDataPlg(ArchiveFileBinaryReader reader)
        {
            materialSplits = new List<MaterialSplit>();

            for (Int32 iDeepIterator = 0; iDeepIterator < binMeshPlg.iMaterialSplitCount; iDeepIterator++)
            {
                UInt32 uiSplitSize;
                UInt32 uiDMASize;
                long lSectionStart;
                long lSectionEnd;
                long lDMASectionEnd;
                bool bReachedEnd = false;

                MaterialSplit newMaterialSplit = new MaterialSplit();

                // NOTE: Begin parsing the DMA packet data...
                uiSplitSize = reader.ReadUInt32();
                reader.SeekCurrent(4);

                lSectionStart = reader.Position();

                reader.SeekCurrent(4);
                uiDMASize = reader.ReadUInt32() * 16;
                reader.SeekBeginning(lSectionStart);

                lDMASectionEnd = lSectionStart + uiDMASize;
                lSectionEnd = lSectionStart + uiSplitSize;

                newMaterialSplit.iMaterialIndex = binMeshPlg.binMeshes[iDeepIterator].iMaterialIndex;

                while ((reader.Position() < lDMASectionEnd) && (bReachedEnd == false))
                {
                    byte bDMAId;
                    UInt32 uiDataOffset;
                    UInt32 uiDataType;

                    long lDataPosition;
                    long lOldPosition;

                    reader.SeekCurrent(3);
                    bDMAId = reader.ReadByte();
                    uiDataOffset = reader.ReadUInt32();
                    reader.SeekCurrent(4);
                    uiDataType = reader.ReadUInt32();

                    lDataPosition = lSectionStart + (uiDataOffset * 16);
                    uiDataType &= 0xFF00FFFF;

                    switch (bDMAId)
                    {
                        case 0x30:
                            {
                                switch (uiDataType)
                                {
                                    // NOTE: Vertex data
                                    case 0x68008000: Debug.Log("Vertex 3 x float32"); break;
                                    case 0x6D008000: // 4 x Int16
                                        {
                                            lOldPosition = reader.Position();
                                            reader.SeekBeginning(lDataPosition);

                                            newMaterialSplit.Vertices = new Vector3[binMeshPlg.binMeshes[iDeepIterator].iIndexCount];

                                            for (Int32 iIterator = 0; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                                            {
                                                newMaterialSplit.Vertices[iIterator].x = Convert.ToSingle(reader.ReadInt16());
                                                newMaterialSplit.Vertices[iIterator].y = Convert.ToSingle(reader.ReadInt16());
                                                newMaterialSplit.Vertices[iIterator].z = Convert.ToSingle(reader.ReadInt16());
                                                reader.SeekCurrent(2);
                                            }

                                            newMaterialSplit.bHasVertexData = true;

                                            reader.SeekBeginning(lOldPosition);
                                        }
                                        break;

                                    // NOTE: UV data
                                    case 0x64008001: Debug.Log("UV 2 x float32"); break;
                                    case 0x65008001: // 2 x Int16
                                        {
                                            lOldPosition = reader.Position();
                                            reader.SeekBeginning(lDataPosition);

                                            newMaterialSplit.UV = new Vector2[binMeshPlg.binMeshes[iDeepIterator].iIndexCount];

                                            for (Int32 iIterator = 0; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                                            {
                                                newMaterialSplit.UV[iIterator].x = Convert.ToSingle(reader.ReadInt16());
                                                newMaterialSplit.UV[iIterator].y = Convert.ToSingle(reader.ReadInt16());
                                            }

                                            newMaterialSplit.bHasUVData = true;

                                            reader.SeekBeginning(lOldPosition);
                                        }
                                        break;
                                    case 0x6D008001: // 4 x Int16
                                        {
                                            lOldPosition = reader.Position();
                                            reader.SeekBeginning(lDataPosition);

                                            newMaterialSplit.UV = new Vector2[binMeshPlg.binMeshes[iDeepIterator].iIndexCount];
                                            newMaterialSplit.UV2 = new Vector2[binMeshPlg.binMeshes[iDeepIterator].iIndexCount];

                                            for (Int32 iIterator = 0; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                                            {
                                                newMaterialSplit.UV[iIterator].x = Convert.ToSingle(reader.ReadInt16());
                                                newMaterialSplit.UV[iIterator].y = Convert.ToSingle(reader.ReadInt16());
                                                
                                                newMaterialSplit.UV2[iIterator].x = Convert.ToSingle(reader.ReadInt16());
                                                newMaterialSplit.UV2[iIterator].y = Convert.ToSingle(reader.ReadInt16());
                                            }

                                            newMaterialSplit.bHasUV2Data = true;

                                            reader.SeekBeginning(lOldPosition);
                                        }
                                        break;

                                    // NOTE: RGBA data
                                    case 0x6D00C002: Debug.Log("RGBA 8 x uint8"); break;
                                    case 0x6E00C002: // 4 x UInt8
                                        {
                                            lOldPosition = reader.Position();
                                            reader.SeekBeginning(lDataPosition);

                                            newMaterialSplit.RGBA = new float[binMeshPlg.binMeshes[iDeepIterator].iIndexCount][];

                                            for (Int32 iIterator = 0; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                                            {
                                                newMaterialSplit.RGBA[iIterator] = new float[4];

                                                newMaterialSplit.RGBA[iIterator][0] = Convert.ToSingle(reader.ReadByte()) / 255;
                                                newMaterialSplit.RGBA[iIterator][1] = Convert.ToSingle(reader.ReadByte()) / 255;
                                                newMaterialSplit.RGBA[iIterator][2] = Convert.ToSingle(reader.ReadByte()) / 255;
                                                newMaterialSplit.RGBA[iIterator][3] = Convert.ToSingle(reader.ReadByte()) / 255;
                                            }

                                            newMaterialSplit.bHasRGBAData = true;

                                            reader.SeekBeginning(lOldPosition);
                                        }
                                        break;

                                    // NOTE: Normal data
                                    case 0x6E008002:
                                    case 0x6E008003: // 4 x Int8
                                        {
                                            lOldPosition = reader.Position();
                                            reader.SeekBeginning(lDataPosition);

                                            newMaterialSplit.Normals = new Vector3[binMeshPlg.binMeshes[iDeepIterator].iIndexCount];

                                            for (Int32 iIterator = 0; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                                            {
                                                newMaterialSplit.Normals[iIterator].x = Convert.ToSingle(reader.ReadByte());
                                                newMaterialSplit.Normals[iIterator].y = Convert.ToSingle(reader.ReadByte());
                                                newMaterialSplit.Normals[iIterator].z = Convert.ToSingle(reader.ReadByte());
                                                reader.SeekCurrent(1);
                                            }

                                            newMaterialSplit.bHasNormalData = true;

                                            reader.SeekBeginning(lOldPosition);
                                        }
                                        break;
                                    case 0x6A008003: Debug.Log("Normal 3 x int8"); break;

                                    // NOTE: Skin weights and indicies data
                                    case 0x6C008004:
                                    case 0x6C008003:
                                    case 0x6C008001: // 4 x float32, 4 x UInt8
                                        {
                                            lOldPosition = reader.Position();
                                            reader.SeekBeginning(lDataPosition);

                                            newMaterialSplit.vertexBoneWeights = new float[binMeshPlg.binMeshes[iDeepIterator].iIndexCount][];
                                            newMaterialSplit.vertexBoneIndices = new Int32[binMeshPlg.binMeshes[iDeepIterator].iIndexCount][];

                                            for (Int32 iIterator = 0; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                                            {
                                                newMaterialSplit.vertexBoneWeights[iIterator] = new float[4];
                                                newMaterialSplit.vertexBoneIndices[iIterator] = new Int32[4];

                                                byte[] bytes = new byte[4];

                                                for (Int32 iNestedIterator = 0; iNestedIterator < 4; iNestedIterator++)
                                                {
                                                    newMaterialSplit.vertexBoneWeights[iIterator][iNestedIterator] = reader.ReadSingle();

                                                    bytes[iNestedIterator] = (byte)(BitConverter.ToUInt32(BitConverter.GetBytes(newMaterialSplit.vertexBoneWeights[iIterator][iNestedIterator]), 0) >> 2);

                                                    if (bytes[iNestedIterator] != 0)
                                                    {
                                                        bytes[iNestedIterator] -= 1;
                                                    }
                                                }

                                                newMaterialSplit.vertexBoneIndices[iIterator][0] = bytes[0];
                                                newMaterialSplit.vertexBoneIndices[iIterator][1] = bytes[1];
                                                newMaterialSplit.vertexBoneIndices[iIterator][2] = bytes[2];
                                                newMaterialSplit.vertexBoneIndices[iIterator][3] = bytes[3];
                                            }

                                            newMaterialSplit.bHasBoneWeightsIndicesData = true;

                                            reader.SeekBeginning(lOldPosition);
                                        }
                                        break;
                                }

                                reader.SeekCurrent(16);
                            }
                            break;

                        case 0x10:
                            bReachedEnd = true;
                            break;

                        default:
                            break;
                    }
                }

                // NOTE: Generate triangle list manually.
                if (newMaterialSplit.bHasVertexData == true)
                {
                    newMaterialSplit.Triangles = new List<Int32>();

                    for (Int32 iIterator = 2; iIterator < binMeshPlg.binMeshes[iDeepIterator].iIndexCount; iIterator++)
                    {
                        if ((newMaterialSplit.Vertices[iIterator].x == newMaterialSplit.Vertices[iIterator - 1].x) && (newMaterialSplit.Vertices[iIterator].y == newMaterialSplit.Vertices[iIterator - 1].y) && (newMaterialSplit.Vertices[iIterator].z == newMaterialSplit.Vertices[iIterator - 1].z) ||
                            (newMaterialSplit.Vertices[iIterator].x == newMaterialSplit.Vertices[iIterator - 2].x) && (newMaterialSplit.Vertices[iIterator].y == newMaterialSplit.Vertices[iIterator - 2].y) && (newMaterialSplit.Vertices[iIterator].z == newMaterialSplit.Vertices[iIterator - 2].z) ||
                            (newMaterialSplit.Vertices[iIterator - 1].x == newMaterialSplit.Vertices[iIterator - 2].x) && (newMaterialSplit.Vertices[iIterator - 1].y == newMaterialSplit.Vertices[iIterator - 2].y) && (newMaterialSplit.Vertices[iIterator - 1].z == newMaterialSplit.Vertices[iIterator - 2].z))
                        {
                            continue;
                        }

                        if ((iIterator % 2) == 0)
                        {
                            newMaterialSplit.Triangles.Add(iIterator);
                            newMaterialSplit.Triangles.Add(iIterator - 2);
                            newMaterialSplit.Triangles.Add(iIterator - 1);
                        }
                        else
                        {
                            newMaterialSplit.Triangles.Add(iIterator - 1);
                            newMaterialSplit.Triangles.Add(iIterator - 2);
                            newMaterialSplit.Triangles.Add(iIterator);
                        }
                    }
                }

                // NOTE: Add the split to the list.
                materialSplits.Add(newMaterialSplit);

                // NOTE: Skip to lSectionEnd.
                reader.SeekCurrent(lSectionEnd - reader.Position());
            }
        }
    }
}