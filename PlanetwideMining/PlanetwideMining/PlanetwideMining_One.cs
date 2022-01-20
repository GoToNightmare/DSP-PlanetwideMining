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
			ref int[] ____tmp_ids, // BuildTool._tmp_ids
			ref Collider[] ____tmp_cols, // BuildTool._tmp_cols
			ref int ___tmpInhandId,
			ref int ___tmpInhandCount,
			ref StorageComponent ___tmpPackage,
			ref int ____overlappedCount,
			ref int[] ____overlappedIds
		)
		{
			// Debug.LogError("[CheckBuildConditions]");
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
					if ((double) forward3.sqrMagnitude < 9.99999974737875E-05)
						forward3 = Maths.SphericalRotation(vector3_2, 0.0f).Forward();
					Quaternion quaternion2 = Quaternion.LookRotation(forward3, vector3_3.normalized);
					bool flag3 = __instance.planet != null && __instance.planet.type == EPlanetType.Gas;
					if ((double) vector3_2.sqrMagnitude < 1.0)
					{
						buildPreview1.condition = EBuildCondition.Failure;
					}
					else
					{
						bool flag4 = buildPreview1.desc.minerType == EMinerType.None && !buildPreview1.desc.isBelt && !buildPreview1.desc.isSplitter && (!buildPreview1.desc.isPowerNode || buildPreview1.desc.isPowerGen || buildPreview1.desc.isAccumulator || buildPreview1.desc.isPowerExchanger) && !buildPreview1.desc.isStation && !buildPreview1.desc.isSilo && !buildPreview1.desc.multiLevel && !buildPreview1.desc.isMonitor;
						Vector3 vector3_4;
						if (buildPreview1.desc.veinMiner)
						{
							Array.Clear((Array) ____tmp_ids, 0, ____tmp_ids.Length);

							PrebuildData prebuildData = default(PrebuildData);
							VeinData[] veinPool = __instance.factory.veinPool;
							prebuildData.InitParametersArray(veinPool.Length);

							// `start
							// Debug.LogError($"[000] veinPool.Length {veinPool.Length}");
							if (prebuildData.parameters != null)
							{
								EVeinType targetVeinType = PlanetwideMining.ResourceForGlobalMining;
								List<int> newPrebuildDataParameters = new List<int>();
								for (int iaa = 0; iaa < veinPool.Length; iaa++)
								{
									if (veinPool[iaa].type != targetVeinType) continue;
									newPrebuildDataParameters.Add(veinPool[iaa].id);
								}

								prebuildData.parameters = newPrebuildDataParameters.ToArray();
							}
							// `end

							prebuildData.paramCount = prebuildData.parameters.Length;
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
									Array.Copy((Array) prebuildData.parameters, 0, (Array) buildPreview1.parameters, buildPreview1.paramCount, prebuildData.paramCount);
									buildPreview1.paramCount += prebuildData.paramCount;
								}
							}
							else
							{
								buildPreview1.parameters = prebuildData.parameters;
								buildPreview1.paramCount = prebuildData.paramCount;
							}


							if (prebuildData.paramCount == 0)
							{
								buildPreview1.condition = EBuildCondition.NeedResource;
								continue;
							}
						}
						else if (buildPreview1.desc.oilMiner)
						{
							Array.Clear((Array) ____tmp_ids, 0, ____tmp_ids.Length);
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
									if ((double) sqrMagnitude < (double) num7)
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
								Array.Clear((Array) ____tmp_ids, 0, ____tmp_ids.Length);
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

						if ((double) (vector3_2 - vector3_8).sqrMagnitude > (double) num12)
						{
							buildPreview1.condition = EBuildCondition.OutOfReach;
						}
						else
						{
							if (__instance.planet != null)
							{
								float num13 = (float) ((double) history.buildMaxHeight + 0.5 + (double) __instance.planet.realRadius * (flag3 ? 1.02499997615814 : 1.0));
								if ((double) vector3_2.sqrMagnitude > (double) num13 * (double) num13)
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
										buildCollider.ext = new Vector3(buildCollider.ext.x, buildCollider.ext.y, (float) ((double) Vector3.Distance(lpos2, vector3_2) * 0.5 + (double) buildCollider.ext.z - 0.5));
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

										if ((double) buildCollider.ext.z < 0.100000001490116)
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
									Array.Clear((Array) ____tmp_cols, 0, ____tmp_cols.Length);
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
												if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.index == buildPreview1.previewIndex || buildPreview1.desc.isInserter && !component.buildPreview.desc.isInserter || !buildPreview1.desc.isInserter && component.buildPreview.desc.isInserter)
													continue;
											}
											else if (buildPreview1.desc.isInserter && objId != 0 && (objId == buildPreview1.inputObjId || objId == buildPreview1.outputObjId || objId == buildPreview2.coverObjId))
												continue;

											flag5 = true;
											if (flag4 && objId != 0)
											{
												ItemProto itemProto = __instance.GetItemProto(objId);
												if (buildPreview1.item.IsSimilar(itemProto))
												{
													Pose objectPose = __instance.GetObjectPose(objId);
													Pose objectPose2 = __instance.GetObjectPose2(objId);
													vector3_4 = objectPose.position - buildPreview1.lpos;
													if ((double) vector3_4.sqrMagnitude < 0.01)
													{
														vector3_4 = objectPose2.position - buildPreview1.lpos2;
														if ((double) vector3_4.sqrMagnitude < 0.01)
														{
															vector3_4 = objectPose.forward - forward1;
															if ((double) vector3_4.sqrMagnitude < 1E-06 || buildPreview1.desc.isInserter)
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
									buildPreview1.output = (BuildPreview) null;
								}

								if (buildPreview1.input == buildPreview2)
								{
									buildPreview1.inputObjId = buildPreview2.coverObjId;
									buildPreview1.input = (BuildPreview) null;
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
									Array.Clear((Array) buildPreview1.nearestPowerObjId, 0, buildPreview1.nearestPowerObjId.Length);
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
												double num18 = (double) x - (double) nodes[index8].x;
												float num19 = y - nodes[index8].y;
												float num20 = z - nodes[index8].z;
												float num21 = (float) (num18 * num18 + (double) num19 * (double) num19 + (double) num20 * (double) num20);
												if ((double) num21 < (double) num17 && ((double) num21 < (double) nodes[index8].connDistance2 || (double) num21 < (double) num16))
												{
													buildPreview1.nearestPowerObjId[index7] = nodePool[nodes[index8].id].entityId;
													num17 = num21;
												}

												if (windForcedPower && nodes[index8].genId > 0 && genPool[nodes[index8].genId].id == nodes[index8].genId && genPool[nodes[index8].genId].wind && (double) num21 < 110.25)
													buildPreview1.condition = EBuildCondition.WindTooClose;
												else if (geothermal && nodes[index8].genId > 0 && genPool[nodes[index8].genId].id == nodes[index8].genId && genPool[nodes[index8].genId].geothermal && (double) num21 < 144.0)
													buildPreview1.condition = EBuildCondition.GeothermalTooClose;
												else if (!buildPreview1.desc.isPowerGen && nodes[index8].genId == 0 && (double) num21 < 12.25)
													buildPreview1.condition = EBuildCondition.PowerTooClose;
												else if ((double) num21 < 12.25)
													buildPreview1.condition = EBuildCondition.PowerTooClose;
											}
										}
									}

									PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
									int prebuildCursor = __instance.factory.prebuildCursor;
									float num22 = 4900f;
									for (int index9 = 1; index9 < prebuildCursor; ++index9)
									{
										if (prebuildPool[index9].id == index9 && prebuildPool[index9].protoId >= (short) 2199 && prebuildPool[index9].protoId <= (short) 2299)
										{
											double num23 = (double) x - (double) prebuildPool[index9].pos.x;
											float num24 = y - prebuildPool[index9].pos.y;
											float num25 = z - prebuildPool[index9].pos.z;
											float num26 = (float) (num23 * num23 + (double) num24 * (double) num24 + (double) num25 * (double) num25);
											if ((double) num26 < (double) num22)
											{
												ItemProto itemProto = LDB.items.Select((int) prebuildPool[index9].protoId);
												if (itemProto != null && itemProto.prefabDesc.isPowerNode)
												{
													if ((double) num26 < (double) itemProto.prefabDesc.powerConnectDistance * (double) itemProto.prefabDesc.powerConnectDistance || (double) num26 < (double) num16)
													{
														buildPreview1.nearestPowerObjId[0] = -index9;
														num22 = num26;
													}

													if (windForcedPower && itemProto.prefabDesc.windForcedPower && (double) num26 < 110.25)
														buildPreview1.condition = EBuildCondition.WindTooClose;
													else if (geothermal && itemProto.prefabDesc.geothermal && (double) num26 < 144.0)
														buildPreview1.condition = EBuildCondition.GeothermalTooClose;
													else if (!buildPreview1.desc.isPowerGen && !itemProto.prefabDesc.isPowerGen && (double) num26 < 12.25)
														buildPreview1.condition = EBuildCondition.PowerTooClose;
													else if ((double) num26 < 12.25)
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
											if ((double) buildPreview1.desc.stationCollectSpeed * __instance.planet.gasTotalHeat != 0.0)
												num27 = 1.0 - (double) buildPreview1.desc.workEnergyPerTick / ((double) buildPreview1.desc.stationCollectSpeed * __instance.planet.gasTotalHeat * (1.0 / 60.0));
											if (num27 <= 0.0)
												buildPreview1.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
										}

										float y = __instance.cursorTarget.y;
										if ((double) y > 0.100000001490116 || (double) y < -0.100000001490116)
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
											if ((double) vector3_4.sqrMagnitude < (double) num34)
												buildPreview1.condition = !stationPool[index11].isVeinCollector ? EBuildCondition.TowerTooClose : EBuildCondition.MK2MinerTooClose;
										}
									}

									for (int index12 = 1; index12 < prebuildCursor; ++index12)
									{
										if (prebuildPool[index12].id == index12)
										{
											ItemProto itemProto = LDB.items.Select((int) prebuildPool[index12].protoId);
											if (itemProto != null && itemProto.prefabDesc.isStation && (!buildPreview1.desc.isVeinCollector || !itemProto.prefabDesc.isVeinCollector))
											{
												float num35 = itemProto.prefabDesc.isStellarStation || buildPreview1.desc.isStellarStation ? num33 : num32;
												if (buildPreview1.desc.isVeinCollector && itemProto.prefabDesc.isVeinCollector)
													num35 = 0.0f;
												double num36 = (double) vector3_2.x - (double) prebuildPool[index12].pos.x;
												float num37 = vector3_2.y - prebuildPool[index12].pos.y;
												float num38 = vector3_2.z - prebuildPool[index12].pos.z;
												if (num36 * num36 + (double) num37 * (double) num37 + (double) num38 * (double) num38 < (double) num35)
													buildPreview1.condition = !itemProto.prefabDesc.isVeinCollector ? EBuildCondition.TowerTooClose : EBuildCondition.MK2MinerTooClose;
											}
										}
									}
								}

								if (!buildPreview1.desc.isInserter && (double) vector3_2.magnitude - (double) __instance.planet.realRadius + (double) buildPreview1.desc.cullingHeight > 4.90000009536743 && !buildPreview1.desc.isEjector)
								{
									EjectorComponent[] ejectorPool = __instance.factory.factorySystem.ejectorPool;
									int ejectorCursor = __instance.factory.factorySystem.ejectorCursor;
									PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
									int prebuildCursor = __instance.factory.prebuildCursor;
									EntityData[] entityPool = __instance.factory.entityPool;
									Vector3 ext = buildPreview1.desc.buildCollider.ext;
									float num39 = 7.2f + Mathf.Sqrt((float) ((double) ext.x * (double) ext.x + (double) ext.z * (double) ext.z));
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
											if ((double) vector3_4.sqrMagnitude < (double) num39 * (double) num39)
												buildPreview1.condition = EBuildCondition.EjectorTooClose;
										}
									}

									for (int index14 = 1; index14 < prebuildCursor; ++index14)
									{
										if (prebuildPool[index14].id == index14)
										{
											ItemProto itemProto = LDB.items.Select((int) prebuildPool[index14].protoId);
											if (itemProto != null && itemProto.prefabDesc.isEjector)
											{
												double num40 = (double) vector3_9.x - (double) prebuildPool[index14].pos.x;
												float num41 = vector3_9.y - prebuildPool[index14].pos.y;
												float num42 = vector3_9.z - prebuildPool[index14].pos.z;
												if (num40 * num40 + (double) num41 * (double) num41 + (double) num42 * (double) num42 < (double) num39 * (double) num39)
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
										if ((double) position.magnitude - (double) __instance.planet.realRadius + (double) prefabDesc.cullingHeight > 4.90000009536743)
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

											double num46 = (double) num43 * (double) num43 + (double) num44 * (double) num44 + (double) num45 * (double) num45;
											Vector3 ext = prefabDesc.buildCollider.ext;
											float num47 = 7.2f + Mathf.Sqrt((float) ((double) ext.x * (double) ext.x + (double) ext.z * (double) ext.z));
											if (prefabDesc.isEjector)
												num47 = 10.6f;
											else if (prefabDesc.isVeinCollector)
												num47 = 14.6f;
											double num48 = (double) num47 * (double) num47;
											if (num46 < num48)
												buildPreview1.condition = EBuildCondition.BlockTooClose;
										}
									}
								}

								if (flag2 && (double) vector3_2.magnitude < (double) __instance.planet.realRadius + 3.0)
								{
									Vector3 ext = buildPreview1.desc.buildCollider.ext;
									float num49 = Mathf.Sqrt((float) ((double) ext.x * (double) ext.x + (double) ext.z * (double) ext.z));
									vector3_4 = vector3_2 - vector3_1;
									if ((double) vector3_4.magnitude - (double) num49 < 3.70000004768372)
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
										Vector3 landPoint = buildPreview1.desc.landPoints[index16];
										{
											landPoint.y = 0.0f;
										}
										;
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
											if ((double) vector3_4.magnitude - (double) __instance.factory.planet.realRadius < -0.300000011920929)
											{
												buildPreview1.condition = EBuildCondition.NeedGround;
											}
											else
											{
												float num50 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
												if ((double) distance - (double) num50 > 0.270000010728836)
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
											Vector3 waterPoint = buildPreview1.desc.waterPoints[index17];
											{
												waterPoint.y = __instance.planet.waterHeight;
											}
											;
											Vector3 origin = vector3_2 + quaternion1 * waterPoint;
											Vector3 normalized = origin.normalized;
											origin += normalized * 3f;
											Vector3 direction = -normalized;
											float num51 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
											if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide))
											{
												float distance = hitInfo.distance;
												if ((double) num51 - (double) distance <= 0.270000010728836)
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

									if ((double) magnitude > (double) num54)
										buildPreview1.condition = EBuildCondition.TooFar;
									else if ((double) magnitude < (double) num55)
										buildPreview1.condition = EBuildCondition.TooClose;
									else if ((double) num52 > (double) num56)
										buildPreview1.condition = EBuildCondition.TooFar;
									else if ((double) num52 < (double) num57)
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
				__instance.actionBuild.model.cursorText = "点击鼠标建造".Translate();
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


		// public static bool CheckBuildConditions(
		// 	ref BuildTool_Click buildToolClick,
		// 	ref int[] this_tmp_ids,
		// 	ref Collider[] thisTmpCols,
		// 	ref int thisTmpInhandId,
		// 	ref int thisTmpInhandCount,
		// 	ref int thisOverlappedCount,
		// 	ref int[] this_overlappedIds,
		// 	ref StorageComponent this_storageComponent)
		// {
		// 	if (buildToolClick.buildPreviews.Count == 0)
		// 	{
		// 		return false;
		// 	}
		//
		// 	GameHistoryData history = buildToolClick.actionBuild.history;
		// 	bool flag = false;
		// 	int num = 1;
		// 	List<BuildPreview> templatePreviews = buildToolClick.actionBuild.templatePreviews;
		// 	if (templatePreviews.Count > 0)
		// 	{
		// 		num = templatePreviews.Count;
		// 	}
		//
		// 	bool flag2 = false;
		// 	Vector3 b = Vector3.zero;
		// 	if (buildToolClick.planet.id == buildToolClick.planet.galaxy.birthPlanetId && history.SpaceCapsuleExist())
		// 	{
		// 		b = buildToolClick.planet.birthPoint;
		// 		flag2 = true;
		// 	}
		//
		// 	for (int i = 0; i < buildToolClick.buildPreviews.Count; i++)
		// 	{
		// 		BuildPreview buildPreview = buildToolClick.buildPreviews[i];
		// 		BuildPreview buildPreview2 = buildToolClick.buildPreviews[i / num * num];
		// 		if (buildPreview.condition == EBuildCondition.Ok)
		// 		{
		// 			Vector3 vector = buildPreview.lpos;
		// 			Quaternion quaternion = buildPreview.lrot;
		// 			Vector3 lpos = buildPreview.lpos2;
		// 			Quaternion lrot = buildPreview.lrot2;
		// 			Pose pose = new Pose(buildPreview.lpos, buildPreview.lrot);
		// 			Pose pose2 = new Pose(buildPreview.lpos2, buildPreview.lrot2);
		// 			Vector3 forward = pose.forward;
		// 			Vector3 forward2 = pose2.forward;
		// 			Vector3 up = pose.up;
		// 			Vector3 a = Vector3.Lerp(vector, lpos, 0.5f);
		// 			Vector3 forward3 = lpos - vector;
		// 			if (forward3.sqrMagnitude < 0.0001f)
		// 			{
		// 				forward3 = Maths.SphericalRotation(vector, 0f).Forward();
		// 			}
		//
		// 			Quaternion quaternion2 = Quaternion.LookRotation(forward3, a.normalized);
		// 			bool flag3 = buildToolClick.planet != null && buildToolClick.planet.type == EPlanetType.Gas;
		// 			if (vector.sqrMagnitude < 1f)
		// 			{
		// 				buildPreview.condition = EBuildCondition.Failure;
		// 			}
		// 			else
		// 			{
		// 				bool flag4 = buildPreview.desc.minerType == EMinerType.None
		// 				             && !buildPreview.desc.isBelt
		// 				             && !buildPreview.desc.isSplitter
		// 				             && (!buildPreview.desc.isPowerNode || buildPreview.desc.isPowerGen || buildPreview.desc.isAccumulator || buildPreview.desc.isPowerExchanger)
		// 				             && !buildPreview.desc.isStation
		// 				             && !buildPreview.desc.isSilo
		// 				             && !buildPreview.desc.multiLevel
		// 				             && !buildPreview.desc.isMonitor;
		// 				if (buildPreview.desc.veinMiner)
		// 				{
		// 					PrebuildData prebuildData = default(PrebuildData);
		// 					VeinData[] veinPool = buildToolClick.factory.veinPool;
		// 					prebuildData.InitParametersArray(veinPool.Length);
		//
		// 					// `start
		// 					// Debug.LogError($"[000] veinPool.Length {veinPool.Length}");
		// 					if (prebuildData.parameters != null)
		// 					{
		// 						EVeinType targetVeinType = PlanetwideMining.ResourceForGlobalMining;
		// 						List<int> newPrebuildDataParameters = new List<int>();
		// 						for (int iaa = 0; iaa < veinPool.Length; iaa++)
		// 						{
		// 							if (veinPool[iaa].type != targetVeinType) continue;
		// 							newPrebuildDataParameters.Add(veinPool[iaa].id);
		// 						}
		//
		// 						prebuildData.parameters = newPrebuildDataParameters.ToArray();
		// 					}
		// 					// `end
		//
		// 					prebuildData.paramCount = prebuildData.parameters.Length;
		// 					prebuildData.ArrageParametersArray();
		// 					buildPreview.parameters = prebuildData.parameters;
		// 					buildPreview.paramCount = prebuildData.paramCount;
		//
		// 					// Debug.LogError($"[333] prebuildData.paramCount {prebuildData.paramCount}");
		// 					// Debug.LogError($"[444] prebuildData.parameters {prebuildData.parameters?.Length} :: {prebuildData.parameters?.Count(v => v != default)}\n");
		//
		// 					Array.Clear(this_tmp_ids, 0, this_tmp_ids.Length);
		// 					if (prebuildData.paramCount == 0)
		// 					{
		// 						buildPreview.condition = EBuildCondition.NeedResource;
		// 						goto IL_1DC7;
		// 					}
		// 				}
		// 				else if (buildPreview.desc.oilMiner)
		// 				{
		// 					Array.Clear(this_tmp_ids, 0, this_tmp_ids.Length);
		// 					Vector3 vector5 = vector;
		// 					Vector3 vector6 = -up;
		// 					int veinsInAreaNonAlloc2 = buildToolClick.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector5, 10f, this_tmp_ids);
		// 					PrebuildData prebuildData2 = default(PrebuildData);
		// 					prebuildData2.InitParametersArray(veinsInAreaNonAlloc2);
		// 					VeinData[] veinPool2 = buildToolClick.factory.veinPool;
		// 					int num4 = 0;
		// 					float num5 = 100f;
		// 					Vector3 pos = vector5;
		// 					for (int k = 0; k < veinsInAreaNonAlloc2; k++)
		// 					{
		// 						if (this_tmp_ids[k] != 0 && veinPool2[this_tmp_ids[k]].id == this_tmp_ids[k] && veinPool2[this_tmp_ids[k]].type == EVeinType.Oil)
		// 						{
		// 							Vector3 pos2 = veinPool2[this_tmp_ids[k]].pos;
		// 							Vector3 vector7 = pos2 - vector5;
		// 							float d = Vector3.Dot(vector6, vector7);
		// 							float sqrMagnitude2 = (vector7 - vector6 * d).sqrMagnitude;
		// 							if (sqrMagnitude2 < num5)
		// 							{
		// 								num5 = sqrMagnitude2;
		// 								num4 = this_tmp_ids[k];
		// 								pos = pos2;
		// 							}
		// 						}
		// 					}
		//
		// 					if (num4 == 0)
		// 					{
		// 						buildPreview.condition = EBuildCondition.NeedResource;
		// 						goto IL_1DC7;
		// 					}
		//
		// 					prebuildData2.parameters[0] = num4;
		// 					prebuildData2.paramCount = 1;
		// 					prebuildData2.ArrageParametersArray();
		// 					buildPreview.parameters = prebuildData2.parameters;
		// 					buildPreview.paramCount = prebuildData2.paramCount;
		// 					Vector3 vector8 = buildToolClick.factory.planet.aux.Snap(pos, true);
		// 					vector = (pose.position = (buildPreview.lpos2 = (buildPreview.lpos = vector8)));
		// 					quaternion = (pose.rotation = (buildPreview.lrot2 = (buildPreview.lrot = Maths.SphericalRotation(vector8, buildToolClick.yaw))));
		// 					forward = pose.forward;
		// 					up = pose.up;
		// 					Array.Clear(this_tmp_ids, 0, this_tmp_ids.Length);
		// 				}
		//
		// 				if (buildPreview.desc.isTank || buildPreview.desc.isStorage || buildPreview.desc.isLab || buildPreview.desc.isSplitter)
		// 				{
		// 					int num6 = buildPreview.desc.isLab ? history.labLevel : history.storageLevel;
		// 					int num7 = buildPreview.desc.isLab ? 15 : 8;
		// 					int num8 = 0;
		// 					bool flag5;
		// 					int objId;
		// 					int num9;
		// 					buildToolClick.factory.ReadObjectConn(buildPreview.inputObjId, 14, out flag5, out objId, out num9);
		// 					while (objId != 0)
		// 					{
		// 						num8++;
		// 						buildToolClick.factory.ReadObjectConn(objId, 14, out flag5, out objId, out num9);
		// 					}
		//
		// 					if (num8 >= num6 - 1)
		// 					{
		// 						flag = (num6 >= num7);
		// 						buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
		// 						goto IL_1DC7;
		// 					}
		// 				}
		//
		// 				Vector3 vector9 = buildToolClick.player.position;
		// 				float num10 = buildToolClick.player.mecha.buildArea * buildToolClick.player.mecha.buildArea;
		// 				if (flag3)
		// 				{
		// 					vector9 = vector9.normalized;
		// 					vector9 *= buildToolClick.planet.realRadius;
		// 					num10 *= 6f;
		// 				}
		//
		// 				if ((vector - vector9).sqrMagnitude > num10)
		// 				{
		// 					buildPreview.condition = EBuildCondition.OutOfReach;
		// 				}
		// 				else
		// 				{
		// 					if (buildToolClick.planet != null)
		// 					{
		// 						float num11 = history.buildMaxHeight + 0.5f + buildToolClick.planet.realRadius * (flag3 ? 1.025f : 1f);
		// 						if (vector.sqrMagnitude > num11 * num11)
		// 						{
		// 							buildPreview.condition = EBuildCondition.OutOfReach;
		// 							goto IL_1DC7;
		// 						}
		// 					}
		//
		// 					if (buildPreview.desc.hasBuildCollider)
		// 					{
		// 						ColliderData[] buildColliders = buildPreview.desc.buildColliders;
		// 						for (int l = 0; l < buildColliders.Length; l++)
		// 						{
		// 							ColliderData colliderData = buildPreview.desc.buildColliders[l];
		// 							if (buildPreview.desc.isInserter)
		// 							{
		// 								colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y, Vector3.Distance(lpos, vector) * 0.5f + colliderData.ext.z - 0.5f);
		// 								if (buildToolClick.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt))
		// 								{
		// 									colliderData.pos.z = colliderData.pos.z - 0.35f;
		// 									colliderData.ext.z = colliderData.ext.z + 0.35f;
		// 								}
		// 								else if (buildPreview.inputObjId == 0 && buildPreview.input == null)
		// 								{
		// 									colliderData.pos.z = colliderData.pos.z - 0.35f;
		// 									colliderData.ext.z = colliderData.ext.z + 0.35f;
		// 								}
		//
		// 								if (buildToolClick.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt))
		// 								{
		// 									colliderData.pos.z = colliderData.pos.z + 0.35f;
		// 									colliderData.ext.z = colliderData.ext.z + 0.35f;
		// 								}
		// 								else if (buildPreview.outputObjId == 0 && buildPreview.output == null)
		// 								{
		// 									colliderData.pos.z = colliderData.pos.z + 0.35f;
		// 									colliderData.ext.z = colliderData.ext.z + 0.35f;
		// 								}
		//
		// 								if (colliderData.ext.z < 0.1f)
		// 								{
		// 									colliderData.ext.z = 0.1f;
		// 								}
		//
		// 								colliderData.pos = a + quaternion2 * colliderData.pos;
		// 								colliderData.q = quaternion2 * colliderData.q;
		// 								colliderData.DebugDraw();
		// 							}
		// 							else
		// 							{
		// 								colliderData.pos = vector + quaternion * colliderData.pos;
		// 								colliderData.q = quaternion * colliderData.q;
		// 							}
		//
		// 							int mask = 428032;
		// 							if (buildPreview.desc.veinMiner || buildPreview.desc.oilMiner)
		// 							{
		// 								mask = 425984;
		// 							}
		//
		// 							Array.Clear(thisTmpCols, 0, thisTmpCols.Length);
		// 							int num12 = Physics.OverlapBoxNonAlloc(colliderData.pos, colliderData.ext, thisTmpCols, colliderData.q, mask, QueryTriggerInteraction.Collide);
		// 							if (num12 > 0)
		// 							{
		// 								bool flag6 = false;
		// 								PlanetPhysics physics = buildToolClick.player.planetData.physics;
		// 								int num13 = 0;
		// 								while (num13 < num12 && buildPreview.coverObjId == 0)
		// 								{
		// 									ColliderData colliderData3;
		// 									bool colliderData2 = physics.GetColliderData(thisTmpCols[num13], out colliderData3);
		// 									int num14 = 0;
		// 									if (colliderData2 && colliderData3.isForBuild)
		// 									{
		// 										if (colliderData3.objType == EObjectType.Entity)
		// 										{
		// 											num14 = colliderData3.objId;
		// 										}
		// 										else if (colliderData3.objType == EObjectType.Prebuild)
		// 										{
		// 											num14 = -colliderData3.objId;
		// 										}
		// 									}
		//
		// 									Collider collider = thisTmpCols[num13];
		// 									if (collider.gameObject.layer == 18)
		// 									{
		// 										BuildPreviewModel component = collider.GetComponent<BuildPreviewModel>();
		// 										if ((!(component != null) || component.index != buildPreview.previewIndex) && (!buildPreview.desc.isInserter || component.buildPreview.desc.isInserter))
		// 										{
		// 											if (buildPreview.desc.isInserter || !component.buildPreview.desc.isInserter)
		// 											{
		// 												goto IL_B99;
		// 											}
		// 										}
		// 									}
		// 									else if (!buildPreview.desc.isInserter || num14 == 0 || (num14 != buildPreview.inputObjId && num14 != buildPreview.outputObjId && num14 != buildPreview2.coverObjId))
		// 									{
		// 										goto IL_B99;
		// 									}
		//
		// 									IL_CA0:
		// 									num13++;
		// 									continue;
		// 									IL_B99:
		// 									flag6 = true;
		// 									if (!flag4 || num14 == 0)
		// 									{
		// 										goto IL_CA0;
		// 									}
		//
		// 									ItemProto itemProto = buildToolClick.GetItemProto(num14);
		// 									if (!buildPreview.item.IsSimilar(itemProto))
		// 									{
		// 										goto IL_CA0;
		// 									}
		//
		// 									Pose objectPose = buildToolClick.GetObjectPose(num14);
		// 									Pose objectPose2 = buildToolClick.GetObjectPose2(num14);
		// 									if ((double) (objectPose.position - buildPreview.lpos).sqrMagnitude >= 0.01 || (double) (objectPose2.position - buildPreview.lpos2).sqrMagnitude >= 0.01 || ((double) (objectPose.forward - forward).sqrMagnitude >= 1E-06 && !buildPreview.desc.isInserter))
		// 									{
		// 										goto IL_CA0;
		// 									}
		//
		// 									if (buildPreview.item.ID == itemProto.ID)
		// 									{
		// 										buildPreview.coverObjId = num14;
		// 										buildPreview.willRemoveCover = false;
		// 										flag6 = false;
		// 										break;
		// 									}
		//
		// 									buildPreview.coverObjId = num14;
		// 									buildPreview.willRemoveCover = true;
		// 									flag6 = false;
		// 									break;
		// 								}
		//
		// 								if (flag6)
		// 								{
		// 									buildPreview.condition = EBuildCondition.Collide;
		// 									break;
		// 								}
		// 							}
		//
		// 							if (buildPreview.desc.veinMiner && Physics.CheckBox(colliderData.pos, colliderData.ext, colliderData.q, 2048, QueryTriggerInteraction.Collide))
		// 							{
		// 								buildPreview.condition = EBuildCondition.Collide;
		// 								break;
		// 							}
		// 						}
		//
		// 						if (buildPreview.condition != EBuildCondition.Ok)
		// 						{
		// 							goto IL_1DC7;
		// 						}
		// 					}
		//
		// 					if (buildPreview2.coverObjId != 0 && buildPreview.desc.isInserter)
		// 					{
		// 						if (buildPreview.output == buildPreview2)
		// 						{
		// 							buildPreview.outputObjId = buildPreview2.coverObjId;
		// 							buildPreview.output = null;
		// 						}
		//
		// 						if (buildPreview.input == buildPreview2)
		// 						{
		// 							buildPreview.inputObjId = buildPreview2.coverObjId;
		// 							buildPreview.input = null;
		// 						}
		// 					}
		//
		// 					if (buildPreview.coverObjId == 0 || buildPreview.willRemoveCover)
		// 					{
		// 						int id = buildPreview.item.ID;
		// 						int num15 = 1;
		// 						if (thisTmpInhandId == id && thisTmpInhandCount > 0)
		// 						{
		// 							num15 = 1;
		// 							thisTmpInhandCount--;
		// 						}
		// 						else
		// 						{
		// 							this_storageComponent.TakeTailItems(ref id, ref num15, false);
		// 						}
		//
		// 						if (num15 == 0)
		// 						{
		// 							buildPreview.condition = EBuildCondition.NotEnoughItem;
		// 							goto IL_1DC7;
		// 						}
		// 					}
		//
		// 					if (buildPreview.coverObjId == 0)
		// 					{
		// 						if (buildPreview.desc.isPowerNode && !buildPreview.desc.isAccumulator)
		// 						{
		// 							if (buildPreview.nearestPowerObjId == null || buildPreview.nearestPowerObjId.Length != buildPreview.nearestPowerObjId.Length)
		// 							{
		// 								buildPreview.nearestPowerObjId = new int[buildToolClick.factory.powerSystem.netCursor];
		// 							}
		//
		// 							Array.Clear(buildPreview.nearestPowerObjId, 0, buildPreview.nearestPowerObjId.Length);
		// 							float num16 = buildPreview.desc.powerConnectDistance * buildPreview.desc.powerConnectDistance;
		// 							float x = vector.x;
		// 							float y = vector.y;
		// 							float z = vector.z;
		// 							int netCursor = buildToolClick.factory.powerSystem.netCursor;
		// 							PowerNetwork[] netPool = buildToolClick.factory.powerSystem.netPool;
		// 							PowerNodeComponent[] nodePool = buildToolClick.factory.powerSystem.nodePool;
		// 							PowerGeneratorComponent[] genPool = buildToolClick.factory.powerSystem.genPool;
		// 							bool windForcedPower = buildPreview.desc.windForcedPower;
		// 							float num17;
		// 							for (int m = 1; m < netCursor; m++)
		// 							{
		// 								if (netPool[m] != null && netPool[m].id != 0)
		// 								{
		// 									List<Node> nodes = netPool[m].nodes;
		// 									int count = nodes.Count;
		// 									num17 = 4900f;
		// 									for (int n = 0; n < count; n++)
		// 									{
		// 										float num18 = x - nodes[n].x;
		// 										float num19 = y - nodes[n].y;
		// 										float num20 = z - nodes[n].z;
		// 										float num21 = num18 * num18 + num19 * num19 + num20 * num20;
		// 										if (num21 < num17 && (num21 < nodes[n].connDistance2 || num21 < num16))
		// 										{
		// 											buildPreview.nearestPowerObjId[m] = nodePool[nodes[n].id].entityId;
		// 											num17 = num21;
		// 										}
		//
		// 										if (windForcedPower && nodes[n].genId > 0 && genPool[nodes[n].genId].id == nodes[n].genId && genPool[nodes[n].genId].wind && num21 < 110.25f)
		// 										{
		// 											buildPreview.condition = EBuildCondition.WindTooClose;
		// 										}
		// 										else if (!buildPreview.desc.isPowerGen && nodes[n].genId == 0 && num21 < 12.25f)
		// 										{
		// 											buildPreview.condition = EBuildCondition.PowerTooClose;
		// 										}
		// 										else if (num21 < 12.25f)
		// 										{
		// 											buildPreview.condition = EBuildCondition.PowerTooClose;
		// 										}
		// 									}
		// 								}
		// 							}
		//
		// 							PrebuildData[] prebuildPool = buildToolClick.factory.prebuildPool;
		// 							int prebuildCursor = buildToolClick.factory.prebuildCursor;
		// 							num17 = 4900f;
		// 							for (int num22 = 1; num22 < prebuildCursor; num22++)
		// 							{
		// 								if (prebuildPool[num22].id == num22 && prebuildPool[num22].protoId >= 2199 && prebuildPool[num22].protoId <= 2299)
		// 								{
		// 									float num23 = x - prebuildPool[num22].pos.x;
		// 									float num19 = y - prebuildPool[num22].pos.y;
		// 									float num20 = z - prebuildPool[num22].pos.z;
		// 									float num21 = num23 * num23 + num19 * num19 + num20 * num20;
		// 									if (num21 < num17)
		// 									{
		// 										ItemProto itemProto2 = LDB.items.Select((int) prebuildPool[num22].protoId);
		// 										if (itemProto2 != null && itemProto2.prefabDesc.isPowerNode)
		// 										{
		// 											if (num21 < itemProto2.prefabDesc.powerConnectDistance * itemProto2.prefabDesc.powerConnectDistance || num21 < num16)
		// 											{
		// 												buildPreview.nearestPowerObjId[0] = -num22;
		// 												num17 = num21;
		// 											}
		//
		// 											if (windForcedPower && itemProto2.prefabDesc.windForcedPower && num21 < 110.25f)
		// 											{
		// 												buildPreview.condition = EBuildCondition.WindTooClose;
		// 											}
		// 											else if (!buildPreview.desc.isPowerGen && !itemProto2.prefabDesc.isPowerGen && num21 < 12.25f)
		// 											{
		// 												buildPreview.condition = EBuildCondition.PowerTooClose;
		// 											}
		// 											else if (num21 < 12.25f)
		// 											{
		// 												buildPreview.condition = EBuildCondition.PowerTooClose;
		// 											}
		// 										}
		// 									}
		// 								}
		// 							}
		// 						}
		//
		// 						if (buildPreview.desc.isCollectStation)
		// 						{
		// 							if (buildToolClick.planet == null || buildToolClick.planet.gasItems == null || buildToolClick.planet.gasItems.Length == 0)
		// 							{
		// 								buildPreview.condition = EBuildCondition.OutOfReach;
		// 								goto IL_1DC7;
		// 							}
		//
		// 							for (int num24 = 0; num24 < buildToolClick.planet.gasItems.Length; num24++)
		// 							{
		// 								double num25 = 0.0;
		// 								if ((double) buildPreview.desc.stationCollectSpeed * buildToolClick.planet.gasTotalHeat != 0.0)
		// 								{
		// 									num25 = 1.0 - (double) buildPreview.desc.workEnergyPerTick / ((double) buildPreview.desc.stationCollectSpeed * buildToolClick.planet.gasTotalHeat * 0.016666666666666666);
		// 								}
		//
		// 								if (num25 <= 0.0)
		// 								{
		// 									buildPreview.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
		// 								}
		// 							}
		//
		// 							float y2 = buildToolClick.cursorTarget.y;
		// 							if (y2 > 0.1f || y2 < -0.1f)
		// 							{
		// 								buildPreview.condition = EBuildCondition.BuildInEquator;
		// 								goto IL_1DC7;
		// 							}
		// 						}
		//
		// 						if (buildPreview.desc.isStation)
		// 						{
		// 							StationComponent[] stationPool = buildToolClick.factory.transport.stationPool;
		// 							int stationCursor = buildToolClick.factory.transport.stationCursor;
		// 							PrebuildData[] prebuildPool2 = buildToolClick.factory.prebuildPool;
		// 							int prebuildCursor2 = buildToolClick.factory.prebuildCursor;
		// 							EntityData[] entityPool = buildToolClick.factory.entityPool;
		// 							float num26 = 225f;
		// 							float num27 = 841f;
		// 							float num28 = 14297f;
		// 							num27 = (buildPreview.desc.isCollectStation ? num28 : num27);
		// 							for (int num29 = 1; num29 < stationCursor; num29++)
		// 							{
		// 								if (stationPool[num29] != null && stationPool[num29].id == num29)
		// 								{
		// 									float num30 = (stationPool[num29].isStellar || buildPreview.desc.isStellarStation) ? num27 : num26;
		// 									if ((entityPool[stationPool[num29].entityId].pos - vector).sqrMagnitude < num30)
		// 									{
		// 										buildPreview.condition = EBuildCondition.TowerTooClose;
		// 									}
		// 								}
		// 							}
		//
		// 							for (int num31 = 1; num31 < prebuildCursor2; num31++)
		// 							{
		// 								if (prebuildPool2[num31].id == num31)
		// 								{
		// 									ItemProto itemProto3 = LDB.items.Select((int) prebuildPool2[num31].protoId);
		// 									if (itemProto3 != null && itemProto3.prefabDesc.isStation)
		// 									{
		// 										float num32 = (itemProto3.prefabDesc.isStellarStation || buildPreview.desc.isStellarStation) ? num27 : num26;
		// 										float num33 = vector.x - prebuildPool2[num31].pos.x;
		// 										float num34 = vector.y - prebuildPool2[num31].pos.y;
		// 										float num35 = vector.z - prebuildPool2[num31].pos.z;
		// 										if (num33 * num33 + num34 * num34 + num35 * num35 < num32)
		// 										{
		// 											buildPreview.condition = EBuildCondition.TowerTooClose;
		// 										}
		// 									}
		// 								}
		// 							}
		// 						}
		//
		// 						if (!buildPreview.desc.isInserter && vector.magnitude - buildToolClick.planet.realRadius + buildPreview.desc.cullingHeight > 4.9f && !buildPreview.desc.isEjector)
		// 						{
		// 							EjectorComponent[] ejectorPool = buildToolClick.factory.factorySystem.ejectorPool;
		// 							int ejectorCursor = buildToolClick.factory.factorySystem.ejectorCursor;
		// 							PrebuildData[] prebuildPool3 = buildToolClick.factory.prebuildPool;
		// 							int prebuildCursor3 = buildToolClick.factory.prebuildCursor;
		// 							EntityData[] entityPool2 = buildToolClick.factory.entityPool;
		// 							Vector3 ext = buildPreview.desc.buildCollider.ext;
		// 							float num36 = Mathf.Sqrt(ext.x * ext.x + ext.z * ext.z);
		// 							float num37 = 7.2f + num36;
		// 							for (int num38 = 1; num38 < ejectorCursor; num38++)
		// 							{
		// 								if (ejectorPool[num38].id == num38 && (entityPool2[ejectorPool[num38].entityId].pos - vector).sqrMagnitude < num37 * num37)
		// 								{
		// 									buildPreview.condition = EBuildCondition.EjectorTooClose;
		// 								}
		// 							}
		//
		// 							for (int num39 = 1; num39 < prebuildCursor3; num39++)
		// 							{
		// 								if (prebuildPool3[num39].id == num39)
		// 								{
		// 									ItemProto itemProto4 = LDB.items.Select((int) prebuildPool3[num39].protoId);
		// 									if (itemProto4 != null && itemProto4.prefabDesc.isEjector)
		// 									{
		// 										float num40 = vector.x - prebuildPool3[num39].pos.x;
		// 										float num41 = vector.y - prebuildPool3[num39].pos.y;
		// 										float num42 = vector.z - prebuildPool3[num39].pos.z;
		// 										if (num40 * num40 + num41 * num41 + num42 * num42 < num37 * num37)
		// 										{
		// 											buildPreview.condition = EBuildCondition.EjectorTooClose;
		// 										}
		// 									}
		// 								}
		// 							}
		// 						}
		//
		// 						if (buildPreview.desc.isEjector)
		// 						{
		// 							buildToolClick.GetOverlappedObjectsNonAlloc(vector, 12f, 14.5f, false);
		// 							for (int num43 = 0; num43 < thisOverlappedCount; num43++)
		// 							{
		// 								PrefabDesc prefabDesc = buildToolClick.GetPrefabDesc(this_overlappedIds[num43]);
		// 								Vector3 position = buildToolClick.GetObjectPose(this_overlappedIds[num43]).position;
		// 								if (position.magnitude - buildToolClick.planet.realRadius + prefabDesc.cullingHeight > 4.9f)
		// 								{
		// 									float num44 = vector.x - position.x;
		// 									float num45 = vector.y - position.y;
		// 									float num46 = vector.z - position.z;
		// 									float num47 = num44 * num44 + num45 * num45 + num46 * num46;
		// 									Vector3 ext2 = prefabDesc.buildCollider.ext;
		// 									float num48 = Mathf.Sqrt(ext2.x * ext2.x + ext2.z * ext2.z);
		// 									float num49 = 7.2f + num48;
		// 									if (prefabDesc.isEjector)
		// 									{
		// 										num49 = 10.6f;
		// 									}
		//
		// 									if (num47 < num49 * num49)
		// 									{
		// 										buildPreview.condition = EBuildCondition.BlockTooClose;
		// 									}
		// 								}
		// 							}
		// 						}
		//
		// 						if (flag2 && vector.magnitude < buildToolClick.planet.realRadius + 3f)
		// 						{
		// 							Vector3 ext3 = buildPreview.desc.buildCollider.ext;
		// 							float num50 = Mathf.Sqrt(ext3.x * ext3.x + ext3.z * ext3.z);
		// 							if ((vector - b).magnitude - num50 < 3.7f)
		// 							{
		// 								buildPreview.condition = EBuildCondition.Collide;
		// 								goto IL_1DC7;
		// 							}
		// 						}
		//
		// 						if ((!buildPreview.desc.multiLevel || buildPreview.inputObjId == 0) && !buildPreview.desc.isInserter)
		// 						{
		// 							for (int num51 = 0; num51 < buildPreview.desc.landPoints.Length; num51++)
		// 							{
		// 								Vector3 point = buildPreview.desc.landPoints[num51];
		// 								point.y = 0f;
		// 								Vector3 vector10 = vector + quaternion * point;
		// 								Vector3 normalized = vector10.normalized;
		// 								vector10 += normalized * 3f;
		// 								Vector3 direction = -normalized;
		// 								if (flag3)
		// 								{
		// 									Vector3 b2 = buildToolClick.cursorTarget.normalized * buildToolClick.planet.realRadius * 0.025f;
		// 									vector10 -= b2;
		// 								}
		//
		// 								RaycastHit raycastHit;
		// 								if (Physics.Raycast(new Ray(vector10, direction), out raycastHit, 5f, 8704, QueryTriggerInteraction.Collide))
		// 								{
		// 									float distance = raycastHit.distance;
		// 									if (raycastHit.point.magnitude - buildToolClick.factory.planet.realRadius < -0.3f)
		// 									{
		// 										buildPreview.condition = EBuildCondition.NeedGround;
		// 									}
		// 									else
		// 									{
		// 										float num52;
		// 										if (Physics.Raycast(new Ray(vector10, direction), out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
		// 										{
		// 											num52 = raycastHit.distance;
		// 										}
		// 										else
		// 										{
		// 											num52 = 1000f;
		// 										}
		//
		// 										if (distance - num52 > 0.27f)
		// 										{
		// 											buildPreview.condition = EBuildCondition.NeedGround;
		// 										}
		// 									}
		// 								}
		// 								else
		// 								{
		// 									buildPreview.condition = EBuildCondition.NeedGround;
		// 								}
		// 							}
		//
		// 							for (int num53 = 0; num53 < buildPreview.desc.waterPoints.Length; num53++)
		// 							{
		// 								if (buildToolClick.factory.planet.waterItemId <= 0)
		// 								{
		// 									buildPreview.condition = EBuildCondition.NeedWater;
		// 								}
		// 								else
		// 								{
		// 									Vector3 point2 = buildPreview.desc.waterPoints[num53];
		// 									point2.y = 0f;
		// 									Vector3 vector11 = vector + quaternion * point2;
		// 									Vector3 normalized2 = vector11.normalized;
		// 									vector11 += normalized2 * 3f;
		// 									Vector3 direction2 = -normalized2;
		// 									RaycastHit raycastHit;
		// 									float num54;
		// 									if (Physics.Raycast(new Ray(vector11, direction2), out raycastHit, 5f, 8704, QueryTriggerInteraction.Collide))
		// 									{
		// 										num54 = raycastHit.distance;
		// 									}
		// 									else
		// 									{
		// 										num54 = 1000f;
		// 									}
		//
		// 									if (Physics.Raycast(new Ray(vector11, direction2), out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
		// 									{
		// 										float distance2 = raycastHit.distance;
		// 										if (num54 - distance2 <= 0.27f)
		// 										{
		// 											buildPreview.condition = EBuildCondition.NeedWater;
		// 										}
		// 									}
		// 									else
		// 									{
		// 										buildPreview.condition = EBuildCondition.NeedWater;
		// 									}
		// 								}
		// 							}
		// 						}
		//
		// 						if (buildPreview.desc.isInserter && buildPreview.condition == EBuildCondition.Ok)
		// 						{
		// 							bool flag7 = buildToolClick.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt);
		// 							bool flag8 = buildToolClick.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt);
		// 							Vector3 posR = Vector3.zero;
		// 							Vector3 vector12;
		// 							if (buildPreview.output != null)
		// 							{
		// 								vector12 = buildPreview.output.lpos;
		// 							}
		// 							else
		// 							{
		// 								vector12 = buildToolClick.GetObjectPose(buildPreview.outputObjId).position;
		// 							}
		//
		// 							Vector3 vector13;
		// 							if (buildPreview.input != null)
		// 							{
		// 								vector13 = buildPreview.input.lpos;
		// 							}
		// 							else
		// 							{
		// 								vector13 = buildToolClick.GetObjectPose(buildPreview.inputObjId).position;
		// 							}
		//
		// 							if (flag7 && !flag8)
		// 							{
		// 								posR = vector12;
		// 							}
		// 							else if (!flag7 && flag8)
		// 							{
		// 								posR = vector13;
		// 							}
		// 							else
		// 							{
		// 								posR = (vector12 + vector13) * 0.5f;
		// 							}
		//
		// 							float num55 = buildToolClick.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(posR, buildPreview.lpos, buildPreview.lpos2);
		// 							float num56 = num55;
		// 							float magnitude = forward3.magnitude;
		// 							float num57 = 5.5f;
		// 							float num58 = 0.6f;
		// 							float num59 = 3.499f;
		// 							float num60 = 0.88f;
		// 							if (flag7 && flag8)
		// 							{
		// 								num58 = 0.4f;
		// 								num57 = 5f;
		// 								num59 = 3.2f;
		// 								num60 = 0.8f;
		// 							}
		// 							else if (!flag7 && !flag8)
		// 							{
		// 								num58 = 0.9f;
		// 								num57 = 7.5f;
		// 								num59 = 3.799f;
		// 								num60 = 1.451f;
		// 								num56 -= 0.3f;
		// 							}
		//
		// 							if (magnitude > num57)
		// 							{
		// 								buildPreview.condition = EBuildCondition.TooFar;
		// 							}
		// 							else if (magnitude < num58)
		// 							{
		// 								buildPreview.condition = EBuildCondition.TooClose;
		// 							}
		// 							else if (num55 > num59)
		// 							{
		// 								buildPreview.condition = EBuildCondition.TooFar;
		// 							}
		// 							else if (num55 < num60)
		// 							{
		// 								buildPreview.condition = EBuildCondition.TooClose;
		// 							}
		// 							else
		// 							{
		// 								int oneParameter = Mathf.RoundToInt(Mathf.Clamp(num56, 1f, 3f));
		// 								buildPreview.SetOneParameter(oneParameter);
		// 							}
		// 						}
		// 					}
		// 				}
		// 			}
		// 		}
		//
		// 		IL_1DC7: ;
		// 	}
		//
		// 	bool flag9 = true;
		// 	int num61 = 0;
		// 	while (num61 < buildToolClick.buildPreviews.Count)
		// 	{
		// 		BuildPreview buildPreview3 = buildToolClick.buildPreviews[num61];
		// 		if (buildPreview3.condition != EBuildCondition.Ok && buildPreview3.condition != EBuildCondition.NeedConn)
		// 		{
		// 			flag9 = false;
		// 			buildToolClick.actionBuild.model.cursorState = -1;
		// 			buildToolClick.actionBuild.model.cursorText = buildPreview3.conditionText;
		// 			if (buildPreview3.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag)
		// 			{
		// 				BuildModel model = buildToolClick.actionBuild.model;
		// 				model.cursorText += "垂直建造可升级".Translate();
		// 				break;
		// 			}
		//
		// 			break;
		// 		}
		// 		else
		// 		{
		// 			num61++;
		// 		}
		// 	}
		//
		// 	if (flag9)
		// 	{
		// 		buildToolClick.actionBuild.model.cursorState = 0;
		// 		buildToolClick.actionBuild.model.cursorText = "点击鼠标建造".Translate();
		// 	}
		//
		// 	if (!flag9 && !VFInput.onGUI)
		// 	{
		// 		UICursor.SetCursor(ECursor.Ban);
		// 	}
		//
		// 	return flag9;
		// }
	}
}