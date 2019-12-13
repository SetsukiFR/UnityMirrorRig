/**
 * 
 * AUTHOR : Vincent Paquin
 * Licence : CC0, although I wouldn't mind hearing about your project
 * contact : vincent.paquin@gmail.com
 * 
 */

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MirrorRigTools
{
	public class MirrorRig : MonoBehaviour
	{
		public static Vector3 ReflectionOverPlane(Vector3 point, Plane plane)
		{
			Vector3 pointOnPlane = plane.ClosestPointOnPlane(point);
			return pointOnPlane + (pointOnPlane - point);
		}

		[System.Serializable]
		public class MirrorPair
		{
			public string name;
			public Vector3 offset;
			public Transform left;
			public Transform right;
			public Transform commonParent;
			public Vector3 planeNormal;
			public MirrorPair(Transform left, Transform right, string name)
			{
				this.left = left;
				this.right = right;
				this.name = name;
				FindCommonParent();
				planeNormal = left.position - right.position;
				planeNormal = Quaternion.Inverse(commonParent.rotation) * planeNormal;
				CalculateOffset();
			}

			private void CalculateOffset()
			{
				Plane mirror = GetMirror();

				Vector3 lookatTarget = left.position + left.forward;
				lookatTarget = ReflectionOverPlane(lookatTarget, mirror);


				Vector3 up = left.position + left.up;
				up = ReflectionOverPlane(up, mirror);

				Quaternion originalRotation = right.rotation;

				right.LookAt(lookatTarget, up - right.position);

				offset = -(Quaternion.Inverse(originalRotation) * right.rotation).eulerAngles;

				right.rotation = originalRotation;
			}

			private void FindCommonParent()
			{
				Transform parentL = left.parent;
				while (parentL != null)
				{
					Transform parentR = right.parent;
					while (parentR != null)
					{
						if (parentL == parentR)
						{
							commonParent = parentL;
							return;
						}
						parentR = parentR.parent;
					}
					parentL = parentL.parent;
				}
			}

			public Plane GetMirror()
			{
				Vector3 normal = commonParent.rotation * this.planeNormal;

				return new Plane(normal, commonParent.position);
			}
		}
		[SerializeField] private List<MirrorPair> _pairs = new List<MirrorPair>();

#if UNITY_EDITOR
		public static string[] LEFT_END;
		public static string[] RIGHT_END;
		public static string[] LEFT_START;
		public static string[] RIGHT_START;


		private void Reset()
		{
			_pairs.Clear();
			GetAllMirrorBones(transform);
		}

		private void GetAllMirrorBones(Transform transform)
		{
			if (GetMirrorObject(transform, out var result))
			{
				MirrorPair pair = new MirrorPair(result.left, result.right, result.name);
				_pairs.Add(pair);
			}
			foreach (Transform t in transform)
			{
				GetAllMirrorBones(t);
			}
		}


		private bool GetMirrorObject(Transform t, out (Transform left, Transform right, string name) result, (List<int> ignoreStart, List<int> ignoreEnd)? ignores = null)
		{
			if (ignores == null)
			{
				ignores = (new List<int>(), new List<int>());
			}
			result.left = null;
			result.right = null;
			result.name = null;

			int index = -1;

			string replaceFrom = string.Empty, replaceTo = string.Empty;

			for (int i = 0; i < LEFT_END.Length; ++i)
			{
				if (!ignores.Value.ignoreEnd.Contains(i) && t.name.EndsWith(LEFT_END[i]))
				{
					index = i;
					replaceFrom = LEFT_END[i];
					replaceTo = RIGHT_END[i];
					result.left = t;
					result.name = t.name.Remove(t.name.Length - LEFT_END[i].Length);
					break;
				}
			}

			for (int i = 0; i < LEFT_START.Length; ++i)
			{
				if (!ignores.Value.ignoreStart.Contains(i) && t.name.StartsWith(LEFT_START[i]))
				{
					index = i;
					replaceFrom = LEFT_START[i];
					replaceTo = RIGHT_START[i];
					result.name = t.name.Remove(0, LEFT_START[i].Length);
					result.left = t;
					break;
				}
			}



			if (index == -1)
			{
				//we don't know this suffix
				return false;
			}

			string mirrorName = AnimationUtility.CalculateTransformPath(t, t.root).Replace(replaceFrom, replaceTo);
			Transform mirror = t.root.Find(mirrorName);
			if (mirror != null)
			{
				if (result.left != null)
					result.right = mirror;
				else
					result.left = mirror;
				return true;
			}
			return GetMirrorObject(t, out result, ignores);
		}

		public Transform GetMirrorTransform(Transform t)
		{
			foreach (var pair in _pairs)
			{
				if (pair.left == t)
				{
					return pair.right;
				}
				if (pair.right == t)
				{
					return pair.left;
				}
			}
			return null;
		}

		public Plane GetMirrorPlane(Transform t)
		{
			foreach (var pair in _pairs)
			{
				if (pair.right == t || pair.left == t)
				{
					return pair.GetMirror();
				}
			}
			return new Plane();
		}

		public Vector3 GetRotationOffset(Transform t)
		{
			foreach (var pair in _pairs)
			{
				if (pair.right == t || pair.left == t)
				{
					return pair.offset;
				}
			}
			return Vector3.zero;
		}
#endif
	}
}