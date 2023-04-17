using System.Collections.Generic;
using System.Linq;
using FishNet.Component.ColliderRollback;
using FishNet.Connection;
using FishNet.Managing.Timing;
using FishNet.Object;
using UnityEngine;

namespace LagCompensationProjectiles
{
    public class CombatManager : NetworkedSingleton<CombatManager>
    {
		public const float RadiusThreshold = 0.5f;
	    public Dictionary<int, Projectile> ActiveProjectiles = new();

		public bool RegisterProjectile(int id, Projectile projectile)
		{
			return ActiveProjectiles.TryAdd(id, projectile);
		}

	    public bool UnregisterProjectile(int id)
	    {
		    if (ActiveProjectiles.TryGetValue(id, out var projectile))
		    {
			    if (projectile != null)
			    {
				    Destroy(projectile.gameObject);
				}
				
			    return ActiveProjectiles.Remove(id);
			}

		    return false;
	    }

	    public Projectile GetProjectile(int id)
	    {
		    return ActiveProjectiles.TryGetValue(id, out var projectile) ? projectile : null;
	    }

	    [ServerRpc(RequireOwnership = false)]
	    public void ServerDespawnProjectile(int projectileId)
	    {
		    Instance.ObserversDespawnProjectile(projectileId);
	    }

		[ObserversRpc]
	    void ObserversDespawnProjectile(int projectileId)
	    {
		    UnregisterProjectile(projectileId);
	    }

	    [ServerRpc(RequireOwnership = false)]
		public void ServerHitWithValidationSingle(PlayerFire shooter, int targetNetworkId, Vector3 position, float radius, PreciseTick preciseTick, int damage)
	    {
			Instance.RollbackManager.Rollback(preciseTick, RollbackManager.PhysicsType.ThreeDimensional, Instance.IsOwner);
			
			var colliders = Physics.OverlapSphere(position, radius + RadiusThreshold, LayerMask.GetMask("Hostile", "Ally"));
			foreach (var col in colliders)
			{
				if (!col.TryGetComponentInParent<NetworkObject>(out var networkObject) || networkObject.OwnerId != targetNetworkId)
				{
					continue;
				}

				if (networkObject.TryGetComponent<HealthController>(out var healthController))
				{
					healthController.TakeDamage(damage);
					shooter.SuccessfulHit(shooter.Owner);
					break;
				}
			}

			Instance.RollbackManager.Return();
	    }
	}
}
