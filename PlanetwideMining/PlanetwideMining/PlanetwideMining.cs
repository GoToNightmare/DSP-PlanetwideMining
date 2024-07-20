using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace PlanetwideMining
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class PlanetwideMining : BaseUnityPlugin
    {
        private const string PluginGuid = "PlanetwideMining";
        private const string PluginName = "PlanetwideMining";
        private const string PluginVersion = "0.12";

        public static EVeinType ResourceForGlobalMining = EVeinType.None;


        public static ConfigEntry<bool> SpeedControlsEnabled;
        private bool speedControlsEnabledField;


        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginName} is loaded!");

            InitConfig(Config);

            var harmony = new Harmony(PluginName);
            harmony.PatchAll();
        }


        private void InitConfig(ConfigFile config)
        {
            SpeedControlsEnabled = config.Bind("1. SpeedControls", nameof(SpeedControlsEnabled),
                true, new ConfigDescription("SpeedControlsEnabled", new AcceptableValueRange<bool>(false, true)));

            if (SpeedControlsEnabled != null)
            {
                speedControlsEnabledField = SpeedControlsEnabled.Value;
            }
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                SwitchEnumValue(1);
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                SwitchEnumValue(-1);
            }


            if (speedControlsEnabledField)
            {
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
        }


        private void SwitchGameSpeed(float speed)
        {
            Time.timeScale = speed;
        }


        private int LastUsedIndex { get; set; }


        private void SwitchEnumValue(int indexChange)
        {
            var newIndex = LastUsedIndex + indexChange;

            if (!IndexInRange(newIndex))
            {
                if (indexChange > 0)
                {
                    newIndex = 0;
                }
                else
                {
                    newIndex = ResourceTypes.Count - 1;
                }
            }

            LastUsedIndex = newIndex;

            ResourceForGlobalMining = ResourceTypes[newIndex];
        }


        private bool IndexInRange(int index)
        {
            var totalElements = ResourceTypes.Count;
            if (index >= 0 && index < totalElements)
            {
                return true;
            }

            return false;
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
            EVeinType.Oil, // Oil, should not be mined
            EVeinType.Fireice, // Fire Ice
            EVeinType.Diamond, // Kimberlite Ore
            EVeinType.Fractal, // Fractal Silicon
            EVeinType.Crysrub, // ???
            EVeinType.Grat, // Optical Grating Crystal
            EVeinType.Bamboo, // Spiniform Stalagmite Crystal
            EVeinType.Mag, // Unipolar Magnet
            // EVeinType.Max, // WHAT IS THAT
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

            EVeinType targetVeinType = PlanetwideMining.ResourceForGlobalMining;
            if (targetVeinType != EVeinType.None)
            {
                PlanetData localPlanetData = GameMain.localPlanet;
                if (localPlanetData != null && localPlanetData.type != EPlanetType.Gas)
                {
                    var localPlanetFactory = localPlanetData.factory;
                    if (localPlanetFactory != null)
                    {
                        var buildPreviews = __instance.buildPreviews;
                        bool isOnlyOneMinerToBeBuild = buildPreviews.Count == 1;
                        if (isOnlyOneMinerToBeBuild)
                        {
                            var buildPreview = buildPreviews[0];
                            if (buildPreview != null)
                            {
                                if (buildPreview.paramCount == 0)
                                {
                                    buildPreview.parameters = new int[2048];
                                    buildPreview.paramCount = 2048;
                                }


                                if (buildPreview.desc.isVeinCollector)
                                {
                                    List<int> newPrebuildDataParameters = new List<int>();

                                    var veinPool = localPlanetFactory.veinPool;
                                    foreach (VeinData veinData in veinPool)
                                    {
                                        var id = veinData.id;
                                        var type = veinData.type;
                                        if (type == targetVeinType)
                                        {
                                            newPrebuildDataParameters.Add(id);
                                        }
                                    }


                                    var totalVeins = newPrebuildDataParameters.Count;
                                    if (totalVeins > 0)
                                    {
                                        PrebuildData prebuildData = default(PrebuildData);
                                        prebuildData.InitParametersArray(veinPool.Length);


                                        prebuildData.parameters = newPrebuildDataParameters.ToArray();
                                        prebuildData.paramCount = totalVeins;
                                        prebuildData.ArrangeParametersArray();


                                        // Not sure wtf it's needed in source code?
                                        Array.Resize(ref buildPreview.parameters, buildPreview.paramCount + prebuildData.paramCount);
                                        Array.Copy(prebuildData.parameters, 0, buildPreview.parameters, buildPreview.paramCount, prebuildData.paramCount);
                                        buildPreview.paramCount += prebuildData.paramCount;
                                    }
                                    else
                                    {
                                        buildPreview.condition = EBuildCondition.NeedResource;
                                    }


                                    Array.Clear(____tmp_ids, 0, ____tmp_ids.Length);

                                    __result = true;
                                    flagRunOriginalMethod = false;
                                }
                            }
                        }
                    }
                }
            }


            // Original method if more than 1 building in build previews or its not a vein miner
            return flagRunOriginalMethod;
        }
    }
}