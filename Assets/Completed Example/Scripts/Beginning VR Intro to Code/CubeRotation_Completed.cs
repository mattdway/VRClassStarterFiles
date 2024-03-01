// Include the UnityEngine namespace to access Unity's functionality
using UnityEngine;

// Declare a class named CubeRotation that inherits from MonoBehaviour
public class CubeRotation_Completed : MonoBehaviour
{
    // Public float variable to store the rotation speed on the X-axis, with a default value of 50.0f
    public float rotationSpeedX = 50.0f;

    // Public float variable to store the rotation speed on the Y-axis, with a default value of 50.0f
    public float rotationSpeedY = 50.0f;

    // Public float variable to store the rotation speed on the Z-axis, with a default value of 50.0f
    public float rotationSpeedZ = 50.0f;

    // Update method.  Update is called once per frame
    void Update()
    {
        // Rotate the cube based on the specified speed and axis (X-axis) multiplied by Time.deltaTime
        transform.Rotate(Vector3.right * rotationSpeedX * Time.deltaTime);

        // Rotate the cube based on the specified speed and axis (Y-axis) multiplied by Time.deltaTime
        transform.Rotate(Vector3.up * rotationSpeedY * Time.deltaTime);

        // Rotate the cube based on the specified speed and axis (Z-axis) multiplied by Time.deltaTime
        transform.Rotate(Vector3.forward * rotationSpeedZ * Time.deltaTime);

        // By multiplying your movement or rotation values by Time.deltaTime, you ensure that the movement
        // is scaled by the time it took to render the last frame. This way, the object will move the same
        // distance over time, regardless of the frame rate.
    }
}