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

			var harmony = new Harmony("PlanetwideMining");
			harmony.PatchAll();
		}
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