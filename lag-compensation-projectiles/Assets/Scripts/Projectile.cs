using FishNet;
using FishNet.Managing.Timing;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace LagCompensationProjectiles
{
	public class Projectile : MonoBehaviour
	{
		[HideInInspector] public int Id;
		[HideInInspector] public PlayerFire Owner;
		[HideInInspector] public bool IsOwner;

		[SerializeField] protected int Damage = 10;
		[SerializeField] float _moveRate = 10f;
		[SerializeField] GameObject _vfxPrefab;
		[SerializeField] bool _useGravity;
		[SerializeField] bool useServerHitValidation = true;

		protected const float CatchupRate = 0.08f;
		protected float PassedTime;
		protected float PassedTimeDelta;
		protected float Radius;
		protected Vector3 Direction;
		protected Vector3 Velocity;
		protected Collider Collider;

		// TODO: Add lifetime

		public void Initialize(int id, Vector3 direction, float passedTime, PlayerFire owner)
		{
			Id = id;
			Owner = owner;
			IsOwner = owner != null;

			Direction = direction;
			PassedTime = passedTime;
		}

		void Awake()
		{
			Setup();
		}

		void Update()
		{
			Move();
			RotateTowardsDirection();
		}

		void Setup()
		{
			// Should use SphereCollider in 99.9% of the cases
			if (TryGetComponent<SphereCollider>(out var sphereCollider))
			{
				Radius = sphereCollider.radius;
				Collider = sphereCollider;
			}
			else if (TryGetComponent<CapsuleCollider>(out var capsuleCollider))
			{
				Radius = capsuleCollider.radius;
				Collider = capsuleCollider;
			}
			else if (TryGetComponent<Collider>(out Collider))
			{
				Radius = Collider.bounds.size.x / 2f;
			}
			else
			{
				Debug.LogError($"No collider found on {gameObject.name}.");
			}
		}

		void Move()
		{
			var delta = Time.deltaTime;

			PassedTimeDelta = 0f;
			if (PassedTime > 0f)
			{
				var step = (PassedTime * CatchupRate);
				PassedTime -= step;

				if (PassedTime <= (delta / 2f))
				{
					step += PassedTime;
					PassedTime = 0f;
				}
				PassedTimeDelta = step;
			}


			Velocity = Direction * _moveRate;
			if (_useGravity)
			{
				Velocity += Physics.gravity * (delta + PassedTimeDelta);
			}

			Direction = Velocity.normalized;
			transform.position += Velocity * (delta + PassedTimeDelta);
		}

		void RotateTowardsDirection()
		{
			transform.forward = Direction;
		}

		[Client]
		void OnTriggerEnter(Collider otherCollider)
		{
			Instantiate(_vfxPrefab, transform.position, Quaternion.identity);

			if (IsOwner)
			{
				if (!otherCollider.TryGetComponentInParent<IDamageable>(out var damageable))
				{
					return;
				}
				
				if (useServerHitValidation && otherCollider.TryGetComponentInParent<NetworkObject>(out var networkObject))
				{
					var preciseTick = InstanceFinder.TimeManager.GetPreciseTick(InstanceFinder.TimeManager.LastPacketTick);
					CombatManager.Instance.ServerHitWithValidationSingle(Owner, networkObject.OwnerId, transform.position, Radius, preciseTick, Damage);
				}
				else
				{
					damageable?.TakeDamage(Damage);
				}
				
				CombatManager.Instance.ServerDespawnProjectile(Id);
			}

			CombatManager.Instance.UnregisterProjectile(Id);
			Destroy(gameObject);
		}
	}
}
