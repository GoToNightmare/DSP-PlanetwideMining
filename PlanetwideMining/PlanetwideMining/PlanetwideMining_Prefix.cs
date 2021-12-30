using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanetwideMining
{
	public partial class PatchMiners
	{
		public static bool PrefixPatch(BuildTool_Click __instance, bool __result)
		{
			__result = CheckBuildConditions(__instance);

			return false;
		}


		public static bool CheckBuildConditions(BuildTool_Click __instance)
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
							Array.Clear((Array) BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
							Vector3 center = vector3_2 + forward1 * -1.2f;
							Vector3 rhs1 = -forward1;
							Vector3 lhs = up1;
							int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 12f, BuildTool._tmp_ids);
							PrebuildData prebuildData = new PrebuildData();
							prebuildData.InitParametersArray(veinsInAreaNonAlloc);
							VeinData[] veinPool = __instance.factory.veinPool;
							int num2 = 0;
							for (int index2 = 0; index2 < veinsInAreaNonAlloc; ++index2)
							{
								if (BuildTool._tmp_ids[index2] != 0 && veinPool[BuildTool._tmp_ids[index2]].id == BuildTool._tmp_ids[index2])
								{
									if (veinPool[BuildTool._tmp_ids[index2]].type != EVeinType.Oil)
									{
										Vector3 rhs2 = veinPool[BuildTool._tmp_ids[index2]].pos - center;
										float f = Vector3.Dot(lhs, rhs2);
										Vector3 vector3_5 = rhs2 - lhs * f;
										double sqrMagnitude = (double) vector3_5.sqrMagnitude;
										float num3 = Vector3.Dot(vector3_5.normalized, rhs1);
										if (sqrMagnitude <= 961.0 / 16.0 && (double) num3 >= 0.730000019073486 && (double) Mathf.Abs(f) <= 2.0)
											prebuildData.parameters[num2++] = BuildTool._tmp_ids[index2];
									}
								}
								else
									Assert.CannotBeReached();
							}

							prebuildData.paramCount = num2;
							prebuildData.ArrageParametersArray();
							buildPreview1.parameters = prebuildData.parameters;
							buildPreview1.paramCount = prebuildData.paramCount;
							Array.Clear((Array) BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
							if (prebuildData.paramCount == 0)
							{
								buildPreview1.condition = EBuildCondition.NeedResource;
								continue;
							}
						}
						else if (buildPreview1.desc.oilMiner)
						{
							Array.Clear((Array) BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
							Vector3 center = vector3_2;
							Vector3 lhs = -up1;
							int veinsInAreaNonAlloc = __instance.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 10f, BuildTool._tmp_ids);
							PrebuildData prebuildData = new PrebuildData();
							prebuildData.InitParametersArray(veinsInAreaNonAlloc);
							VeinData[] veinPool = __instance.factory.veinPool;
							int num4 = 0;
							float num5 = 100f;
							Vector3 pos1 = center;
							for (int index3 = 0; index3 < veinsInAreaNonAlloc; ++index3)
							{
								if (BuildTool._tmp_ids[index3] != 0 && veinPool[BuildTool._tmp_ids[index3]].id == BuildTool._tmp_ids[index3] && veinPool[BuildTool._tmp_ids[index3]].type == EVeinType.Oil)
								{
									Vector3 pos2 = veinPool[BuildTool._tmp_ids[index3]].pos;
									Vector3 rhs = pos2 - center;
									float num6 = Vector3.Dot(lhs, rhs);
									float sqrMagnitude = (rhs - lhs * num6).sqrMagnitude;
									if ((double) sqrMagnitude < (double) num5)
									{
										num5 = sqrMagnitude;
										num4 = BuildTool._tmp_ids[index3];
										pos1 = pos2;
									}
								}
							}

							if (num4 != 0)
							{
								prebuildData.parameters[0] = num4;
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
								Array.Clear((Array) BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
							}
							else
							{
								buildPreview1.condition = EBuildCondition.NeedResource;
								continue;
							}
						}

						if (buildPreview1.desc.isTank || buildPreview1.desc.isStorage || buildPreview1.desc.isLab || buildPreview1.desc.isSplitter)
						{
							int num7 = buildPreview1.desc.isLab ? history.labLevel : history.storageLevel;
							int num8 = buildPreview1.desc.isLab ? 15 : 8;
							int num9 = 0;
							bool isOutput;
							int otherObjId;
							int otherSlot;
							for (__instance.factory.ReadObjectConn(buildPreview1.inputObjId, 14, out isOutput, out otherObjId, out otherSlot); otherObjId != 0; __instance.factory.ReadObjectConn(otherObjId, 14, out isOutput, out otherObjId, out otherSlot))
								++num9;
							if (num9 >= num7 - 1)
							{
								flag1 = num7 >= num8;
								buildPreview1.condition = EBuildCondition.OutOfVerticalConstructionHeight;
								continue;
							}
						}

						Vector3 vector3_8 = __instance.player.position;
						float num10 = __instance.player.mecha.buildArea * __instance.player.mecha.buildArea;
						if (flag3)
						{
							vector3_8 = vector3_8.normalized;
							vector3_8 *= __instance.planet.realRadius;
							num10 *= 6f;
						}

						if ((double) (vector3_2 - vector3_8).sqrMagnitude > (double) num10)
						{
							buildPreview1.condition = EBuildCondition.OutOfReach;
						}
						else
						{
							if (__instance.planet != null)
							{
								float num11 = (float) ((double) history.buildMaxHeight + 0.5 + (double) __instance.planet.realRadius * (flag3 ? 1.02499997615814 : 1.0));
								if ((double) vector3_2.sqrMagnitude > (double) num11 * (double) num11)
								{
									buildPreview1.condition = EBuildCondition.OutOfReach;
									continue;
								}
							}

							if (buildPreview1.desc.hasBuildCollider)
							{
								ColliderData[] buildColliders = buildPreview1.desc.buildColliders;
								for (int index4 = 0; index4 < buildColliders.Length; ++index4)
								{
									ColliderData buildCollider = buildPreview1.desc.buildColliders[index4];
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
									Array.Clear((Array) BuildTool._tmp_cols, 0, BuildTool._tmp_cols.Length);
									int num12 = Physics.OverlapBoxNonAlloc(buildCollider.pos, buildCollider.ext, BuildTool._tmp_cols, buildCollider.q, mask, QueryTriggerInteraction.Collide);
									if (num12 > 0)
									{
										bool flag5 = false;
										PlanetPhysics physics = __instance.player.planetData.physics;
										for (int index5 = 0; index5 < num12 && buildPreview1.coverObjId == 0; ++index5)
										{
											ColliderData cd;
											int num13 = physics.GetColliderData(BuildTool._tmp_cols[index5], out cd) ? 1 : 0;
											int objId = 0;
											if (num13 != 0 && cd.isForBuild)
											{
												if (cd.objType == EObjectType.Entity)
													objId = cd.objId;
												else if (cd.objType == EObjectType.Prebuild)
													objId = -cd.objId;
											}

											Collider tmpCol = BuildTool._tmp_cols[index5];
											if (tmpCol.gameObject.layer == 18)
											{
												BuildPreviewModel component = tmpCol.GetComponent<BuildPreviewModel>();
												if ((Object) component != (Object) null && component.index == buildPreview1.previewIndex || buildPreview1.desc.isInserter && !component.buildPreview.desc.isInserter || !buildPreview1.desc.isInserter && component.buildPreview.desc.isInserter)
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
								if (__instance.tmpInhandId == id && __instance.tmpInhandCount > 0)
								{
									count = 1;
									--__instance.tmpInhandCount;
								}
								else
									__instance.tmpPackage.TakeTailItems(ref id, ref count);

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
									float num14 = buildPreview1.desc.powerConnectDistance * buildPreview1.desc.powerConnectDistance;
									float x = vector3_2.x;
									float y = vector3_2.y;
									float z = vector3_2.z;
									int netCursor = __instance.factory.powerSystem.netCursor;
									PowerNetwork[] netPool = __instance.factory.powerSystem.netPool;
									PowerNodeComponent[] nodePool = __instance.factory.powerSystem.nodePool;
									PowerGeneratorComponent[] genPool = __instance.factory.powerSystem.genPool;
									bool windForcedPower = buildPreview1.desc.windForcedPower;
									for (int index6 = 1; index6 < netCursor; ++index6)
									{
										if (netPool[index6] != null && netPool[index6].id != 0)
										{
											List<PowerNetworkStructures.Node> nodes = netPool[index6].nodes;
											int count = nodes.Count;
											float num15 = 4900f;
											for (int index7 = 0; index7 < count; ++index7)
											{
												double num16 = (double) x - (double) nodes[index7].x;
												float num17 = y - nodes[index7].y;
												float num18 = z - nodes[index7].z;
												float num19 = (float) (num16 * num16 + (double) num17 * (double) num17 + (double) num18 * (double) num18);
												if ((double) num19 < (double) num15 && ((double) num19 < (double) nodes[index7].connDistance2 || (double) num19 < (double) num14))
												{
													buildPreview1.nearestPowerObjId[index6] = nodePool[nodes[index7].id].entityId;
													num15 = num19;
												}

												if (windForcedPower && nodes[index7].genId > 0 && genPool[nodes[index7].genId].id == nodes[index7].genId && genPool[nodes[index7].genId].wind && (double) num19 < 110.25)
													buildPreview1.condition = EBuildCondition.WindTooClose;
												else if (!buildPreview1.desc.isPowerGen && nodes[index7].genId == 0 && (double) num19 < 12.25)
													buildPreview1.condition = EBuildCondition.PowerTooClose;
												else if ((double) num19 < 12.25)
													buildPreview1.condition = EBuildCondition.PowerTooClose;
											}
										}
									}

									PrebuildData[] prebuildPool = __instance.factory.prebuildPool;
									int prebuildCursor = __instance.factory.prebuildCursor;
									float num20 = 4900f;
									for (int index8 = 1; index8 < prebuildCursor; ++index8)
									{
										if (prebuildPool[index8].id == index8 && prebuildPool[index8].protoId >= (short) 2199 && prebuildPool[index8].protoId <= (short) 2299)
										{
											double num21 = (double) x - (double) prebuildPool[index8].pos.x;
											float num22 = y - prebuildPool[index8].pos.y;
											float num23 = z - prebuildPool[index8].pos.z;
											float num24 = (float) (num21 * num21 + (double) num22 * (double) num22 + (double) num23 * (double) num23);
											if ((double) num24 < (double) num20)
											{
												ItemProto itemProto = LDB.items.Select((int) prebuildPool[index8].protoId);
												if (itemProto != null && itemProto.prefabDesc.isPowerNode)
												{
													if ((double) num24 < (double) itemProto.prefabDesc.powerConnectDistance * (double) itemProto.prefabDesc.powerConnectDistance || (double) num24 < (double) num14)
													{
														buildPreview1.nearestPowerObjId[0] = -index8;
														num20 = num24;
													}

													if (windForcedPower && itemProto.prefabDesc.windForcedPower && (double) num24 < 110.25)
														buildPreview1.condition = EBuildCondition.WindTooClose;
													else if (!buildPreview1.desc.isPowerGen && !itemProto.prefabDesc.isPowerGen && (double) num24 < 12.25)
														buildPreview1.condition = EBuildCondition.PowerTooClose;
													else if ((double) num24 < 12.25)
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
										for (int index9 = 0; index9 < __instance.planet.gasItems.Length; ++index9)
										{
											double num25 = 0.0;
											if ((double) buildPreview1.desc.stationCollectSpeed * __instance.planet.gasTotalHeat != 0.0)
												num25 = 1.0 - (double) buildPreview1.desc.workEnergyPerTick / ((double) buildPreview1.desc.stationCollectSpeed * __instance.planet.gasTotalHeat * (1.0 / 60.0));
											if (num25 <= 0.0)
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
									float num26 = 225f;
									float num27 = 841f;
									float num28 = 14297f;
									float num29 = buildPreview1.desc.isCollectStation ? num28 : num27;
									for (int index10 = 1; index10 < stationCursor; ++index10)
									{
										if (stationPool[index10] != null && stationPool[index10].id == index10)
										{
											float num30 = stationPool[index10].isStellar || buildPreview1.desc.isStellarStation ? num29 : num26;
											vector3_4 = entityPool[stationPool[index10].entityId].pos - vector3_2;
											if ((double) vector3_4.sqrMagnitude < (double) num30)
												buildPreview1.condition = EBuildCondition.TowerTooClose;
										}
									}

									for (int index11 = 1; index11 < prebuildCursor; ++index11)
									{
										if (prebuildPool[index11].id == index11)
										{
											ItemProto itemProto = LDB.items.Select((int) prebuildPool[index11].protoId);
											if (itemProto != null && itemProto.prefabDesc.isStation)
											{
												float num31 = itemProto.prefabDesc.isStellarStation || buildPreview1.desc.isStellarStation ? num29 : num26;
												double num32 = (double) vector3_2.x - (double) prebuildPool[index11].pos.x;
												float num33 = vector3_2.y - prebuildPool[index11].pos.y;
												float num34 = vector3_2.z - prebuildPool[index11].pos.z;
												if (num32 * num32 + (double) num33 * (double) num33 + (double) num34 * (double) num34 < (double) num31)
													buildPreview1.condition = EBuildCondition.TowerTooClose;
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
									float num35 = 7.2f + Mathf.Sqrt((float) ((double) ext.x * (double) ext.x + (double) ext.z * (double) ext.z));
									for (int index12 = 1; index12 < ejectorCursor; ++index12)
									{
										if (ejectorPool[index12].id == index12)
										{
											vector3_4 = entityPool[ejectorPool[index12].entityId].pos - vector3_2;
											if ((double) vector3_4.sqrMagnitude < (double) num35 * (double) num35)
												buildPreview1.condition = EBuildCondition.EjectorTooClose;
										}
									}

									for (int index13 = 1; index13 < prebuildCursor; ++index13)
									{
										if (prebuildPool[index13].id == index13)
										{
											ItemProto itemProto = LDB.items.Select((int) prebuildPool[index13].protoId);
											if (itemProto != null && itemProto.prefabDesc.isEjector)
											{
												double num36 = (double) vector3_2.x - (double) prebuildPool[index13].pos.x;
												float num37 = vector3_2.y - prebuildPool[index13].pos.y;
												float num38 = vector3_2.z - prebuildPool[index13].pos.z;
												if (num36 * num36 + (double) num37 * (double) num37 + (double) num38 * (double) num38 < (double) num35 * (double) num35)
													buildPreview1.condition = EBuildCondition.EjectorTooClose;
											}
										}
									}
								}

								if (buildPreview1.desc.isEjector)
								{
									__instance.GetOverlappedObjectsNonAlloc(vector3_2, 12f, 14.5f);
									for (int index14 = 0; index14 < BuildTool._overlappedCount; ++index14)
									{
										PrefabDesc prefabDesc = __instance.GetPrefabDesc(BuildTool._overlappedIds[index14]);
										Vector3 position = __instance.GetObjectPose(BuildTool._overlappedIds[index14]).position;
										if ((double) position.magnitude - (double) __instance.planet.realRadius + (double) prefabDesc.cullingHeight > 4.90000009536743)
										{
											double num39 = (double) vector3_2.x - (double) position.x;
											float num40 = vector3_2.y - position.y;
											float num41 = vector3_2.z - position.z;
											double num42 = num39 * num39 + (double) num40 * (double) num40 + (double) num41 * (double) num41;
											Vector3 ext = prefabDesc.buildCollider.ext;
											float num43 = 7.2f + Mathf.Sqrt((float) ((double) ext.x * (double) ext.x + (double) ext.z * (double) ext.z));
											if (prefabDesc.isEjector)
												num43 = 10.6f;
											double num44 = (double) num43 * (double) num43;
											if (num42 < num44)
												buildPreview1.condition = EBuildCondition.BlockTooClose;
										}
									}
								}

								if (flag2 && (double) vector3_2.magnitude < (double) __instance.planet.realRadius + 3.0)
								{
									Vector3 ext = buildPreview1.desc.buildCollider.ext;
									float num45 = Mathf.Sqrt((float) ((double) ext.x * (double) ext.x + (double) ext.z * (double) ext.z));
									vector3_4 = vector3_2 - vector3_1;
									if ((double) vector3_4.magnitude - (double) num45 < 3.70000004768372)
									{
										buildPreview1.condition = EBuildCondition.Collide;
										continue;
									}
								}

								if ((!buildPreview1.desc.multiLevel || buildPreview1.inputObjId == 0) && !buildPreview1.desc.isInserter)
								{
									RaycastHit hitInfo;
									for (int index15 = 0; index15 < buildPreview1.desc.landPoints.Length; ++index15)
									{
										Vector3 landPoint = buildPreview1.desc.landPoints[index15] with
										{
											y = 0.0f
										};
										Vector3 vector3_9 = vector3_2 + quaternion1 * landPoint;
										Vector3 normalized = vector3_9.normalized;
										Vector3 origin = vector3_9 + normalized * 3f;
										Vector3 direction = -normalized;
										if (flag3)
										{
											Vector3 vector3_10 = __instance.cursorTarget.normalized * __instance.planet.realRadius * 0.025f;
											origin -= vector3_10;
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
												float num46 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
												if ((double) distance - (double) num46 > 0.270000010728836)
													buildPreview1.condition = EBuildCondition.NeedGround;
											}
										}
										else
											buildPreview1.condition = EBuildCondition.NeedGround;
									}

									for (int index16 = 0; index16 < buildPreview1.desc.waterPoints.Length; ++index16)
									{
										if (__instance.factory.planet.waterItemId <= 0)
										{
											buildPreview1.condition = EBuildCondition.NeedWater;
										}
										else
										{
											Vector3 waterPoint = buildPreview1.desc.waterPoints[index16] with
											{
												y = 0.0f
											};
											Vector3 vector3_11 = vector3_2 + quaternion1 * waterPoint;
											Vector3 normalized = vector3_11.normalized;
											Vector3 origin = vector3_11 + normalized * 3f;
											Vector3 direction = -normalized;
											float num47 = !Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 8704, QueryTriggerInteraction.Collide) ? 1000f : hitInfo.distance;
											if (Physics.Raycast(new Ray(origin, direction), out hitInfo, 5f, 16, QueryTriggerInteraction.Collide))
											{
												float distance = hitInfo.distance;
												if ((double) num47 - (double) distance <= 0.270000010728836)
													buildPreview1.condition = EBuildCondition.NeedWater;
											}
											else
												buildPreview1.condition = EBuildCondition.NeedWater;
										}
									}
								}

								if (buildPreview1.desc.isInserter && buildPreview1.condition == EBuildCondition.Ok)
								{
									bool flag6 = __instance.ObjectIsBelt(buildPreview1.inputObjId) || buildPreview1.input != null && buildPreview1.input.desc.isBelt;
									bool flag7 = __instance.ObjectIsBelt(buildPreview1.outputObjId) || buildPreview1.output != null && buildPreview1.output.desc.isBelt;
									Vector3 zero = Vector3.zero;
									Vector3 vector3_12 = buildPreview1.output == null ? __instance.GetObjectPose(buildPreview1.outputObjId).position : buildPreview1.output.lpos;
									Vector3 vector3_13 = buildPreview1.input == null ? __instance.GetObjectPose(buildPreview1.inputObjId).position : buildPreview1.input.lpos;
									float num48 = __instance.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(!flag6 || flag7 ? (!(!flag6 & flag7) ? (vector3_12 + vector3_13) * 0.5f : vector3_13) : vector3_12, buildPreview1.lpos, buildPreview1.lpos2);
									float num49 = num48;
									float magnitude = forward3.magnitude;
									float num50 = 5.5f;
									float num51 = 0.6f;
									float num52 = 3.499f;
									float num53 = 0.88f;
									if (flag6 & flag7)
									{
										num51 = 0.4f;
										num50 = 5f;
										num52 = 3.2f;
										num53 = 0.8f;
									}
									else if (!flag6 && !flag7)
									{
										num51 = 0.9f;
										num50 = 7.5f;
										num52 = 3.799f;
										num53 = 1.451f;
										num49 -= 0.3f;
									}

									if ((double) magnitude > (double) num50)
										buildPreview1.condition = EBuildCondition.TooFar;
									else if ((double) magnitude < (double) num51)
										buildPreview1.condition = EBuildCondition.TooClose;
									else if ((double) num48 > (double) num52)
										buildPreview1.condition = EBuildCondition.TooFar;
									else if ((double) num48 < (double) num53)
									{
										buildPreview1.condition = EBuildCondition.TooClose;
									}
									else
									{
										int num54 = Mathf.RoundToInt(Mathf.Clamp(num49, 1f, 3f));
										buildPreview1.SetOneParameter(num54);
									}
								}
							}
						}
					}
				}
			}

			bool flag8 = true;
			for (int index = 0; index < __instance.buildPreviews.Count; ++index)
			{
				BuildPreview buildPreview = __instance.buildPreviews[index];
				if (buildPreview.condition != EBuildCondition.Ok && buildPreview.condition != EBuildCondition.NeedConn)
				{
					flag8 = false;
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

			if (flag8)
			{
				__instance.actionBuild.model.cursorState = 0;
				__instance.actionBuild.model.cursorText = "点击鼠标建造".Translate();
			}

			if (!flag8 && !VFInput.onGUI)
				UICursor.SetCursor(ECursor.Ban);
			return flag8;
		}
	}
}