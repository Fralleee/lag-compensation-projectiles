using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LagCompensationProjectiles
{
	public static class TransformExtensions
	{
		public static bool TryGetComponentInParent<T>(this Transform transform, out T component) where T : class
		{
			if (transform.parent != null)
			{
				component = transform.parent.GetComponentInParent<T>();
				return component != null;
			}

			component = null;
			return false;
		}

		public static bool TryGetComponentInParent<T>(this Collider collider, out T component) where T : class
		{
			var transform = collider.transform;
			if (transform.parent != null)
			{
				component = transform.parent.GetComponentInParent<T>();
				return component != null;
			}

			component = null;
			return false;
		}
	}
}
