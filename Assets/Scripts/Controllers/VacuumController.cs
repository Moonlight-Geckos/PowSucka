using UnityEngine;

public class VacuumController : MonoBehaviour
{
    [SerializeField]
    private Transform liquidFill;

    [SerializeField]
    private ParticleSystem collectedPRJPS;

    private void Awake()
    {
        liquidFill.localScale = new Vector3(1, 0, 1);
        EventsPool.PickedupProjectileEvent.AddListener(PickupProjectile);
    }
    private void PickupProjectile(Projectile prj)
    {
        if (liquidFill.localScale.y < 1)
        {
            liquidFill.localScale = liquidFill.localScale + new Vector3(0, 0.01f, 0);
            collectedPRJPS.Play();
        }
    }
}