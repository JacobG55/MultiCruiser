using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiTruck.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPatch("LoadNewNodeIfAffordable")]
        [HarmonyPrefix]
        public static bool patchPurchase(TerminalNode node, Terminal __instance, 
                ref bool ___vehicleInDropship, ref TerminalNodesList ___terminalNodes, ref int ___totalCostOfItems, ref Item[] ___buyableItemsList, ref int[] ___itemSalesPercentages, ref int ___groupCredits, 
                ref bool ___hasWarrantyTicket, ref int ___numberOfItemsInDropship, ref int ___orderedVehicleFromTerminal
            )
        {
            if (node.buyVehicleIndex == -1)
            {
                return true;
            }

            int num = ___buyableItemsList.Length + node.buyVehicleIndex;
            ___totalCostOfItems = (int)((float)node.itemCost * ((float)___itemSalesPercentages[num] / 100f));

            if (___groupCredits < ___totalCostOfItems && !___hasWarrantyTicket)
            {
                __instance.LoadNewNode(___terminalNodes.specialNodes[2]);
                return false;
            }

            if (___numberOfItemsInDropship > 0)
            {
                __instance.LoadNewNode(___terminalNodes.specialNodes[27]);
                return false;
            }
            if (___vehicleInDropship && node.buyItemIndex != -1)
            {
                __instance.LoadNewNode(___terminalNodes.specialNodes[25]);
                return false;
            }

            ItemDropship itemDropship = UnityEngine.Object.FindObjectOfType<ItemDropship>();
            if (___vehicleInDropship || (itemDropship != null && itemDropship.deliveringVehicle))
            {
                __instance.LoadNewNode(___terminalNodes.specialNodes[26]);
                return false;
            }

            if (!node.isConfirmationNode)
            {
                if (!___hasWarrantyTicket)
                {
                    ___groupCredits = Mathf.Clamp(___groupCredits - ___totalCostOfItems, 0, 10000000);
                }

                ___orderedVehicleFromTerminal = node.buyVehicleIndex;
                ___vehicleInDropship = true;
                Debug.Log($"Is server?: {__instance.IsServer}");
                if (!__instance.IsServer)
                {
                    SyncBoughtVehicleWithServer(__instance, ___orderedVehicleFromTerminal);
                }
                else
                {
                    ___hasWarrantyTicket = !___hasWarrantyTicket;
                    BuyVehicleClientRpc(__instance, ___groupCredits, ___hasWarrantyTicket);
                }
            }
            __instance.LoadNewNode(node);
            return false;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Terminal), "SyncBoughtVehicleWithServer")]
        public static void SyncBoughtVehicleWithServer(object instance, int vehicleID) =>
            throw new NotImplementedException("It's a stub");

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Terminal), "BuyVehicleClientRpc")]
        public static void BuyVehicleClientRpc(object instance, int newGroupCredits, bool giveWarranty) =>
            throw new NotImplementedException("It's a stub");
    }
}
