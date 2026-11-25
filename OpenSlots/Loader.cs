using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
namespace OpenSlots
{
    [BepInPlugin(GUID,Name, Version)]
    internal class Loader : BaseUnityPlugin
    {
        internal const string GUID = "coolchair.openslots";
        internal const string Name = "Open Slots";
        internal const string Version = "1.0.0";
        internal static Harmony harmony;
        internal static ManualLogSource logger;
        void Awake()
        {
            Logger.LogDebug("Started OpenSlots mod");
            logger = Logger;
            harmony = new Harmony(GUID);
            harmony.PatchAll();
            Logger.LogDebug("OpenSlots mod loaded succesfully!");
        }
    }
}
