using UnityEngine;

public class PyramidGenerator : MonoBehaviour
{
    public Material pyramidMaterial;

    public Vector3 pyramidCenter;
    public Vector3 pyramidRotation;
    public float baseSize = 1f;
    public float height = 1f;
    public float focalLength = 10f;
    public float depthOffset = 2f; // Distance between front and back pyramid

    private void OnPostRender()
    {
        DrawPyramid();
    }

    private Vector3[] GetBaseVertices(float zOffset)
    {
        float halfSize = baseSize * 0.5f;

        return new[]
        {
            pyramidCenter + new Vector3(-halfSize, 0, -halfSize + zOffset),
            pyramidCenter + new Vector3(halfSize, 0, -halfSize + zOffset),
            pyramidCenter + new Vector3(halfSize, 0, halfSize + zOffset),
            pyramidCenter + new Vector3(-halfSize, 0, halfSize + zOffset)
        };
    }

    private Vector3 GetApex(float zOffset)
    {
        return pyramidCenter + new Vector3(0, height, zOffset);
    }

    private void DrawPyramid()
    {
        if (pyramidMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        pyramidMaterial.SetPass(0);

        // Get unrotated front and back vertices
        Vector3[] frontBase = GetBaseVertices(0);
        Vector3[] backBase = GetBaseVertices(depthOffset);
        Vector3 frontApex = GetApex(0);
        Vector3 backApex = GetApex(depthOffset);

        // Apply rotation
        RotateVertices(ref frontBase);
        RotateVertices(ref backBase);
        frontApex = RotatePoint(frontApex);
        backApex = RotatePoint(backApex);

        // Perspective scaling
        float frontScale = focalLength / (pyramidCenter.z + focalLength);
        float backScale = focalLength / ((pyramidCenter.z + depthOffset) + focalLength);

        // Draw front and back faces
        DrawBase(frontBase, frontScale);
        DrawEdges(frontBase, frontApex, frontScale);

        DrawBase(backBase, backScale);
        DrawEdges(backBase, backApex, backScale);

        // Connect base edges front to back
        for (int i = 0; i < frontBase.Length; i++)
        {
            Vector3 frontPoint = frontBase[i] * frontScale;
            Vector3 backPoint = backBase[i] * backScale;

            GL.Vertex3(frontPoint.x, frontPoint.y, 0);
            GL.Vertex3(backPoint.x, backPoint.y, 0);
        }

        // Connect apex front to back
        Vector3 scaledFrontApex = frontApex * frontScale;
        Vector3 scaledBackApex = backApex * backScale;

        GL.Vertex3(scaledFrontApex.x, scaledFrontApex.y, 0);
        GL.Vertex3(scaledBackApex.x, scaledBackApex.y, 0);

        GL.End();
        GL.PopMatrix();
    }

    private void RotateVertices(ref Vector3[] vertices)
    {
        Quaternion rotation = Quaternion.Euler(pyramidRotation);
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 localPos = vertices[i] - pyramidCenter;
            vertices[i] = rotation * localPos + pyramidCenter;
        }
    }

    private Vector3 RotatePoint(Vector3 point)
    {
        Quaternion rotation = Quaternion.Euler(pyramidRotation);
        Vector3 localPos = point - pyramidCenter;
        return rotation * localPos + pyramidCenter;
    }

    private void DrawBase(Vector3[] baseVertices, float scale)
    {
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 p1 = baseVertices[i] * scale;
            Vector3 p2 = baseVertices[(i + 1) % baseVertices.Length] * scale;

            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
        }
    }

    private void DrawEdges(Vector3[] baseVertices, Vector3 apex, float scale)
    {
        Vector3 scaledApex = apex * scale;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 pBase = baseVertices[i] * scale;

            GL.Vertex3(pBase.x, pBase.y, 0);
            GL.Vertex3(scaledApex.x, scaledApex.y, 0);
        }
    }
}
