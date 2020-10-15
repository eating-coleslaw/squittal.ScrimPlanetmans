using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace squittal.ScrimPlanetmans.Models.Planetside
{
    public class ItemVariantGroup
    {
        [Required]
        public int BaseItemId { get; set; }
        public string BaseItemName { get; set; } // TODO: rename GroupName?

        public List<int> ItemVariants { get; private set; }

        public ItemVariantGroup(Item item)
        {
            BaseItemId = item.Id;
            BaseItemName = item.Name;

            ItemVariants = new List<int>()
            {
                item.Id
            };
        }

        public bool ContainsItemId(int itemId)
        {
            return ItemVariants.Contains(itemId);
        }

        public bool TryAddItemVariantId(int variantId)
        {
            if (ItemVariants.Contains(variantId))
            {
                return false;
            }
            else
            {
                ItemVariants.Add(variantId);
                return true;
            }
        }

        public void AddItemVariantIds(IEnumerable<int> variantIds)
        {
            ItemVariants.AddRange(variantIds.Distinct().Where(v => !ItemVariants.Contains(v)));
        }

    }
}
