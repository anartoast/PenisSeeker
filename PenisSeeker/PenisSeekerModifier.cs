using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace PenisSeeker;


public class PenisSeekerModifier : MonoBehaviour {

    // This class handles runtime instanced modification of fired penis balls, allowing you to set variables
    // of the penis blast when fired.

    public void Start()
    {
        ProjectileController projectileController = gameObject.GetComponent<ProjectileController>();

        if (projectileController == null) {
            Log.Debug("Help me twin. Help me.");
            return;
        }

        if (projectileController.owner == null || projectileController.owner.GetComponent<CharacterBody>() == null) {
            Log.Debug("Seekin' mah penits.");
            return;
        }

        ProjectileDirectionalTargetFinder projectileDirectionalTargetFinder = gameObject.GetComponent<ProjectileDirectionalTargetFinder>();
        ProjectileSteerTowardTarget projectileSteerTowardTarget = gameObject.GetComponent<ProjectileSteerTowardTarget>();
        ProjectileSimple projectileFuckingSimple = gameObject.GetComponent<ProjectileSimple>();
        int chakras = projectileController.owner.GetComponent<CharacterBody>().GetBuffCount(DLC2Content.Buffs.ChakraBuff);

        if (projectileDirectionalTargetFinder)
        {
            projectileDirectionalTargetFinder.lookRange = 20 + (3 * chakras); // 3, 6,
            projectileDirectionalTargetFinder.lookCone = 7 + (2 * chakras);
        }

        if (projectileSteerTowardTarget)
        {
            projectileSteerTowardTarget.rotationSpeed = 20f * chakras;
        }

        if (projectileFuckingSimple)
        {
            projectileFuckingSimple.velocityOverLifetime = AnimationCurve.Linear(1, 1, 2, 1 * chakras * 15);
        }
    }
}
