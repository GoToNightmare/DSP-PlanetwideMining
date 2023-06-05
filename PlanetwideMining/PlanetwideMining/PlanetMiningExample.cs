// using System;
// using System.Collections.Generic;
// using System.Reflection.Emit;
// using HarmonyLib;
// using UnityEngine;
// #pragma warning disable CS0162
//
// [HarmonyPatch(typeof(BuildTool_Click), nameof(BuildTool_Click.CheckBuildConditions))]
// public static class PlanetMiningExample
// {
//     public static void MinerBuildConditionPatch(BuildTool_Click instance, BuildPreview buildPreview)
//     {
//         Debug.LogWarning("Your patch goes here");
//
//         PrebuildData prebuildData = default(PrebuildData);
//         VeinData[] veinPool = instance.factory.veinPool;
//         prebuildData.InitParametersArray(veinPool.Length);
//
//         // `start
//         // Debug.LogError($"[000] veinPool.Length {veinPool.Length}");
//         if (prebuildData.parameters != null)
//         {
//             EVeinType targetVeinType = EVeinType.Iron;
//             List<int> newPrebuildDataParameters = new List<int>();
//             for (int iaa = 0; iaa < veinPool.Length; iaa++)
//             {
//                 if (veinPool[iaa].type != targetVeinType) continue;
//                 newPrebuildDataParameters.Add(veinPool[iaa].id);
//             }
//
//             prebuildData.parameters = newPrebuildDataParameters.ToArray();
//         }
//         // `end
//
//         prebuildData.paramCount = prebuildData.parameters.Length;
//         prebuildData.ArrageParametersArray();
//         buildPreview.parameters = prebuildData.parameters;
//         buildPreview.paramCount = prebuildData.paramCount;
//
//         // Debug.LogError($"[333] prebuildData.paramCount {prebuildData.paramCount}");
//         // Debug.LogError($"[444] prebuildData.parameters {prebuildData.parameters?.Length} :: {prebuildData.parameters?.Count(v => v != default)}\n");
//
//         // Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
//         if (prebuildData.paramCount == 0)
//         {
//             buildPreview.condition = EBuildCondition.NeedResource;
//         }
//     }
//
//     private static CodeMatcher PlayWithIL(List<CodeInstruction> il_copy, ILGenerator generator)
//     {
//         var matcher = new CodeMatcher(il_copy, generator);
//
//         // Name of position/pointer of CodeMatch?
//         const string buildPreviewLdlocMatch = nameof(buildPreviewLdlocMatch);
//
//         // Matches the `if (buildPreview.desc.veinMiner)`
//         matcher.MatchForward(true, new[]
//         {
//             new CodeMatch(OpCodes.Ldloc_S, name: buildPreviewLdlocMatch),
//             new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(BuildPreview), nameof(BuildPreview.desc))),
//             new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PrefabDesc), nameof(PrefabDesc.veinMiner))),
//             new CodeMatch(OpCodes.Brfalse)
//         });
//
//         Debug.LogWarning("Matcher at pos: " + matcher.Pos);
//         Debug.LogWarning("Remaining: " + matcher.Remaining);
//
//         var buildPreviewLocalIdx = matcher.NamedMatch(buildPreviewLdlocMatch).operand;
//         var exitLabel = (Label)matcher.Operand;
//
//         Debug.LogWarning($"{nameof(buildPreviewLocalIdx)}: {buildPreviewLocalIdx} ({buildPreviewLocalIdx.GetType()})");
//
//         Debug.LogWarning($"{nameof(exitLabel)}: {exitLabel}");
//
//         // Insert a call to your code and get out of the branch after that
//         // This changes the `if ... else if ...` into `if ... if ...`
//         // A good enough compromise in this case, looking at the rest of the code in the method
//
//         matcher.Advance(1)
//             .Insert(new CodeInstruction(OpCodes.Ldarg_0), // first param is `this`, which is BuildTool_Click
//                 new CodeInstruction(OpCodes.Ldloc_S, buildPreviewLocalIdx), // second param is `buildPreview`, the local variable we want to modify
//                 new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlanetMiningExample), nameof(MinerBuildConditionPatch))),
//                 new CodeInstruction(OpCodes.Br, exitLabel)
//             );
//
//         return matcher;
//     }
//
//     private const bool DryRun = true;
//
//     [HarmonyTranspiler]
//     public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
//     {
//         var il_copy = new List<CodeInstruction>(instructions);
//         try
//         {
//             PlayWithIL(il_copy, generator);
//         }
//         catch (Exception e)
//         {
//             Debug.LogError(e);
//         }
//
//         if (DryRun)
//         {
//             foreach (var ins in il_copy)
//             {
//                 yield return ins;
//             }
//         }
//         else
//         {
//             var modified_ins = PlayWithIL(il_copy, generator).InstructionEnumeration();
//             foreach (var ins in modified_ins)
//             {
//                 yield return ins;
//             }
//         }
//     }
// }