using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSlots
{
    internal static class Extensions
    {
        internal static ItemInstanceData CopyBackpackData(this ItemInstanceData src)
        {
            var dst = new ItemInstanceData(System.Guid.NewGuid());
            ItemInstanceDataHandler.AddInstanceData(dst);
            if (src == null || !src.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out var srcBp))
                return dst;
            var dstBp = dst.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
            dstBp.litRocketData = srcBp.litRocketData;
            for (byte i = 0; i < srcBp.itemSlots.Length; i++)
            {
                var s = srcBp.itemSlots[i];
                if (s == null || s.IsEmpty() || s.prefab == null) continue;
                ItemInstanceData slotData;
                if (s.data != null && s.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out _))
                    slotData = CopyBackpackData(s.data);
                else
                {
                    slotData = s.data != null ? s.data.Copy() : new ItemInstanceData(System.Guid.NewGuid());
                    ItemInstanceDataHandler.AddInstanceData(slotData);
                }
                dstBp.AddItem(s.prefab, slotData, i);
            }
            return dst;
        }
        internal static ItemSlot GetSlot(this BackpackData bp, ItemInstanceData data)
        {
            foreach(ItemSlot slot in bp.itemSlots)
            {
                if (slot == null) continue;
                if (slot.data == data)
                    return slot;
            }
            return null;
        }
    }
}
