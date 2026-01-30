using Items.Data;
using UnityEngine;

namespace Items
{
    [System.Serializable]
    public class Accessory : Item
    {
        public AccessoryData accessoryData;
        public Accessory(AccessoryData data) : base(data)
        {
            accessoryData = data;
        }
        
        protected override void HandleItemLevelUp()
        {
            base.HandleItemLevelUp();
        }
    }
}