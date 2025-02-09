using System.Collections;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using ProjectM;
using ProjectM.Physics;
using ProjectM.Scripting;
using SanguinePact.SanguinePact.Services;

namespace SanguinePact.Common;

internal static class Core
{
    public static World Server { get; } = GetWorld("Server") ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");
    public static EntityManager EntityManager { get; } = Server.EntityManager;
    public static GameDataSystem GameDataSystem { get; } = Server.GetExistingSystemManaged<GameDataSystem>();
    public static PrefabCollectionSystem PrefabCollectionSystem { get; internal set; }
    public static RelicDestroySystem RelicDestroySystem { get; internal set; }
    public static ServerScriptMapper ServerScriptMapper { get; internal set; }
    public static double ServerTime => ServerGameManager.ServerTime;
    public static ServerGameManager ServerGameManager => ServerScriptMapper.GetServerGameManager();
    public static ServerGameSettingsSystem ServerGameSettingsSystem { get; internal set; }

    public static ManualLogSource Log { get; } = Plugin.LogInstance;

    static MonoBehaviour monoBehaviour;

    public const int MAX_REPLY_LENGTH = 509;

    // Services
    public static PlayerService Players { get; internal set; }
    public static SanguinePactService SanguinePactService { get; internal set; }

    public static void LogException(System.Exception e, [CallerMemberName] string caller = null)
    {
        Core.Log.LogError($"Failure in {caller}\nMessage: {e.Message} Inner:{e.InnerException?.Message}\n\nStack: {e.StackTrace}\nInner Stack: {e.InnerException?.StackTrace}");
    }

    internal static void InitializeAfterLoaded()
    {
        if (_hasInitialized) return;

        PrefabCollectionSystem = Server.GetExistingSystemManaged<PrefabCollectionSystem>();
        RelicDestroySystem = Server.GetExistingSystemManaged<RelicDestroySystem>();
        ServerGameSettingsSystem = Server.GetExistingSystemManaged<ServerGameSettingsSystem>();
        ServerScriptMapper = Server.GetExistingSystemManaged<ServerScriptMapper>();

        Players = new();
        SanguinePactService = new();

        _hasInitialized = true;
        Log.LogInfo($"{nameof(InitializeAfterLoaded)} completed");
    }
    private static bool _hasInitialized = false;

    private static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }

        return null;
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (monoBehaviour == null)
        {
            var go = new GameObject("SanguinePact");
            monoBehaviour = go.AddComponent<IgnorePhysicsDebugSystem>();
            Object.DontDestroyOnLoad(go);
        }

        return monoBehaviour.StartCoroutine(routine.WrapToIl2Cpp());
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        if (monoBehaviour == null)
        {
            return;
        }

        monoBehaviour.StopCoroutine(coroutine);
    }
}
