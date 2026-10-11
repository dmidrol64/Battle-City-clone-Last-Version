using UnityEngine;

public class DestroyBricks : MonoBehaviour,IDamageable
{
    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        
            Destroy(gameObject);
        
    }
}
