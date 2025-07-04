
using EntityStates;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace PenisSeeker

{

    public class PENISBLAST : BaseState, SteppedSkillDef.IStepSetter
    {

        public enum Gauntlet
        {

            Explode
        }



        [SerializeField]
        public GameObject projectileprefab = plugin.exampleProjectilePrefab;


        [SerializeField]
        public GameObject projectileFinisherPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/SpiritPunchFinisherProjectile.prefab").WaitForCompletion();

        [SerializeField]
        public GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/SpiritPunchMuzzleFlashVFX.prefab").WaitForCompletion();

        [SerializeField]
        public float procCoefficient = 1;

        [SerializeField]
        public float damageCoefficient = 3;

        [SerializeField]
        public float dmgBuffIncrease = 0.5f;

        [SerializeField]
        public float comboDamageCoefficient = 1;

        [SerializeField]
        public float force = 3075f;

        public static float attackSpeedAltAnimationThreshold = 1;

        [SerializeField]
        public float baseDuration = 1;

        [SerializeField]
        public float paddingBetweenAttacks = 0.3f;

        [SerializeField]
        public string attackSoundString;

        [SerializeField]
        public string attackSoundStringAlt;

        [SerializeField]
        public float attackSoundPitch;

        [SerializeField]
        public bool enableVelocityOverLifetime;


        public float lifetime;

        public bool oscillate;
        //Enable gay mode for projectile

        public float bloom;

        private float duration;
        //Controls ANIMATION duration btw smh

        private bool hasFiredGauntlet;

        private string muzzleString;

        private Transform muzzleTransform;

        private Animator animator;

        private ChildLocator childLocator;

        private Gauntlet gauntlet;

        private float extraDmgFromBuff;

        public static float recoilAmplitude;

        private string animationStateName;

        [field: SerializeField]
        public float DamageCoefficient { get; set; }

        public static event Action<bool> onSpiritOrbFired;

        public void SetStep(int i)
        {
            gauntlet = (Gauntlet)i;
        }

        public override void OnEnter()
        {



            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.characterBody.SetAimTimer(2f);
            animator = GetModelAnimator();
            if ((bool)animator)
            {
                childLocator = animator.GetComponent<ChildLocator>();


                muzzleString = "MuzzleEnergyBomb";
                animationStateName = "SpiritPunchFinisher";
                extraDmgFromBuff = dmgBuffIncrease * (float)base.characterBody.GetBuffCount(DLC2Content.Buffs.ChakraBuff);

            }

            bool num = animator.GetBool("isMoving");
            bool flag = animator.GetBool("isGrounded");
            if (!num && flag)
            {
                PlayCrossfade("FullBody, Override", animationStateName, "FireGauntlet.playbackRate", duration, 0.025f);
                return;
            }
            PlayCrossfade("Gesture, Additive", animationStateName, "FireGauntlet.playbackRate", duration, 0.025f);
            PlayCrossfade("Gesture, Override", animationStateName, "FireGauntlet.playbackRate", duration, 0.025f);

            int chakras = characterBody.GetBuffCount(DLC2Content.Buffs.ChakraBuff);
            var simplefuckingprojectile = plugin.exampleProjectilePrefab.GetComponent<ProjectileSimple>();
            if (simplefuckingprojectile)
            {
                simplefuckingprojectile.updateAfterFiring = characterBody.GetBuffCount(DLC2Content.Buffs.ChakraBuff) > (3);
                simplefuckingprojectile.enableVelocityOverLifetime = true;
                simplefuckingprojectile.velocityOverLifetime = AnimationCurve.Linear(1, 1, 4, 3 * chakras * 3);
                // Controls the projectile velocity overlifetime. First number is frame, second is starting velocity, third is how long it exists for, fourth is ending velocity
                simplefuckingprojectile.oscillateMagnitude = 15;
                //Controll how gay the prsojectile is
                simplefuckingprojectile.lifetime = 60f;
            }
        }





        private void FireGauntlet()
        {
            if (!hasFiredGauntlet)
            {
                base.characterBody.AddSpreadBloom(bloom);
                Ray ray = GetAimRay();
                TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref ray, projectileprefab, base.gameObject);
                if ((bool)childLocator)
                {
                    muzzleTransform = childLocator.FindChild(muzzleString);
                }
                if ((bool)muzzleflashEffectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, muzzleString, transmit: false);
                }
                if (base.isAuthority)
                {
                    float damage = damageStat * (DamageCoefficient + extraDmgFromBuff);
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = plugin.exampleProjectilePrefab,
                        position = muzzleTransform.position,
                        rotation = Util.QuaternionSafeLookRotation(ray.direction),
                        owner = base.gameObject,
                        damage = damage,
                        force = force,
                        crit = Util.CheckRoll(critStat, base.characterBody.master),
                        damageColorIndex = DamageColorIndex.Default,
                        speedOverride = 70f + attackSpeedStat * 2f,
                        damageTypeOverride = DamageTypeCombo.GenericPrimary
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
                AddRecoil(0.2f * recoilAmplitude, 0.1f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
            }
        }

        public override void FixedUpdate()
        {

            base.FixedUpdate();
            if (base.fixedAge >= duration - duration * 0.40f || hasFiredGauntlet)
            {
                if (gauntlet == Gauntlet.Explode && !hasFiredGauntlet)
                {
                    Util.PlayAttackSpeedSound(attackSoundStringAlt, base.gameObject, attackSoundPitch);
                    projectileprefab = plugin.exampleProjectilePrefab;
                    DamageCoefficient = comboDamageCoefficient;
                    FireGauntlet();
                    PENISBLAST.onSpiritOrbFired?.Invoke(obj: true);
                    _ = base.characterMotor.isGrounded;
                    hasFiredGauntlet = true;
                }
                else if (!hasFiredGauntlet)
                {
                    Util.PlayAttackSpeedSound(attackSoundString, base.gameObject, attackSoundPitch);
                    FireGauntlet();
                    hasFiredGauntlet = true;
                    PENISBLAST.onSpiritOrbFired?.Invoke(obj: false);
                }
                if (base.isAuthority && base.fixedAge >= duration + duration * paddingBetweenAttacks)
                {
                    outer.SetNextStateToMain();
                }
            }
        }


        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)gauntlet);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            gauntlet = (Gauntlet)reader.ReadByte();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            //Interrupted by anything
            return InterruptPriority.Skill;
        }
        //public override void OnExit()
        // {
        // Debug.Log("HELP: ");
        // base.OnExit();
        // }
    }
}



