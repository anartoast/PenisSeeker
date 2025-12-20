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

namespace PenisSeeker;

public class PENISBLAST : BaseState, SteppedSkillDef.IStepSetter
{
    
    public GameObject projectileprefab = PenisSeekerPlugin.seekerProjectilePrefab;
    private GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Seeker.SpiritPunchMuzzleFlashVFX_prefab).WaitForCompletion();
    private Transform muzzleTransform;
    private Animator animator;
    private ChildLocator childLocator;
    private Gauntlet gauntlet;
    private float dmgBuffIncrease = 0.5f;
    public float comboDamageCoefficient = 1;
    private float baseDuration = 1;
    private float paddingBetweenAttacks = 0.3f;
    private string attackSoundString = "Play_seeker_skill1_fire";
    private string attackSoundStringAlt = "Play_seeker_skill1_fire_orb";
    private float attackSoundPitch = 0;
    private float bloom = 0;
    private float recoilAmplitude = 0;
    private float duration; //Controls ANIMATION duration btw smh
    private bool hasFiredGauntlet;
    private string muzzleString;
    private float extraDmgFromBuff;
    private string animationStateName;
    private float DamageCoefficient { get; set; }
    private static event Action<bool> onSpiritOrbFired;
    public float force = 3075f; // i want to make ts scale off chakra aswell
    
    private enum Gauntlet // what .,.,,.
    {
        Explode
    }

    public void SetStep(int i)
    {
        gauntlet = (Gauntlet)i;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        duration = baseDuration / attackSpeedStat;
        characterBody.SetAimTimer(2f);
        animator = GetModelAnimator();
        
        if (animator)
        {
            childLocator = animator.GetComponent<ChildLocator>();
            
            muzzleString = "MuzzleEnergyBomb";
            animationStateName = "SpiritPunchFinisher";
            extraDmgFromBuff = dmgBuffIncrease * characterBody.GetBuffCount(DLC2Content.Buffs.ChakraBuff);
        }
        
        if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
        {
            PlayCrossfade("FullBody, Override", animationStateName, "FireGauntlet.playbackRate", duration, 0.025f);
            return;
        }
        
        PlayCrossfade("Gesture, Additive", animationStateName, "FireGauntlet.playbackRate", duration, 0.025f);
        PlayCrossfade("Gesture, Override", animationStateName, "FireGauntlet.playbackRate", duration, 0.025f);
    }

    private void FireGauntlet()
    {
        if (hasFiredGauntlet) return;
        
        characterBody.AddSpreadBloom(bloom);
        Ray ray = GetAimRay();
        TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref ray, projectileprefab, gameObject);
        
        if ((bool)childLocator)
        {
            muzzleTransform = childLocator.FindChild(muzzleString);
        }
        
        if ((bool)muzzleflashEffectPrefab)
        {
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, muzzleString, transmit: false);
        }
        
        if (isAuthority)
        {
            float damage = damageStat * (DamageCoefficient + extraDmgFromBuff);
            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
            {
                projectilePrefab = PenisSeekerPlugin.seekerProjectilePrefab,
                position = muzzleTransform.position,
                rotation = Util.QuaternionSafeLookRotation(ray.direction),
                owner = gameObject,
                damage = damage,
                force = force,
                crit = Util.CheckRoll(critStat, characterBody.master),
                damageColorIndex = DamageColorIndex.Default,
                speedOverride = 70f + attackSpeedStat * 2f,
                damageTypeOverride = DamageTypeCombo.GenericPrimary
            };
            
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }
        
        AddRecoil(0.2f * recoilAmplitude, 0.1f * recoilAmplitude, -1f * recoilAmplitude, 1f * recoilAmplitude);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (!(fixedAge >= duration - duration * 0.40f) && !hasFiredGauntlet) return;
        
        if (gauntlet == Gauntlet.Explode && !hasFiredGauntlet)
        {
            Util.PlayAttackSpeedSound(attackSoundStringAlt, gameObject, attackSoundPitch);
            projectileprefab = PenisSeekerPlugin.seekerProjectilePrefab;
            DamageCoefficient = comboDamageCoefficient;
            FireGauntlet();
            onSpiritOrbFired?.Invoke(true);
            hasFiredGauntlet = true;
        }
        else if (!hasFiredGauntlet)
        {
            Util.PlayAttackSpeedSound(attackSoundString, gameObject, attackSoundPitch);
            FireGauntlet();
            hasFiredGauntlet = true;
            onSpiritOrbFired?.Invoke(false);
        }
        
        if (isAuthority && fixedAge >= duration + duration * paddingBetweenAttacks)
        {
            outer.SetNextStateToMain();
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
}




