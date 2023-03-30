using FishNet;
using UnityEngine;

namespace LagCompensationProjectiles
{
	public class PredictedProjectile : MonoBehaviour
	{
		[SerializeField] float moveRate = 10f;

		Vector3 _direction;
		float _passedTime;

		public void Initialize(Vector3 direction, float passedTime)
		{
			_direction = direction;
			_passedTime = passedTime;
		}

		void Update()
		{
			Move();
		}

		void Move()
		{
			var delta = Time.deltaTime;
			var passedTimeDelta = 0f;
			if (_passedTime > 0f)
			{
				var step = (_passedTime * 0.08f);
				_passedTime -= step;

				if (_passedTime <= (delta / 2f))
				{
					step += _passedTime;
					_passedTime = 0f;
				}
				passedTimeDelta = step;
			}

			transform.position += _direction * (moveRate * (delta + passedTimeDelta));
		}

		void OnTriggerEnter(Collider otherCollider)
		{
			/* These projectiles are instantiated locally, as in,
			 * they are not networked. Because of this there is a very
			 * small chance the occasional projectile may not align with
			 * 100% accuracy. But, the differences are generally
			 * insignifcant and will not affect gameplay. */

			if (InstanceFinder.IsClient)
			{
				//Show VFX.
				//Play Audio.
			}
			if (InstanceFinder.IsServer)
			{
				var health = otherCollider.GetComponentInParent<Health>();
				if (health)
				{
					health.TakeDamage(10);
					if (InstanceFinder.NetworkManager.gameObject.TryGetComponent<AudioSource>(out var audioSource))
					{
						audioSource.Play();
					}
				}
			}

			Destroy(gameObject);
		}
	}
}
