﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif
using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine;

namespace Sekia
{
    /// <summary>
    /// Defines if Unity will copy the depth that can be bound in shaders as _CameraDepthTexture after the opaques pass or after the transparents pass.
    /// </summary>
    public enum CopyDepthMode
    {
        /// <summary>Depth will be copied after the opaques pass</summary>
        AfterOpaques,
        /// <summary>Depth will be copied after the transparents pass</summary>
        AfterTransparents
    }

    [Serializable, ReloadGroup, ExcludeFromPreset]
    [URPHelpURL("urp-universal-renderer")]
    public class UniversalRendererData : ScriptableRendererData, ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]
        internal class CreateUniversalRendererAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = SekiaPipelineAsset.CreateRendererAsset(pathName, RendererType.UniversalRenderer) as UniversalRendererData;
                Selection.activeObject = instance;
            }
        }

        [MenuItem("Assets/Create/Rendering/Sekia Universal Renderer", priority = CoreUtils.Sections.section3 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority + 2)]
        static void CreateUniversalRendererData()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateUniversalRendererAsset>(), "New Custom Universal Renderer Data.asset", null, null);
        }

#endif

        [Serializable, ReloadGroup]
        public sealed class ShaderResources
        {
            [Reload("Shaders/Utils/Blit.shader")]
            public Shader blitPS;

            [Reload("Shaders/Utils/CopyDepth.shader")]
            public Shader copyDepthPS;

            [Reload("Shaders/Utils/Sampling.shader")]
            public Shader samplingPS;

            [Reload("Shaders/Utils/StencilDeferred.shader")]
            public Shader stencilDeferredPS;

            [Reload("Shaders/Utils/FallbackError.shader")]
            public Shader fallbackErrorPS;

            [Reload("Shaders/Utils/MaterialError.shader")]
            public Shader materialErrorPS;

            // Core blitter shaders, adapted from HDRP
            // TODO: move to core and share with HDRP
            [Reload("Shaders/Utils/CoreBlit.shader"), SerializeField]
            internal Shader coreBlitPS;
            [Reload("Shaders/Utils/CoreBlitColorAndDepth.shader"), SerializeField]
            internal Shader coreBlitColorAndDepthPS;


            [Reload("Shaders/CameraMotionVectors.shader")]
            public Shader cameraMotionVector;

            [Reload("Shaders/ObjectMotionVectors.shader")]
            public Shader objectMotionVector;
        }

        public PostProcessData postProcessData = null;

#if ENABLE_VR && ENABLE_XR_MODULE
        [Reload("Runtime/Data/XRSystemData.asset")]
        public XRSystemData xrSystemData = null;
#endif

        public ShaderResources shaders = null;

        const int k_LatestAssetVersion = 2;
        [SerializeField] int m_AssetVersion = 0;
        [SerializeField] LayerMask m_OpaqueLayerMask = -1;
        [SerializeField] LayerMask m_TransparentLayerMask = -1;
        [SerializeField] bool m_ShadowTransparentReceive = true;
        [SerializeField] CopyDepthMode m_CopyDepthMode = CopyDepthMode.AfterTransparents;
        [SerializeField] bool m_ClusteredRendering = false;
        const TileSize k_DefaultTileSize = TileSize._32;
        [SerializeField] TileSize m_TileSize = k_DefaultTileSize;
        [SerializeField] IntermediateTextureMode m_IntermediateTextureMode = IntermediateTextureMode.Auto;

        protected override ScriptableRenderer Create()
        {
            if (!Application.isPlaying)
            {
                ReloadAllNullProperties();
            }
            return new UniversalRenderer(this);
        }

        /// <summary>
        /// Use this to configure how to filter opaque objects.
        /// </summary>
        public LayerMask opaqueLayerMask
        {
            get => m_OpaqueLayerMask;
            set
            {
                SetDirty();
                m_OpaqueLayerMask = value;
            }
        }

        /// <summary>
        /// Use this to configure how to filter transparent objects.
        /// </summary>
        public LayerMask transparentLayerMask
        {
            get => m_TransparentLayerMask;
            set
            {
                SetDirty();
                m_TransparentLayerMask = value;
            }
        }

        /// <summary>
        /// True if transparent objects receive shadows.
        /// </summary>
        public bool shadowTransparentReceive
        {
            get => m_ShadowTransparentReceive;
            set
            {
                SetDirty();
                m_ShadowTransparentReceive = value;
            }
        }

        /// <summary>
        /// Copy depth mode.
        /// </summary>
        public CopyDepthMode copyDepthMode
        {
            get => m_CopyDepthMode;
            set
            {
                SetDirty();
                m_CopyDepthMode = value;
            }
        }

        internal bool clusteredRendering
        {
            get => m_ClusteredRendering;
            set
            {
                SetDirty();
                m_ClusteredRendering = value;
            }
        }

        internal TileSize tileSize
        {
            get => m_TileSize;
            set
            {
                Assert.IsTrue(value.IsValid());
                SetDirty();
                m_TileSize = value;
            }
        }

        /// <summary>
        /// Controls when URP renders via an intermediate texture.
        /// </summary>
        public IntermediateTextureMode intermediateTextureMode
        {
            get => m_IntermediateTextureMode;
            set
            {
                SetDirty();
                m_IntermediateTextureMode = value;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!m_TileSize.IsValid())
            {
                m_TileSize = k_DefaultTileSize;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Upon asset creation, OnEnable is called and `shaders` reference is not yet initialized
            // We need to call the OnEnable for data migration when updating from old versions of UniversalRP that
            // serialized resources in a different format. Early returning here when OnEnable is called
            // upon asset creation is fine because we guarantee new assets get created with all resources initialized.
            if (shaders == null)
                return;

            ReloadAllNullProperties();
        }

        private void ReloadAllNullProperties()
        {
#if UNITY_EDITOR
            ResourceReloader.TryReloadAllNullIn(this, SekiaPipelineAsset.packagePath);
#if ENABLE_VR && ENABLE_XR_MODULE
            ResourceReloader.TryReloadAllNullIn(xrSystemData, UniversalRenderPipelineAsset.packagePath);
#endif
#endif
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_AssetVersion = k_LatestAssetVersion;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_AssetVersion <= 0)
            {
                var anyNonUrpRendererFeatures = false;

                foreach (var feature in m_RendererFeatures)
                {
                    try
                    {
                        if (feature.GetType().Assembly == typeof(UniversalRendererData).Assembly)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        // If we hit any exceptions while poking around assemblies,
                        // conservatively assume there was a non URP renderer feature.
                    }

                    anyNonUrpRendererFeatures = true;
                }

                // Replicate old intermediate texture behaviour in case of any non-URP renderer features,
                // where we cannot know if they properly declare needed inputs.
                m_IntermediateTextureMode = anyNonUrpRendererFeatures ? IntermediateTextureMode.Always : IntermediateTextureMode.Auto;
            }

            if (m_AssetVersion <= 1)
            {
                // To avoid breaking existing projects, keep the old AfterOpaques behaviour. The new AfterTransparents default will only apply to new projects.
                m_CopyDepthMode = CopyDepthMode.AfterOpaques;
            }


            m_AssetVersion = k_LatestAssetVersion;
        }
    }
}
