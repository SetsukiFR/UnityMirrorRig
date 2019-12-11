using UnityEditor;
using UnityEngine;

namespace MirrorRigTools
{
	[InitializeOnLoad]
public class Startup
{
	static Startup()
	{
		MirrorToolSettings.MirrorRigSettings.Write();
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
			},
		};

		return provider;
	}

	// Create a new type of Settings Asset.
	public class MirrorRigSettings : ScriptableObject
	{
		public const string k_MyCustomSettingsPath = "Assets/UnityMirrorRig/Editor/Settings.asset";

		public string[] LEFT_END = new string[] { "_l", ".l", "_L", ".L"," L", "left","Left " };
		public string[] RIGHT_END = new string[] { "_r", ".r", "_R", ".R", " R", "right", "Right" };

		public string[] LEFT_START = new string[] { "l_", "l_", "L_", "L_", "L ", "l ", "left", "Left" };
		public string[] RIGHT_START = new string[] { "r_", "r_", "R_", "R_", "R ", "r ", "right", "Right" };



		internal static MirrorRigSettings GetOrCreateSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<MirrorRigSettings>(k_MyCustomSettingsPath);
			if (settings == null)
			{
				settings = ScriptableObject.CreateInstance<MirrorRigSettings>();
				settings.LEFT_END = new string[] { "_l", ".l", "_L", ".L", "L Target", "l Target", " L", "left 1", "left 2", "left 3", "left 4", "left 5", "left adj", "left 1 adj", "left 2 adj", "left upper adj 1", "left upper adj 2", "left upper adj 3", "left upper adj 4", "left lower adj 1", "left lower adj 2", "left lower adj 3", "left lower adj 4" };
				settings.RIGHT_END = new string[] { "_r", ".r", "_R", ".R", "R Target", "r Target", " R", "right 1", "right 2", "right 3", "right 4", "right 5", "right adj", "right 1 adj", "right 2 adj", "right upper adj 1", "right upper adj 2", "right upper adj 3", "right upper adj 4", "right lower adj 1", "right lower adj 2", "right lower adj 3", "right lower adj 4" };

				settings.LEFT_START = new string[] { "l_", "L_", "L ", "l ", "left ", "Left" };
				settings.RIGHT_START = new string[] { "r_", "R_", "R ", "r ", "right ", "Right" };

				AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		internal static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}

		internal static void Write()
		{
			var settings = GetOrCreateSettings();
			MirrorRig.LEFT_END = settings.LEFT_END;
			MirrorRig.LEFT_START = settings.LEFT_START;
			MirrorRig.RIGHT_END = settings.RIGHT_END;
			MirrorRig.RIGHT_START= settings.RIGHT_START;
		}
	}
}
}