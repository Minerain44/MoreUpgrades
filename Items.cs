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

        class EnergyDrink : Item
        {
            public EnergyDrink()
            {
                Price = 250;
                Name = "Energy Drink";
                Description = "A drink that increases your speed and unlimits your stamia for a short time.";
            }
        }
    }
}
