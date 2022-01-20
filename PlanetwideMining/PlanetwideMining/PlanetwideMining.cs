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
			EVeinType.Iron, // check
			EVeinType.Copper, // check
			EVeinType.Silicium, // check
			EVeinType.Titanium, // check
			EVeinType.Stone, // check
			EVeinType.Coal, // check
			// EVeinType.Oil, //
			EVeinType.Fireice, // check
			EVeinType.Diamond, // check
			EVeinType.Fractal, // check
			EVeinType.Crysrub, // organic?
			EVeinType.Grat,
			EVeinType.Bamboo, // spimiform
			EVeinType.Mag, // unipolar
			// EVeinType.Max, // ????????????????
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