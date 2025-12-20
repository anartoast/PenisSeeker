using BepInEx;
using EntityStates;
using R2API;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using BepInEx.Bootstrap;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace PenisSeeker
{
    [BepInDependency("com.cwmlolzlz.skills", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("iDeathHD.UnityHotReload", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class PenisSeekerPlugin : BaseUnityPlugin
    {
        private const string PluginGUID = PluginAuthor + "toastyteam" + PluginName;
        private const string PluginAuthor = "toastyteam";
        private const string PluginName = "PenisSeeker";
        private const string PluginVersion = "1.2.2";

        public static GameObject seekerProjectilePrefab;
        public static bool SPPInstalled => Chainloader.PluginInfos.ContainsKey("com.cwmlolzlz.skills");
        public static bool UHRInstalled => Chainloader.PluginInfos.ContainsKey("iDeathHD.UnityHotReload");

        public void Awake()
        {
            Log.Init(Logger);
            Log.Debug("PENIS BLAST!!");

            AssetBundle penisBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "penisblast"));
            GameObject SeekerBodyPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Seeker.SeekerBody_prefab).WaitForCompletion();
            SkillDef PenisSeekerSkillDef = ScriptableObject.CreateInstance<SkillDef>();

            PenisSeekerSkillDef.activationState = new SerializableEntityStateType(typeof(PENISBLAST));
            PenisSeekerSkillDef.activationStateMachineName = "Weapon";
            PenisSeekerSkillDef.baseMaxStock = 3;
            PenisSeekerSkillDef.baseRechargeInterval = 3f;
            PenisSeekerSkillDef.beginSkillCooldownOnSkillEnd = true;
            PenisSeekerSkillDef.canceledFromSprinting = false;
            PenisSeekerSkillDef.cancelSprintingOnActivation = true;
            PenisSeekerSkillDef.fullRestockOnAssign = true;
            PenisSeekerSkillDef.interruptPriority = InterruptPriority.Any;
            PenisSeekerSkillDef.isCombatSkill = true;
            PenisSeekerSkillDef.mustKeyPress = false;
            PenisSeekerSkillDef.rechargeStock = 3;
            PenisSeekerSkillDef.requiredStock = 1;
            PenisSeekerSkillDef.stockToConsume = 1;
            ((ScriptableObject)PenisSeekerSkillDef).name = "PENISBLAST"; // need this for skills plus plus to work !!
            
            // For the skill icon, you will have to load a Sprite from your own AssetBundle
            PenisSeekerSkillDef.icon = penisBundle.LoadAsset<Sprite>("appendageblast");
            PenisSeekerSkillDef.skillDescriptionToken = "PENISEEEKER_SEEKER_PRIMARY_ALT_DESC";
            PenisSeekerSkillDef.skillNameToken = "PENIS_BLAST_NAME";

            // This adds our skilldef. If you don't do this, the skill will not work.
            ContentAddition.AddSkillDef(PenisSeekerSkillDef);
            ContentAddition.AddEntityState(typeof(PENISBLAST), out _);

            // Now we add our skill to one of the survivor's skill families
            // You can change component.primary to component.secondary, component.utility and component.special
            SkillLocator skillLocator = SeekerBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            // Cloned prefab, homing
            seekerProjectilePrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Seeker.SpiritPunchFinisherProjectile_prefab).WaitForCompletion().InstantiateClone("PenisSeekerProjectile");

            ProjectileTargetComponent targetcuck = seekerProjectilePrefab.AddComponent<ProjectileTargetComponent>();

            ProjectileSimple simplefuckingprojectile = seekerProjectilePrefab.GetComponent<ProjectileSimple>();
            simplefuckingprojectile.desiredForwardSpeed = 15f;
            simplefuckingprojectile.enableVelocityOverLifetime = true;
            simplefuckingprojectile.oscillateMagnitude = 1000f;

            ProjectileDirectionalTargetFinder homingpiss = seekerProjectilePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
            homingpiss.lookRange = 35;
            homingpiss.lookCone = 15;
            homingpiss.targetSearchInterval = 0.1f;
            homingpiss.onlySearchIfNoTarget = true;
            homingpiss.allowTargetLoss = true;
            homingpiss.testLoS = true;
            homingpiss.ignoreAir = false;
            homingpiss.flierAltitudeTolerance = float.PositiveInfinity;
            homingpiss.targetComponent = targetcuck;

            ProjectileSteerTowardTarget sterpiss = seekerProjectilePrefab.AddComponent<ProjectileSteerTowardTarget>();
            sterpiss.targetComponent = targetcuck;
            sterpiss.rotationSpeed = 135;
            sterpiss.yAxisOnly = false;

            seekerProjectilePrefab.AddComponent<PenisSeekerModifier>(); // ACANTHI CODE :3 <- i lov ethat guy !!! 
            seekerProjectilePrefab.RegisterNetworkPrefab();
            ContentAddition.AddProjectile(seekerProjectilePrefab);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[^1] = new SkillFamily.Variant
            {
                skillDef = PenisSeekerSkillDef,
                viewableNode = new ViewablesCatalog.Node(PenisSeekerSkillDef.skillNameToken, false)
            };

            if (SPPInstalled)
            {
                SPPSupport.init();
            }
            else
            {
                Log.Debug("sigh ,.,,. no skills plus plus ,..,.,,.,.,.,. its okay .,.,.,.,, i get it ,.,.,,.,.,., its fine .,.,.,.,");
            }
        }

#if DEBUG
        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.F8))
            {
                if (UHRInstalled)
                {
                    //for extra wins add a build event that copies it over to the install dir !!
                    UHRSupport.hotReload(typeof(PenisSeekerPlugin).Assembly, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "PenisSeeker.dll"));
                }
                else
                {
                    Log.Debug("couldnt finds unity hot reload !!");
                }
            }
        }
#endif  
    }
}
