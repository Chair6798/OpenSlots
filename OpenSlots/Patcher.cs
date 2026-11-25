using HarmonyLib;
using Photon.Pun;
using Zorro.Core;
using UnityEngine;
using Mono.Cecil;
namespace OpenSlots
{
    [HarmonyPatch(typeof(Item))]
    internal static class ItemPatcher
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostfix(Item __instance)
        {
            var d = __instance.UIData;
            if(!d.canBackpack)
                d.canBackpack = true;
            if(!d.canDrop)
                d.canDrop = true;
            if(!d.canThrow)
                d.canThrow = true;
            if(!d.canPocket)
                d.canPocket = true;
        }
    }
    [HarmonyPatch(typeof(BackpackWheel))]
    internal static class BackpackWheelPatch
    {
        [HarmonyPatch("Choose")]
        [HarmonyPrefix]
        static bool Choose(BackpackWheel __instance, Optionable<BackpackWheelSlice.SliceData> ___chosenSlice)
        {
            Loader.logger.LogDebug("Got choose!");
            if (!___chosenSlice.IsSome)
                return true;
            var s = ___chosenSlice.Value;
            if (s.isBackpackWear || s.isStashSlice || s.isJetpackFuelSlice)
                return true;
            if (!s.backpackReference.exists) 
                return true;
            var slotO = s.backpackReference.GetData().itemSlots[s.slotID];
            if (slotO == null||slotO.IsEmpty()) return true;
            ItemInstanceData data1 = slotO.data;
            if (data1.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out BackpackData bp1))
            {
                Loader.logger.LogDebug("Choosed backpack!");
                ItemInstanceData data2 = data1.CopyBackpackData();
                if (Player.localPlayer.HasEmptySlot(slotO.prefab.itemID))
                {
                    Player.localPlayer.AddItem(slotO.prefab.itemID, data2, out var slot);
                }
                else
                {
                    var ct = Camera.main.transform;
                    var go =PhotonNetwork.Instantiate("0_Items/" + slotO.GetPrefabName(), ct.position+ct.forward*2, Quaternion.identity);
                    go.GetComponent<Backpack>().data = data2;
                    go.GetComponent<Backpack>().photonView.RPC("SetItemInstanceDataRPC", RpcTarget.All, data2);
                }
                    
                s.backpackReference.GetData().GetSlot(data1).EmptyOut();
                s.backpackReference.GetVisuals().gameObject.GetComponent<Backpack>().photonView.RPC("SetItemInstanceDataRPC", RpcTarget.All, s.backpackReference.GetVisuals().gameObject.GetComponent<Backpack>().data);
                return false;
            }

            return true;
        }
        
    }
}
