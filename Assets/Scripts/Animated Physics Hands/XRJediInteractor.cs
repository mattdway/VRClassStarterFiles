using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Weblication.XR.Grabbable;

namespace Weblication.XR.Base
{
    [RequireComponent(typeof(ActionBasedController))]
    public class XRJediInteractor : XRRayInteractor
    {
        #region Variables

        [SerializeField] private float hoverTime = 0.8f;

        private float _hoverTimer;
        private bool _hovering;

        IXRSelectInteractable _interactable = null;

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (_hovering)
            {
                _hoverTimer += Time.deltaTime;

                if (_hoverTimer > hoverTime)
                {
                    if (_interactable != null)
                        interactionManager.SelectEnter(this, _interactable);

                    _hovering = false;
                    _hoverTimer = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            _interactable = args.interactableObject as IXRSelectInteractable;

            _hovering = true;
            _hoverTimer = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

            _hovering = false;
            _hoverTimer = 0;

            _interactable = null;
        }

        #endregion
    }
}