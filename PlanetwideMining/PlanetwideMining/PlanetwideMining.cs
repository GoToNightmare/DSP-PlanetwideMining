using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace PlanetwideMining
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class PlanetwideMining : BaseUnityPlugin
    {
        private const string PluginGuid = "930f5bae-66d2-4917-988b-162fe2456643";
        private const string PluginName = "PlanetwideMining";
        private const string PluginVersion = "0.1";

        public static EVeinType ResourceForGlobalMining = EVeinType.None;


        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginName} is loaded!");

            var harmony = new Harmony("PlanetwideMining");
            harmony.PatchAll();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                SwitchEnumValue();
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SwitchGameSpeed(1f);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SwitchGameSpeed(2f);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                SwitchGameSpeed(4f);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SwitchGameSpeed(8f);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                SwitchGameSpeed(16f);
            }
        }


        private void SwitchGameSpeed(float speed)
        {
            Time.timeScale = speed;
        }


        private void SwitchEnumValue()
        {
            Assert.NotNull(ResourceTypes);
            Assert.True(ResourceTypes.Count > 0);

            var newElement = ResourceTypes.SkipWhile(x => x != ResourceForGlobalMining).Skip(1).DefaultIfEmpty(ResourceTypes[0]).FirstOrDefault();
            ResourceForGlobalMining = newElement;
        }


        private static readonly List<EVeinType> ResourceTypes = new List<EVeinType>()
        {
            // EVeinType.None,
            EVeinType.Iron, // Iron Ore
            EVeinType.Copper, // Copper Ore
            EVeinType.Silicium, // Silicon Ore
            EVeinType.Titanium, // Titanium Ore
            EVeinType.Stone, // Stone
            EVeinType.Coal, // Coal
            // EVeinType.Oil, 
            EVeinType.Fireice, // Fire Ice
            EVeinType.Diamond, // Kimberlite Ore
            EVeinType.Fractal, // Fractal Silicon
            EVeinType.Crysrub, // ???
            EVeinType.Grat, // Optical Grating Crystal
            EVeinType.Bamboo, // Spiniform Stalagmite Crystal
            EVeinType.Mag, // Unipolar Magnet
            // EVeinType.Max, 
        };
    }


    [HarmonyPatch(typeof(BuildTool_Click))]
    [HarmonyPatch("CheckBuildConditions")]
    public static partial class PatchMiners
    {
        public static bool Prefix(
            BuildTool_Click __instance, // required
            ref bool __result, // required
            ref int[] ____tmp_ids, // BuildTool._tmp_ids
            ref Collider[] ____tmp_cols, // BuildTool._tmp_cols
            ref int ___tmpInhandId,
            ref int ___tmpInhandCount,
            ref StorageComponent ___tmpPackage,
            ref int ____overlappedCount,
            ref int[] ____overlappedIds
        )
        {
            __result = CheckBuildConditions(
                __instance, // required
                ref __result, // required
                ref ____tmp_ids, // BuildTool._tmp_ids
                ref ____tmp_cols, // BuildTool._tmp_cols
                ref ___tmpInhandId,
                ref ___tmpInhandCount,
                ref ___tmpPackage,
                ref ____overlappedCount,
                ref ____overlappedIds
            );

            return false;
        }
    }
}