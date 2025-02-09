using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;

namespace SanguinePact;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
public class Plugin : BasePlugin
{
    internal static Harmony Harmony;
    public static ManualLogSource LogInstance { get; private set; }

    private const float DefaultResistMultiplier = -4.0f; // Take 400% more damage
    private const float DefaultDamageMultiplier = 2.0f; // Deal 100% more damage

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
        ResistMultiplier = Config.Bind("ResistMultiplier", "multiplier", DefaultResistMultiplier, "Resist multiplier. -4 means 400% more damage.");
        DamageMultiplier = Config.Bind("DamageMultiplier", "multiplier", DefaultDamageMultiplier, "Damage multiplier. 2 means 100% more damage.");
        Log.LogInfo($"Loaded Sanguine Pact Multipliers: Resist={ResistMultiplier.Value}, Damage={DamageMultiplier.Value}");
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        Harmony.UnpatchSelf();
        return true;
    }
}
