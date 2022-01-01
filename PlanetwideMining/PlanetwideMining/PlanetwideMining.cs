using System;
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
			EVeinType.Iron, //
			EVeinType.Copper, //
			EVeinType.Silicium, //
			EVeinType.Titanium, //
			EVeinType.Stone, //
			EVeinType.Coal, //
			// EVeinType.Oil, //
			EVeinType.Fireice, //
			EVeinType.Diamond, // kimberit ore?
			EVeinType.Fractal, //
			// EVeinType.Crysrub,
			// EVeinType.Grat,
			// EVeinType.Bamboo,
			// EVeinType.Mag,
			// EVeinType.Max,
		};
	}


	[HarmonyPatch(typeof(BuildTool_Click))]
	[HarmonyPatch("CheckBuildConditions")]
	public static partial class PatchMiners
	{
		public static bool Prefix(BuildTool_Click __instance,
			ref bool __result,
			ref int[] ____tmp_ids,
			ref Collider[] ____tmp_cols,
			ref int ___tmpInhandId,
			ref int ___tmpInhandCount,
			ref int ____overlappedCount,
			ref int[] ____overlappedIds,
			ref StorageComponent ___tmpPackage)
		{
			__result = CheckBuildConditions(ref __instance,
				ref ____tmp_ids,
				ref ____tmp_cols,
				ref ___tmpInhandId,
				ref ___tmpInhandCount,
				ref ____overlappedCount,
				ref ____overlappedIds,
				ref ___tmpPackage);

			return false;
		}
	}
}