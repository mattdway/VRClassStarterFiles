
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weblication.XR.Grabbable
{
    public class XRJediGrabbable : XRGrabInteractable
    {
        #region Constants

        const string TAG_LEFTHAND = "LeftHand";
        const string TAG_RIGHTHAND = "RightHand";

        const string LAYER_ATTACHED = "Attached";
        const string LAYER_HELD = "Held";

        #endregion

        #region Variables

        private static HashSet<Mesh> registeredMeshes = new();

        public enum Mode
        {
            OutlineAll,
            OutlineVisible,
            OutlineHidden,
            OutlineAndSilhouette,
            SilhouetteOnly
        }

        public enum Hand
        {
            None,
            LeftHand,
            RightHand
        }

        public enum Button
        {
            Trigger,
            Grip
        }

        [Serializable]
        private class ListVector3
        {
            public List<Vector3> data;
        }

        [Header("Actions")]
        public Button grabButton = Button.Trigger;

        [Header("Highlight")]
        [Range(0, 10f)]
        public float highlightWidth = 10f;

        [Header("Left Hand")]
        public Color leftHandColor = Color.green;
        public Transform leftAttachTransform;

        [Header("Right Hand")]
        public Color rightHandColor = Color.blue;
        public Transform rightAttachTransform;

        private Mode _outlineMode;
        private Color _outlineColor = Color.green;
        private float _outlineWidth = 0f;
        private bool _precomputeOutline = true;

        [SerializeField, HideInInspector]
        private List<Mesh> _bakeKeys = new();

        [SerializeField, HideInInspector]
        private List<ListVector3> _bakeValues = new();

        private Renderer[] _renderers;
        private Material _outlineMaskMaterial;
        private Material _outlineFillMaterial;

        private Vector3 _initialLocalPos;
        private Quaternion _initialLocalRot;

        private bool _attached;
        private bool _needsUpdate;

        private Hand _holdingHand = Hand.None;
        private XRBaseInteractor _interactor = null;

        private int _currentLayer = 0;

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Cache renderers
            _renderers = GetComponentsInChildren<Renderer>();
            _renderers = _renderers.Where(r => r is not ParticleSystemRenderer).ToArray();

            _currentLayer = gameObject.layer;

            // Instantiate outline materials
            _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
            _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

            _outlineMaskMaterial.name = "OutlineMask (Instance)";
            _outlineFillMaterial.name = "OutlineFill (Instance)";

            // Retrieve or generate smooth normals
            LoadSmoothNormals();

            // Apply material properties immediately
            _needsUpdate = true;
        }

        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            if (!attachTransform)
            {
                GameObject attachPoint = new("Offset Grab Pivot");

                attachPoint.transform.SetParent(transform, false);
                attachTransform = attachPoint.transform;
            }
            else
            {
                _initialLocalPos = attachTransform.localPosition;
                _initialLocalRot = attachTransform.localRotation;
            }

            _outlineMode = Mode.OutlineAll;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update()
        {
            if (_needsUpdate)
            {
                _needsUpdate = false;

                UpdateMaterialProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnable()
        {
            foreach (var renderer in _renderers)
            {
                // Append outline shaders
                var materials = renderer.sharedMaterials.ToList();

                materials.Add(_outlineMaskMaterial);
                materials.Add(_outlineFillMaterial);

                renderer.materials = materials.ToArray();
            }

            base.OnEnable();
        }

        /// <summary>
        /// 
        /// </summary>
        void OnValidate()
        {
            // Update material properties
            _needsUpdate = true;

            // Clear cache when baking is disabled or corrupted
            if (!_precomputeOutline && _bakeKeys.Count != 0 || _bakeKeys.Count != _bakeValues.Count)
            {
                _bakeKeys.Clear();
                _bakeValues.Clear();
            }

            // Generate smooth normals when baking is enabled
            if (_precomputeOutline && _bakeKeys.Count == 0)
            {
                Bake();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var renderer in _renderers)
            {
                // Remove outline shaders
                var materials = renderer.sharedMaterials.ToList();

                materials.Remove(_outlineMaskMaterial);
                materials.Remove(_outlineFillMaterial);

                renderer.materials = materials.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Destroy material instances
            Destroy(_outlineMaskMaterial);
            Destroy(_outlineFillMaterial);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            if (_holdingHand == Hand.None && !_attached)
            {
                _outlineColor = args.interactorObject.transform.CompareTag(TAG_LEFTHAND) ? rightHandColor : leftHandColor;
                _outlineWidth = highlightWidth;

                _needsUpdate = true;
            }

            hoverEntered?.Invoke(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

            if (_holdingHand == Hand.None || _attached)
            {
                _outlineWidth = 0;

                _needsUpdate = true;
            }

            hoverExited?.Invoke(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            _interactor = args.interactorObject as XRBaseInteractor;

            if (args.interactorObject.transform.CompareTag(TAG_LEFTHAND))
            {
                _holdingHand = Hand.LeftHand;

                if (leftAttachTransform != null)
                    attachTransform = leftAttachTransform;
            }
            else
            {
                if (args.interactorObject.transform.CompareTag(TAG_RIGHTHAND))
                {
                    _holdingHand = Hand.RightHand;

                    if (rightAttachTransform != null)
                        attachTransform = rightAttachTransform;
                }
            }

            if ((_holdingHand == Hand.LeftHand && leftAttachTransform == null) || (_holdingHand == Hand.RightHand && rightAttachTransform == null))
            {
                if (args.interactorObject is XRDirectInteractor)
                {
                    attachTransform.position = args.interactorObject.transform.position;
                    attachTransform.rotation = args.interactorObject.transform.rotation;
                }
                else
                {
                    attachTransform.localPosition = _initialLocalPos;
                    attachTransform.localRotation = _initialLocalRot;
                }
            }

            if (args.interactorObject is XRSocketInteractor)
            {
                _attached = true;

                args.interactableObject.transform.gameObject.layer = LayerMask.NameToLayer(LAYER_ATTACHED);
            }
            else
                args.interactableObject.transform.gameObject.layer = LayerMask.NameToLayer(LAYER_HELD);

            _outlineWidth = 0;
            _needsUpdate = true;

            selectEntered?.Invoke(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (args.interactorObject is XRSocketInteractor)
                _attached = false;

            _holdingHand = Hand.None;
            _interactor = null;

            args.interactableObject.transform.gameObject.layer = _currentLayer;

            selectExited?.Invoke(args);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        [HideInInspector]
        public Hand HeldInHand
        {
            get { return _holdingHand; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool BeingHeld
        {
            get { return isSelected; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float OutlineWidth
        {
            get { return _outlineWidth; }
            set
            {
                _outlineWidth = value;
                _needsUpdate = true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void DropObject()
        {
            if (_interactor != null)
            {
                interactionManager.SelectCancel(_interactor, this as IXRSelectInteractable);

                attachTransform = null;
            }
        }

        #endregion

        #region Private Functions


        /// <summary>
        /// 
        /// </summary>
        void Bake()
        {

            // Generate smooth normals for each mesh
            var bakedMeshes = new HashSet<Mesh>();

            foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
            {

                // Skip duplicates
                if (!bakedMeshes.Add(meshFilter.sharedMesh))
                {
                    continue;
                }

                // Serialize smooth normals
                var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

                _bakeKeys.Add(meshFilter.sharedMesh);
                _bakeValues.Add(new ListVector3() { data = smoothNormals });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void LoadSmoothNormals()
        {
            // Retrieve or generate smooth normals
            foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
            {

                // Skip if smooth normals have already been adopted
                if (!registeredMeshes.Add(meshFilter.sharedMesh))
                {
                    continue;
                }

                // Retrieve or generate smooth normals
                var index = _bakeKeys.IndexOf(meshFilter.sharedMesh);
                var smoothNormals = (index >= 0) ? _bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

                try
                {
                    // Store smooth normals in UV3
                    meshFilter.sharedMesh.SetUVs(3, smoothNormals);
                }
                catch (Exception) { }

                // Combine submeshes
                var renderer = meshFilter.GetComponent<Renderer>();

                if (renderer != null)
                    CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }

            // Clear UV3 on skinned mesh renderers
            foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                // Skip if UV3 has already been reset
                if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                    continue;

                // Clear UV3
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

                // Combine submeshes
                CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        List<Vector3> SmoothNormals(Mesh mesh)
        {

            // Group vertices by location
            var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

            // Copy normals to a new list
            var smoothNormals = new List<Vector3>(mesh.normals);

            // Average normals for grouped vertices
            foreach (var group in groups)
            {

                // Skip single vertices
                if (group.Count() == 1)
                {
                    continue;
                }

                // Calculate the average normal
                var smoothNormal = Vector3.zero;

                foreach (var pair in group)
                {
                    smoothNormal += smoothNormals[pair.Value];
                }

                smoothNormal.Normalize();

                // Assign smooth normal to each vertex
                foreach (var pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="materials"></param>
        void CombineSubmeshes(Mesh mesh, Material[] materials)
        {

            // Skip meshes with a single submesh
            if (mesh.subMeshCount == 1)
            {
                return;
            }

            // Skip if submesh count exceeds material count
            if (mesh.subMeshCount > materials.Length)
            {
                return;
            }

            // Append combined submesh
            mesh.subMeshCount++;
            mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateMaterialProperties()
        {

            // Apply properties according to mode
            _outlineFillMaterial.SetColor("_OutlineColor", _outlineColor);

            switch (_outlineMode)
            {
                case Mode.OutlineAll:
                    _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                    break;

                case Mode.OutlineVisible:
                    _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                    break;

                case Mode.OutlineHidden:
                    _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                    break;

                case Mode.OutlineAndSilhouette:
                    _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                    break;

                case Mode.SilhouetteOnly:
                    _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    _outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                    break;
            }
        }

        #endregion
    }

}