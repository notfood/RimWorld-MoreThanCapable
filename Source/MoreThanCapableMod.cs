using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreThanCapable
{
    public class MoreThanCapableMod : Mod
    {
        public static Settings Settings;

        static Dictionary<WorkTags, HashSet<WorkTypeDef>> workTypes = new Dictionary<WorkTags, HashSet<WorkTypeDef>>();

        public MoreThanCapableMod(ModContentPack content) : base(content)
        {
            new Harmony("rimworld.moreThanCapable").PatchAll();

            LongEventHandler.ExecuteWhenFinished(delegate {
                Setup();
                Settings = GetSettings<Settings>();
            });
        }

        public static void Setup()
        {
            foreach (var workTag in Enum.GetValues(typeof(WorkTags)).Cast<WorkTags>()) {
                var jobList = DefDatabase<WorkTypeDef>
                    .AllDefs
                    .Where(wtd => wtd.visible
                        && (wtd.workTags & workTag) != WorkTags.None
                        && wtd.defName != "FinishingOff")
                    .ToHashSet();

                workTypes.Add(workTag, jobList);
            }

            var hatesDumbLaborTrait = TraitDef.Named("HatesDumbLabor");

            var hatesDumbLabor = new[] {
                WorkTags.ManualDumb,
                WorkTags.Cleaning,
                WorkTags.Hauling
            };

            foreach (var backstory in DefDatabase<BackstoryDef>.AllDefs) {

                BackstoryTrait entry = null;

                foreach (var tag in hatesDumbLabor) {
                    if ((tag & backstory.workDisables) == tag) {
                        entry = new BackstoryTrait() {
                            def = hatesDumbLaborTrait, 
                            degree = 0
                        };
                        break;
                    }
                }

                if (entry != null) {
                    if (backstory.forcedTraits.NullOrEmpty()) {
                        backstory.forcedTraits = new List<BackstoryTrait>();
                    }
                    backstory.forcedTraits.Add(entry);
                }
            }
        }

        public static void ResetHatedWorkTypes(Pawn pawn)
        {
            IEnumerable<WorkTypeDef> badwork = workTypes
                .Where(kvp => (kvp.Key & pawn.CombinedDisabledWorkTags) != WorkTags.None)
                .SelectMany(l => l.Value);

            foreach (var wtd in badwork) {
                pawn.workSettings.SetPriority(wtd, 0);
            }
        }

        public static bool CanDisable(Pawn pawn, WorkTags w)
        {
            if (Settings.obeyLifeStages && pawn.RaceProps.lifeStageWorkSettings.Any(stage => (stage.workType.workTags & w) != 0 && stage.IsDisabled(pawn))) {
                return true;
            }

            return CanDisable(pawn);
        }

        public static bool CanDisable(Pawn pawn, WorkTypeDef wtd)
        {
            if (Settings.obeyLifeStages && pawn.RaceProps.lifeStageWorkSettings.Any(stage => stage.workType == wtd && stage.IsDisabled(pawn))) {
                return true;
            }
            
            return CanDisable(pawn);
        }

        public static bool CanDisable(Pawn pawn)
        {
            return Settings.obeyGuestExceptions && pawn.GuestStatus > 0;
        }

        public static bool IsBadWorkActive(Pawn pawn, WorkTypeDef wtd)
        {
            return pawn.workSettings.WorkIsActive(wtd) && IsBadWork(pawn, wtd);
        }

        public static bool IsBadWork(Pawn pawn, WorkTypeDef wtd)
        {
            return (wtd.workTags & pawn.CombinedDisabledWorkTags) != WorkTags.None;
        }

        public static bool HasBadWork(Pawn pawn)
        {
            return workTypes.Any(kvp => (kvp.Key & pawn.CombinedDisabledWorkTags) != WorkTags.None && kvp.Value.Any(pawn.workSettings.WorkIsActive));
        }

        public static bool IsNonViolentArmed(Pawn pawn)
        {
            if ((pawn.CombinedDisabledWorkTags & WorkTags.Violent) == 0) {
                return false;
            }

            if (pawn.equipment?.Primary == null) {
                return false;
            }

            var primaryDef = pawn.equipment.Primary.def;
            if (!primaryDef.IsWeapon) {
                return false;
            }

            if (Settings.ignoredWeapons?.Contains(primaryDef) ?? false) {
                return false;
            }

            return true;
        }

        public override string SettingsCategory() => "MTC.MoreThanCapable".Translate();

        public override void DoSettingsWindowContents(Rect inRect) => Settings.DoWindowContents(inRect);
    }

    public class Settings : ModSettings
    {
        public bool allowFDJ = true;

        public bool obeyLifeStages = true;

        public bool obeyGuestExceptions = true;

        public List<ThingDef> ignoredWeapons;   

        public void DoWindowContents(Rect canvas)
        {
            var list = new Listing_Standard {
                ColumnWidth = canvas.width
            };

            list.Begin(canvas);
            list.CheckboxLabeled("MTC.allowFDJ".Translate(), ref allowFDJ, "MTC.allowFDJTip".Translate());
            list.CheckboxLabeled("MTC.obeyLifeStages".Translate(), ref obeyLifeStages, "MTC.obeyLifeStagesTip".Translate());
            list.CheckboxLabeled("MTC.obeyGuestExceptions".Translate(), ref obeyGuestExceptions, "MTC.obeyGuestExceptionsTip".Translate());
            list.Gap();
            list.Label("Ignored Weapons setting is edited manually for now.");
            list.Label("Config/Mod_1803932954_MoreThanCapableMod.xml");
            list.End();
        }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref allowFDJ, "allowFDJ", true);
            Scribe_Values.Look(ref obeyLifeStages, "obeyLifeStages", true);
            Scribe_Values.Look(ref obeyGuestExceptions, "obeyGuestExceptions", true);
            Scribe_Collections.Look(ref ignoredWeapons, "ignoredWeapons", LookMode.Def);
        }
    }

    [StaticConstructorOnStartup]
    public static class Resources
    {
        public static Texture2D workBoxBGTexDespised = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Despised");
    }
}
