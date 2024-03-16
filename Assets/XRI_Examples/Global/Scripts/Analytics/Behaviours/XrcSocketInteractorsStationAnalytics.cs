using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction.Analytics
{
    /// <summary>
    /// Class that connects the Socket Interactors station scene objects with their respective analytics events.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    class XrcSocketInteractorsStationAnalytics : MonoBehaviour
    {
        [Header("Socket Simple Object Substation")]
        [SerializeField]
        XRSocketInteractor m_SimpleSocket;

        [Header("Perler Machine")]
        [SerializeField]
        XRSocketInteractor m_BatterySlotSocket;

        [SerializeField]
        XRSocketInteractor[] m_InfinityPegSockets;

        [SerializeField]
        Transform m_GridCenter;

        void Start()
        {
            XrcAnalyticsUtils.Register(m_SimpleSocket, new ConnectSocketSimpleObject(), new DisconnectSocketSimpleObject());

            XrcAnalyticsUtils.Register(m_BatterySlotSocket, new ConnectPerlerMachineBattery());

            var grabPerlerBeadParameter = new GrabPerlerBead();
            foreach (var socket in m_InfinityPegSockets)
            {
                if (socket != null) // Check if the socket is not null
                {
                    foreach (var interactable in socket.interactablesSelected)
                    {
                        if (interactable != null) // Check if the interactable is not null
                        {
                            XrcAnalyticsUtils.Register(interactable as XRBaseInteractable, grabPerlerBeadParameter);
                        }
                    }

                    socket.selectEntered.AddListener(args =>
                    {
                        if (args.interactableObject != null) // Check if the interactableObject is not null
                        {
                            XrcAnalyticsUtils.Register(args.interactableObject as XRBaseInteractable, grabPerlerBeadParameter);
                        }
                    });
                }
            }
        }
    }
}