using UnityEngine;

public class HandPhysicalPosLeft_Completed : MonoBehaviour
{
    // Public variables
    public Transform controller;
    public Renderer nonPhysicalHand;
    public float showNonPhysicalHandDistance = 0.05f;

    // Private variables
    private Rigidbody rb;
    private Collider[] handColliders;
    private bool isGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component attached to the object
        rb = GetComponent<Rigidbody>();

        // Get all the Colliders attached to the object and its children
        handColliders = GetComponentsInChildren<Collider>();
    }

    // Enable the hand colliders
    public void EnableHandCollider()
    {
        // Only enable the colliders if the hand is not grabbed
        if (!isGrabbed)
        {
            // Loop through all the colliders and enable them
            foreach (var item in handColliders)
            {
                item.enabled = true;
            }
        }
    }

    // Enable the hand colliders with a delay
    public void EnableHandColliderDelay(float delay)
    {
        // Invoke the EnableHandCollider() method after a specified delay
        Invoke("EnableHandCollider", delay);

        // Set isGrabbed to false
        isGrabbed = false;
    }

    // Disable the hand colliders
    public void DisableHandCollider()
    {
        // Loop through all the colliders and disable them
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }

        // Set isGrabbed to true
        isGrabbed = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance between the hand and the controller
        float distance = Vector3.Distance(transform.position, controller.position);

        // If the distance is greater than the specified distance, enable the non-physical hand
        if (distance > showNonPhysicalHandDistance)
        {
            nonPhysicalHand.enabled = true;
        }
        else
        {
            // Otherwise, disable the non-physical hand
            nonPhysicalHand.enabled = false;
        }
    }

    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        // Calculate the velocity needed to move the hand towards the controller
        rb.velocity = (controller.position - transform.position) / Time.fixedDeltaTime;

        // Calculate the rotation difference between the controller and the hand
        Quaternion roationDifference = controller.rotation * Quaternion.Euler(new Vector3(0, 0, 90)) * Quaternion.Inverse(transform.rotation);

        // Convert the rotation difference to an angle and axis representation
        roationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        // Initialize the rotation difference in degrees to zero
        Vector3 rotationDifferenceInDegree = Vector3.zero;

        // If the angle is not NaN or infinite, calculate the rotation difference in degrees
        if (!float.IsNaN(angleInDegree) && !float.IsInfinity(angleInDegree))
        {
            rotationDifferenceInDegree = angleInDegree * rotationAxis;
        }

        // If the rotation difference in degrees is finite, calculate the angular velocity needed to rotate the hand
        if (float.IsFinite(rotationDifferenceInDegree.x) && float.IsFinite(rotationDifferenceInDegree.y) && float.IsFinite(rotationDifferenceInDegree.z))
        {
            rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }
    }
}