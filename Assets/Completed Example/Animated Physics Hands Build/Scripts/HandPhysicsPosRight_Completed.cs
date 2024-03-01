// These are the namespaces being used in this script.
using UnityEngine;

// This is the class definition for the HandPhysicsPosRight_Completed script.
public class HandPhysicalPosRight_Completed : MonoBehaviour
{
    // These are public variables that can be accessed from other scripts and the inspector.
    public Transform controller; // This is a reference to the transform of the controller that the hand is attached to.
    public Renderer nonPhysicalHand; // This is a reference to the renderer of the non-physical hand.
    public float showNonPhysicalHandDistance = 0.05f; // This is the distance at which the non-physical hand should be shown.

    // These are private variables that can only be accessed within this script.
    private Rigidbody rb; // This is a reference to the Rigidbody component attached to the hand.
    private Collider[] handColliders; // This is an array of colliders attached to the hand.
    private bool isGrabbed = false; // This is a flag to keep track of whether the hand is currently grabbed.

    // This method is called once at the beginning of the script.
    void Start()
    {
        // This gets the Rigidbody component attached to the hand.
        rb = GetComponent<Rigidbody>();

        // This gets all the colliders attached to the hand.
        handColliders = GetComponentsInChildren<Collider>();
    }

    // This method enables the hand colliders.
    public void EnableHandCollider()
    {
        // This checks if the hand is not currently grabbed.
        if (!isGrabbed)
        {
            // This enables all the colliders attached to the hand.
            foreach (var item in handColliders)
            {
                item.enabled = true;
            }
        }
    }

    // This method enables the hand colliders after a delay.
    public void EnableHandColliderDelay(float delay)
    {
        // This invokes the EnableHandCollider method after the specified delay.
        Invoke("EnableHandCollider", delay);

        // This sets the isGrabbed flag to false.
        isGrabbed = false;
    }

    // This method disables the hand colliders.
    public void DisableHandCollider()
    {
        // This disables all the colliders attached to the hand.
        foreach (var item in handColliders)
        {
            item.enabled = false;
            isGrabbed = true;
        }
    }

    // This method is called once per frame.
    private void Update()
    {
        // This calculates the distance between the hand and the controller.
        float distance = Vector3.Distance(transform.position, controller.position);

        // This shows or hides the non-physical hand depending on the distance.
        if (distance > showNonPhysicalHandDistance)
        {
            nonPhysicalHand.enabled = true;
        }
        else
            nonPhysicalHand.enabled = false;
    }

    // This method is called once per physics update.
    void FixedUpdate()
    {
        // This calculates the position of the hand based on the position of the controller.
        rb.velocity = (controller.position - transform.position) / Time.fixedDeltaTime;

        // This calculates the rotation of the hand based on the rotation of the controller.
        // Calculate the difference between the rotations of the controller and the current object.
        Quaternion rotationDifference = controller.rotation * Quaternion.Euler(new Vector3(0, 0, -90)) * Quaternion.Inverse(transform.rotation);

        // Convert the rotation difference to an angle and axis representation.
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
        // Calculate the angular velocity needed to rotate by the rotation difference over a single frame
        Vector3 rotationDifferenceInDegree = Vector3.zero;
        if (!float.IsNaN(angleInDegree) && !float.IsInfinity(angleInDegree))
        {
            // Calculate the angular velocity needed to rotate by the rotation difference over a single frame
            rotationDifferenceInDegree = angleInDegree * rotationAxis;
        }
        if (float.IsFinite(rotationDifferenceInDegree.x) && float.IsFinite(rotationDifferenceInDegree.y) && float.IsFinite(rotationDifferenceInDegree.z))
        {
            // Set the angular velocity of the rigid body to rotate towards the controller's rotation
            rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }
    }
}