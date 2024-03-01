using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysicalPosLeft : MonoBehaviour
{
    public Transform controller;
    private Rigidbody rb;
    public Renderer nonPhysicalHand;
    public float showNonPhysicalHandDistance = 0.05f;
    private Collider[] handColliders;
    private bool isGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>();
    }

    public void EnableHandCollider()
    {
        if (!isGrabbed)
        {
            foreach (var item in handColliders)
            {
                item.enabled = true;
            }
        }
    }

    public void EnableHandColliderDelay(float delay)
    {
        Invoke("EnableHandCollider", delay);
        isGrabbed = false;
    }

    public void DisableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
            isGrabbed = true;
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, controller.position);

        if (distance > showNonPhysicalHandDistance)
        {
            nonPhysicalHand.enabled = true;
        }
        else
            nonPhysicalHand.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Position
        rb.velocity = (controller.position - transform.position) / Time.fixedDeltaTime;

        // Rotation Using Target Velocity
        // Calculate the difference between the rotations of the controller and the current object
        Quaternion roationDifference = controller.rotation * Quaternion.Euler(new Vector3(0, 0, 90)) * Quaternion.Inverse(transform.rotation);

        // Convert the rotation difference to an angle and axis representation
        roationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        // Calculate the angular velocity needed to rotate by the rotation difference over a single frame
        Vector3 rotationDifferenceInDegree = Vector3.zero;

        if (!float.IsNaN(angleInDegree) && !float.IsInfinity(angleInDegree))
        {
            // Calculate the angular velocity needed to rotate by the rotation difference over a single frame
            rotationDifferenceInDegree = angleInDegree * rotationAxis;
        }

        if (float.IsFinite(rotationDifferenceInDegree.x) && float.IsFinite(rotationDifferenceInDegree.y) && float.IsFinite(rotationDifferenceInDegree.z))
        {
            rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }
    }
}