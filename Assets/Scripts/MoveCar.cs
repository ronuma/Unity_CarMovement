/*
Funcionalidad de movimiento del coche.

Rodrigo Nunez, 2023-11-13
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    // Displacement vector
    [SerializeField] Vector3 displacement;
    // Wheel Game Objects
    [SerializeField] GameObject FrontLeftWheel;
    [SerializeField] GameObject FrontRightWheel;
    [SerializeField] GameObject RearLeftWheel;
    [SerializeField] GameObject RearRightWheel;
    // Factor in which the wheels are scaled in order to fit the car
    float wheelScale = 0.35f;
    // All objects' meshes
    Mesh CarMesh;
    Mesh FrontLeftWheelMesh;
    Mesh FrontRightWheelMesh;
    Mesh RearLeftWheelMesh;
    Mesh RearRightWheelMesh;
    // Angle of rotation of the vehicle, which will be calculated
    int angle;
    // Vertex arrays for the meshes and transformed vertices
    Vector3[] baseVertices;
    Vector3[] newVertices;
    Vector3[] baseFLWVertices;
    Vector3[] newFLWVertices;
    Vector3[] baseFRWVertices;
    Vector3[] newFRWVertices;
    Vector3[] baseRLWVertices;
    Vector3[] newRLWVertices;
    Vector3[] baseRRWVertices;
    Vector3[] newRRWVertices;
    // Start is called before the first frame update
    void Start()
    {
        // Get the meshes
        CarMesh = GetComponentInChildren<MeshFilter>().mesh;
        FrontLeftWheelMesh = FrontLeftWheel.GetComponentInChildren<MeshFilter>().mesh;
        FrontRightWheelMesh = FrontRightWheel.GetComponentInChildren<MeshFilter>().mesh;
        RearLeftWheelMesh = RearLeftWheel.GetComponentInChildren<MeshFilter>().mesh;
        RearRightWheelMesh = RearRightWheel.GetComponentInChildren<MeshFilter>().mesh;
        // Assign mesh vertices to the vertex arrays
        baseVertices = CarMesh.vertices;
        baseFLWVertices = FrontLeftWheelMesh.vertices;
        baseFRWVertices = FrontRightWheelMesh.vertices;
        baseRLWVertices = RearLeftWheelMesh.vertices;
        baseRRWVertices = RearRightWheelMesh.vertices;

        // Allocate memory for the copy of the vertex list
        newVertices = new Vector3[baseVertices.Length];
        newFLWVertices = new Vector3[baseFLWVertices.Length];
        newFRWVertices = new Vector3[baseFRWVertices.Length];
        newRLWVertices = new Vector3[baseRLWVertices.Length];
        newRRWVertices = new Vector3[baseRRWVertices.Length];
        // Calculate rotation angle
        Vector3 target = new Vector3(displacement.x, 0f, displacement.z);
        Vector3 relative = transform.InverseTransformPoint(target);
        float calculatedAngle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        angle = (int)calculatedAngle;
        // Copy the coordinates
        for (int i = 0; i < baseVertices.Length; i++)
        {
            newVertices[i] = baseVertices[i];
        }
        for (int i = 0; i < baseFLWVertices.Length; i++)
        {
            newFLWVertices[i] = baseFLWVertices[i];
        }
        for (int i = 0; i < baseFRWVertices.Length; i++)
        {
            newFRWVertices[i] = baseFRWVertices[i];
        }
        for (int i = 0; i < baseRLWVertices.Length; i++)
        {
            newRLWVertices[i] = baseRLWVertices[i];
        }
        for (int i = 0; i < baseRRWVertices.Length; i++)
        {
            newRRWVertices[i] = baseRRWVertices[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        DoTransform();
    }

    void DoTransform()
    {
        // Create the matrices
        // Y AXIS is ignored so that it can never go up
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x * Time.time, 0, displacement.z * Time.time);
        Matrix4x4 rotate = HW_Transforms.RotateMat(90 * Time.time, AXIS.X);
        // Calculate rotation angle given displacement
        Matrix4x4 rotateObj = HW_Transforms.RotateMat(-angle, AXIS.Y);
        Matrix4x4 scaleWheel = HW_Transforms.ScaleMat(wheelScale, wheelScale, wheelScale);
        // ------------- CAR ----------------
        // Combine all the matrices into a single one
        Matrix4x4 composite = move * rotateObj;
        // Multiply each vertex in the mesh by the composite matrix
        for (int i = 0; i < newVertices.Length; i++)
        {
            var prev = baseVertices[i];
            Vector4 temp = new Vector4(prev.x, prev.y, prev.z, 1);
            newVertices[i] = composite * temp;
        }
        // Replace the vertices in the mesh
        CarMesh.vertices = newVertices;
        CarMesh.RecalculateNormals();

        // ------------- WHEELS ----------------
        // -- Front Left Wheel --
        Matrix4x4 positionFLWheel = HW_Transforms.TranslationMat(-0.95f, 0.4f, 1.5f);
        Matrix4x4 compositeFLWheel = composite * positionFLWheel * rotate * scaleWheel;
        for (int i = 0; i < newFLWVertices.Length; i++)
        {
            var prev = baseFLWVertices[i];
            Vector4 temp = new Vector4(prev.x, prev.y, prev.z, 1);
            newFLWVertices[i] = compositeFLWheel * temp;
        }
        FrontLeftWheelMesh.vertices = newFLWVertices;
        FrontLeftWheelMesh.RecalculateNormals();

        // -- Front Right Wheel --
        Matrix4x4 positionFRWheel = HW_Transforms.TranslationMat(0.95f, 0.4f, 1.5f);
        Matrix4x4 compositeFRWheel = composite * positionFRWheel * rotate * scaleWheel;
        for (int i = 0; i < newFRWVertices.Length; i++)
        {
            var prev = baseFRWVertices[i];
            Vector4 temp = new Vector4(prev.x, prev.y, prev.z, 1);
            newFRWVertices[i] = compositeFRWheel * temp;
        }
        FrontRightWheelMesh.vertices = newFRWVertices;
        FrontRightWheelMesh.RecalculateNormals();

        // -- Rear Left Wheel --
        Matrix4x4 positionRLWheel = HW_Transforms.TranslationMat(-0.95f, 0.4f, -1.4f);
        Matrix4x4 compositeRLWheel = composite * positionRLWheel * rotate * scaleWheel;
        for (int i = 0; i < newRLWVertices.Length; i++)
        {
            var prev = baseRLWVertices[i];
            Vector4 temp = new Vector4(prev.x, prev.y, prev.z, 1);
            newRLWVertices[i] = compositeRLWheel * temp;
        }
        RearLeftWheelMesh.vertices = newRLWVertices;
        RearLeftWheelMesh.RecalculateNormals();

        // -- Rear Right Wheel --
        Matrix4x4 positionRRWheel = HW_Transforms.TranslationMat(0.95f, 0.4f, -1.4f);
        Matrix4x4 compositeRRWheel = composite * positionRRWheel * rotate * scaleWheel;
        for (int i = 0; i < newRRWVertices.Length; i++)
        {
            var prev = baseRRWVertices[i];
            Vector4 temp = new Vector4(prev.x, prev.y, prev.z, 1);
            newRRWVertices[i] = compositeRRWheel * temp;
        }
        RearRightWheelMesh.vertices = newRRWVertices;
        RearRightWheelMesh.RecalculateNormals();
    }
}
