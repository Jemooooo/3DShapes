using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLShapeGenerator : MonoBehaviour
{
    public Material lineMaterial;
    public float focalLength = 5f;

    private void OnPostRender()
    {
        if (lineMaterial == null)
        {
            Debug.LogError("Please assign a material!");
            return;
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        lineMaterial.SetPass(0);

        // Draw each shape
        DrawPyramid();
        DrawCylinder(10);  // 10 segments for smoothness
        DrawRectangle();
        DrawSphere(10, 10); // 10 latitudes & longitudes

        GL.End();
        GL.PopMatrix();
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
    }

    // --- 3D Shape Functions ---

    // **Pyramid** (Triangular sides)
    private void DrawPyramid()
    {
        Vector3 top = new Vector3(0, 1, 0);
        Vector3[] baseVertices = {
            new Vector3(-1, 0, -1),
            new Vector3(1, 0, -1),
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
        };

        // Base square
        DrawLine(baseVertices[0], baseVertices[1]);
        DrawLine(baseVertices[1], baseVertices[2]);
        DrawLine(baseVertices[2], baseVertices[3]);
        DrawLine(baseVertices[3], baseVertices[0]);

        // Connecting top point to base
        foreach (var v in baseVertices)
        {
            DrawLine(top, v);
        }
    }

    // **Cylinder** (Multiple segments forming a circle)
    private void DrawCylinder(int segments)
    {
        float radius = 1f;
        float height = 2f;
        List<Vector3> topCircle = new List<Vector3>();
        List<Vector3> bottomCircle = new List<Vector3>();

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            topCircle.Add(new Vector3(x, height / 2, z));
            bottomCircle.Add(new Vector3(x, -height / 2, z));
        }

        // Draw circles
        for (int i = 0; i < segments; i++)
        {
            DrawLine(topCircle[i], topCircle[(i + 1) % segments]);
            DrawLine(bottomCircle[i], bottomCircle[(i + 1) % segments]);
            DrawLine(topCircle[i], bottomCircle[i]);
        }
    }

    // **Rectangle (Box Shape)**
    private void DrawRectangle()
    {
        Vector3[] vertices = {
            new Vector3(-1, 0, -0.5f), new Vector3(1, 0, -0.5f),
            new Vector3(1, 0, 0.5f), new Vector3(-1, 0, 0.5f),
            new Vector3(-1, 1, -0.5f), new Vector3(1, 1, -0.5f),
            new Vector3(1, 1, 0.5f), new Vector3(-1, 1, 0.5f),
        };

        int[,] edges = {
            {0,1}, {1,2}, {2,3}, {3,0}, // Bottom face
            {4,5}, {5,6}, {6,7}, {7,4}, // Top face
            {0,4}, {1,5}, {2,6}, {3,7}  // Connecting edges
        };

        for (int i = 0; i < edges.GetLength(0); i++)
        {
            DrawLine(vertices[edges[i, 0]], vertices[edges[i, 1]]);
        }
    }

    // **Sphere** (Approximated using latitudes and longitudes)
    private void DrawSphere(int latSegments, int lonSegments)
    {
        float radius = 1f;
        List<Vector3> points = new List<Vector3>();

        for (int lat = 0; lat <= latSegments; lat++)
        {
            float theta = lat * Mathf.PI / latSegments;
            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float phi = lon * 2 * Mathf.PI / lonSegments;
                float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
                float y = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
                points.Add(new Vector3(x, y, z));
            }
        }

        // Connect the points to form the sphere
        int count = points.Count;
        for (int i = 0; i < count - lonSegments - 1; i++)
        {
            if ((i + 1) % lonSegments == 0) continue;
            DrawLine(points[i], points[i + 1]);
            DrawLine(points[i], points[i + lonSegments + 1]);
        }
    }
}
