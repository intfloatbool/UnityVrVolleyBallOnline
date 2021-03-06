﻿#if UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Passer {
    using Humanoid;
    using Humanoid.Tracking;

    [InitializeOnLoad]
    public class HumanoidConfiguration : MonoBehaviour {
        static HumanoidConfiguration() {
            //Debug.Log("Initializing Humanoid Dlls");
#if hLEAP
            LeapDevice.LoadDlls();
#endif
#if hORBBEC
            AstraDevice.LoadDlls();
#endif
#if hNEURON
            NeuronDevice.LoadDlls();
#endif
        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        // Have we loaded the prefs yet
        public static Configuration configuration;

#if !UNITY_2018_3_OR_NEWER
        private static string configurationString = "DefaultConfiguration";
        private static bool prefsLoaded = false;

        // Add preferences section named "My Preferences" to the Preferences Window
        [PreferenceItem("Humanoid")]
        public static void PreferencesGUI() {
            if (!prefsLoaded) {
                configurationString = EditorPrefs.GetString("HumanoidConfigurationKey", "DefaultConfiguration");

                LoadConfiguration(configurationString);
                if (configuration == null) {
                    configurationString = "DefaultConfiguration";
                    LoadConfiguration(configurationString);
                }
                prefsLoaded = true;
            }

            if (configuration == null) {
                Debug.Log("Created new Default Configuration");

                string humanoidPath = Configuration_Editor.FindHumanoidFolder();
                // Create new Default Configuration
                configuration = ScriptableObject.CreateInstance<Configuration>();
                humanoidPath = humanoidPath.Substring(0, humanoidPath.Length - 1); // strip last /
                humanoidPath = humanoidPath.Substring(0, humanoidPath.LastIndexOf('/') + 1); // strip Scripts;
                string path = "Assets" + humanoidPath + configurationString + ".asset";
                AssetDatabase.CreateAsset(configuration, path);
            }

            configuration = (Configuration)EditorGUILayout.ObjectField("Configuration", configuration, typeof(Configuration), false);
            SerializedObject serializedConfiguration = new SerializedObject(configuration);

            bool anyChanged = Configuration_Editor.ConfigurationGUI(configuration, serializedConfiguration);
            serializedConfiguration.ApplyModifiedProperties();
            if (configuration != null) {
                if (GUI.changed) {
                    configurationString = configuration.name;
                    EditorPrefs.SetString("HumanoidConfigurationKey", configurationString);
                }

                if (GUI.changed || anyChanged) {
                    EditorUtility.SetDirty(configuration);
                    Configuration_Editor.CheckExtensions(configuration);
                }
            }
        }
#endif

        private static void LoadConfiguration(string configurationName) {
            string[] foundAssets = AssetDatabase.FindAssets(configurationName + " t:Configuration");
            if (foundAssets.Length == 0)
                return;

            string path = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
            configuration = AssetDatabase.LoadAssetAtPath<Configuration>(path);
        }
    }

}
#endif