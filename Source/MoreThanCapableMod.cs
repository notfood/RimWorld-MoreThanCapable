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
            LongEventHandler.QueueLongEvent(Setup, "LibraryStartup", false, null);

            new Harmony("rimworld.moreThanCapable").PatchAll();

            LongEventHandler.ExecuteWhenFinished(delegate {
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

            foreach (var backstory in BackstoryDatabase.allBackstories.Values) {

                TraitEntry entry = null;

                foreach (var tag in hatesDumbLabor) {
                    if ((tag & backstory.workDisables) == tag) {
                        entry = new TraitEntry(hatesDumbLaborTrait, 0);
                        break;
                    }
                }

                if (entry != null) {
                    if (backstory.forcedTraits.NullOrEmpty()) {
                        backstory.forcedTraits = new List<TraitEntry>();
                    }
                    backstory.forcedTraits.Add(entry);
                }
            }

            if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name.Contains("Force Do Job"))) {
                Log.Error("MoreThanCapable :: You should remove \"Force Do Job\" from your mod list, as \"MoreThanCapable\" incorporates its functionality, and if both mods are running, you may encounter odd behavior.");
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

        public static bool HasWeapon(Pawn pawn)
        {
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
        public List<ThingDef> ignoredWeapons;

        public bool allowFDJ = true;
        public void DoWindowContents(Rect canvas)
        {
            var list = new Listing_Standard {
                ColumnWidth = canvas.width
            };

            list.Begin(canvas);
            list.Gap();
            list.CheckboxLabeled("MTC.allowFDJ".Translate(), ref allowFDJ, "MTC.allowFDJTip".Translate());
            list.End();
        }
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref ignoredWeapons, "ignoredWeapons", LookMode.Def);
            Scribe_Values.Look(ref allowFDJ, "allowFDJ", true);
        }
    }
}
