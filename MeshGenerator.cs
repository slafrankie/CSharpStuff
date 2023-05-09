//script is designed to generate a rough terrain mesh in unity as part of a small game project
//Author: Stephen LaFrankie 
//5-9-23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    //Initialization 
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //CreateShape(); this is the original and can be used fine, but for the Coroutine's sake graying it out for now
        //UpdateMesh();

        StartCoroutine(CreateShape());
    }

    private void Update() //can delete and utilize the UpdateMesh in void Start, this is only so the Coroutine will work
    {
        UpdateMesh();
    }
    // Loops over/creates all vertices
    //void CreateShape()
    IEnumerator CreateShape() //using Coroutine to slow down creation of triangles to easily view and understand how they are created in the game
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f; //PerlinNoise used to create uneven hilly terrain
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        //array helps define the triangles that make up the grid - vertices ordered from left to right
        //imagine iterator needs to go up using xSize+1, and then goes back down to 1 on the y axis
        triangles = new int[xSize * zSize * 6]; //uses 6 points per quad, xSize and zSize dictate how many of these points we will need
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for(int x = 0; x < xSize; x++)
            {
                triangles [tris + 0] = vert + 0;
                triangles [tris + 1] = vert + xSize + 1;
                triangles [tris + 2] = vert + 1;
                triangles [tris + 3] = vert + 1;
                triangles [tris + 4] = vert + xSize + 1;
                triangles [tris + 5] = vert + xSize + 2;

                vert++; //allows triangles to shift by 1 to the right each time for loop iteration is complete
                tris += 6; //allows current triangle array to not be overwritten, shifts for loop to next set of our list 

                yield return new WaitForSeconds(.1f);
            }
            vert++; //makes it so that program will not be trying to connect next square to previous square (was causing a weird error that shifted the whole model)
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    //Gizmos display vertices in the editor
    private void OnDrawGizmos()
    {
        //makes sure program does not perform method if no vertices exist
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
