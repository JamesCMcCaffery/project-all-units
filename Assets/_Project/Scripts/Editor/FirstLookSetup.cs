using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProjectAllUnits.Editor
{
    /// <summary>One-click warm, stylised lighting and post-processing for the open scene; every value stays editable afterwards.</summary>
    public static class FirstLookSetup
    {
        const string ProfileFolder = "Assets/_Project/Art/Lighting";
        const string ProfilePath = ProfileFolder + "/FirstLook_Volume.asset";
        const string PrefabFolder = "Assets/_Project/Prefabs";

        [MenuItem("Tools/Project All Units/Apply First Look Lighting")]
        static void Apply()
        {
            if (GraphicsSettings.defaultRenderPipeline is not UniversalRenderPipelineAsset)
            {
                Debug.LogError("[FirstLook] No URP Asset in Project Settings > Graphics > Default Render Pipeline. See the README step 1.5.");
                return;
            }

            SetUpGlobalVolume(LoadOrCreateProfile());
            SetUpSun();
            SetUpAmbientAndFog();
            EnablePostProcessingOnSceneCameras();
            EnablePostProcessingOnPrefabCameras();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[FirstLook] Applied. Tweak the Global Volume and Sun in the Hierarchy, then Ctrl+S.");
        }

        static VolumeProfile LoadOrCreateProfile()
        {
            var existing = AssetDatabase.LoadAssetAtPath<VolumeProfile>(ProfilePath);
            if (existing != null) return existing;

            if (!AssetDatabase.IsValidFolder(ProfileFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Art", "Lighting");
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, ProfilePath);

            var tonemapping = AddOverride<Tonemapping>(profile);
            tonemapping.mode.Override(TonemappingMode.Neutral);

            var colour = AddOverride<ColorAdjustments>(profile);
            colour.postExposure.Override(0.2f);
            colour.contrast.Override(10f);
            colour.saturation.Override(15f);
            colour.colorFilter.Override(new Color(1f, 0.97f, 0.92f));

            var whiteBalance = AddOverride<WhiteBalance>(profile);
            whiteBalance.temperature.Override(10f);

            // Slightly cool shadows against warm light is what gives the stylised "golden afternoon" read.
            var tones = AddOverride<ShadowsMidtonesHighlights>(profile);
            tones.shadows.Override(new Vector4(0.94f, 0.97f, 1.08f, 0f));

            var bloom = AddOverride<Bloom>(profile);
            bloom.threshold.Override(1f);
            bloom.intensity.Override(0.35f);
            bloom.scatter.Override(0.6f);

            var vignette = AddOverride<Vignette>(profile);
            vignette.intensity.Override(0.18f);
            vignette.smoothness.Override(0.4f);

            AssetDatabase.SaveAssets();
            return profile;
        }

        static T AddOverride<T>(VolumeProfile profile) where T : VolumeComponent
        {
            var component = profile.Add<T>(true);
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(component, profile);
            return component;
        }

        static void SetUpGlobalVolume(VolumeProfile profile)
        {
            var volume = Object.FindFirstObjectByType<Volume>();
            if (volume == null)
            {
                var go = new GameObject("Global Volume");
                Undo.RegisterCreatedObjectUndo(go, "Create Global Volume");
                volume = go.AddComponent<Volume>();
            }
            Undo.RecordObject(volume, "Set Global Volume");
            volume.isGlobal = true;
            volume.sharedProfile = profile;
        }

        static void SetUpSun()
        {
            Light sun = RenderSettings.sun;
            if (sun == null)
            {
                foreach (var light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
                    if (light.type == LightType.Directional) { sun = light; break; }
            }
            if (sun == null)
            {
                var go = new GameObject("Sun");
                Undo.RegisterCreatedObjectUndo(go, "Create Sun");
                sun = go.AddComponent<Light>();
                sun.type = LightType.Directional;
            }

            Undo.RecordObject(sun, "Set Sun");
            Undo.RecordObject(sun.transform, "Set Sun");
            sun.color = new Color(1f, 0.9f, 0.76f);
            sun.intensity = 1.4f;
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.85f;
            sun.transform.rotation = Quaternion.Euler(38f, -35f, 0f);
            RenderSettings.sun = sun;
        }

        static void SetUpAmbientAndFog()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.72f, 0.88f);
            RenderSettings.ambientEquatorColor = new Color(0.58f, 0.55f, 0.5f);
            RenderSettings.ambientGroundColor = new Color(0.34f, 0.29f, 0.25f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.004f;
            RenderSettings.fogColor = new Color(0.8f, 0.84f, 0.9f);
        }

        static void EnablePostProcessingOnSceneCameras()
        {
            foreach (var camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var data = camera.GetUniversalAdditionalCameraData();
                Undo.RecordObject(data, "Enable Post Processing");
                data.renderPostProcessing = true;
                data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            }
        }

        static void EnablePostProcessingOnPrefabCameras()
        {
            if (!AssetDatabase.IsValidFolder(PrefabFolder)) return;

            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject root = PrefabUtility.LoadPrefabContents(path);
                var cameras = root.GetComponentsInChildren<Camera>(true);
                foreach (var camera in cameras)
                {
                    var data = camera.GetUniversalAdditionalCameraData();
                    data.renderPostProcessing = true;
                    data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                }
                if (cameras.Length > 0) PrefabUtility.SaveAsPrefabAsset(root, path);
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
