using BepInEx.Configuration;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;

namespace PenisSeeker;

public class PenisSeekerUpgrade
{
    [SkillsPlusPlus.Modifiers.SkillLevelModifier("PENISBLAST", typeof(PENISBLAST))]
    class PenisBlastModifier : SkillsPlusPlus.Modifiers.SimpleSkillModifier<PENISBLAST> {
        public override void OnSkillEnter(PENISBLAST skillState, int level)
        {
            skillState.force = AdditiveScaling(3075f, 3075f * forceScaling.Value, level);
            if (skillState.projectileprefab.TryGetComponent(out ProjectileExplosion explode))
            {
                Log.Debug(explode.blastRadius);
                explode.blastRadius = AdditiveScaling(7, radiusScaling.Value, level);
            }
            base.OnSkillEnter(skillState, level);
        }

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef)
        {
            base.OnSkillLeveledUp(level, characterBody, skillDef);
        }

        public override void OnSkillExit(PENISBLAST skillState, int level)
        {
            base.OnSkillExit(skillState, level);
        }

        public override void SetupConfig(ConfigFile config)
        {
            base.SetupConfig(config);
            
            forceScaling = config.Bind("Skills++ Config",
                "force scaling", 
                .30f, 
                "how much force in percent to add per level !!");
            
            radiusScaling = config.Bind("Skills++ Config",
                "blast radius scaling", 
                2f, 
                "how much to increase blast radius by each level (skill is 7m and adds 2m by default !!");

            //roo is a hard dependency of spp so its probably fine !! 
            
            RiskOfOptions.OptionConfigs.StepSliderConfig forceScalingSliderConfig = new RiskOfOptions.OptionConfigs.StepSliderConfig()
            {
                min = 0f,
                max = 2f,
                FormatString = "{0}%"
            };
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.StepSliderOption(PenisSeekerUpgrade.PenisBlastModifier.forceScaling, forceScalingSliderConfig));
                
            RiskOfOptions.OptionConfigs.StepSliderConfig radiusScalingSliderConfig = new RiskOfOptions.OptionConfigs.StepSliderConfig()
            {
                min = 0f,
                max = 15f,
                FormatString = "{0:0}"
            };
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.StepSliderOption(PenisSeekerUpgrade.PenisBlastModifier.radiusScaling, radiusScalingSliderConfig));
        }

        public static ConfigEntry<float> forceScaling;
        public static ConfigEntry<float> radiusScaling;
    }
}