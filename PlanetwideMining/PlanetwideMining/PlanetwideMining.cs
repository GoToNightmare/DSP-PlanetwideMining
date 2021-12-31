using BepInEx;
using HarmonyLib;

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
		public static bool Prefix(BuildTool_Click __instance, ref bool __result,  ref int[] ____tmp_ids)
		{
			__result = CheckBuildConditions(ref __instance, ref ____tmp_ids);
			return false;
		}
	}
}