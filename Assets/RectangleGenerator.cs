using UnityEngine;

public class RectangleGenerator : MonoBehaviour
{
    public Material rectangleMaterial;

    public Vector3 rectangleCenter;
    public Vector3 rectangleRotation; // Rotation in degrees (Euler angles)
    public float width = 2f;
    public float height = 1f;
    public float depthOffset = 2f;
    public float focalLength = 10f;

    private void OnPostRender()
    {
        if (rectangleMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        rectangleMaterial.SetPass(0);

        // Create rotation matrix
        Quaternion rotation = Quaternion.Euler(rectangleRotation);

        // Get front and back face vertices
        Vector3[] frontFace = GetRectangleVertices(0, rotation);
        Vector3[] backFace = GetRectangleVertices(depthOffset, rotation);

        // Perspective scale
        float frontScale = focalLength / (rectangleCenter.z + focalLength);
        float backScale = focalLength / ((rectangleCenter.z + depthOffset) + focalLength);

        // Draw front and back faces
        DrawFace(frontFace, frontScale);
        DrawFace(backFace, backScale);

        // Connect corners between front and back
        for (int i = 0; i < frontFace.Length; i++)
        {
            Vector3 p1 = frontFace[i] * frontScale;
            Vector3 p2 = backFace[i] * backScale;

            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
        }

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetRectangleVertices(float zOffset, Quaternion rotation)
    {
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-halfWidth, -halfHeight, zOffset),
            new Vector3(halfWidth, -halfHeight, zOffset),
            new Vector3(halfWidth, halfHeight, zOffset),
            new Vector3(-halfWidth, halfHeight, zOffset)
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rotation * vertices[i] + rectangleCenter;
        }

        return vertices;
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
