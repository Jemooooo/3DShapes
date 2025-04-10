using UnityEngine;

public class RectangleGenerator : MonoBehaviour
{
    public Material rectangleMaterial;

    public Vector3 rectangleCenter;
    public Vector3 rectangleRotation;
    public float width = 2f;
    public float height = 1f;
    public float depthOffset = 2f; // Distance between front and back face
    public float focalLength = 10f;

    private void OnPostRender()
    {
        DrawRectangle();
    }

    private Vector3[] GetRectangleVertices(float zOffset)
    {
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        return new[]
        {
            rectangleCenter + new Vector3(-halfWidth, -halfHeight, zOffset),
            rectangleCenter + new Vector3(halfWidth, -halfHeight, zOffset),
            rectangleCenter + new Vector3(halfWidth, halfHeight, zOffset),
            rectangleCenter + new Vector3(-halfWidth, halfHeight, zOffset)
        };
    }

    private void DrawRectangle()
    {
        if (rectangleMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        rectangleMaterial.SetPass(0);

        // Front and back vertices
        Vector3[] frontFace = GetRectangleVertices(0);
        Vector3[] backFace = GetRectangleVertices(depthOffset);

        // Apply perspective scaling
        float frontScale = focalLength / (rectangleCenter.z + focalLength);
        float backScale = focalLength / ((rectangleCenter.z + depthOffset) + focalLength);

        // Draw front and back faces
        DrawFace(frontFace, frontScale);
        DrawFace(backFace, backScale);

        // Connect front and back vertices (edges)
        for (int i = 0; i < frontFace.Length; i++)
        {
            Vector3 frontPoint = frontFace[i] * frontScale;
            Vector3 backPoint = backFace[i] * backScale;

            GL.Vertex3(frontPoint.x, frontPoint.y, 0);
            GL.Vertex3(backPoint.x, backPoint.y, 0);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void DrawFace(Vector3[] vertices, float scale)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 p1 = vertices[i] * scale;
            Vector3 p2 = vertices[(i + 1) % vertices.Length] * scale;

            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
        }
    }
}
