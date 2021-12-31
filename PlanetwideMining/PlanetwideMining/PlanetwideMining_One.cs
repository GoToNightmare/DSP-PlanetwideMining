using System;
using System.Collections.Generic;
using PowerNetworkStructures;
using UnityEngine;

namespace PlanetwideMining
{
	public static partial class PatchMiners
	{
		public static bool CheckBuildConditions(
			ref BuildTool_Click buildToolClick, 
			ref int[] this_tmp_ids, 
			ref Collider[] thisTmpCols,
			ref int thisTmpInhandId, 
			ref int thisTmpInhandCount, 
			ref int thisOverlappedCount, 
			ref int[] this_overlappedIds, 
			ref StorageComponent this_storageComponent)
		{
			if (buildToolClick.buildPreviews.Count == 0)
			{
				return false;
			}

			GameHistoryData history = buildToolClick.actionBuild.history;
			bool flag = false;
			int num = 1;
			List<BuildPreview> templatePreviews = buildToolClick.actionBuild.templatePreviews;
			if (templatePreviews.Count > 0)
			{
				num = templatePreviews.Count;
			}

			bool flag2 = false;
			Vector3 b = Vector3.zero;
			if (buildToolClick.planet.id == buildToolClick.planet.galaxy.birthPlanetId && history.SpaceCapsuleExist())
			{
				b = buildToolClick.planet.birthPoint;
				flag2 = true;
			}

			for (int i = 0; i < buildToolClick.buildPreviews.Count; i++)
			{
				BuildPreview buildPreview = buildToolClick.buildPreviews[i];
				BuildPreview buildPreview2 = buildToolClick.buildPreviews[i / num * num];
				if (buildPreview.condition == EBuildCondition.Ok)
				{
					Vector3 vector = buildPreview.lpos;
					Quaternion quaternion = buildPreview.lrot;
					Vector3 lpos = buildPreview.lpos2;
					Quaternion lrot = buildPreview.lrot2;
					Pose pose = new Pose(buildPreview.lpos, buildPreview.lrot);
					Pose pose2 = new Pose(buildPreview.lpos2, buildPreview.lrot2);
					Vector3 forward = pose.forward;
					Vector3 forward2 = pose2.forward;
					Vector3 up = pose.up;
					Vector3 a = Vector3.Lerp(vector, lpos, 0.5f);
					Vector3 forward3 = lpos - vector;
					if (forward3.sqrMagnitude < 0.0001f)
					{
						forward3 = Maths.SphericalRotation(vector, 0f).Forward();
					}

					Quaternion quaternion2 = Quaternion.LookRotation(forward3, a.normalized);
					bool flag3 = buildToolClick.planet != null && buildToolClick.planet.type == EPlanetType.Gas;
					if (vector.sqrMagnitude < 1f)
					{
						buildPreview.condition = EBuildCondition.Failure;
					}
					else
					{
						bool flag4 = buildPreview.desc.minerType == EMinerType.None
						             && !buildPreview.desc.isBelt
						             && !buildPreview.desc.isSplitter
						             && (!buildPreview.desc.isPowerNode || buildPreview.desc.isPowerGen || buildPreview.desc.isAccumulator || buildPreview.desc.isPowerExchanger)
						             && !buildPreview.desc.isStation
						             && !buildPreview.desc.isSilo
						             && !buildPreview.desc.multiLevel
						             && !buildPreview.desc.isMonitor;
						if (buildPreview.desc.veinMiner)
						{
							PrebuildData prebuildData = default(PrebuildData);
							VeinData[] veinPool = buildToolClick.factory.veinPool;
							prebuildData.InitParametersArray(veinPool.Length);

							// `start
							// Debug.LogError($"[000] veinPool.Length {veinPool.Length}");
							if (prebuildData.parameters != null)
							{
								const EVeinType cachedVeinType = EVeinType.Copper;
								List<int> newPrebuildDataParameters = new List<int>();
								for (int iaa = 0; iaa < veinPool.Length; iaa++)
								{
									if (veinPool[iaa].type != cachedVeinType) continue;
									newPrebuildDataParameters.Add(veinPool[iaa].id);
								}

								prebuildData.parameters = newPrebuildDataParameters.ToArray();
							}
							// `end

							prebuildData.paramCount = prebuildData.parameters.Length;
							prebuildData.ArrageParametersArray();
							buildPreview.parameters = prebuildData.parameters;
							buildPreview.paramCount = prebuildData.paramCount;

							// Debug.LogError($"[333] prebuildData.paramCount {prebuildData.paramCount}");
							// Debug.LogError($"[444] prebuildData.parameters {prebuildData.parameters?.Length} :: {prebuildData.parameters?.Count(v => v != default)}\n");

							Array.Clear(this_tmp_ids, 0, this_tmp_ids.Length);
							if (prebuildData.paramCount == 0)
							{
								buildPreview.condition = EBuildCondition.NeedResource;
								goto IL_1DC7;
							}
						}
						else if (buildPreview.desc.oilMiner)
						{
							Array.Clear(this_tmp_ids, 0, this_tmp_ids.Length);
							Vector3 vector5 = vector;
							Vector3 vector6 = -up;
							int veinsInAreaNonAlloc2 = buildToolClick.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector5, 10f, this_tmp_ids);
							PrebuildData prebuildData2 = default(PrebuildData);
							prebuildData2.InitParametersArray(veinsInAreaNonAlloc2);
							VeinData[] veinPool2 = buildToolClick.factory.veinPool;
							int num4 = 0;
							float num5 = 100f;
							Vector3 pos = vector5;
							for (int k = 0; k < veinsInAreaNonAlloc2; k++)
							{
								if (this_tmp_ids[k] != 0 && veinPool2[this_tmp_ids[k]].id == this_tmp_ids[k] && veinPool2[this_tmp_ids[k]].type == EVeinType.Oil)
								{
									Vector3 pos2 = veinPool2[this_tmp_ids[k]].pos;
									Vector3 vector7 = pos2 - vector5;
									float d = Vector3.Dot(vector6, vector7);
									float sqrMagnitude2 = (vector7 - vector6 * d).sqrMagnitude;
									if (sqrMagnitude2 < num5)
									{
										num5 = sqrMagnitude2;
										num4 = this_tmp_ids[k];
										pos = pos2;
									}
								}
							}

							if (num4 == 0)
							{
								buildPreview.condition = EBuildCondition.NeedResource;
								goto IL_1DC7;
							}

							prebuildData2.parameters[0] = num4;
							prebuildData2.paramCount = 1;
							prebuildData2.ArrageParametersArray();
							buildPreview.parameters = prebuildData2.parameters;
							buildPreview.paramCount = prebuildData2.paramCount;
							Vector3 vector8 = buildToolClick.factory.planet.aux.Snap(pos, true);
							vector = (pose.position = (buildPreview.lpos2 = (buildPreview.lpos = vector8)));
							quaternion = (pose.rotation = (buildPreview.lrot2 = (buildPreview.lrot = Maths.SphericalRotation(vector8, buildToolClick.yaw))));
							forward = pose.forward;
							up = pose.up;
							Array.Clear(this_tmp_ids, 0, this_tmp_ids.Length);
						}

						if (buildPreview.desc.isTank || buildPreview.desc.isStorage || buildPreview.desc.isLab || buildPreview.desc.isSplitter)
						{
							int num6 = buildPreview.desc.isLab ? history.labLevel : history.storageLevel;
							int num7 = buildPreview.desc.isLab ? 15 : 8;
							int num8 = 0;
							bool flag5;
							int objId;
							int num9;
							buildToolClick.factory.ReadObjectConn(buildPreview.inputObjId, 14, out flag5, out objId, out num9);
							while (objId != 0)
							{
								num8++;
								buildToolClick.factory.ReadObjectConn(objId, 14, out flag5, out objId, out num9);
							}

							if (num8 >= num6 - 1)
							{
								flag = (num6 >= num7);
								buildPreview.condition = EBuildCondition.OutOfVerticalConstructionHeight;
								goto IL_1DC7;
							}
						}

						Vector3 vector9 = buildToolClick.player.position;
						float num10 = buildToolClick.player.mecha.buildArea * buildToolClick.player.mecha.buildArea;
						if (flag3)
						{
							vector9 = vector9.normalized;
							vector9 *= buildToolClick.planet.realRadius;
							num10 *= 6f;
						}

						if ((vector - vector9).sqrMagnitude > num10)
						{
							buildPreview.condition = EBuildCondition.OutOfReach;
						}
						else
						{
							if (buildToolClick.planet != null)
							{
								float num11 = history.buildMaxHeight + 0.5f + buildToolClick.planet.realRadius * (flag3 ? 1.025f : 1f);
								if (vector.sqrMagnitude > num11 * num11)
								{
									buildPreview.condition = EBuildCondition.OutOfReach;
									goto IL_1DC7;
								}
							}

							if (buildPreview.desc.hasBuildCollider)
							{
								ColliderData[] buildColliders = buildPreview.desc.buildColliders;
								for (int l = 0; l < buildColliders.Length; l++)
								{
									ColliderData colliderData = buildPreview.desc.buildColliders[l];
									if (buildPreview.desc.isInserter)
									{
										colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y, Vector3.Distance(lpos, vector) * 0.5f + colliderData.ext.z - 0.5f);
										if (buildToolClick.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt))
										{
											colliderData.pos.z = colliderData.pos.z - 0.35f;
											colliderData.ext.z = colliderData.ext.z + 0.35f;
										}
										else if (buildPreview.inputObjId == 0 && buildPreview.input == null)
										{
											colliderData.pos.z = colliderData.pos.z - 0.35f;
											colliderData.ext.z = colliderData.ext.z + 0.35f;
										}

										if (buildToolClick.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt))
										{
											colliderData.pos.z = colliderData.pos.z + 0.35f;
											colliderData.ext.z = colliderData.ext.z + 0.35f;
										}
										else if (buildPreview.outputObjId == 0 && buildPreview.output == null)
										{
											colliderData.pos.z = colliderData.pos.z + 0.35f;
											colliderData.ext.z = colliderData.ext.z + 0.35f;
										}

										if (colliderData.ext.z < 0.1f)
										{
											colliderData.ext.z = 0.1f;
										}

										colliderData.pos = a + quaternion2 * colliderData.pos;
										colliderData.q = quaternion2 * colliderData.q;
										colliderData.DebugDraw();
									}
									else
									{
										colliderData.pos = vector + quaternion * colliderData.pos;
										colliderData.q = quaternion * colliderData.q;
									}

									int mask = 428032;
									if (buildPreview.desc.veinMiner || buildPreview.desc.oilMiner)
									{
										mask = 425984;
									}

									Array.Clear(thisTmpCols, 0, thisTmpCols.Length);
									int num12 = Physics.OverlapBoxNonAlloc(colliderData.pos, colliderData.ext, thisTmpCols, colliderData.q, mask, QueryTriggerInteraction.Collide);
									if (num12 > 0)
									{
										bool flag6 = false;
										PlanetPhysics physics = buildToolClick.player.planetData.physics;
										int num13 = 0;
										while (num13 < num12 && buildPreview.coverObjId == 0)
										{
											ColliderData colliderData3;
											bool colliderData2 = physics.GetColliderData(thisTmpCols[num13], out colliderData3);
											int num14 = 0;
											if (colliderData2 && colliderData3.isForBuild)
											{
												if (colliderData3.objType == EObjectType.Entity)
												{
													num14 = colliderData3.objId;
												}
												else if (colliderData3.objType == EObjectType.Prebuild)
												{
													num14 = -colliderData3.objId;
												}
											}

											Collider collider = thisTmpCols[num13];
											if (collider.gameObject.layer == 18)
											{
												BuildPreviewModel component = collider.GetComponent<BuildPreviewModel>();
												if ((!(component != null) || component.index != buildPreview.previewIndex) && (!buildPreview.desc.isInserter || component.buildPreview.desc.isInserter))
												{
													if (buildPreview.desc.isInserter || !component.buildPreview.desc.isInserter)
													{
														goto IL_B99;
													}
												}
											}
											else if (!buildPreview.desc.isInserter || num14 == 0 || (num14 != buildPreview.inputObjId && num14 != buildPreview.outputObjId && num14 != buildPreview2.coverObjId))
											{
												goto IL_B99;
											}

											IL_CA0:
											num13++;
											continue;
											IL_B99:
											flag6 = true;
											if (!flag4 || num14 == 0)
											{
												goto IL_CA0;
											}

											ItemProto itemProto = buildToolClick.GetItemProto(num14);
											if (!buildPreview.item.IsSimilar(itemProto))
											{
												goto IL_CA0;
											}

											Pose objectPose = buildToolClick.GetObjectPose(num14);
											Pose objectPose2 = buildToolClick.GetObjectPose2(num14);
											if ((double) (objectPose.position - buildPreview.lpos).sqrMagnitude >= 0.01 || (double) (objectPose2.position - buildPreview.lpos2).sqrMagnitude >= 0.01 || ((double) (objectPose.forward - forward).sqrMagnitude >= 1E-06 && !buildPreview.desc.isInserter))
											{
												goto IL_CA0;
											}

											if (buildPreview.item.ID == itemProto.ID)
											{
												buildPreview.coverObjId = num14;
												buildPreview.willRemoveCover = false;
												flag6 = false;
												break;
											}

											buildPreview.coverObjId = num14;
											buildPreview.willRemoveCover = true;
											flag6 = false;
											break;
										}

										if (flag6)
										{
											buildPreview.condition = EBuildCondition.Collide;
											break;
										}
									}

									if (buildPreview.desc.veinMiner && Physics.CheckBox(colliderData.pos, colliderData.ext, colliderData.q, 2048, QueryTriggerInteraction.Collide))
									{
										buildPreview.condition = EBuildCondition.Collide;
										break;
									}
								}

								if (buildPreview.condition != EBuildCondition.Ok)
								{
									goto IL_1DC7;
								}
							}

							if (buildPreview2.coverObjId != 0 && buildPreview.desc.isInserter)
							{
								if (buildPreview.output == buildPreview2)
								{
									buildPreview.outputObjId = buildPreview2.coverObjId;
									buildPreview.output = null;
								}

								if (buildPreview.input == buildPreview2)
								{
									buildPreview.inputObjId = buildPreview2.coverObjId;
									buildPreview.input = null;
								}
							}

							if (buildPreview.coverObjId == 0 || buildPreview.willRemoveCover)
							{
								int id = buildPreview.item.ID;
								int num15 = 1;
								if (thisTmpInhandId == id && thisTmpInhandCount > 0)
								{
									num15 = 1;
									thisTmpInhandCount--;
								}
								else
								{
									this_storageComponent.TakeTailItems(ref id, ref num15, false);
								}

								if (num15 == 0)
								{
									buildPreview.condition = EBuildCondition.NotEnoughItem;
									goto IL_1DC7;
								}
							}

							if (buildPreview.coverObjId == 0)
							{
								if (buildPreview.desc.isPowerNode && !buildPreview.desc.isAccumulator)
								{
									if (buildPreview.nearestPowerObjId == null || buildPreview.nearestPowerObjId.Length != buildPreview.nearestPowerObjId.Length)
									{
										buildPreview.nearestPowerObjId = new int[buildToolClick.factory.powerSystem.netCursor];
									}

									Array.Clear(buildPreview.nearestPowerObjId, 0, buildPreview.nearestPowerObjId.Length);
									float num16 = buildPreview.desc.powerConnectDistance * buildPreview.desc.powerConnectDistance;
									float x = vector.x;
									float y = vector.y;
									float z = vector.z;
									int netCursor = buildToolClick.factory.powerSystem.netCursor;
									PowerNetwork[] netPool = buildToolClick.factory.powerSystem.netPool;
									PowerNodeComponent[] nodePool = buildToolClick.factory.powerSystem.nodePool;
									PowerGeneratorComponent[] genPool = buildToolClick.factory.powerSystem.genPool;
									bool windForcedPower = buildPreview.desc.windForcedPower;
									float num17;
									for (int m = 1; m < netCursor; m++)
									{
										if (netPool[m] != null && netPool[m].id != 0)
										{
											List<Node> nodes = netPool[m].nodes;
											int count = nodes.Count;
											num17 = 4900f;
											for (int n = 0; n < count; n++)
											{
												float num18 = x - nodes[n].x;
												float num19 = y - nodes[n].y;
												float num20 = z - nodes[n].z;
												float num21 = num18 * num18 + num19 * num19 + num20 * num20;
												if (num21 < num17 && (num21 < nodes[n].connDistance2 || num21 < num16))
												{
													buildPreview.nearestPowerObjId[m] = nodePool[nodes[n].id].entityId;
													num17 = num21;
												}

												if (windForcedPower && nodes[n].genId > 0 && genPool[nodes[n].genId].id == nodes[n].genId && genPool[nodes[n].genId].wind && num21 < 110.25f)
												{
													buildPreview.condition = EBuildCondition.WindTooClose;
												}
												else if (!buildPreview.desc.isPowerGen && nodes[n].genId == 0 && num21 < 12.25f)
												{
													buildPreview.condition = EBuildCondition.PowerTooClose;
												}
												else if (num21 < 12.25f)
												{
													buildPreview.condition = EBuildCondition.PowerTooClose;
												}
											}
										}
									}

									PrebuildData[] prebuildPool = buildToolClick.factory.prebuildPool;
									int prebuildCursor = buildToolClick.factory.prebuildCursor;
									num17 = 4900f;
									for (int num22 = 1; num22 < prebuildCursor; num22++)
									{
										if (prebuildPool[num22].id == num22 && prebuildPool[num22].protoId >= 2199 && prebuildPool[num22].protoId <= 2299)
										{
											float num23 = x - prebuildPool[num22].pos.x;
											float num19 = y - prebuildPool[num22].pos.y;
											float num20 = z - prebuildPool[num22].pos.z;
											float num21 = num23 * num23 + num19 * num19 + num20 * num20;
											if (num21 < num17)
											{
												ItemProto itemProto2 = LDB.items.Select((int) prebuildPool[num22].protoId);
												if (itemProto2 != null && itemProto2.prefabDesc.isPowerNode)
												{
													if (num21 < itemProto2.prefabDesc.powerConnectDistance * itemProto2.prefabDesc.powerConnectDistance || num21 < num16)
													{
														buildPreview.nearestPowerObjId[0] = -num22;
														num17 = num21;
													}

													if (windForcedPower && itemProto2.prefabDesc.windForcedPower && num21 < 110.25f)
													{
														buildPreview.condition = EBuildCondition.WindTooClose;
													}
													else if (!buildPreview.desc.isPowerGen && !itemProto2.prefabDesc.isPowerGen && num21 < 12.25f)
													{
														buildPreview.condition = EBuildCondition.PowerTooClose;
													}
													else if (num21 < 12.25f)
													{
														buildPreview.condition = EBuildCondition.PowerTooClose;
													}
												}
											}
										}
									}
								}

								if (buildPreview.desc.isCollectStation)
								{
									if (buildToolClick.planet == null || buildToolClick.planet.gasItems == null || buildToolClick.planet.gasItems.Length == 0)
									{
										buildPreview.condition = EBuildCondition.OutOfReach;
										goto IL_1DC7;
									}

									for (int num24 = 0; num24 < buildToolClick.planet.gasItems.Length; num24++)
									{
										double num25 = 0.0;
										if ((double) buildPreview.desc.stationCollectSpeed * buildToolClick.planet.gasTotalHeat != 0.0)
										{
											num25 = 1.0 - (double) buildPreview.desc.workEnergyPerTick / ((double) buildPreview.desc.stationCollectSpeed * buildToolClick.planet.gasTotalHeat * 0.016666666666666666);
										}

										if (num25 <= 0.0)
										{
											buildPreview.condition = EBuildCondition.NotEnoughEnergyToWorkCollection;
										}
									}

									float y2 = buildToolClick.cursorTarget.y;
									if (y2 > 0.1f || y2 < -0.1f)
									{
										buildPreview.condition = EBuildCondition.BuildInEquator;
										goto IL_1DC7;
									}
								}

								if (buildPreview.desc.isStation)
								{
									StationComponent[] stationPool = buildToolClick.factory.transport.stationPool;
									int stationCursor = buildToolClick.factory.transport.stationCursor;
									PrebuildData[] prebuildPool2 = buildToolClick.factory.prebuildPool;
									int prebuildCursor2 = buildToolClick.factory.prebuildCursor;
									EntityData[] entityPool = buildToolClick.factory.entityPool;
									float num26 = 225f;
									float num27 = 841f;
									float num28 = 14297f;
									num27 = (buildPreview.desc.isCollectStation ? num28 : num27);
									for (int num29 = 1; num29 < stationCursor; num29++)
									{
										if (stationPool[num29] != null && stationPool[num29].id == num29)
										{
											float num30 = (stationPool[num29].isStellar || buildPreview.desc.isStellarStation) ? num27 : num26;
											if ((entityPool[stationPool[num29].entityId].pos - vector).sqrMagnitude < num30)
											{
												buildPreview.condition = EBuildCondition.TowerTooClose;
											}
										}
									}

									for (int num31 = 1; num31 < prebuildCursor2; num31++)
									{
										if (prebuildPool2[num31].id == num31)
										{
											ItemProto itemProto3 = LDB.items.Select((int) prebuildPool2[num31].protoId);
											if (itemProto3 != null && itemProto3.prefabDesc.isStation)
											{
												float num32 = (itemProto3.prefabDesc.isStellarStation || buildPreview.desc.isStellarStation) ? num27 : num26;
												float num33 = vector.x - prebuildPool2[num31].pos.x;
												float num34 = vector.y - prebuildPool2[num31].pos.y;
												float num35 = vector.z - prebuildPool2[num31].pos.z;
												if (num33 * num33 + num34 * num34 + num35 * num35 < num32)
												{
													buildPreview.condition = EBuildCondition.TowerTooClose;
												}
											}
										}
									}
								}

								if (!buildPreview.desc.isInserter && vector.magnitude - buildToolClick.planet.realRadius + buildPreview.desc.cullingHeight > 4.9f && !buildPreview.desc.isEjector)
								{
									EjectorComponent[] ejectorPool = buildToolClick.factory.factorySystem.ejectorPool;
									int ejectorCursor = buildToolClick.factory.factorySystem.ejectorCursor;
									PrebuildData[] prebuildPool3 = buildToolClick.factory.prebuildPool;
									int prebuildCursor3 = buildToolClick.factory.prebuildCursor;
									EntityData[] entityPool2 = buildToolClick.factory.entityPool;
									Vector3 ext = buildPreview.desc.buildCollider.ext;
									float num36 = Mathf.Sqrt(ext.x * ext.x + ext.z * ext.z);
									float num37 = 7.2f + num36;
									for (int num38 = 1; num38 < ejectorCursor; num38++)
									{
										if (ejectorPool[num38].id == num38 && (entityPool2[ejectorPool[num38].entityId].pos - vector).sqrMagnitude < num37 * num37)
										{
											buildPreview.condition = EBuildCondition.EjectorTooClose;
										}
									}

									for (int num39 = 1; num39 < prebuildCursor3; num39++)
									{
										if (prebuildPool3[num39].id == num39)
										{
											ItemProto itemProto4 = LDB.items.Select((int) prebuildPool3[num39].protoId);
											if (itemProto4 != null && itemProto4.prefabDesc.isEjector)
											{
												float num40 = vector.x - prebuildPool3[num39].pos.x;
												float num41 = vector.y - prebuildPool3[num39].pos.y;
												float num42 = vector.z - prebuildPool3[num39].pos.z;
												if (num40 * num40 + num41 * num41 + num42 * num42 < num37 * num37)
												{
													buildPreview.condition = EBuildCondition.EjectorTooClose;
												}
											}
										}
									}
								}

								if (buildPreview.desc.isEjector)
								{
									buildToolClick.GetOverlappedObjectsNonAlloc(vector, 12f, 14.5f, false);
									for (int num43 = 0; num43 < thisOverlappedCount; num43++)
									{
										PrefabDesc prefabDesc = buildToolClick.GetPrefabDesc(this_overlappedIds[num43]);
										Vector3 position = buildToolClick.GetObjectPose(this_overlappedIds[num43]).position;
										if (position.magnitude - buildToolClick.planet.realRadius + prefabDesc.cullingHeight > 4.9f)
										{
											float num44 = vector.x - position.x;
											float num45 = vector.y - position.y;
											float num46 = vector.z - position.z;
											float num47 = num44 * num44 + num45 * num45 + num46 * num46;
											Vector3 ext2 = prefabDesc.buildCollider.ext;
											float num48 = Mathf.Sqrt(ext2.x * ext2.x + ext2.z * ext2.z);
											float num49 = 7.2f + num48;
											if (prefabDesc.isEjector)
											{
												num49 = 10.6f;
											}

											if (num47 < num49 * num49)
											{
												buildPreview.condition = EBuildCondition.BlockTooClose;
											}
										}
									}
								}

								if (flag2 && vector.magnitude < buildToolClick.planet.realRadius + 3f)
								{
									Vector3 ext3 = buildPreview.desc.buildCollider.ext;
									float num50 = Mathf.Sqrt(ext3.x * ext3.x + ext3.z * ext3.z);
									if ((vector - b).magnitude - num50 < 3.7f)
									{
										buildPreview.condition = EBuildCondition.Collide;
										goto IL_1DC7;
									}
								}

								if ((!buildPreview.desc.multiLevel || buildPreview.inputObjId == 0) && !buildPreview.desc.isInserter)
								{
									for (int num51 = 0; num51 < buildPreview.desc.landPoints.Length; num51++)
									{
										Vector3 point = buildPreview.desc.landPoints[num51];
										point.y = 0f;
										Vector3 vector10 = vector + quaternion * point;
										Vector3 normalized = vector10.normalized;
										vector10 += normalized * 3f;
										Vector3 direction = -normalized;
										if (flag3)
										{
											Vector3 b2 = buildToolClick.cursorTarget.normalized * buildToolClick.planet.realRadius * 0.025f;
											vector10 -= b2;
										}

										RaycastHit raycastHit;
										if (Physics.Raycast(new Ray(vector10, direction), out raycastHit, 5f, 8704, QueryTriggerInteraction.Collide))
										{
											float distance = raycastHit.distance;
											if (raycastHit.point.magnitude - buildToolClick.factory.planet.realRadius < -0.3f)
											{
												buildPreview.condition = EBuildCondition.NeedGround;
											}
											else
											{
												float num52;
												if (Physics.Raycast(new Ray(vector10, direction), out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
												{
													num52 = raycastHit.distance;
												}
												else
												{
													num52 = 1000f;
												}

												if (distance - num52 > 0.27f)
												{
													buildPreview.condition = EBuildCondition.NeedGround;
												}
											}
										}
										else
										{
											buildPreview.condition = EBuildCondition.NeedGround;
										}
									}

									for (int num53 = 0; num53 < buildPreview.desc.waterPoints.Length; num53++)
									{
										if (buildToolClick.factory.planet.waterItemId <= 0)
										{
											buildPreview.condition = EBuildCondition.NeedWater;
										}
										else
										{
											Vector3 point2 = buildPreview.desc.waterPoints[num53];
											point2.y = 0f;
											Vector3 vector11 = vector + quaternion * point2;
											Vector3 normalized2 = vector11.normalized;
											vector11 += normalized2 * 3f;
											Vector3 direction2 = -normalized2;
											RaycastHit raycastHit;
											float num54;
											if (Physics.Raycast(new Ray(vector11, direction2), out raycastHit, 5f, 8704, QueryTriggerInteraction.Collide))
											{
												num54 = raycastHit.distance;
											}
											else
											{
												num54 = 1000f;
											}

											if (Physics.Raycast(new Ray(vector11, direction2), out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
											{
												float distance2 = raycastHit.distance;
												if (num54 - distance2 <= 0.27f)
												{
													buildPreview.condition = EBuildCondition.NeedWater;
												}
											}
											else
											{
												buildPreview.condition = EBuildCondition.NeedWater;
											}
										}
									}
								}

								if (buildPreview.desc.isInserter && buildPreview.condition == EBuildCondition.Ok)
								{
									bool flag7 = buildToolClick.ObjectIsBelt(buildPreview.inputObjId) || (buildPreview.input != null && buildPreview.input.desc.isBelt);
									bool flag8 = buildToolClick.ObjectIsBelt(buildPreview.outputObjId) || (buildPreview.output != null && buildPreview.output.desc.isBelt);
									Vector3 posR = Vector3.zero;
									Vector3 vector12;
									if (buildPreview.output != null)
									{
										vector12 = buildPreview.output.lpos;
									}
									else
									{
										vector12 = buildToolClick.GetObjectPose(buildPreview.outputObjId).position;
									}

									Vector3 vector13;
									if (buildPreview.input != null)
									{
										vector13 = buildPreview.input.lpos;
									}
									else
									{
										vector13 = buildToolClick.GetObjectPose(buildPreview.inputObjId).position;
									}

									if (flag7 && !flag8)
									{
										posR = vector12;
									}
									else if (!flag7 && flag8)
									{
										posR = vector13;
									}
									else
									{
										posR = (vector12 + vector13) * 0.5f;
									}

									float num55 = buildToolClick.actionBuild.planetAux.mainGrid.CalcSegmentsAcross(posR, buildPreview.lpos, buildPreview.lpos2);
									float num56 = num55;
									float magnitude = forward3.magnitude;
									float num57 = 5.5f;
									float num58 = 0.6f;
									float num59 = 3.499f;
									float num60 = 0.88f;
									if (flag7 && flag8)
									{
										num58 = 0.4f;
										num57 = 5f;
										num59 = 3.2f;
										num60 = 0.8f;
									}
									else if (!flag7 && !flag8)
									{
										num58 = 0.9f;
										num57 = 7.5f;
										num59 = 3.799f;
										num60 = 1.451f;
										num56 -= 0.3f;
									}

									if (magnitude > num57)
									{
										buildPreview.condition = EBuildCondition.TooFar;
									}
									else if (magnitude < num58)
									{
										buildPreview.condition = EBuildCondition.TooClose;
									}
									else if (num55 > num59)
									{
										buildPreview.condition = EBuildCondition.TooFar;
									}
									else if (num55 < num60)
									{
										buildPreview.condition = EBuildCondition.TooClose;
									}
									else
									{
										int oneParameter = Mathf.RoundToInt(Mathf.Clamp(num56, 1f, 3f));
										buildPreview.SetOneParameter(oneParameter);
									}
								}
							}
						}
					}
				}

				IL_1DC7: ;
			}

			bool flag9 = true;
			int num61 = 0;
			while (num61 < buildToolClick.buildPreviews.Count)
			{
				BuildPreview buildPreview3 = buildToolClick.buildPreviews[num61];
				if (buildPreview3.condition != EBuildCondition.Ok && buildPreview3.condition != EBuildCondition.NeedConn)
				{
					flag9 = false;
					buildToolClick.actionBuild.model.cursorState = -1;
					buildToolClick.actionBuild.model.cursorText = buildPreview3.conditionText;
					if (buildPreview3.condition == EBuildCondition.OutOfVerticalConstructionHeight && !flag)
					{
						BuildModel model = buildToolClick.actionBuild.model;
						model.cursorText += "垂直建造可升级".Translate();
						break;
					}

					break;
				}
				else
				{
					num61++;
				}
			}

			if (flag9)
			{
				buildToolClick.actionBuild.model.cursorState = 0;
				buildToolClick.actionBuild.model.cursorText = "点击鼠标建造".Translate();
			}

			if (!flag9 && !VFInput.onGUI)
			{
				UICursor.SetCursor(ECursor.Ban);
			}

			return flag9;
		}
	}
}