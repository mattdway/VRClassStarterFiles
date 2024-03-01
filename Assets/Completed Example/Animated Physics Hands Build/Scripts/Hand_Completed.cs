// Import required libraries and modules
using UnityEngine;

// Define class named Hand, which inherits from MonoBehaviour
public class Hand_Completed : MonoBehaviour
{
    // Declare variables
    Animator animator; // reference to Animator component
    SkinnedMeshRenderer mesh; // reference to SkinnedMeshRenderer component
    public float speed; // a public variable that can be set in Unity's inspector
    private float gripTarget; // private variable to store target grip value
    private float triggerTarget; // private variable to store target trigger value
    private float gripCurrent; // private variable to store current grip value
    private float triggerCurrent; // private variable to store current trigger value
    private string animatorGripParam = "Grip"; // private variable to store Animator's parameter for grip value
    private string animatorTriggerParam = "Trigger"; // private variable to store Animator's parameter for trigger value

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the Animator component attached to the same GameObject as this script
        animator = GetComponent<Animator>();
        // Get a reference to the SkinnedMeshRenderer component attached to a child of the same GameObject as this script
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call AnimateHand function to update hand animation
        AnimateHand();
    }

    // Set the target grip value to the passed-in value
    internal void SetGrip(float v)
    {
        gripTarget = v;
    }

    // Set the target trigger value to the passed-in value
    internal void SetTrigger(float v)
    {
        triggerTarget = v;
    }

    // Animate the hand by updating the grip and trigger values
    void AnimateHand()
    {
        // If the current grip value is not equal to the target grip value
        if (gripCurrent != gripTarget)
        {
            // Update the current grip value by moving towards the target grip value at a speed determined by the speed variable and the amount of time passed since the last frame
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * speed);
            // Set the grip parameter in the Animator to the current grip value
            animator.SetFloat(animatorGripParam, gripCurrent);
        }
        // If the current trigger value is not equal to the target trigger value
        if (triggerCurrent != triggerTarget)
        {
            // Update the current trigger value by moving towards the target trigger value at a speed determined by the speed variable and the amount of time passed since the last frame
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * speed);
            // Set the trigger parameter in the Animator to the current trigger value
            animator.SetFloat(animatorTriggerParam, triggerCurrent);
        }
    }

    // Toggle the visibility of the SkinnedMeshRenderer component
    public void ToggleVisibility()
    {
        mesh.enabled = !mesh.enabled;
    }
}