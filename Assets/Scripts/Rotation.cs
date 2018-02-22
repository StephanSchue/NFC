using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{

    public float rotationSpeed = 10;

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * rotationSpeed);
    }
}
