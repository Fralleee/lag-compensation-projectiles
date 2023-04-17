using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace LagCompensationProjectiles
{
    public class HealthController : NetworkBehaviour, IDamageable
    {
		public int MaxHealth = 100;
		[SyncVar] public int CurrentHealth = 100;

		[ServerRpc(RequireOwnership = false)]
        public void TakeDamage(int damage)
        {
	        CurrentHealth -= damage;
	        if (CurrentHealth <= 0)
			{
				CurrentHealth = MaxHealth;
			}
        }
    }
}
