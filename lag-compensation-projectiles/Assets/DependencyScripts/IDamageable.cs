using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LagCompensationProjectiles
{
    public interface IDamageable
    {
        public void TakeDamage(int damage);
    }
}
