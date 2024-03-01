// This script allows the user to setup hand poses for XRGrabInteractable object 
// to control the hand model of an OculusSampleFramework HandData component
// based on the provided HandData of the right and left hands.
// The script also includes a function that recursively searches for a HandData component in children GameObjects.

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// The #if UNITY_EDITOR directive allows code to be included in the script only when in the Unity Editor.
// This prevents compilation errors when using UnityEditor-specific code in non-editor builds.
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrabHandPose : MonoBehaviour
{
    // Fields for setting the duration of the pose transition and the right and left hand HandData components
    public float poseTransitionDuration = 0.2f;
    public HandData RightHandPose;
    public HandData LeftHandPose;

    // Private fields to store hand positions and rotations for setting up and resetting hand poses
    private Vector3 _startingHandPosition;
    private Vector3 _finalHandPosition;
    private Quaternion _startingHandRotation;
    private Quaternion _finalHandRotation;

    private Quaternion[] _startingFingerRotations;
    private Quaternion[] _finalFingerRotations;

    // Start is called before the first frame update
    void Start()
    {
        // Get the XRGrabInteractable component from this object and add listeners to its selectEntered and selectExited events
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        // Event listeners listening for an object to be picked up or dropped.  
        // When selectEntered is triggered the SetupPose method is called.  When the selectExited is triggered the UnSetPose method is called. 
        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        // Deactivate both hand models initially
        RightHandPose.gameObject.SetActive(false);
        LeftHandPose.gameObject.SetActive(false);
    }

    // Set up the hand pose for the given interactor object
    public void SetupPose(BaseInteractionEventArgs arg)
    {
        // Debug.Log("Entered SetupPose Method");

        // Check if the interactor object is a XRDirectInteractor
        if (arg.interactorObject is XRDirectInteractor)
        {
            // Debug.Log("SetupPose Method is XRDirectInteractor");

            // Get the HandData component from the interactable object and disable its animator
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;

            // Debug.Log("HandData component found on game object: " + handData.gameObject.name);

            // Set the hand data values based on whether the hand model is for the right or left hand
            if (handData.handType == HandData.HandModelType.Right)
            {
                SetHandDataValues(handData, RightHandPose);
                // Debug.Log("Hand Type is RightHandPose");
            }
            else
            {
                SetHandDataValues(handData, LeftHandPose);
                // Debug.Log("Hand Type is LeftHandPose");
            }

            // Start a coroutine to smoothly transition to the new hand pose
            StartCoroutine(SetHandDataRoutine(handData, _finalHandPosition, _finalHandRotation, _finalFingerRotations, _startingHandPosition, _startingHandRotation, _startingFingerRotations));
        }
    }

    // Un-set the hand pose for the given interactor object
    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        // Debug.Log("Entered UnSetPose Method");

        // Check if the interactor object is a XRDirectInteractor
        if (arg.interactorObject is XRDirectInteractor)
        {
            // Get the HandData component from the interactable object and enable its animator
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = true;

            // Start a coroutine to smoothly transition to the new hand pose
            StartCoroutine(SetHandDataRoutine(handData, _startingHandPosition, _startingHandRotation, _startingFingerRotations, _finalHandPosition, _finalHandRotation, _finalFingerRotations));
        }
    }

    // This method sets the starting and final position and rotation values for two hand data objects.
    public void SetHandDataValues(HandData h1, HandData h2)
    {
        // Debug.Log("Entered SetHandDataValues Method");

        // Calculate the starting hand position by dividing the local position of the hand's root by the local scale of the hand.
        _startingHandPosition = new Vector3(h1.root.localPosition.x / h1.root.localScale.x,
            h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z);

        // Calculate the final hand position by dividing the local position of the hand's root by the local scale of the hand.
        _finalHandPosition = new Vector3(h2.root.localPosition.x / h2.root.localScale.x,
            h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z);

        // Set the starting hand rotation to the local rotation of the hand's root object.
        _startingHandRotation = h1.root.localRotation;

        // Set the final hand rotation to the local rotation of the hand's root object.
        _finalHandRotation = h2.root.localRotation;

        // Initialize an array of Quaternion objects to hold the starting finger rotations.
        _startingFingerRotations = new Quaternion[h1.fingerBones.Length];

        // Initialize an array of Quaternion objects to hold the final finger rotations.
        _finalFingerRotations = new Quaternion[h2.fingerBones.Length];

        // Loop through each finger bone in the first hand.
        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            // Set the starting finger rotation to the local rotation of the finger bone.
            _startingFingerRotations[i] = h1.fingerBones[i].localRotation;

            // Set the final finger rotation to the local rotation of the corresponding finger bone in the second hand.
            _finalFingerRotations[i] = h2.fingerBones[i].localRotation;
        }
    }

    // This method sets the position and rotation values for a single hand data object.
    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        // Debug.Log("Entered SetHandData Method");

        // Set the local position of the hand's root object to the given new position.
        h.root.localPosition = newPosition;

        // Set the local rotation of the hand's root object to the given new rotation.
        h.root.localRotation = newRotation;

        // Loop through each finger bone in the hand.
        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            // Set the local rotation of the finger bone to the corresponding new rotation value.
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }

    public IEnumerator SetHandDataRoutine(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation)
    {
        // Debug.Log("Entered SetHandDataRoutine CoRoutine");

        // Initialize a timer for the transition duration
        float timer = 0;

        // While the timer is less than the pose transition duration, perform the following:
        while(timer < poseTransitionDuration)
        {
            // Calculate the new position and rotation of the hand using Lerp
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);

            // Update the hand's position and rotation
            h.root.localPosition = p;
            h.root.localRotation = r;

            // Update the finger bones' rotations
            for (int i = 0; i < newBonesRotation.Length; i++)
            {
                h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], timer / poseTransitionDuration);
            }

            // Update the timer and wait for the next frame
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // This method is only executed in the Unity editor
#if UNITY_EDITOR

    // Define a menu item that allows the user to mirror the right hand pose
    [MenuItem("Tools/Mirror Selected Right Grab Pose")]
    public static void MirrorRightPose()
    {
        //Debug.Log("MIRROR RIGHT POSE");

        // Get the GrabHandPose script attached to the currently selected GameObject
        GrabHandPose _handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();

        // Mirror the right hand pose based on the left hand pose
        _handPose.MirrorPose(_handPose.LeftHandPose, _handPose.RightHandPose);
    }

    // Define a menu item that allows the user to mirror the left hand pose
    [MenuItem("Tools/Mirror Selected Left Grab Pose")]

    public static void MirrorLeftPose()
    {
        //Debug.Log("MIRROR RIGHT POSE");

        // Get the GrabHandPose script attached to the currently selected GameObject
        GrabHandPose _handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();

        // Mirror the right hand pose based on the left hand pose
        _handPose.MirrorPose(_handPose.RightHandPose, _handPose.LeftHandPose);
    }

#endif

    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror)
    {
        // Mirror the position of the hand along the x-axis
        Vector3 mirroredPosition = poseUsedToMirror.root.localPosition;
        mirroredPosition.x *= -1;

        // Mirror the rotation of the hand along the y and z axes
        Quaternion mirroredQuaternion = poseUsedToMirror.root.localRotation;
        mirroredQuaternion.y *= -1;
        mirroredQuaternion.z *= -1;

        // Update the mirrored hand's position and rotation
        poseToMirror.root.localPosition = mirroredPosition;
        poseToMirror.root.localRotation = mirroredQuaternion;

        // Update the finger bones' rotations
        for (int i = 0; i < poseUsedToMirror.fingerBones.Length; i++)
        {
            poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
        }
    }
}