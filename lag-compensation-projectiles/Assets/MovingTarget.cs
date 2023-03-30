using FishNet.Object;
using UnityEngine;

namespace VersausSurvival
{
	public class MovingTarget : NetworkBehaviour
	{
		[SerializeField] float _speed = 2f;
		[SerializeField] float _distance = 5f;

		Vector3 _startPosition;
		float _direction = 1f;

		void Start()
		{
			_startPosition = transform.position;
		}

		void Update()
		{
			if (!IsServer)
			{
				return;
			}

			var movement = _direction * _speed * Time.deltaTime;
			var rightDirection = transform.right;

			var newPosition = transform.position + (rightDirection * movement);

			if (Mathf.Abs(Vector3.Dot(newPosition - _startPosition, rightDirection)) >= _distance)
			{
				_direction *= -1;
				var overshoot = Mathf.Abs(Vector3.Dot(newPosition - _startPosition, rightDirection)) - _distance;
				newPosition = transform.position + (rightDirection * _direction * overshoot);
			}

			transform.position = newPosition;
		}
	}
}
