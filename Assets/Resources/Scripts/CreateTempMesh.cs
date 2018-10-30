using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTempMesh : MonoBehaviour
{
    public Texture grassTexture;

    void Start()

    {

        //Make sure the requirements are set. Otherwise, don't run

        if (!grassTexture)

        {

            Debug.Log("Failed to start. Update the textures.");

            return;

        }

        //Set the the GameObject so it has the required mesh materials and settings

        gameObject.AddComponent<MeshFilter>();

        gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        //Create the vertices for the cube.

        mesh.vertices = new Vector3[] {

//front - z (looking at it from the side, this is the coord

//that lets you see how thin the triangle is. Remember this for

//UV mapping. It makes the logic easier.)

new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0.5f, 1, 0),

new Vector3(0.5f, 1, 0), new Vector3(0.5f, 0, 0), new Vector3(0, 0, 0),

//back - z

new Vector3(0, 0, 1), new Vector3(0.5f, 1, 1), new Vector3(0, 1, 1),

new Vector3(0.5f, 1, 1), new Vector3(0, 0, 1), new Vector3(0.5f, 0, 1),

//right - x

new Vector3(0.5f, 0, 0), new Vector3(0.5f, 1, 0), new Vector3(0.5f, 0, 1),

new Vector3(0.5f, 1, 1), new Vector3(0.5f, 0, 1), new Vector3(0.5f, 1, 0),

//left - x

new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0),

new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 1),

//top - y

new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(0.5f, 1, 0),

new Vector3(0.5f, 1, 0), new Vector3(0, 1, 1), new Vector3(0.5f, 1, 1),

//bottom - y

new Vector3(0, 0, 0), new Vector3(0.5f, 0, 0), new Vector3(0, 0, 1),

new Vector3(0.5f, 0, 0), new Vector3(0.5f, 0, 1), new Vector3(0, 0, 1)

};

        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        for (int i = 0; i < uvs.Length;)

        {

            if (mesh.vertices[i].x == mesh.vertices[i + 1].x && mesh.vertices[i].x == mesh.vertices[i + 2].x)

            {

                uvs[i] = new Vector2(mesh.vertices[i].y, mesh.vertices[i].z);

                uvs[i + 1] = new Vector2(mesh.vertices[i + 1].y, mesh.vertices[i + 1].z);

                uvs[i + 2] = new Vector2(mesh.vertices[i + 2].y, mesh.vertices[i + 2].z);

            }

            else if (mesh.vertices[i].y == mesh.vertices[i + 1].y && mesh.vertices[i].y == mesh.vertices[i + 2].y)

            {

                uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);

                uvs[i + 1] = new Vector2(mesh.vertices[i + 1].x, mesh.vertices[i + 1].z);

                uvs[i + 2] = new Vector2(mesh.vertices[i + 2].x, mesh.vertices[i + 2].z);

            }

            else if (mesh.vertices[i].z == mesh.vertices[i + 1].z && mesh.vertices[i].z == mesh.vertices[i + 2].z)

            {

                uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].y);

                uvs[i + 1] = new Vector2(mesh.vertices[i + 1].x, mesh.vertices[i + 1].y);

                uvs[i + 2] = new Vector2(mesh.vertices[i + 2].x, mesh.vertices[i + 2].y);

            }
            else
            {

                uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].y);

                uvs[i + 1] = new Vector2(mesh.vertices[i + 1].x, mesh.vertices[i + 1].y);

                uvs[i + 2] = new Vector2(mesh.vertices[i + 2].x, mesh.vertices[i + 2].y);

            }

            i += 3;

        }

        mesh.uv = uvs;
        var Temp = mesh.vertices;
        for (int i = 0; i < Temp.Length; i++)
        {
            Temp[i] += new Vector3(-0.5f, -0.5f, -0.5f);
        }
        mesh.vertices = Temp;
        //Add the triangles to render the cube

        int[] triangleNumbers = new int[mesh.vertices.Length];

        for (int i = 0; i < mesh.vertices.Length; i++)

            triangleNumbers[i] = i;

        mesh.triangles = triangleNumbers;
        //foreach (var Vert in mesh.vertices)
        //{
        //    var temp = Vert + new Vector3(-0.5f, -0.5f, -0.5f);
        //    Vert = temp;
        //    //Vert += new Vector3(-0.5f, -0.5f, -0.5f);
        //}




        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        //Set the cube's texture and add a collider

        GetComponent<MeshRenderer>().material.mainTexture = grassTexture;

        gameObject.AddComponent<MeshCollider>();

        //gameObject.transform.localScale = (new Vector3(10,1,10));

    }

}
