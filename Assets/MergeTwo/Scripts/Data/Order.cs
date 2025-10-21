using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class Order
    {
        [HideInInspector] public string Name;
        public List<IconOrder> IconsToCollect;
        public int ID;
        public int OrderReward = 1;

        public Order Clone() 
        {
            string str = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Order>(str);
        }

        public override string ToString()
        {
            if (IconsToCollect != null && IconsToCollect.Count > 0) 
            {
                StringBuilder sb = new StringBuilder();
                IconsToCollect.ForEach(x => sb.Append($"{x}|"));
                return sb.ToString();
            }

            return base.ToString();
        }
    }
}