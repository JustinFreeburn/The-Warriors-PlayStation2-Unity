using System;
using UnityEngine;

namespace TheWarriors
{
    public static class RenderWareModel
    {
        public static Mesh UnityModelFromMaterialSplit(NativeDataPlgStructure.MaterialSplit materialSplit_)
        {
            Mesh mesh = new Mesh();

            if (materialSplit_.bHasVertexData == true)
            {
                mesh.vertices = materialSplit_.Vertices;
                mesh.triangles = materialSplit_.Triangles.ToArray();
            }

            if (materialSplit_.bHasUVData == true || materialSplit_.bHasUV2Data == true)
            {
                mesh.uv = materialSplit_.UV;
            }

            if (materialSplit_.bHasNormalData == true)
            {
                mesh.normals = materialSplit_.Normals;
            }

            return mesh;
        }

        public static void ScaleMaterialSplit(ref NativeDataPlgStructure.MaterialSplit materialSplit_, float fVertexScale_, float fUVScale_, float fUnknownScale_)
        {
            float fVertexScale = fVertexScale_;
            float fUVScale = fUVScale_;
            float fNormalScale = fUnknownScale_;

            if (materialSplit_.bHasVertexData == true)
            {
                for (Int32 iIterator = 0; iIterator < materialSplit_.Vertices.Length; iIterator++)
                {
                    materialSplit_.Vertices[iIterator].x = materialSplit_.Vertices[iIterator].x * fVertexScale;
                    materialSplit_.Vertices[iIterator].y = materialSplit_.Vertices[iIterator].y * fVertexScale;
                    materialSplit_.Vertices[iIterator].z = materialSplit_.Vertices[iIterator].z * fVertexScale;
                }
            }

            if (materialSplit_.bHasUVData == true || materialSplit_.bHasUV2Data == true)
            {
                for (Int32 iIterator = 0; iIterator < materialSplit_.UV.Length; iIterator++)
                {
                    materialSplit_.UV[iIterator].x = materialSplit_.UV[iIterator].x * fUVScale;
                    materialSplit_.UV[iIterator].y = materialSplit_.UV[iIterator].y * fUVScale;
                }
            }

            /*
            if (materialSplit_.bHasNormalData == true)
            {
                for (Int32 iIterator = 0; iIterator < materialSplit_.Normals.Length; iIterator++)
                {
                    materialSplit_.Normals[iIterator].x = materialSplit_.Normals[iIterator].x * fNormalScale;
                    materialSplit_.Normals[iIterator].y = materialSplit_.Normals[iIterator].y * fNormalScale;
                    materialSplit_.Normals[iIterator].z = materialSplit_.Normals[iIterator].z * fNormalScale;
                }
            }
            */
        }
    }
}