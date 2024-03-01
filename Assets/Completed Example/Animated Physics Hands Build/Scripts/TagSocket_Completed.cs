using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class TagSocket_Completed : XRSocketInteractor
{
    [SerializeField] private List<string> tags = new List<string>();

    private SkinnedMeshRenderer skinnedMeshRenderer;

    protected override void Awake()
    {
        base.Awake();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Get the selected object's Rigidbody component and disable gravity and kinematic
        XRBaseInteractable selectedInteractable = args.interactableObject as XRBaseInteractable;
        Rigidbody rb = selectedInteractable?.gameObject.GetComponentInChildren<Rigidbody>();

        // Set the gravity of the socketed object to true and the isKinematic to false
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        // Hide the Hand Mesh Renderer
        skinnedMeshRenderer.enabled = false;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ToggleMesh(true);

        // Get the selected object's Rigidbody component and disable gravity and kinematic
        XRBaseInteractable selectedInteractable = args.interactableObject as XRBaseInteractable;
        Rigidbody rb = selectedInteractable?.gameObject.GetComponentInChildren<Rigidbody>();

        // Set the gravity of the socketed object to true and the isKinematic to false
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        // Show the Hand Mesh Renderer
        skinnedMeshRenderer.enabled = true;
    }

    private void ToggleMesh(bool value)
    {
        skinnedMeshRenderer.enabled = value;
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && HasRelavantTag(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && HasRelavantTag(interactable);
    }

    private bool HasRelavantTag(IXRInteractable interactable)
    {
        return tags.Contains(interactable.transform.tag);
    }
}
