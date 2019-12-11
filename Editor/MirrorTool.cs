using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[CustomEditor(typeof(Transform), true)]
[CanEditMultipleObjects]
public class MirrorTool : Editor
{
	private const string MIRROR_SHORTCUT_ID = "MirrorTool/Mirror";
	[Shortcut(MIRROR_SHORTCUT_ID, KeyCode.M)]
    static void Mirror(ShortcutArguments args) {
		_mirrorEnabled = !_mirrorEnabled;
		if(_target!=null)EditorUtility.SetDirty(_target);
	}

	private static bool _mirrorEnabled=true;
	private static bool _mirrorPos = true;
	private static bool _mirrorSca = true;
	private static bool _mirrorRot = true;

	//Unity's built-in editor
	Editor _defaultEditor;
	Vector3 _originalPosition;
	Quaternion _originalRotation;
	Vector3 _originalScale;
	static Transform _target;
	void OnEnable()
	{
		//When this inspector is created, also create the built-in inspector
		_defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));

		//calculate rotation offsets
		_originalPosition = ((Transform)target).position;
		_originalScale= ((Transform)target).localScale;
		_originalRotation= ((Transform)target).rotation;
		_target = ((Transform)target);
		_mirrorEnabled = false;
	}

	void OnDisable()
	{
		//When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
		//Also, make sure to call any required methods like OnDisable
		MethodInfo disableMethod = _defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		if (disableMethod != null)
			disableMethod.Invoke(_defaultEditor, null);
		DestroyImmediate(_defaultEditor);
	}

	public override void OnInspectorGUI()
	{
		bool bHasMirror = false;

		foreach (var obj in targets)
		{

			Transform t = ((Transform)obj);
			MirrorRig rig = t.GetComponentInParent<MirrorRig>();
			if (rig != null)
				bHasMirror = true;
		}

		#region base tools
		if (bHasMirror)
		{
			EditorGUILayout.BeginHorizontal();

			_mirrorEnabled = EditorGUILayout.Toggle("Mirror (M)", _mirrorEnabled);

			if (GUILayout.Button("Select Mirror"))
			{
				List<UnityEngine.Object> selection = new List<UnityEngine.Object>();
				foreach (var obj in targets)
				{
					Transform mirror = ((Transform)obj).GetComponentInParent<MirrorRig>()?.GetMirrorTransform((Transform)obj);

					if (mirror != null)
						selection.Add(mirror.gameObject);
				}
				if (selection.Count > 0)
				{
					Selection.objects = selection.ToArray();
				}
			}
			EditorGUILayout.EndHorizontal();
			if (_mirrorEnabled)
			{
				_mirrorPos = EditorGUILayout.Toggle("Mirror Position", _mirrorPos);
				_mirrorRot = EditorGUILayout.Toggle("Mirror Rotation", _mirrorRot);
				_mirrorSca = EditorGUILayout.Toggle("Mirror Scale", _mirrorSca);
			}
		}
		#endregion

		_defaultEditor.OnInspectorGUI();
		if (!bHasMirror)
			return;

		Transform referenceTarget = ((Transform)target);
		//mirror the values
		if (_mirrorEnabled &&
			(referenceTarget.position != _originalPosition
			|| referenceTarget.rotation != _originalRotation
			|| referenceTarget.localScale != _originalScale
			))
		{
			_originalPosition = referenceTarget.position;
			_originalRotation = referenceTarget.rotation;
			_originalScale = referenceTarget.localScale;
			Plane mirror;
			foreach (var obj in targets)
			{

				Transform t = ((Transform)obj);
				MirrorRig rig = t.GetComponentInParent<MirrorRig>();
				if (rig == null)
					return;
				Transform m = rig.GetMirrorTransform(t);
				if (m == null)
					return;

				Vector3 lookatTarget,up;

				mirror = rig.GetMirrorPlane(t);

				//we have a mirror for this object
				Undo.RecordObject(m, "Mirror");
				if (_mirrorPos)
				{
					m.position = MirrorRig.ReflectionOverPlane(t.position, mirror);

				}
				if (_mirrorSca)
				{
					m.localScale = t.localScale;
				}
				if (_mirrorRot)
				{
					//we're going to calculate rotation based on lookat
					lookatTarget = t.position + t.forward;
					lookatTarget = MirrorRig.ReflectionOverPlane(lookatTarget, mirror);
					up = t.position + t.up;
					up = MirrorRig.ReflectionOverPlane(up, mirror);

					m.LookAt(lookatTarget, up - m.position);

					m.Rotate(rig.GetRotationOffset(t));
				}
				
			}
		}
	}
	private void OnSceneGUI()
	{
		if (_mirrorEnabled)
		{
			Transform t = ((Transform)target);
			MirrorRig rig = t.GetComponentInParent<MirrorRig>();
			if (rig == null)
				return;
			Transform m = rig.GetMirrorTransform(t);
			if (m != null)
			{
				Handles.color = Color.red;
				Handles.DrawLine(m.position, m.position + m.right * .05f);
				Handles.color = Color.green;
				Handles.DrawLine(m.position, m.position + m.up * .05f);
				Handles.color = Color.blue;
				Handles.DrawLine(m.position, m.position + m.forward * .05f);
			}
		}
	}
}
