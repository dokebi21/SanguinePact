using HarmonyLib;
using ProjectM;
using Unity.Collections;
using SanguinePact.Common.KindredCommands.Data;

namespace SanguinePact.SanguinePact.Patches;

[HarmonyPatch(typeof(BuffSystem_Spawn_Server), nameof(BuffSystem_Spawn_Server.OnUpdate))]
public static class BuffSystem_Spawn_ServerPatch
{
    public static void Prefix(BuffSystem_Spawn_Server __instance)
    {
        if (Core.SanguinePactService == null) return;
        EntityManager entityManager = __instance.EntityManager;
        NativeArray<Entity> entities = __instance.__query_401358634_0.ToEntityArray(Allocator.Temp);
        foreach (var buffEntity in entities)
        {
            var prefabGUID = buffEntity.Read<PrefabGUID>();
            Entity owner = buffEntity.Read<EntityOwner>().Owner;
            if (!owner.Has<PlayerCharacter>()) continue;
            if (!Core.SanguinePactService.IsSanguinePactPlayer(owner)) continue;
            if (prefabGUID == Prefabs.SanguinePactBuff)
            {
                Core.SanguinePactService.UpdateSanguinePactBuff(buffEntity);
            }
        }
    }
}
