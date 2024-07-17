using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiTruck.Patches
{
    [HarmonyPatch(typeof(VehicleCollisionTrigger))]
    internal class VehicleCollisionTriggerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void patchStart()
        {
            if (MultiTruckBase.enableCruiserCollision.Value == true)
            {
                Physics.IgnoreLayerCollision(30, 13, false);
            }
        }

        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        public static void patchOnTriggerEnter(Collider other, ref VehicleController ___mainScript)
        {
            if (other.gameObject.layer == 30 && MultiTruckBase.enableCruiserCollision.Value == true)
            {
                if (other.gameObject.TryGetComponent<VehicleController>(out VehicleController cruiser))
                {
                    if (cruiser != ___mainScript)
                    {
                        cruiser.PushTruckServerRpc(___mainScript.transform.position, cruiser.transform.position - ___mainScript.transform.position);

                        if (MultiTruckBase.enableCruiserCollisionDamage.Value == true)
                        {
                            int damage = 0;

                            if (___mainScript.averageVelocity.magnitude > 27f)
                            {
                                damage += 4;
                            }
                            else if (___mainScript.averageVelocity.magnitude > 19f)
                            {
                                damage += 2;
                            }
                            
                            if (___mainScript.averageVelocity.magnitude > 11f)
                            {
                                VehicleControllerPatch.DealPermanentDamage(cruiser, Math.Max(0, MultiTruckBase.cruiserCollisionBaseDamage.Value + damage));
                                VehicleControllerPatch.DealPermanentDamage(___mainScript, Math.Max(0, MultiTruckBase.cruiserCollisionSelfDamage.Value + (damage / 2)));
                            }
                        }
                    }
                }
            }
        }
    }
}
