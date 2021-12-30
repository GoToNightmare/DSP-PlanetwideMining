using System;
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


		private void Awake()
		{
			Logger.LogInfo($"Plugin {PluginName} is loaded!");

			PatchMiners.ApplyPatch();
		}


		private void OnDestroy()
		{
			PatchMiners.RemovePatch();
		}
	}

	public partial class PatchMiners
	{
		private static Harmony harmonyInstance;

		private const string MethodToPatch = "CheckBuildConditions";
		private static readonly Type TypeToPatch = typeof(BuildTool_Click);

		private static readonly string PatchId = $"__Method patch: {MethodToPatch}";


		public static void ApplyPatch()
		{
			try
			{
				harmonyInstance = new Harmony(PatchId);

				var original = TypeToPatch.GetMethod(MethodToPatch);

				var prefix = typeof(PatchMiners).GetMethod(nameof(PrefixPatch));

				harmonyInstance.Patch(original, prefix: new HarmonyMethod(prefix));
			}
			catch (Exception e)
			{
				Debug.LogError($"[PlanetwideMining] ApplyPatch for \'{MethodToPatch}\' failed.\n{e}");
			}
		}


		public static void RemovePatch()
		{
			try
			{
				if (harmonyInstance == null) return;

				var original = TypeToPatch.GetMethod(MethodToPatch);

				harmonyInstance.Unpatch(original, HarmonyPatchType.Prefix, PatchId);
			}
			catch (Exception e)
			{
				Debug.LogError($"[PlanetwideMining] RemovePatch for \'{MethodToPatch}\' failed.\n{e}");
			}
		}
	}
}