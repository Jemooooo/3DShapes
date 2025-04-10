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

    private Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        var firstAxis = axis1 * Mathf.Cos(angle) - axis2 * Mathf.Sin(angle);
        var secondAxis = axis2 * Mathf.Cos(angle) + axis1 * Mathf.Sin(angle);
        return new Vector2(firstAxis, secondAxis);
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

        // Front and back vertices
        Vector3[] frontBase = GetBaseVertices(0);
        Vector3[] backBase = GetBaseVertices(depthOffset);

        Vector3 frontApex = GetApex(0);
        Vector3 backApex = GetApex(depthOffset);

        // Apply rotation to front and back base
        RotateVertices(ref frontBase);
        RotateVertices(ref backBase);

        // Perspective scaling
        float frontScale = focalLength / (pyramidCenter.z + focalLength);
        float backScale = focalLength / ((pyramidCenter.z + depthOffset) + focalLength);

        // Draw front face
        DrawBase(frontBase, frontScale);
        DrawEdges(frontBase, frontApex, frontScale);

        // Draw back face
        DrawBase(backBase, backScale);
        DrawEdges(backBase, backApex, backScale);

        // Connect front and back vertices
        for (int i = 0; i < frontBase.Length; i++)
        {
            Vector3 frontPoint = frontBase[i] * frontScale;
            Vector3 backPoint = backBase[i] * backScale;

            GL.Vertex3(frontPoint.x, frontPoint.y, 0);
            GL.Vertex3(backPoint.x, backPoint.y, 0);
        }

        // Connect front apex to back apex
        Vector3 scaledFrontApex = frontApex * frontScale;
        Vector3 scaledBackApex = backApex * backScale;

        GL.Vertex3(scaledFrontApex.x, scaledFrontApex.y, 0);
        GL.Vertex3(scaledBackApex.x, scaledBackApex.y, 0);

        GL.End();
        GL.PopMatrix();
    }

    private void RotateVertices(ref Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = pyramidCenter - vertices[i];
            Vector2 rotated = RotateBy(pyramidRotation.z, offset.x, offset.y);
            vertices[i] = new Vector3(rotated.x, rotated.y, vertices[i].z) + pyramidCenter;
        }
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
