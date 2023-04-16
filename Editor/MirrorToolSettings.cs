using UnityEditor;
using UnityEngine;

namespace MirrorRigTools
{
	[InitializeOnLoad]
	public class Startup
	{
		static Startup()
		{
			MirrorRigSettings.Write();
		}
	}
	static class MirrorToolSettings
	{
		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			// First parameter is the path in the Settings window.
			// Second parameter is the scope of this setting: it only appears in the Project Settings window.
			var provider = new SettingsProvider("Project/Mirror Rig Settings", SettingsScope.Project)
			{
				// By default the last token of the path is used as display name if no label is provided.
				label = "Mirror Rig Settings",
				// Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
				guiHandler = (searchContext) =>
				{
					var settings = MirrorRigSettings.GetSerializedSettings();
					EditorGUILayout.LabelField("Mirror Objects beginning with :");
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(settings.FindProperty("LEFT_START"), new GUIContent("Left"), true);
					EditorGUILayout.PropertyField(settings.FindProperty("RIGHT_START"), new GUIContent("Right"), true);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Mirror Objects ending with :");
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(settings.FindProperty("LEFT_END"), new GUIContent("Left"), true);
					EditorGUILayout.PropertyField(settings.FindProperty("RIGHT_END"), new GUIContent("Right"), true);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Mirror Objects containing :");
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(settings.FindProperty("LEFT_CONTAINS"), new GUIContent("Left"), true);
					EditorGUILayout.PropertyField(settings.FindProperty("RIGHT_CONTAINS"), new GUIContent("Right"), true);
					EditorGUILayout.EndHorizontal();
				},
			};

			return provider;
		}
	}
}