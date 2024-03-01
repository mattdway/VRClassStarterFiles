using UnityEngine;

public class Dart_Sticky_Completed : MonoBehaviour
{
    // Keep track of the object that collided
    private GameObject collidedObject;

    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collided with object tagged: " + other.gameObject.tag);

        // Store the collided object
        collidedObject = other.gameObject;

        if (other.gameObject.tag == "Wall")
        {
            //Debug.Log("Dart Hit Wall If Statement");

            // Freeze the position and rotation of the collider and its parent
            FreezePositionsAndRotations();

            return;
        }

        if (other.gameObject.tag == "Dart")
        {
            //Debug.Log("Dart Hit Dart If Statement");

            // Unfreeze the position and rotation of the collider and its parent
            UnfreezePositionsAndRotations();

            return;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject == collidedObject)
        {
            //Debug.Log("Dart Unstuck From Wall!");

            // Unfreeze the position and rotation of the collider and its parent
            UnfreezePositionsAndRotations();

            // Clear the collided object reference
            collidedObject = null;
        }
    }

    void FreezePositionsAndRotations()
    {
        //Debug.Log("Method Reached: Freezing Position and Rotation");

        // Get the collider of the object that collided
        Collider collider = collidedObject.GetComponent<Collider>();

        // Get the rigidbody of the collider
        Rigidbody colliderRigidbody = collider.attachedRigidbody;

        // If the collider has a rigidbody component
        if (colliderRigidbody != null)
        {
            // Freeze the position and rotation in all directions
            colliderRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // If the game object has a collider
        if (collider != null)
        {
            // Freeze the position and rotation of the collider
            collider.transform.position = collider.transform.position;
            collider.transform.rotation = collider.transform.rotation;
        }

        // Get the parent object's collider and rigidbody components
        Collider parentCollider = transform.parent.GetComponent<Collider>();
        Rigidbody parentRigidbody = transform.parent.GetComponent<Rigidbody>();

        // If the parent object has a rigidbody component
        if (parentRigidbody != null)
        {
            // Freeze the position and rotation in all directions
            parentRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // If the parent game object has a collider
        if (parentCollider != null)
        {
            // Freeze the position and rotation of the parent object's collider
            parentCollider.transform.position = parentCollider.transform.position;
            parentCollider.transform.rotation = parentCollider.transform.rotation;
        }

        // Disable the StickyDart component to prevent further collisions
        enabled = false;
    }

    void UnfreezePositionsAndRotations()
    {
        //Debug.Log("Method Reached: Unfreezing Position and Rotation");

        Collider collidedObjCollider = collidedObject.GetComponent<Collider>();
        if (collidedObjCollider != null)
        {
            // Check if collidedObject is not null
            if (collidedObject != null)
            {
                // Get the collider of the object that collided
                Collider collider = collidedObject.GetComponent<Collider>();

                // Get the rigidbody of the collider
                Rigidbody colliderRigidbody = collider.attachedRigidbody;

                // If the collider has a rigidbody component
                if (colliderRigidbody != null)
                {
                    // Unfreeze the position and rotation in all directions
                    colliderRigidbody.constraints = RigidbodyConstraints.None;
                }

                // Get the parent object's collider and rigidbody components
                Collider parentCollider = transform.parent.GetComponent<Collider>();
                Rigidbody parentRigidbody = transform.parent.GetComponent<Rigidbody>();

                // If the parent object has a rigidbody component
                if (parentRigidbody != null)
                {
                    // Unfreeze the position and rotation in all directions
                    parentRigidbody.constraints = RigidbodyConstraints.None;
                }
            }
        }
    }
}