using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    public Material sphereMaterial;

    public Vector3 sphereCenter;
    public Vector3 sphereRotation;
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

        // Draw front and back circles
        Vector3[] frontCircle = GetCircleVertices(0);
        Vector3[] backCircle = GetCircleVertices(depthOffset);

        RotateVertices(ref frontCircle);
        RotateVertices(ref backCircle);

        float frontScale = focalLength / (sphereCenter.z + focalLength);
        float backScale = focalLength / ((sphereCenter.z + depthOffset) + focalLength);

        // Draw front face
        DrawCircle(frontCircle, frontScale);

        // Draw back face
        DrawCircle(backCircle, backScale);

        // Connect front and back circle vertices
        for (int i = 0; i < frontCircle.Length; i++)
        {
            Vector3 frontPoint = frontCircle[i] * frontScale;
            Vector3 backPoint = backCircle[i] * backScale;

            GL.Vertex3(frontPoint.x, frontPoint.y, 0);
            GL.Vertex3(backPoint.x, backPoint.y, 0);
        }

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetCircleVertices(float zOffset)
    {
        Vector3[] vertices = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices[i] = sphereCenter + new Vector3(x, y, zOffset);
        }
        return vertices;
    }

    private void RotateVertices(ref Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = sphereCenter - vertices[i];
            Vector2 rotated = RotateBy(sphereRotation.z, offset.x, offset.y);
            vertices[i] = new Vector3(rotated.x, rotated.y, vertices[i].z) + sphereCenter;
        }
    }

    private Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        var firstAxis = axis1 * Mathf.Cos(angle) - axis2 * Mathf.Sin(angle);
        var secondAxis = axis2 * Mathf.Cos(angle) + axis1 * Mathf.Sin(angle);
        return new Vector2(firstAxis, secondAxis);
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
