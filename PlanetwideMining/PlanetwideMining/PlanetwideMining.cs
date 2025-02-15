using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace PlanetwideMining
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class PlanetwideMining : BaseUnityPlugin
    {
        private const string PluginGuid = "PlanetwideMining";
        private const string PluginName = "PlanetwideMining";
        private const string PluginVersion = "0.0.13";

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


        public static bool NumlockStateActive { get; set; }


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

            if (Input.GetKeyDown(KeyCode.Numlock))
            {
                NumlockStateActive = !NumlockStateActive;
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
            RefreshResourcesEnumsForCurrentPlanet();


            var tempArrayWithAllVeins = veinTypes.OrderBy(v => v).ToArray();
            var newIndex = LastUsedIndex + indexChange;

            if (!IndexInRange(newIndex))
            {
                if (indexChange > 0)
                {
                    newIndex = 0;
                }
                else
                {
                    newIndex = tempArrayWithAllVeins.Length - 1;
                }
            }

            LastUsedIndex = newIndex;

            ResourceForGlobalMining = tempArrayWithAllVeins[newIndex];
        }


        private readonly HashSet<EVeinType> veinTypes = new HashSet<EVeinType>();


        private void RefreshResourcesEnumsForCurrentPlanet()
        {
            var allVeinTypes = veinTypes;
            allVeinTypes.Clear();
            allVeinTypes.Add(EVeinType.None); // Add default value, when mod logic is inactive

            PlanetData localPlanetData = GameMain.localPlanet;
            if (localPlanetData != null)
            {
                var localPlanetFactory = localPlanetData.factory;
                if (localPlanetFactory != null)
                {
                    var veinPool = localPlanetFactory.veinPool;
                    foreach (VeinData veinData in veinPool)
                    {
                        var type = veinData.type;
                        if (allVeinTypes.Contains(type))
                        {
                            continue;
                        }

                        allVeinTypes.Add(type);
                    }
                }
            }
        }


        private bool IndexInRange(int index)
        {
            var totalElements = veinTypes.Count;
            if (index >= 0 && index < totalElements)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// TODO remove this, not used anymore, only keepts bcs of compatibility patches
        /// </summary>
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
}