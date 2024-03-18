using UnityEngine;

[System.Serializable]
public class OverpassData
{
    public OverpassElement[] elements;
}

[System.Serializable]
public class OverpassElement
{
    public string type;
    public long id;
    public long[] nodes;
    public OverpassTags tags;
}

[System.Serializable]
public class OverpassTags
{
    public string building;
    public string height;
    public string levels;
    public string addr_housenumber;
    public string addr_street;
}

public class BuildingGenerator : MonoBehaviour
{
    public float scaleFactor = 1f;

    public void CreateBuildings(string jsonData)
    {
        OverpassData overpassData = JsonUtility.FromJson<OverpassData>(jsonData);

        if (overpassData != null && overpassData.elements != null)
        {
            foreach (var element in overpassData.elements)
            {
                if (element.type == "way" && element.nodes != null && element.nodes.Length >= 3)
                {
                    CreateBuilding(element);
                }
            }
        }
    }

    void CreateBuilding(OverpassElement element)
    {
        Vector3[] buildingVertices = new Vector3[element.nodes.Length];
        for (int i = 0; i < element.nodes.Length; i++)
        {
            OverpassNode node = GetNodeById(element.nodes[i]);
            buildingVertices[i] = new Vector3((float)node.lon, 0f, (float)node.lat) * scaleFactor;
        }

        // Create a GameObject representing the building
        GameObject building = new GameObject("Building");
        MeshFilter meshFilter = building.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = building.AddComponent<MeshRenderer>();

        // Create a mesh for the building using the node coordinates
        Mesh mesh = new Mesh();
        mesh.vertices = buildingVertices;
        mesh.triangles = Triangulate(buildingVertices);

        meshFilter.mesh = mesh;

        // Extrude the building by raising the vertices along the Y-axis
        ExtrudeBuilding(mesh, GetBuildingHeight(element) * scaleFactor);

        // You may want to assign materials, textures, or customize the appearance further
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    void ExtrudeBuilding(Mesh mesh, float height)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] extrudedVertices = new Vector3[vertices.Length * 2];

        // Duplicate the vertices to create the top part of the building
        for (int i = 0; i < vertices.Length; i++)
        {
            extrudedVertices[i] = vertices[i];
            extrudedVertices[i].y += height;
        }

        // Combine the original and extruded vertices to create the building
        System.Array.Copy(vertices, 0, extrudedVertices, vertices.Length, vertices.Length);
        mesh.vertices = extrudedVertices;

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    OverpassNode GetNodeById(long nodeId)
    {
        // Implement logic to retrieve the node information based on its ID
        // This is a placeholder; replace it with your own logic or data retrieval method
        OverpassNode node = new OverpassNode();
        // Retrieve node information using nodeId
        // ...

        return node;
    }

    int[] Triangulate(Vector3[] vertices)
    {
        // Implement triangulation logic based on the vertices
        // This is a placeholder; replace it with your own triangulation method
        // For simplicity, assuming a simple triangulation for a convex polygon
        int[] triangles = new int[(vertices.Length - 2) * 3];
        int index = 0;

        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = i;
            triangles[index++] = i + 1;
        }

        return triangles;
    }

    float GetBuildingHeight(OverpassElement element)
    {
        float defaultHeight = 10f; // Default height if no height information is available

        // Parse height information from the OverpassElement tags
        if (!string.IsNullOrEmpty(element.tags.height))
        {
            float parsedHeight;
            if (float.TryParse(element.tags.height, out parsedHeight))
            {
                return parsedHeight;
            }
        }

        // If height information is not available, use a default value
        return defaultHeight;
    }
}

[System.Serializable]
public class OverpassNode
{
    public long id;
    public double lat;
    public double lon;
}
