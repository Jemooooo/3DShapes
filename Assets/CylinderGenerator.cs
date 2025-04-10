using UnityEngine;

public class CylinderGenerator : MonoBehaviour
{
    public Material cylinderMaterial;

    public Vector3 cylinderCenter;
    public Vector3 cylinderRotation;
    public float radius = 1f;
    public float height = 2f;
    public float focalLength = 10f;
    public float depthOffset = 2f;
    public int segments = 20; // Controls smoothness

    private void OnPostRender()
    {
        DrawCylinder();
    }

    private void DrawCylinder()
    {
        if (cylinderMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        cylinderMaterial.SetPass(0);

        // Generate front and back circles
        Vector3[] frontTop = GetCircleVertices(height / 2, 0);
        Vector3[] frontBottom = GetCircleVertices(-height / 2, 0);
        Vector3[] backTop = GetCircleVertices(height / 2, depthOffset);
        Vector3[] backBottom = GetCircleVertices(-height / 2, depthOffset);

        // Apply rotation
        RotateVertices(ref frontTop);
        RotateVertices(ref frontBottom);
        RotateVertices(ref backTop);
        RotateVertices(ref backBottom);

        // Perspective scaling
        float frontScale = focalLength / (cylinderCenter.z + focalLength);
        float backScale = focalLength / ((cylinderCenter.z + depthOffset) + focalLength);

        // Draw circles
        DrawCircle(frontTop, frontScale);
        DrawCircle(frontBottom, frontScale);
        DrawCircle(backTop, backScale);
        DrawCircle(backBottom, backScale);

        // Connect front to back
        ConnectCircles(frontTop, backTop, frontScale, backScale);
        ConnectCircles(frontBottom, backBottom, frontScale, backScale);

        // Connect top to bottom
        ConnectVerticalEdges(frontTop, frontBottom, frontScale);
        ConnectVerticalEdges(backTop, backBottom, backScale);

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetCircleVertices(float yOffset, float zOffset)
    {
        Vector3[] vertices = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (2 * Mathf.PI / segments);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = cylinderCenter + new Vector3(x, yOffset, z + zOffset);
        }
        return vertices;
    }

    private void RotateVertices(ref Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = vertices[i] - cylinderCenter;
            Quaternion rotation = Quaternion.Euler(cylinderRotation);
            vertices[i] = cylinderCenter + rotation * offset;
        }
    }

    private void DrawCircle(Vector3[] vertices, float scale)
    {
        for (int i = 0; i < segments; i++)
        {
            Vector3 p1 = vertices[i] * scale;
            Vector3 p2 = vertices[(i + 1) % segments] * scale;

            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
        }
    }

    private void ConnectCircles(Vector3[] front, Vector3[] back, float frontScale, float backScale)
    {
        for (int i = 0; i < segments; i++)
        {
            Vector3 frontPoint = front[i] * frontScale;
            Vector3 backPoint = back[i] * backScale;

            GL.Vertex3(frontPoint.x, frontPoint.y, 0);
            GL.Vertex3(backPoint.x, backPoint.y, 0);
        }
    }

    private void ConnectVerticalEdges(Vector3[] top, Vector3[] bottom, float scale)
    {
        for (int i = 0; i < segments; i++)
        {
            Vector3 topPoint = top[i] * scale;
            Vector3 bottomPoint = bottom[i] * scale;

            GL.Vertex3(topPoint.x, topPoint.y, 0);
            GL.Vertex3(bottomPoint.x, bottomPoint.y, 0);
        }
    }
}
