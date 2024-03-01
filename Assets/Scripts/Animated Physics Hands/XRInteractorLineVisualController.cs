using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractorLineVisualController : MonoBehaviour
{
    public XRInteractorLineVisual leftHandInteractorLineVisual;
    public XRInteractorLineVisual rightHandInteractorLineVisual;

    public void DisableInteractorLineVisual()
    {
        leftHandInteractorLineVisual.enabled = false;
        rightHandInteractorLineVisual.enabled = false;
    }

    public void EnableInteractorLineVisual()
    {
        leftHandInteractorLineVisual.enabled = true;
        rightHandInteractorLineVisual.enabled = true;
    }
}