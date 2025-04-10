using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    public Material sphereMaterial;

    public Vector3 sphereCenter;
    public Vector3 sphereRotation; // Rotation in degrees
    public float radius = 1f;
    public int segments = 20;
    public float focalLength = 10f;
    public float depthOffset = 2f;

    private void OnPostRender()
    {
        DrawSphere();
    }

    private void DrawSphere()
    {
        if (sphereMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        sphereMaterial.SetPass(0);

        // Create rotation
        Quaternion rotation = Quaternion.Euler(sphereRotation);

        // Get rotated front and back circles
        Vector3[] frontCircle = GetCircleVertices(0, rotation);
        Vector3[] backCircle = GetCircleVertices(depthOffset, rotation);

        float frontScale = focalLength / (sphereCenter.z + focalLength);
        float backScale = focalLength / ((sphereCenter.z + depthOffset) + focalLength);

        // Draw circles
        DrawCircle(frontCircle, frontScale);
        DrawCircle(backCircle, backScale);

        // Connect front and back
        for (int i = 0; i < frontCircle.Length; i++)
        {
            Vector3 p1 = frontCircle[i] * frontScale;
            Vector3 p2 = backCircle[i] * backScale;

            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
        }

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetCircleVertices(float zOffset, Quaternion rotation)
    {
        Vector3[] vertices = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            // Apply rotation and offset from center
            vertices[i] = rotation * new Vector3(x, y, zOffset) + sphereCenter;
        }
        return vertices;
    }

    private void DrawCircle(Vector3[] vertices, float scale)
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
