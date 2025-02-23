using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace PlanetwideMining;

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

        if (PlanetwideMining.NumlockStateActive)
        {
            // Reset mining state
            PlanetwideMining.ResourceForGlobalMining = EVeinType.None;


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
                            var desc = buildPreview.desc;
                            if (!desc.isVeinCollector && !desc.veinMiner)
                            {
                                __result = true;
                                flagRunOriginalMethod = false;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            var targetVeinType = PlanetwideMining.ResourceForGlobalMining;
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
                                var desc = buildPreview.desc;
                                if (desc.isVeinCollector || desc.veinMiner)
                                {
                                    if (buildPreview.paramCount == 0)
                                    {
                                        buildPreview.parameters = new int[2048];
                                        buildPreview.paramCount = 2048;
                                    }

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
        }

        // Original method if more than 1 building in build previews or its not a vein miner
        return flagRunOriginalMethod;
    }
}