
using BepInEx;
using EntityStates;
using R2API;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using System.Reflection;


namespace PenisSeeker
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class plugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "toastyteam" + PluginName;
        public const string PluginAuthor = "toastyteam";
        public const string PluginName = "PenisSeeker";
        public const string PluginVersion = "1.0.0";

        public static GameObject exampleProjectilePrefab;
    
            public void Awake()
            {


                Log.Init(Logger);


                Log.Debug("PENIS BLAST!!");



                GameObject SeekerBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/SeekerBody.prefab").WaitForCompletion();

                // We use LanguageAPI to add strings to the game, in the form of tokens
                // Please note that it is instead recommended that you use a language file.
                // More info in https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
                //LanguageAPI.Add("COMMANDO_PRIMARY_SIMPLEBULLET_NAME", "Simple Bullet Attack");
                //LanguageAPI.Add("COMMANDO_PRIMARY_SIMPLEBULLET_DESCRIPTION", $"Fire a bullet from your right pistol for <style=cIsDamage>300% damage</style>.");

        
    
                SkillDef PenisSeekerSkillDef = ScriptableObject.CreateInstance<SkillDef>();



                PenisSeekerSkillDef.activationState = new SerializableEntityStateType(typeof(PenisSeeker.PENISBLAST));
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



                // For the skill icon, you will have to load a Sprite from your own AssetBundle
                PenisSeekerSkillDef.icon = null;
                PenisSeekerSkillDef.skillDescriptionToken = "Fire <style=cIsHealing>homing</style> palms at enemies that explode for <style=cIsDamage>300% damage</style>";
                PenisSeekerSkillDef.skillName = "PENIS_BLAST";
                PenisSeekerSkillDef.skillNameToken = "Penis Blast";

                // This adds our skilldef. If you don't do this, the skill will not work.
                ContentAddition.AddSkillDef(PenisSeekerSkillDef);

                // Now we add our skill to one of the survivor's skill families
                // You can change component.primary to component.secondary, component.utility and component.special
                SkillLocator skillLocator = SeekerBodyPrefab.GetComponent<SkillLocator>();
                SkillFamily skillFamily = skillLocator.primary.skillFamily;

                // Cloned prefab, homing
                // NEEDS to have three to work, everything else is optional
                // ProjectileTargetComponent is needed in order to find the target
                //ProjectileDirectionalTargetFinder is needed in order to see it
                //ProjectileSteerTowardTarget is what moves it

                exampleProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/SpiritPunchFinisherProjectile.prefab").WaitForCompletion().InstantiateClone("ExampleProjectile");


                var targetcuck = exampleProjectilePrefab.AddComponent<ProjectileTargetComponent>();


                var homingpiss = exampleProjectilePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
                homingpiss.lookRange = 55;
                homingpiss.lookCone = 80;
                homingpiss.targetSearchInterval = 0.1f;
                homingpiss.onlySearchIfNoTarget = true;
                homingpiss.allowTargetLoss = false;
                homingpiss.testLoS = false;
                homingpiss.ignoreAir = false;
                homingpiss.flierAltitudeTolerance = float.PositiveInfinity;
                homingpiss.targetComponent = targetcuck;


                var sterpiss = exampleProjectilePrefab.AddComponent<ProjectileSteerTowardTarget>();
                sterpiss.targetComponent = targetcuck;
                sterpiss.rotationSpeed = 45;
                sterpiss.yAxisOnly = false;

                var simplefuckingprojectile = exampleProjectilePrefab.GetComponent<ProjectileSimple>();
                if (simplefuckingprojectile)
                {
                    simplefuckingprojectile.updateAfterFiring = true;
                }

                PrefabAPI.RegisterNetworkPrefab(exampleProjectilePrefab);
                ContentAddition.AddProjectile(exampleProjectilePrefab);


                //
                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = PenisSeekerSkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(PenisSeekerSkillDef.skillNameToken, false, null)
                };

            }
        }
    }
