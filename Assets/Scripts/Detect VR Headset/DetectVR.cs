using UnityEngine;
using UnityEngine.XR.Management;

public class DetectVR : MonoBehaviour
{
    public bool startInVR = true;
    //public GameObject xrOrigin;
    public GameObject xrSimulator;

    // Start is called before the first frame update
    void Start()
    {
        if (startInVR)
        {
            var xrSettings = XRGeneralSettings.Instance;
            if (xrSettings == null)
            {
                //Debug.Log("XRGeneralSettings is null");
                return;
            }

            var xrManager = xrSettings.Manager;
            if (xrManager == null)
            {
                //Debug.Log("XRManagerSettings is null");
                return;
            }

            var xrLoader = xrManager.activeLoader;
            if (xrLoader == null)
            {
                //Debug.Log("XRLoader is null");
                //xrOrigin.SetActive(false);
                xrSimulator.SetActive(true);
                return;
            }

            //Debug.Log("XRLoader is not null");
            //xrOrigin.SetActive(true);
            xrSimulator.SetActive(false);
        }
        else
        {
            //xrOrigin.SetActive(false);
            xrSimulator.SetActive(true);
        }
    }
}