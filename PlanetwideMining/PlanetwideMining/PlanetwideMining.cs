using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
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

            var harmony = new Harmony(PluginName);
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
        [UsedImplicitly]
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
            bool flagRunOriginalMethod = true;

            // Check if only 1 miner to be build
            var pr = __instance.buildPreviews;
            if (pr != null && pr.Count == 1)
            {
                for (int i = 0; i < pr.Count; i++)
                {
                    var element = pr[i];
                    var desc = element.desc;
                    if (desc != null)
                    {
                        if (desc.veinMiner)
                        {
                            Array.Clear(____tmp_ids, 0, ____tmp_ids.Length);

                            PrebuildData prebuildData = default(PrebuildData);

                            VeinData[] veinPool = __instance.factory.veinPool;

                            prebuildData.InitParametersArray(veinPool.Length);

                            if (prebuildData.parameters != null)
                            {
                                EVeinType targetVeinType = PlanetwideMining.ResourceForGlobalMining;

                                List<int> newPrebuildDataParameters = new List<int>();

                                for (int iaa = 0; iaa < veinPool.Length; iaa++)
                                {
                                    if (veinPool[iaa].type != targetVeinType) continue;
                                    newPrebuildDataParameters.Add(veinPool[iaa].id);
                                }

                                prebuildData.parameters = newPrebuildDataParameters.ToArray();
                            }

                            prebuildData.paramCount = prebuildData.parameters.Length; // init in `InitParametersArray`
                            prebuildData.ArrageParametersArray();

                            if (element.desc.isVeinCollector)
                            {
                                if (element.paramCount == 0)
                                {
                                    element.parameters = new int[2048];
                                    element.paramCount = 2048;
                                }

                                if (prebuildData.paramCount > 0)
                                {
                                    Array.Resize(ref element.parameters, element.paramCount + prebuildData.paramCount);
                                    Array.Copy(prebuildData.parameters, 0, element.parameters, element.paramCount, prebuildData.paramCount);
                                    element.paramCount += prebuildData.paramCount;
                                }
                            }
                            else
                            {
                                element.parameters = prebuildData.parameters;
                                element.paramCount = prebuildData.paramCount;
                            }

                            if (prebuildData.paramCount == 0)
                            {
                                element.condition = EBuildCondition.NeedResource;
                            }

                            __result = true;
                            flagRunOriginalMethod = false;
                        }
                    }
                }
            }

            // Original method if more than 1 building in build previews or its not a vein miner
            return flagRunOriginalMethod;
        }
    }
}