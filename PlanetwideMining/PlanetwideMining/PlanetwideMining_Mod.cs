using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetwideMining
{
    public static partial class PatchMiners
    {
        private static bool CheckBuildConditions(
            BuildTool_Click __instance, // required
            ref bool __result, // required
            ref int[] ____tmp_ids, // ____tmp_ids
            ref Collider[] ____tmp_cols, // ____tmp_cols
            ref int ___tmpInhandId,
            ref int ___tmpInhandCount,
            ref StorageComponent ___tmpPackage,
            ref int ____overlappedCount,
            ref int[] ____overlappedIds
        )
        {
            if (__instance.buildPreviews.Count == 0)
                return false;
            GameHistoryData history = __instance.actionBuild.history;
            bool flag1 = false;
            int num1 = 1;
            List<BuildPreview> templatePreviews = __instance.actionBuild.templatePreviews;
            if (templatePreviews.Count > 0)
                num1 = templatePreviews.Count;
            bool flag2 = false;
            Vector3 vector3_1 = Vector3.zero;
            if (__instance.planet.id == __instance.planet.galaxy.birthPlanetId && history.SpaceCapsuleExist())
            {
                vector3_1 = __instance.planet.birthPoint;
                flag2 = true;
            }

            for (int index1 = 0; index1 < __instance.buildPreviews.Count; ++index1)
            {
                BuildPreview buildPreview1 = __instance.buildPreviews[index1];
                BuildPreview buildPreview2 = __instance.buildPreviews[index1 / num1 * num1];
                if (buildPreview1.condition == EBuildCondition.Ok)
                {
                    Vector3 vector3_2 = buildPreview1.lpos;
                    Quaternion quaternion1 = buildPreview1.lrot;
                    Vector3 lpos2 = buildPreview1.lpos2;
                    Quaternion lrot2 = buildPreview1.lrot2;
                    Pose pose1 = new Pose(buildPreview1.lpos, buildPreview1.lrot);
                    Pose pose2 = new Pose(buildPreview1.lpos2, buildPreview1.lrot2);
                    Vector3 forward1 = pose1.forward;
                    Vector3 forward2 = pose2.forward;
                    Vector3 up1 = pose1.up;
                    Vector3 vector3_3 = Vector3.Lerp(vector3_2, lpos2, 0.5f);
                    Vector3 forward3 = lpos2 - vector3_2;
                    if ((double)forward3.sqrMagnitude < 9.999999747378752E-05)
                        forward3 = Maths.SphericalRotation(vector3_2, 0.0f).Forward();
                    Quaternion quaternion2 = Quaternion.LookRotation(forward3, vector3_3.normalized);
                    bool flag3 = __instance.planet != null && __instance.planet.type == EPlanetType.Gas;
                    if ((double)vector3_2.sqrMagnitude < 1.0)
                    {
                        buildPreview1.condition = EBuildCondition.Failure;
                    }
                    else
                    {
                        bool flag4 = buildPreview1.desc.minerType == EMinerType.None && !buildPreview1.desc.isBelt && !buildPreview1.desc.isSplitter && (!buildPreview1.desc.isPowerNode || buildPreview1.desc.isPowerGen || buildPreview1.desc.isAccumulator || buildPreview1.desc.isPowerExchanger) && !buildPreview1.desc.isStation && !buildPreview1.desc.isSilo && !buildPreview1.desc.multiLevel && !buildPreview1.desc.isMonitor;
                        Vector3 vector3_4;
                        if (buildPreview1.desc.veinMiner)
                        {
                            Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                            PrebuildData prebuildData = new PrebuildData();
                            int num2 = 0;
                            if (buildPreview1.desc.isVeinCollector)
                            {
                                Vector3 center = vector3_2.normalized * __instance.controller.cmd.test.magnitude + forward1 * -10.5f;
                                Vector3 rhs = -forward1;
                                Vector3 right = pose1.right;
                                int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 12f, ____tmp_ids);
                                prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                                VeinData[] veinPool = __instance.factory.veinPool;
                                EVeinType eveinType = EVeinType.None;
                                for (int index2 = 0; index2 < veinsInAreaNonAlloc; ++index2)
                                {
                                    if (____tmp_ids[index2] != 0 && veinPool[____tmp_ids[index2]].id == ____tmp_ids[index2])
                                    {
                                        if (veinPool[____tmp_ids[index2]].type != EVeinType.Oil)
                                        {
                                            Vector3 lhs = veinPool[____tmp_ids[index2]].pos - center;
                                            double sqrMagnitude = (double)lhs.sqrMagnitude;
                                            float num3 = Mathf.Abs(Vector3.Dot(lhs, rhs));
                                            float num4 = Mathf.Abs(Vector3.Dot(lhs, right));
                                            if (sqrMagnitude <= 100.0 && (double)num3 <= 7.0 && (double)num4 <= 5.5)
                                            {
                                                if (eveinType != veinPool[____tmp_ids[index2]].type)
                                                {
                                                    if (eveinType == EVeinType.None)
                                                        eveinType = veinPool[____tmp_ids[index2]].type;
                                                    else
                                                        buildPreview1.condition = EBuildCondition.NeedSingleResource;
                                                }

                                                prebuildData.parameters[num2++] = ____tmp_ids[index2];
                                            }
                                        }
                                    }
                                    else
                                        Assert.CannotBeReached();
                                }
                            }
                            else
                            {
                                Vector3 center = vector3_2.normalized * __instance.controller.cmd.test.magnitude + forward1 * -1.2f;
                                Vector3 rhs1 = -forward1;
                                Vector3 lhs = up1;
                                int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 12f, ____tmp_ids);
                                prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                                VeinData[] veinPool = __instance.factory.veinPool;
                                EVeinType eveinType = EVeinType.None;
                                for (int index3 = 0; index3 < veinsInAreaNonAlloc; ++index3)
                                {
                                    if (____tmp_ids[index3] != 0 && veinPool[____tmp_ids[index3]].id == ____tmp_ids[index3])
                                    {
                                        if (veinPool[____tmp_ids[index3]].type != EVeinType.Oil)
                                        {
                                            Vector3 rhs2 = veinPool[____tmp_ids[index3]].pos - center;
                                            float f = Vector3.Dot(lhs, rhs2);
                                            Vector3 vector3_5 = rhs2 - lhs * f;
                                            double sqrMagnitude = (double)vector3_5.sqrMagnitude;
                                            float num5 = Vector3.Dot(vector3_5.normalized, rhs1);
                                            if (sqrMagnitude <= 961.0 / 16.0 && (double)num5 >= 0.7300000190734863 && (double)Mathf.Abs(f) <= 2.0)
                                            {
                                                if (eveinType != veinPool[____tmp_ids[index3]].type)
                                                {
                                                    if (eveinType == EVeinType.None)
                                                        eveinType = veinPool[____tmp_ids[index3]].type;
                                                    else
                                                        buildPreview1.condition = EBuildCondition.NeedResource;
                                                }

                                                prebuildData.parameters[num2++] = ____tmp_ids[index3];
                                            }
                                        }
                                    }
                                    else
                                        Assert.CannotBeReached();
                                }
                            }

                            prebuildData.paramCount = num2;
                            prebuildData.ArrageParametersArray();
                            if (buildPreview1.desc.isVeinCollector)
                            {
                                if (buildPreview1.paramCount == 0)
                                {
                                    buildPreview1.parameters = new int[2048];
                                    buildPreview1.paramCount = 2048;
                                }

                                if (prebuildData.paramCount > 0)
                                {
                                    Array.Resize<int>(ref buildPreview1.parameters, buildPreview1.paramCount + prebuildData.paramCount);
                                    Array.Copy((Array)prebuildData.parameters, 0, (Array)buildPreview1.parameters, buildPreview1.paramCount, prebuildData.paramCount);
                                    buildPreview1.paramCount += prebuildData.paramCount;
                                }
                            }
                            else
                            {
                                buildPreview1.parameters = prebuildData.parameters;
                                buildPreview1.paramCount = prebuildData.paramCount;
                            }

                            Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                            if (prebuildData.paramCount == 0)
                            {
                                buildPreview1.condition = EBuildCondition.NeedResource;
                                continue;
                            }
                        }
                        else if (buildPreview1.desc.oilMiner)
                        {
                            Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                            Vector3 center = vector3_2;
                            Vector3 lhs = -up1;
                            int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 10f, ____tmp_ids);
                            PrebuildData prebuildData = new PrebuildData();
                            prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                            VeinData[] veinPool = __instance.factory.veinPool;
                            int num6 = 0;
                            float num7 = 100f;
                            Vector3 pos1 = center;
                            for (int index4 = 0; index4 < veinsInAreaNonAlloc; ++index4)
                            {
                                if (____tmp_ids[index4] != 0 && veinPool[____tmp_ids[index4]].id == ____tmp_ids[index4] && veinPool[____tmp_ids[index4]].type == EVeinType.Oil)
                                {
                                    Vector3 pos2 = veinPool[____tmp_ids[index4]].pos;
                                    Vector3 rhs = pos2 - center;
                                    float num8 = Vector3.Dot(lhs, rhs);
                                    float sqrMagnitude = (rhs - lhs * num8).sqrMagnitude;
                                    if ((double)sqrMagnitude < (double)num7)
                                    {
                                        num7 = sqrMagnitude;
                                        num6 = ____tmp_ids[index4];
                                        pos1 = pos2;
                                    }
                                }
                            }

                            if (num6 != 0)
                            {
                                prebuildData.parameters[0] = num6;
                                prebuildData.paramCount = 1;
                                prebuildData.ArrageParametersArray();
                                buildPreview1.parameters = prebuildData.parameters;
                                buildPreview1.paramCount = prebuildData.paramCount;
                                Vector3 pos3 = __instance.factory.planet.aux.Snap(pos1, true);
                                ref Pose local = ref pose1;
                                BuildPreview buildPreview3 = buildPreview1;
                                buildPreview1.lpos = vector3_4 = pos3;
                                Vector3 vector3_6;
                                vector3_4 = vector3_6 = vector3_4;
                                buildPreview3.lpos2 = vector3_6;
                                Vector3 vector3_7;
                                vector3_2 = vector3_7 = vector3_4;
                                local.position = vector3_7;
                                pose1.rotation = quaternion1 = buildPreview1.lrot2 = buildPreview1.lrot = Maths.SphericalRotation(pos3, __instance.yaw);
                                forward1 = pose1.forward;
                                Vector3 up2 = pose1.up;
                                Array.Clear((Array)____tmp_ids, 0, ____tmp_ids.Length);
                            }
                            else
                            {
                                buildPreview1.condition = EBuildCondition.NeedResource;
                                continue;
                            }
                        }

                        if (buildPreview1.desc.isTank || buildPreview1.desc.isStorage || buildPreview1.desc.isLab || buildPreview1.desc.isSplitter)
                        {
                            int num9 = buildPreview1.desc.isLab ? history.labLevel : history.storageLevel;
                            int num10 = buildPreview1.desc.isLab ? 15 : 8;
                            int num11 = 0;
                            bool isOutput;
                            int otherObjId;
                            int otherSlot;
                            for (__instance.factory.ReadObjectConn(buildPreview1.inputObjId, 14, out isOutput, out otherObjId, out otherSlot); otherObjId != 0; __instance.factory.ReadObjectConn(otherObjId, 14, out isOutput, out otherObjId, out otherSlot))
                                ++num11;
                            if (num11 >= num9 - 1)
                            {
                                flag1 = num9 >= num10;
                                buildPreview1.condition = EBuildCondition.OutOfVerticalConstructionHeight;
                                continue;
                            }
                        }

                        Vector3 vector3_8 = __instance.player.position;
                        float num12 = __instance.player.mecha.buildArea * __instance.player.mecha.buildArea;
                        if (flag3)
                        {
                            vector3_8 = vector3_8.normalized;
                            vector3_8 *= __instance.planet.realRadius;
                            num12 *= 6f;
                        }

                        if ((double)(vector3_2 - vector3_8).sqrMagnitude > (double)num12)
                        {
                            buildPreview1.condition = EBuildCondition.OutOfReach;
                        }
                        else
                        {
                            if (__instance.planet != null)
                            {
                                float num13 = (float)((double)history.buildMaxHeight + 0.5 + (double)__instance.planet.realRadius * (flag3 ? 1.024999976158142 : 1.0));
                                if ((double)vector3_2.sqrMagnitude > (double)num13 * (double)num13)
                                {
                                    buildPreview1.condition = EBuildCondition.OutOfReach;
                                    continue;
                                }
                            }

                            if (buildPreview1.desc.hasBuildCollider)
                            {
                                ColliderData[] buildColliders = buildPreview1.desc.buildColliders;
                                for (int index5 = 0; index5 < buildColliders.Length; ++index5)
                                {
                                    ColliderData buildCollider = buildPreview1.desc.buildColliders[index5];
                                    if (buildPreview1.desc.isInserter)
                                    {
                                        buildCollider.ext = new Vector3(buildCollider.ext.x, buildCollider.ext.y, (float)((double)Vector3.Distance(lpos2, vector3_2) * 0.5 + (double)buildCollider.ext.z - 0.5));
                                        if (__instance.ObjectIsBelt(buildPreview1.inputObjId) || buildPreview1.input != null && buildPreview1.input.desc.isBelt)
                                        {
                                            buildCollider.pos.z -= 0.35f;
                                            buildCollider.ext.z += 0.35f;
                                        }
                                        else if (buildPreview1.inputObjId == 0 && buildPreview1.input == null)
                                        {
                                            buildCollider.pos.z -= 0.35f;
                                            buildCollider.ext.z += 0.35f;
                                        }

                                        if (__instance.ObjectIsBelt(buildPreview1.outputObjId) || buildPreview1.output != null && buildPreview1.output.desc.isBelt)
                                        {
                                            buildCollider.pos.z += 0.35f;
                                            buildCollider.ext.z += 0.35f;
                                        }
                                        else if (buildPreview1.outputObjId == 0 && buildPreview1.output == null)
                                        {
                                            buildCollider.pos.z += 0.35f;
                                            buildCollider.ext.z += 0.35f;
                                        }

                                        if ((double)buildCollider.ext.z < 0.10000000149011612)
                                            buildCollider.ext.z = 0.1f;
                                        buildCollider.pos = vector3_3 + quaternion2 * buildCollider.pos;
                                        buildCollider.q = quaternion2 * buildCollider.q;
                                        buildCollider.DebugDraw();
                                    }
                                    else
                                    {
                                        buildCollider.pos = vector3_2 + quaternion1 * buildCollider.pos;
                                        buildCollider.q = quaternion1 * buildCollider.q;
                                    }

                                    int mask = 428032;
                                    if (buildPreview1.desc.veinMiner || buildPreview1.desc.oilMiner)
                                        mask = 425984;
                                    Array.Clear((Array)____tmp_cols, 0, ____tmp_cols.Length);
                                    int num14 = Physics.OverlapBoxNonAlloc(buildCollider.pos, buildCollider.ext, ____tmp_cols, buildCollider.q, mask, QueryTriggerInteraction.Collide);
                                    if (num14 > 0)
                                    {
                                        bool flag5 = false;
                                        PlanetPhysics physics = __instance.player.planetData.physics;
                                        for (int index6 = 0; index6 < num14 && buildPreview1.coverObjId == 0; ++index6)
                                        {
                                            ColliderData cd;
                                            int num15 = physics.GetColliderData(____tmp_cols[index6], out cd) ? 1 : 0;
                                            int objId = 0;
                                            if (num15 != 0 && cd.isForBuild)
                                            {
                                                if (cd.objType == EObjectType.Entity)
                                                    objId = cd.objId;
                                                else if (cd.objType == EObjectType.Prebuild)
                                                    objId = -cd.objId;
                                            }

                                            Collider tmpCol = ____tmp_cols[index6];
                                            if (tmpCol.gameObject.layer == 18)
                                            {
                                                BuildPreviewModel component = tmpCol.GetComponent<BuildPreviewModel>();
                                                if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.index == buildPreview1.previewIndex || buildPreview1.desc.isInserter && !component.buildPreview.desc.isInserter || !buildPreview1.desc.isInserter && component.buildPreview.desc.isInserter)
                                                    continue;
                                            }
                                            else if (buildPreview1.desc.isInserter)
                                            {
                                                if (objId != 0 && (objId == buildPreview1.inputObjId || objId == buildPreview1.outputObjId || objId == buildPreview2.coverObjId))
                                                    continue;
                                            }
                                            else if (buildPreview1.desc.isStorage && objId != 0 && __instance.GetItemProto(objId).prefabDesc.addonType == EAddonType.Storage)
                                            {
                                                int otherObjId;
                                                __instance.factory.ReadObjectConn(objId, 0, out bool _, out otherObjId, out int _);
                                                if (otherObjId != 0 && (otherObjId < 0 ? (int)__instance.factory.prebuildPool[-otherObjId].protoId : (int)__instance.factory.entityPool[otherObjId].protoId) == buildPreview1.item.ID)
                                                    continue;
                                            }

                                            flag5 = true;
                                            if (flag4 && objId != 0)
                                            {
                                                ItemProto itemProto = __instance.GetItemProto(objId);
                                                if (buildPreview1.item.IsSimilar(itemProto))
                                                {
                                                    Pose objectPose = __instance.GetObjectPose(objId);
                                                    Pose objectPose2 = __instance.GetObjectPose2(objId);
                                                    vector3_4 = objectPose.position - buildPreview1.lpos;
                                                    if ((double)vector3_4.sqrMagnitude < 0.01)
                                                    {
                                                        vector3_4 = objectPose2.position - buildPreview1.lpos2;
                                                        if ((double)vector3_4.sqrMagnitude < 0.01)
                                                        {
                                                            vector3_4 = objectPose.forward - forward1;
                                                            if ((double)vector3_4.sqrMagnitude < 1E-06 || buildPreview1.desc.isInserter)
                                                            {
                                                                if (buildPreview1.item.ID == itemProto.ID)
                                                                {
                                                                    buildPreview1.coverObjId = objId;
                                                                    buildPreview1.willRemoveCover = false;
                                                                    flag5 = false;
                                                                    break;
                                                                }

                                                                buildPreview1.coverObjId = objId;
                                                                buildPreview1.willRemoveCover = true;
                                                                flag5 = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (flag5)
                                        {
                                            buildPreview1.condition = EBuildCondition.Collide;
                                            break;
                                        }
                                    }

                                    if (buildPreview1.desc.veinMiner && Physics.CheckBox(buildCollider.pos, buildCollider.ext, buildCollider.q, 2048, QueryTriggerInteraction.Collide))
                                    {
                                        buildPreview1.condition = EBuildCondition.Collide;
                                        break;
                                    }
                                }

                                if (buildPreview1.condition != EBuildCondition.Ok)
                                    continue;
                            }

                            if (buildPreview2.coverObjId != 0 && buildPreview1.desc.isInserter)
                            {
                                if (buildPreview1.output == buildPreview2)
                                {
                                    buildPreview1.outputObjId = buildPreview2.coverObjId;
                                    buildPreview1.output = (BuildPreview)null;
                                }

                                if (buildPreview1.input == buildPreview2)
                                {
                                    buildPreview1.inputObjId = buildPreview2.coverObjId;
                                    buildPreview1.input = (BuildPreview)null;
                                }
                            }

                            if (buildPreview1.coverObjId == 0 || buildPreview1.willRemoveCover)
                            {
                                int id = buildPreview1.item.ID;
                                int count = 1;
                                if (___tmpInhandId == id && ___tmpInhandCount > 0)
                                {
                                    count = 1;
                                    --___tmpInhandCount;
                                }
                                else
                                    ___tmpPackage.TakeTailItems(ref id, ref count, out int _);

                                if (count == 0)
                                {
                                    buildPreview1.condition = EBuildCondition.NotEnoughItem;
                                    continue;
                                }
                            }

                            if (buildPreview1.coverObjId == 0)
                            {
                                if (buildPreview1.desc.isPowerNode && !buildPreview1.desc.isAccumulator)
                                {
                                    if (buildPreview1.nearestPowerObjId == null || buildPreview1.nearestPowerObjId.Length != buildPreview1.nearestPowerObjId.Length)
                                        buildPreview1.nearestPowerObjId = new int[__instance.factory.powerSystem.netCursor];
                                    Array.Clear((Array)buildPreview1.nearestPowerObjId, 0, buildPreview1.nearestPowerObjId.Length);
                                    float num16 = buildPreview1.desc.powerConnectDistance * buildPreview1.desc.powerConnectDistance;
                                    float x = vector3_2.x;
                                    float y = vector3_2.y;
                                    float z = vector3_2.z;
                                    int netCursor = __instance.factory.powerSystem.netCursor;
                                    PowerNetwork[] netPool = __instance.factory.powerSystem.netPool;
                                    PowerNodeComponent[] nodePool = __instance.factory.powerSystem.nodePool;
                                    PowerGeneratorComponent[] genPool = __instance.factory.powerSystem.genPool;
                                    bool windForcedPower = buildPreview1.desc.windForcedPower;
                                    bool geothermal = buildPreview1.desc.geothermal;
                                    for (int index7 = 1; index7 < netCursor; ++index7)
                                    {
                                        if (netPool[index7] != null && netPool[index7].id != 0)
                                        {
                                            List<PowerNetworkStructures.Node> nodes = netPool[index7].nodes;
                                            int count = nodes.Count;
                                            float num17 = 4900f;
                                            for (int index8 = 0; index8 < count; ++index8)
                                            {
                                                double num18 = (double)x - (double)nodes[index8].x;
                                                float num19 = y - nodes[index8].y;
                                                float num20 = z - nodes[index8].z;
                                                float num21 = (float)(num18 * num18 + (double)num19 * (double)num19 + (double)num20 * (double)num20);
                                                if ((double)num21 < (double)num17 && ((double)num21 < (double)nodes[index8].connDistance2 || (double)num21 < (double)num16))
                                                {
                                                    buildPreview1.nearestPowerObjId[index7] = nodePool[nodes[index8].id].entityId;
                                                    num17 = num21;
                                                }

                                                if (windForcedPower && nodes[index8].genId > 0 && genPool[nodes[index8].genId].id == nodes[index8].genId && genPool[nodes[index8].genId].wind && (double)num21 < 110.25)
                                                    buildPreview1.condition = EBuildCondition.WindTooClose;
                                                else if (geothermal && nodes[index8].genId > 0 && genPool[nodes[index8].genId].id == nodes[index8].genId && genPool[nodes[index8].genId].geothermal && (double)num21 < 144.0)
                                                    buildPreview1.condition = EBuildCondition.GeothermalTooClose;
                                                else if (!buildPreview1.desc.isPowerGen && nodes[index8].genId == 0 && (double)num21 < 12.25)
                                                    buildPreview1.condition = EBuildCondition.PowerTooClose;
                                                else if ((double)num21 < 12.25)
                                                    buildPreview1.condition = EBuildCondition.PowerTooClose;
                                            }
                                        }
                                    }

                                    PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
                                    int prebuildCursor = __instance.factory.prebuildCursor;
                                    float num22 = 4900f;
                                    for (int index9 = 1; index9 < prebuildCursor; ++index9)
                                    {
                                        if (prebuildPool[index9].id == index9 && prebuildPool[index9].protoId >= (short)2199 && prebuildPool[index9].protoId <= (short)2299)
                                        {
                                            double num23 = (double)x - (double)prebuildPool[index9].pos.x;
                                            float num24 = y - prebuildPool[index9].pos.y;
                                            float num25 = z - prebuildPool[index9].pos.z;
                                            float num26 = (float)(num23 * num23 + (double)num24 * (double)num24 + (double)num25 * (double)num25);
                                            if ((double)num26 < (double)num22)
                                            {
                                                ItemProto itemProto = LDB.items.Select((int)prebuildPool[index9].protoId);
                                                if (itemProto != null && itemProto.prefabDesc.isPowerNode)
                                                {
                                                    if ((double)num26 < (double)itemProto.prefabDesc.powerConnectDistance * (double)itemProto.prefabDesc.powerConnectDistance || (double)num26 < (double)num16)
                                                    {
                                                        buildPreview1.nearestPowerObjId[0] = -index9;
                                                        num22 = num26;
                                                    }

                                                    if (windForcedPower && itemProto.prefabDesc.windForcedPower && (double)num26 < 110.25)
                                                        buildPreview1.condition = EBuildCondition.WindTooClose;
                                                    else if (geothermal && itemProto.prefabDesc.geothermal && (double)num26 < 144.0)
                                                        buildPreview1.condition = EBuildCondition.GeothermalTooClose;
                                                    else if (!buildPreview1.desc.isPowerGen && !itemProto.prefabDesc.isPowerGen && (double)num26 < 12.25)
                                                        buildPreview1.condition = EBuildCondition.PowerTooClose;
                                                    else if ((double)num26 < 12.25)
                                                        buildPreview1.condition = EBuildCondition.PowerTooClose;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (buildPreview1.desc.isCollectStation)
                                {
                                    if (__instance.planet != null && __instance.planet.gasItems != null && __instance.planet.gasItems.Length != 0)
                                    {
                                        for (int index10 = 0; index10 < __instance.planet.gasItems.Length; ++index10)
                                        {
                                            double num27 = 0.0;
                                            if ((double)buildPreview1.desc.stationCollectSpeed * __instance.planet.gasTotalHeat != 0.0)
                                                num27 = 1.0 - (double)buildPreview1.desc.workEnergyPerTick / ((double)buildPreview1.desc.stationCollectSpeed * __instance.planet.gasTotalHeat * (1.0 / 60.0));
                                            if (num27 <= 0.0)
                                                buildPreview1.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
                                        }

                                        float y = __instance.cursorTarget.y;
                                        if ((double)y > 0.10000000149011612 || (double)y < -0.10000000149011612)
                                        {
                                            buildPreview1.condition = EBuildCondition.BuildInEquator;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        buildPreview1.condition = EBuildCondition.OutOfReach;
                                        continue;
                                    }
                                }

                                if (buildPreview1.desc.isStation)
                                {
                                    StationComponent[] stationPool = __instance.factory.transport.stationPool;
                                    int stationCursor = __instance.factory.transport.stationCursor;
                                    PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
                                    int prebuildCursor = __instance.factory.prebuildCursor;
                                    EntityData[] entityPool = __instance.factory.entityPool;
                                    float num28 = 225f;
                                    float num29 = 625f;
                                    float num30 = 841f;
                                    float num31 = 14297f;
                                    float num32 = buildPreview1.desc.isVeinCollector ? num29 : num28;
                                    float num33 = buildPreview1.desc.isCollectStation ? num31 : num30;
                                    for (int index11 = 1; index11 < stationCursor; ++index11)
                                    {
                                        if (stationPool[index11] != null && stationPool[index11].id == index11 && (!buildPreview1.desc.isVeinCollector || !stationPool[index11].isVeinCollector))
                                        {
                                            float num34 = stationPool[index11].isStellar || buildPreview1.desc.isStellarStation ? num33 : num32;
                                            vector3_4 = entityPool[stationPool[index11].entityId].pos - vector3_2;
                                            if ((double)vector3_4.sqrMagnitude < (double)num34)
                                                buildPreview1.condition = !stationPool[index11].isVeinCollector ? EBuildCondition.TowerTooClose : EBuildCondition.MK2MinerTooClose;
                                        }
                                    }

                                    for (int index12 = 1; index12 < prebuildCursor; ++index12)
                                    {
                                        if (prebuildPool[index12].id == index12)
                                        {
                                            ItemProto itemProto = LDB.items.Select((int)prebuildPool[index12].protoId);
                                            if (itemProto != null && itemProto.prefabDesc.isStation && (!buildPreview1.desc.isVeinCollector || !itemProto.prefabDesc.isVeinCollector))
                                            {
                                                float num35 = itemProto.prefabDesc.isStellarStation || buildPreview1.desc.isStellarStation ? num33 : num32;
                                                if (buildPreview1.desc.isVeinCollector && itemProto.prefabDesc.isVeinCollector)
                                                    num35 = 0.0f;
                                                double num36 = (double)vector3_2.x - (double)prebuildPool[index12].pos.x;
                                                float num37 = vector3_2.y - prebuildPool[index12].pos.y;
                                                float num38 = vector3_2.z - prebuildPool[index12].pos.z;
                                                if (num36 * num36 + (double)num37 * (double)num37 + (double)num38 * (double)num38 < (double)num35)
                                                    buildPreview1.condition = !itemProto.prefabDesc.isVeinCollector ? EBuildCondition.TowerTooClose : EBuildCondition.MK2MinerTooClose;
                                            }
                                        }
                                    }
                                }

                                if (!buildPreview1.desc.isInserter && (double)vector3_2.magnitude - (double)__instance.planet.realRadius + (double)buildPreview1.desc.cullingHeight > 4.900000095367432 && !buildPreview1.desc.isEjector)
                                {
                                    EjectorComponent[] ejectorPool = __instance.factory.factorySystem.ejectorPool;
                                    int ejectorCursor = __instance.factory.factorySystem.ejectorCursor;
                                    PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
                                    int prebuildCursor = __instance.factory.prebuildCursor;
                                    EntityData[] entityPool = __instance.factory.entityPool;
                                    Vector3 ext = buildPreview1.desc.buildCollider.ext;
                                    float num39 = 7.2f + Mathf.Sqrt((float)((double)ext.x * (double)ext.x + (double)ext.z * (double)ext.z));
                                    Vector3 vector3_9 = vector3_2;
                                    if (buildPreview1.desc.isVeinCollector)
                                    {
                                        num39 = 14.6f;
                                        vector3_9 -= forward1 * 10f;
                                    }

                                    for (int index13 = 1; index13 < ejectorCursor; ++index13)
                                    {
                                        if (ejectorPool[index13].id == index13)
                                        {
                                            vector3_4 = entityPool[ejectorPool[index13].entityId].pos - vector3_9;
                                            if ((double)vector3_4.sqrMagnitude < (double)num39 * (double)num39)
                                                buildPreview1.condition = EBuildCondition.EjectorTooClose;
                                        }
                                    }

                                    for (int index14 = 1; index14 < prebuildCursor; ++index14)
                                    {
                                        if (prebuildPool[index14].id == index14)
                                        {
                                            ItemProto itemProto = LDB.items.Select((int)prebuildPool[index14].protoId);
                                            if (itemProto != null && itemProto.prefabDesc.isEjector)
                                            {
                                                double num40 = (double)vector3_9.x - (double)prebuildPool[index14].pos.x;
                                                float num41 = vector3_9.y - prebuildPool[index14].pos.y;
                                                float num42 = vector3_9.z - prebuildPool[index14].pos.z;
                                                if (num40 * num40 + (double)num41 * (double)num41 + (double)num42 * (double)num42 < (double)num39 * (double)num39)
                                                    buildPreview1.condition = EBuildCondition.EjectorTooClose;
                                            }
                                        }
                                    }
                                }

                                if (buildPreview1.desc.isEjector)
                                {
                                    __instance.GetOverlappedObjectsNonAlloc(vector3_2, 20f, 14.5f);
                                    for (int index15 = 0; index15 < ____overlappedCount; ++index15)
                                    {
                                        PrefabDesc prefabDesc = __instance.GetPrefabDesc(____overlappedIds[index15]);
                                        Pose objectPose = __instance.GetObjectPose(____overlappedIds[index15]);
                                        Vector3 position = objectPose.position;
                                        if ((double)position.magnitude - (double)__instance.planet.realRadius + (double)prefabDesc.cullingHeight > 4.900000095367432)
                                        {
                                            float num43 = vector3_2.x - position.x;
                                            float num44 = vector3_2.y - position.y;
                                            float num45 = vector3_2.z - position.z;
                                            if (prefabDesc.isVeinCollector)
                                            {
                                                num43 += objectPose.forward.x * 10f;
                                                num44 += objectPose.forward.y * 10f;
                                                num45 += objectPose.forward.z * 10f;
                                            }

                                            double num46 = (double)num43 * (double)num43 + (double)num44 * (double)num44 + (double)num45 * (double)num45;
                                            Vector3 ext = prefabDesc.buildCollider.ext;
                                            float num47 = 7.2f + Mathf.Sqrt((float)((double)ext.x * (double)ext.x + (double)ext.z * (double)ext.z));
                                            if (prefabDesc.isEjector)
                                                num47 = 10.6f;
                                            else if (prefabDesc.isVeinCollector)
                                                num47 = 14.6f;
                                            double num48 = (double)num47 * (double)num47;
                                            if (num46 < num48)
                                                buildPreview1.condition = EBuildCondition.BlockTooClose;
                                        }
                                    }
                                }

                                if (flag2 && (double)vector3_2.magnitude < (double)__instance.planet.realRadius + 3.0)
                                {
                                    Vector3 ext = buildPreview1.desc.buildCollider.ext;
                                    float num49 = Mathf.Sqrt((float)((double)ext.x * (double)ext.x + (double)ext.z * (double)ext.z));
                                    vector3_4 = vector3_2 - vector3_1;
                                    if ((double)vector3_4.magnitude - (double)num49 < 3.700000047683716)
                                    {
                                        buildPreview1.condition = EBuildCondition.Collide;
                                        continue;
                                    }
                                }

                                if ((!buildPreview1.desc.multiLevel || buildPreview1.inputObjId == 0) && !buildPreview1.desc.isInserter)
                                {
                                    RaycastHit hitInfo;
                                    for (int index16 = 0; index16 < buildPreview1.desc.landPoints.Length; ++index16)
                                    {
                                        Vector3 landPoint = buildPreview1.desc.landPoints[index16] with
                                        {
                                            y = 0.0f
                                        };
                                        Vector3 vector3_10 = vector3_2 + quaternion1 * landPoint;
                                        Vector3 normalized = vector3_10.normalized;
                                        Vector3 origin = vector3_10 + normalized * 3f;
                                        Vector3 direction = -normalized;
                                        if (flag3)
                                        {
                                            Vector3 vector3_11 = __instance.cursorTarget.normalized * __instance.planet.realRadius * 0.025f;
                                            origin -= vector3_11;
                                        }

                                        if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide))
                                        {
                                            float distance = hitInfo.distance;
                                            vector3_4 = hitInfo.point;
                                            if ((double)vector3_4.magnitude - (double)__instance.factory.planet.realRadius < -0.30000001192092896)
                                            {
                                                buildPreview1.condition = EBuildCondition.NeedGround;
                                            }
                                            else
                                            {
                                                float num50 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
                                                if ((double)distance - (double)num50 > 0.27000001072883606)
                                                    buildPreview1.condition = EBuildCondition.NeedGround;
                                            }
                                        }
                                        else
                                            buildPreview1.condition = EBuildCondition.NeedGround;
                                    }

                                    for (int index17 = 0; index17 < buildPreview1.desc.waterPoints.Length; ++index17)
                                    {
                                        bool flag6 = false;
                                        for (int index18 = 0; index18 < buildPreview1.desc.waterTypes.Length; ++index18)
                                        {
                                            if (__instance.factory.planet.waterItemId == buildPreview1.desc.waterTypes[index18])
                                            {
                                                flag6 = true;
                                                break;
                                            }
                                        }

                                        if (!flag6)
                                        {
                                            buildPreview1.condition = !buildPreview1.desc.geothermal ? EBuildCondition.NeedWater : EBuildCondition.NeedGeothermalResource;
                                        }
                                        else
                                        {
                                            Vector3 waterPoint = buildPreview1.desc.waterPoints[index17] with
                                            {
                                                y = __instance.planet.waterHeight
                                            };
                                            Vector3 origin = vector3_2 + quaternion1 * waterPoint;
                                            Vector3 normalized = origin.normalized;
                                            origin += normalized * 3f;
                                            Vector3 direction = -normalized;
                                            float num51 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
                                            if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide))
                                            {
                                                float distance = hitInfo.distance;
                                                if ((double)num51 - (double)distance <= 0.27000001072883606)
                                                    buildPreview1.condition = !buildPreview1.desc.geothermal ? EBuildCondition.NeedWater : EBuildCondition.NeedGeothermalResource;
                                            }
                                            else
                                                buildPreview1.condition = !buildPreview1.desc.geothermal ? EBuildCondition.NeedWater : EBuildCondition.NeedGeothermalResource;
                                        }
                                    }
                                }

                                if (buildPreview1.desc.isInserter && buildPreview1.condition == EBuildCondition.Ok)
                                {
                                    bool flag7 = __instance.ObjectIsBelt(buildPreview1.inputObjId) || buildPreview1.input != null && buildPreview1.input.desc.isBelt;
                                    bool flag8 = __instance.ObjectIsBelt(buildPreview1.outputObjId) || buildPreview1.output != null && buildPreview1.output.desc.isBelt;
                                    Vector3 zero = Vector3.zero;
                                    Vector3 vector3_12 = buildPreview1.output == null ? __instance.GetObjectPose(buildPreview1.outputObjId).position : buildPreview1.output.lpos;
                                    Vector3 vector3_13 = buildPreview1.input == null ? __instance.GetObjectPose(buildPreview1.inputObjId).position : buildPreview1.input.lpos;
                                    float num52 = __instance.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(!flag7 || flag8 ? (!(!flag7 & flag8) ? (vector3_12 + vector3_13) * 0.5f : vector3_13) : vector3_12, buildPreview1.lpos, buildPreview1.lpos2);
                                    float num53 = num52;
                                    float magnitude = forward3.magnitude;
                                    float num54 = 5.5f;
                                    float num55 = 0.6f;
                                    float num56 = 3.499f;
                                    float num57 = 0.88f;
                                    if (flag7 & flag8)
                                    {
                                        num55 = 0.4f;
                                        num54 = 5f;
                                        num56 = 3.2f;
                                        num57 = 0.8f;
                                    }
                                    else if (!flag7 && !flag8)
                                    {
                                        num55 = 0.9f;
                                        num54 = 7.5f;
                                        num56 = 3.799f;
                                        num57 = 1.451f;
                                        num53 -= 0.3f;
                                    }

                                    if ((double)magnitude > (double)num54)
                                        buildPreview1.condition = EBuildCondition.TooFar;
                                    else if ((double)magnitude < (double)num55)
                                        buildPreview1.condition = EBuildCondition.TooClose;
                                    else if ((double)num52 > (double)num56)
                                        buildPreview1.condition = EBuildCondition.TooFar;
                                    else if ((double)num52 < (double)num57)
                                    {
                                        buildPreview1.condition = EBuildCondition.TooClose;
                                    }
                                    else
                                    {
                                        int num58 = Mathf.RoundToInt(Mathf.Clamp(num53, 1f, 3f));
                                        buildPreview1.SetOneParameter(num58);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            bool flag9 = true;
            for (int index = 0; index < __instance.buildPreviews.Count; ++index)
            {
                BuildPreview buildPreview = __instance.buildPreviews[index];
                if (buildPreview.condition != EBuildCondition.Ok && buildPreview.condition != EBuildCondition.NeedConn)
                {
                    flag9 = false;
                    __instance.actionBuild.model.cursorState = -1;
                    __instance.actionBuild.model.cursorText = buildPreview.conditionText;
                    if (buildPreview.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag1)
                    {
                        __instance.actionBuild.model.cursorText += "垂直建造可升级".Translate();
                        break;
                    }

                    break;
                }
            }

            if (flag9)
            {
                __instance.actionBuild.model.cursorState = 0;
                string str = __instance.dotCount > 1 ? "    (" + __instance.dotCount.ToString() + ")" : "";
                __instance.actionBuild.model.cursorText = "点击鼠标建造".Translate() + str;
                if (__instance.buildPreviews.Count == 1 && __instance.buildPreviews[0].desc.geothermal)
                {
                    float geothermalStrenth = __instance.factory.powerSystem.CalculateGeothermalStrenth(__instance.buildPreviews[0].lpos, __instance.buildPreviews[0].lrot);
                    BuildModel model = __instance.actionBuild.model;
                    model.cursorText = model.cursorText + "\r\n" + "地热".Translate() + ":" + (geothermalStrenth * 100f).ToString("0") + "%";
                }
            }

            if (!flag9 && !VFInput.onGUI)
                UICursor.SetCursor(ECursor.Ban);
            return flag9;
        }
    }
}