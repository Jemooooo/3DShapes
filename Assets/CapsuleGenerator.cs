using UnityEngine;

public class CapsuleGenerator : MonoBehaviour
{
    public Material capsuleMaterial;

    public Vector3 capsuleCenter;
    public Vector3 capsuleRotation;
    public float radius = 1f;
    public float height = 2f;
    public int segments = 20;
    public int arcSegments = 10; // For the rounded caps
    public float focalLength = 10f;

    private void OnPostRender()
    {
        if (capsuleMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        capsuleMaterial.SetPass(0);

        float scale = focalLength / (capsuleCenter.z + focalLength);

        // Generate top and bottom circles
        Vector3[] topCircle = GetCircleVertices(height / 2);
        Vector3[] bottomCircle = GetCircleVertices(-height / 2);

        RotateVertices(ref topCircle);
        RotateVertices(ref bottomCircle);

        // Draw cylinder body
        DrawCircle(topCircle, scale);
        DrawCircle(bottomCircle, scale);
        ConnectCircles(topCircle, bottomCircle, scale);

        // Draw hemispheres and connect them to circles
        ConnectHemisphere(height / 2, true, scale, topCircle);
        ConnectHemisphere(-height / 2, false, scale, bottomCircle);

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GetCircleVertices(float yOffset)
    {
        Vector3[] vertices = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = capsuleCenter + new Vector3(x, yOffset, z);
        }
        return vertices;
    }

    private void RotateVertices(ref Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 offset = capsuleCenter - vertices[i];
            Vector2 rotated = RotateBy(capsuleRotation.y, offset.x, offset.z);
            vertices[i] = new Vector3(rotated.x, vertices[i].y, rotated.y) + capsuleCenter;
        }
    }

    private Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        float firstAxis = axis1 * Mathf.Cos(angle) - axis2 * Mathf.Sin(angle);
        float secondAxis = axis2 * Mathf.Cos(angle) + axis1 * Mathf.Sin(angle);
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

    private void ConnectCircles(Vector3[] top, Vector3[] bottom, float scale)
    {
        for (int i = 0; i < segments; i++)
        {
            Vector3 topPoint = top[i] * scale;
            Vector3 bottomPoint = bottom[i] * scale;

            GL.Vertex3(topPoint.x, topPoint.y, 0);
            GL.Vertex3(bottomPoint.x, bottomPoint.y, 0);
        }
    }

    private void ConnectHemisphere(float yOffset, bool isTop, float scale, Vector3[] circleVertices)
    {
        Vector3[] previousArc = null;

        for (int i = 0; i <= arcSegments; i++)
        {
            float angle = Mathf.PI * (i / (float)arcSegments);
            float y = Mathf.Cos(angle) * radius;
            float xzRadius = Mathf.Sin(angle) * radius;

            Vector3[] arcVertices = new Vector3[segments];
            for (int j = 0; j < segments; j++)
            {
                float circleAngle = j * Mathf.PI * 2 / segments;
                float x = Mathf.Cos(circleAngle) * xzRadius;
                float z = Mathf.Sin(circleAngle) * xzRadius;

                arcVertices[j] = capsuleCenter + new Vector3(x, isTop ? y + yOffset : -y + yOffset, z);
            }

            RotateVertices(ref arcVertices);
            DrawCircle(arcVertices, scale);

            // Connect current arc to the circle (first and last arcs only)
            if (i == 0)
            {
                ConnectCircles(circleVertices, arcVertices, scale); // First arc connects to the circle
            }

            if (previousArc != null)
            {
                ConnectCircles(previousArc, arcVertices, scale); // Connect previous arc to the current arc
            }

            previousArc = arcVertices;
        }
    }
}
