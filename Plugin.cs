using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppSystem;
using VampireCommandFramework;

namespace SanguinePact;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
public class Plugin : BasePlugin
{
    internal static Harmony Harmony;
    public static ManualLogSource LogInstance { get; private set; }

    private const float DefaultResistMultiplier = 4; // Take 4x damage
    private const float DefaultDamageMultiplier = 2; // Deal 2x damage

    public static ConfigEntry<float> ResistMultiplier;
    public static ConfigEntry<float> DamageMultiplier;

    public override void Load()
    {
        if (Application.productName != "VRisingServer")
            return;

        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
        LogInstance = Log;

        // Harmony patching
        Harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
        InitConfig();
    }

    private void InitConfig()
    {
        ResistMultiplier = Config.Bind("ResistMultiplier", "multiplier", DefaultResistMultiplier, "Resist multiplier. 4 means 4x damage.");
        DamageMultiplier = Config.Bind("DamageMultiplier", "multiplier", DefaultDamageMultiplier, "Damage multiplier. 2 means 2x damage.");
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        Harmony.UnpatchSelf();
        return true;
    }
}
