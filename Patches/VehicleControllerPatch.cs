using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiTruck.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    internal class VehicleControllerPatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(VehicleController), "DealPermanentDamage")]
        public static void DealPermanentDamage(object instance, int damageAmount, Vector3 damagePosition = default(Vector3)) =>
            throw new NotImplementedException("It's a stub");
    }
}
