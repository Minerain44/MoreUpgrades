using System;
using System.Collections.Generic;
using System.Text;

namespace MoreUpgrades
{
    internal class Items
    {
        abstract class Item
        {
            int price;
            string name;
            string description;

            public int Price { get { return price; } set { price = value; } }
            public string Name { get { return name; } set { name = value; } }
            public string Description { get { return description; } set { description = value; } }
        }
    }
}
