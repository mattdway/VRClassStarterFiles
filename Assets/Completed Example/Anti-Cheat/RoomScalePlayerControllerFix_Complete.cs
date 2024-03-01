// Import necessary Unity libraries and namespaces
using UnityEngine;
using Unity.XR.CoreUtils;

// Ensure the GameObject this script is attached to has the CharacterController and XROrigin components attached to it
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(XROrigin))]

// Define RoomScalePlayerControllerFix class and inherit from MonoBehaviour base class
public class RoomScalePlayerControllerFixComplete : MonoBehaviour
{
    // Private fields to hold references to CharacterController and XROrigin components
    CharacterController _character;
    XROrigin _xrOrigin;

    // Start is called before the first frame update
    void Start()
    {
        // Get the CharacterController and XROrigin components attached to the GameObject
        _character = GetComponent<CharacterController>();
        _xrOrigin = GetComponent<XROrigin>();
    }

    // OnTriggerEnter is called when a collider enters this GameObject's trigger
    void OnTriggerEnter(Collider other)
    {
        // If the colliding object has a tag of "Wall," move the character slightly away from the wall
        if (other.gameObject.tag == "Wall")
        {
            _character.Move(new Vector3(0.001f, -0.001f, 0.001f));
            _character.Move(new Vector3(-0.001f, -0.001f, -0.001f));
            //Debug.Log("Pushback Against a Wall Happened.");
        }
    }

    // OnCollisionEnter is called when a collision occurs with this GameObject
    void OnCollisionEnter(Collision other)
    {
        // If the colliding object has a tag of "Wall," move the character slightly away from the wall
        if (other.gameObject.tag == "Wall")
        {
            _character.Move(new Vector3(0.001f, -0.001f, 0.001f));
            _character.Move(new Vector3(-0.001f, -0.001f, -0.001f));
            //Debug.Log("Pushback Against a Wall Happened.");
        }
    }

    // FixedUpdate is called at a fixed interval
    void FixedUpdate()
    {
        // Update the height of the CharacterController based on the height of the camera in the XROrigin
        _character.height = _xrOrigin.CameraInOriginSpaceHeight + 0.15f;

        // Calculate the center point of the CharacterController based on the position of the camera in the XROrigin
        var centerPoint = transform.InverseTransformPoint(_xrOrigin.Camera.transform.position);
        _character.center = new Vector3(
            centerPoint.x,
            _character.height / 2 + _character.skinWidth,
            centerPoint.z);
    }
}