using UnityEngine;

public class DrawBones : MonoBehaviour
{
    void Update()
    {
        DrawBone(transform);
    }

    void DrawBone(Transform boneTransform)
    {
        foreach (Transform childBoneTransform in boneTransform)
        {
            float len = 0.05f;

            Vector3 loxalX = new Vector3(len, 0, 0);
            Vector3 loxalY = new Vector3(0, len, 0);
            Vector3 loxalZ = new Vector3(0, 0, len);

            loxalX = childBoneTransform.rotation * loxalX;
            loxalY = childBoneTransform.rotation * loxalY;
            loxalZ = childBoneTransform.rotation * loxalZ;

            Debug.DrawLine(boneTransform.position * 0.1f + childBoneTransform.position * 0.9f, boneTransform.position * 0.9f + childBoneTransform.position * 0.1f, Color.white);

            Debug.DrawLine(childBoneTransform.position, childBoneTransform.position + loxalX, Color.red);
            Debug.DrawLine(childBoneTransform.position, childBoneTransform.position + loxalY, Color.green);
            Debug.DrawLine(childBoneTransform.position, childBoneTransform.position + loxalZ, Color.blue);

            DrawBone(childBoneTransform);
        }
    }
}
