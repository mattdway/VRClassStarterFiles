using UnityEngine;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(XROrigin))]

public class RoomScalePlayerControllerFix : MonoBehaviour
{
    CharacterController _character;
    XROrigin _xrOrigin;

    // Start is called before the first frame update
    void Start()
    {
        _character = GetComponent<CharacterController>();
        _xrOrigin = GetComponent<XROrigin>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            _character.Move(new Vector3(0.001f, -0.001f, 0.001f));
            _character.Move(new Vector3(-0.001f, -0.001f, -0.001f));
            //Debug.Log("Pushback Against a Wall Happened.");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            _character.Move(new Vector3(0.001f, -0.001f, 0.001f));
            _character.Move(new Vector3(-0.001f, -0.001f, -0.001f));
            //Debug.Log("Pushback Against a Wall Happened.");
        }
    }

    void FixedUpdate()
    {
        _character.height = _xrOrigin.CameraInOriginSpaceHeight + 0.15f;

        var centerPoint = transform.InverseTransformPoint(_xrOrigin.Camera.transform.position);
        _character.center = new Vector3(
        centerPoint.x,
        _character.height / 2 + _character.skinWidth, centerPoint.z);
    }
}