using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using MultiTruck.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTruck
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MultiTruckBase : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.MultiCruiser";
        private const string modName = "MultiCruiser";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static MultiTruckBase Instance;

        public static ManualLogSource mls;

        public static ConfigEntry<bool> enableCruiserCollision;
        public static ConfigEntry<bool> enableCruiserCollisionDamage;
        public static ConfigEntry<int> cruiserCollisionBaseDamage;
        public static ConfigEntry<int> cruiserCollisionSelfDamage;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            enableCruiserCollision = Config.Bind("Main", "enableCruiserCollision", true, "Enables Cruisers to collide with each-other by 'pushing' each-other when collision detected.");
            enableCruiserCollisionDamage = Config.Bind("Main", "enableCruiserCollisionDamage", true, "If Cruisers should Damage eachother when colliding. Requires CruiserCollision to be enabled.");
            cruiserCollisionBaseDamage = Config.Bind("Main", "cruiserCollisionBaseDamage", 2, "Amount of damage Cruiser-on-Cruiser Collisions deal to the other Cruiser. (+2 for Medium Speed Collisions & +4 for High Speed Collisions)");
            cruiserCollisionSelfDamage = Config.Bind("Main", "cruiserCollisionSelfDamage", 1, "Amount of damage Cruiser-on-Cruiser Collisions deal to themselves. (+1 for Medium Speed Collisions & +2 for High Speed Collisions)");

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Patching Terminal");

            harmony.PatchAll(typeof(TerminalPatch));
            harmony.PatchAll(typeof(VehicleControllerPatch));
            harmony.PatchAll(typeof(VehicleCollisionTriggerPatch));
        }
    }
}
