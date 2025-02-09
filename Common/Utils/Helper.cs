using System.Collections.Generic;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using SanguinePact.Common.KindredCommands.Data;
using ProjectM;
using ProjectM.Scripting;
using ProjectM.Shared;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using VampireCommandFramework;

namespace SanguinePact.Common.Utils;

// This is an anti-pattern, move stuff away from Helper not into it
internal static partial class Helper
{
	public static PrefabGUID GetPrefabGUID(Entity entity)
	{
		var entityManager = Core.EntityManager;
		PrefabGUID guid;
		try
		{
			guid = entityManager.GetComponentData<PrefabGUID>(entity);
		}
		catch
		{
			guid = new PrefabGUID(0);
		}
		return guid;
	}

	public static bool TryGetClanEntityFromPlayer(Entity User, out Entity ClanEntity)
	{
		if (User.Read<TeamReference>().Value._Value.ReadBuffer<TeamAllies>().Length > 0)
		{
			ClanEntity = User.Read<TeamReference>().Value._Value.ReadBuffer<TeamAllies>()[0].Value;
			return true;
		}
		ClanEntity = new Entity();
		return false;
	}

	public static Entity AddItemToInventory(Entity recipient, PrefabGUID guid, int amount)
	{
		try
		{
			ServerGameManager serverGameManager = Core.Server.GetExistingSystemManaged<ServerScriptMapper>()._ServerGameManager;
			var inventoryResponse = serverGameManager.TryAddInventoryItem(recipient, guid, amount);

			return inventoryResponse.NewEntity;
		}
		catch (System.Exception e)
		{
			Core.LogException(e);
		}
		return new Entity();
	}

	public static NativeArray<Entity> GetEntitiesByComponentType<T1>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
	{
		EntityQueryOptions options = EntityQueryOptions.Default;
		if (includeAll) options |= EntityQueryOptions.IncludeAll;
		if (includeDisabled) options |= EntityQueryOptions.IncludeDisabled;
		if (includeSpawn) options |= EntityQueryOptions.IncludeSpawnTag;
		if (includePrefab) options |= EntityQueryOptions.IncludePrefab;
		if (includeDestroyed) options |= EntityQueryOptions.IncludeDestroyTag;

		EntityQueryDesc queryDesc = new()
		{
			All = new ComponentType[] { new(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite) },
			Options = options
		};

		var query = Core.EntityManager.CreateEntityQuery(queryDesc);

		var entities = query.ToEntityArray(Allocator.Temp);
		return entities;
	}

	public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
	{
		EntityQueryOptions options = EntityQueryOptions.Default;
		if (includeAll) options |= EntityQueryOptions.IncludeAll;
		if (includeDisabled) options |= EntityQueryOptions.IncludeDisabled;
		if (includeSpawn) options |= EntityQueryOptions.IncludeSpawnTag;
		if (includePrefab) options |= EntityQueryOptions.IncludePrefab;
		if (includeDestroyed) options |= EntityQueryOptions.IncludeDestroyTag;

		EntityQueryDesc queryDesc = new()
		{
			All = new ComponentType[] { new(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite), new(Il2CppType.Of<T2>(), ComponentType.AccessMode.ReadWrite) },
			Options = options
		};

		var query = Core.EntityManager.CreateEntityQuery(queryDesc);

		var entities = query.ToEntityArray(Allocator.Temp);
		return entities;
	}

	public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2, T3>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
	{
		EntityQueryOptions options = EntityQueryOptions.Default;
		if (includeAll) options |= EntityQueryOptions.IncludeAll;
		if (includeDisabled) options |= EntityQueryOptions.IncludeDisabled;
		if (includeSpawn) options |= EntityQueryOptions.IncludeSpawnTag;
		if (includePrefab) options |= EntityQueryOptions.IncludePrefab;
		if (includeDestroyed) options |= EntityQueryOptions.IncludeDestroyTag;

		EntityQueryDesc queryDesc = new()
		{
			All = new ComponentType[]
			{
				new(Il2CppType.Of<T1>(), ComponentType.AccessMode.ReadWrite),
				new(Il2CppType.Of<T2>(), ComponentType.AccessMode.ReadWrite),
				new(Il2CppType.Of<T3>(), ComponentType.AccessMode.ReadWrite)
			},
			Options = options
		};

		var query = Core.EntityManager.CreateEntityQuery(queryDesc);

		var entities = query.ToEntityArray(Allocator.Temp);
		return entities;
	}

