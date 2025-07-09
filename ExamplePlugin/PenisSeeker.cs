
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
        public const string PluginVersion = "1.0.5";

        public static GameObject exampleProjectilePrefab;

        //NEEDED I THINK
        //public const string LanguageFolder = "Language";

        public void Awake()
        {
            //WRONG SOMEHOW
            //RoR2.Language.collectLanguageRootFolders += Language_collectLanguageFolder;

            Log.Init(Logger);


            Log.Debug("PENIS BLAST!!");
        
            var penisBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "penisblast"));





            GameObject SeekerBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/SeekerBody.prefab").WaitForCompletion();

           





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
        PenisSeekerSkillDef.icon = penisBundle.LoadAsset<Sprite>("appendageblast");
            PenisSeekerSkillDef.skillDescriptionToken = "Fire <style=cIsHealing>homing</style> palms at enemies that explode for <style=cIsDamage>300% damage</style>. Velocity scales off <style=cIsDamage>Tranquility</style> stacks.";
            PenisSeekerSkillDef.skillName = "PENIS_BLAST";
            PenisSeekerSkillDef.skillNameToken = "Penis Blast";
            //REPLACE WITH THIS ONCE WE UNFUCK LANGUAGE JSON FILES
            // PenisSeekerSkillDef.skillDescriptionToken = "PENISEEEKER_SEEKER_PRIMARY_ALT_DESC";
            //PenisSeekerSkillDef.skillName = "PENIS_BLAST";
            //PenisSeekerSkillDef.skillNameToken = "PENIS_BLAST_NAME";

            // This adds our skilldef. If you don't do this, the skill will not work.
            ContentAddition.AddSkillDef(PenisSeekerSkillDef);
            ContentAddition.AddEntityState(typeof(PENISBLAST), out _);

            // Now we add our skill to one of the survivor's skill families
            // You can change component.primary to component.secondary, component.utility and component.special
            SkillLocator skillLocator = SeekerBodyPrefab.GetComponent<SkillLocator>();
                SkillFamily skillFamily = skillLocator.primary.skillFamily;

                // Cloned prefab, homing
                
                exampleProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/SpiritPunchFinisherProjectile.prefab").WaitForCompletion().InstantiateClone("ExampleProjectile");

            
            var targetcuck = exampleProjectilePrefab.AddComponent<ProjectileTargetComponent>();
            


            var homingpiss = exampleProjectilePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
                homingpiss.lookRange = 35;
                homingpiss.lookCone = 15;
                homingpiss.targetSearchInterval = 0.1f;
                homingpiss.onlySearchIfNoTarget = true;
                homingpiss.allowTargetLoss = true;
                homingpiss.testLoS = true;
                homingpiss.ignoreAir = false;
                homingpiss.flierAltitudeTolerance = float.PositiveInfinity;
                homingpiss.targetComponent = targetcuck;


                var sterpiss = exampleProjectilePrefab.AddComponent<ProjectileSteerTowardTarget>();
                sterpiss.targetComponent = targetcuck;
                sterpiss.rotationSpeed = 135;
                sterpiss.yAxisOnly = false;

                

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
            //THIS WAS IN VILIGERS CODE BUT WHERE DOES IT GO
            //private void Language_collectLanguageRootFolders(System.Collections.Generic.List<string> folders)
            //{
            //folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), LanguageFolder));
            //}
            //slur
    }
}