	public static IEnumerable<Entity> GetAllEntitiesInRadius<T>(float2 center, float radius)
	{
		var entities = GetEntitiesByComponentType<T>(includeSpawn: true, includeDisabled: true);
		foreach (var entity in entities)
		{
			if (!entity.Has<Translation>()) continue;
			var pos = entity.Read<Translation>().Value;
			if (math.distance(center, pos.xz) <= radius)
			{
				yield return entity;
			}
		}
		entities.Dispose();
	}

	static EntityQuery tilePositionQuery = default;
	public static Entity FindClosestTilePosition(Vector3 pos)
	{
		if (tilePositionQuery == default)
		{
			tilePositionQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[] {
					new(Il2CppType.Of<TilePosition>(), ComponentType.AccessMode.ReadOnly),
					new(Il2CppType.Of<Translation>(), ComponentType.AccessMode.ReadOnly)
				},
				Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludeSpawnTag
			});
		}

		var closestEntity = Entity.Null;
		var closestDistance = float.MaxValue;
		var entities = tilePositionQuery.ToEntityArray(Allocator.Temp);
		for (var i = 0; i < entities.Length; ++i)
		{
			var entity = entities[i];
			if (!entity.Has<TilePosition>()) continue;
			var entityPos = entity.Read<Translation>().Value;
			var distance = math.distancesq(pos, entityPos);
			if (distance < closestDistance)
			{
				var prefabName = GetPrefabGUID(entity).LookupName();
				if (!prefabName.StartsWith("TM_")) continue;

				closestDistance = distance;
				closestEntity = entity;
			}
		}
		entities.Dispose();

		return closestEntity;
	}
	public static Entity FindClosestTilePosition<T>(Vector3 pos, float maxDist=5f)
	{
		if (tilePositionQuery == default)
		{
			tilePositionQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc
			{
				All = new ComponentType[] {
					new(Il2CppType.Of<TilePosition>(), ComponentType.AccessMode.ReadOnly),
					new(Il2CppType.Of<Translation>(), ComponentType.AccessMode.ReadOnly),
					new(Il2CppType.Of<T>(), ComponentType.AccessMode.ReadOnly)
				},
				Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludeSpawnTag
			});
		}

		var closestEntity = Entity.Null;
		var closestDistance = maxDist;
		var entities = tilePositionQuery.ToEntityArray(Allocator.Temp);
		for (var i = 0; i < entities.Length; ++i)
		{
			var entity = entities[i];
			if (!entity.Has<TilePosition>()) continue;
			var entityPos = entity.Read<Translation>().Value;
			var distance = math.distancesq(pos, entityPos);
			if (distance < closestDistance)
			{
				var prefabName = GetPrefabGUID(entity).LookupName();
				if (!prefabName.StartsWith("TM_")) continue;

				closestDistance = distance;
				closestEntity = entity;
			}
		}
		entities.Dispose();

		return closestEntity;
	}

	public static void RepairGear(Entity Character, bool repair = true)
	{
		Equipment equipment = Character.Read<Equipment>();
		NativeList<Entity> equippedItems = new(Allocator.Temp);
		equipment.GetAllEquipmentEntities(equippedItems);
		foreach (var equippedItem in equippedItems)
		{
			if (equippedItem.Has<Durability>())
			{
				var durability = equippedItem.Read<Durability>();
				if (repair)
				{
					durability.Value = durability.MaxDurability;
				}
				else
				{
					durability.Value = 0;
				}

				equippedItem.Write(durability);
			}
		}
		equippedItems.Dispose();

		for (int i = 0; i < 36; i++)
		{
			if (InventoryUtilities.TryGetItemAtSlot(Core.EntityManager, Character, i, out InventoryBuffer item))
			{
				var itemEntity = item.ItemEntity._Entity;
				if (itemEntity.Has<Durability>())
				{
					var durability = itemEntity.Read<Durability>();
					if (repair)
					{
						durability.Value = durability.MaxDurability;
					}
					else
					{
						durability.Value = 0;
					}

					itemEntity.Write(durability);
				}
			}
		}
	}

	const float ReviveDelaySeconds = 20f;

	public static void ReviveCharacter(Entity Character, Entity User, ChatCommandContext ctx = null)
	{
		if (BuffUtility.TryGetBuff(Core.EntityManager, Character, Prefabs.Buff_General_Vampire_Wounded_Buff, out var buffData))
		{
			Age buffAge = buffData.Read<Age>();
			if (buffAge.Value >= ReviveDelaySeconds)
			{
				ReviveCharacterNow(Character, User, ctx);
			}
			else
			{
				var remainingSeconds = ReviveDelaySeconds - buffAge.Value;
				ctx?.Reply($"Cannot revive yet. Try again in {remainingSeconds:F1} seconds...");
			}
		}
		ReviveCharacterNow(Character, User, ctx);
    }

	public static void ReviveCharacterNow(Entity Character, Entity User, ChatCommandContext ctx = null)
	{
		var health = Character.Read<Health>();
		if (BuffUtility.TryGetBuff(Core.EntityManager, Character, Prefabs.Buff_General_Vampire_Wounded_Buff, out var buffData))
		{
			DestroyUtility.Destroy(Core.EntityManager, buffData, DestroyDebugReason.TryRemoveBuff);
			health.Value = health.MaxHealth;
			health.MaxRecoveryHealth = health.MaxHealth;
			Character.Write(health);
			ctx.Reply($"Revived {User.Read<User>().CharacterName}");
		}
		if (health.IsDead)
		{
			var pos = Character.Read<LocalToWorld>().Position;
			Nullable_Unboxed<float3> spawnLoc = new() { value = pos };
			var sbs = Core.Server.GetExistingSystemManaged<ServerBootstrapSystem>();
			var bufferSystem = Core.Server.GetExistingSystemManaged<EntityCommandBufferSystem>();
			var buffer = bufferSystem.CreateCommandBuffer();
			sbs.RespawnCharacter(buffer, User,
				customSpawnLocation: spawnLoc,
				previousCharacter: Character);
			ctx.Reply($"Revived {User.Read<User>().CharacterName}");
		}
	}

	public static void KickPlayer(Entity userEntity)
	{
		EntityManager entityManager = Core.Server.EntityManager;
		User user = userEntity.Read<User>();

		if (!user.IsConnected || user.PlatformId==0) return;

		Entity entity =  entityManager.CreateEntity(new ComponentType[3]
		{
			ComponentType.ReadOnly<NetworkEventType>(),
			ComponentType.ReadOnly<SendEventToUser>(),
			ComponentType.ReadOnly<KickEvent>()
		});

		entity.Write(new KickEvent()
		{
			PlatformId = user.PlatformId
		});
		entity.Write(new SendEventToUser()
		{
			UserIndex = user.Index
		});
		entity.Write(new NetworkEventType()
		{
			EventId = NetworkEvents.EventId_KickEvent,
			IsAdminEvent = false,
			IsDebugEvent = false
		});
	}

	public static void UnlockWaypoints(Entity userEntity)
	{
		DynamicBuffer<UnlockedWaypointElement> dynamicBuffer = Core.EntityManager.AddBuffer<UnlockedWaypointElement>(userEntity);
		dynamicBuffer.Clear();
		foreach (Entity waypoint in Helper.GetEntitiesByComponentType<ChunkWaypoint>())
			dynamicBuffer.Add(new UnlockedWaypointElement()
			{
				Waypoint = waypoint.Read<NetworkId>()
			});
	}

	public static void RevealMapForPlayer(Entity userEntity)
	{
		var mapZoneElements = Core.EntityManager.GetBuffer<UserMapZoneElement>(userEntity);
		foreach (var mapZone in mapZoneElements)
		{
			var userZoneEntity = mapZone.UserZoneEntity.GetEntityOnServer();
			var revealElements = Core.EntityManager.GetBuffer<UserMapZonePackedRevealElement>(userZoneEntity);
			revealElements.Clear();
			var revealElement = new UserMapZonePackedRevealElement
			{
				PackedPixel = 255
			};
			for (var i = 0; i < 8192; i++)
			{
				revealElements.Add(revealElement);
			}
		}
	}

	public static bool IsPlayerInCombat(Entity player)
	{
		return BuffUtility.HasBuff(Core.EntityManager, player, Prefabs.Buff_InCombat) || BuffUtility.HasBuff(Core.EntityManager, player, Prefabs.Buff_InCombat_PvPVampire);
	}

	public static bool IsPlayerDeathFromPvP(Entity player)
	{
		return BuffUtility.HasBuff(Core.EntityManager, player, Prefabs.Buff_General_VampirePvPDeathDebuff);
	}

	public static float GetPlayerEquipmentLevel(Player player)
	{
		var charEntity = player.Character;
		var equipment = charEntity.Read<Equipment>();
		var characterLevel = equipment.ArmorLevel + equipment.SpellLevel + equipment.WeaponLevel;
		return characterLevel;
	}

	// add the component debugunlock
}
