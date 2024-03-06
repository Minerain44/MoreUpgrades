using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using TerminalApi;
using TerminalApi.Classes;
using UnityEngine;
using static TerminalApi.Events.Events;
using static TerminalApi.TerminalApi;

namespace MoreUpgrades
{
    class UpgradeManager : MonoBehaviour
    {
        PlayerControllerB player;
        bool playerWasInsideFactory = false;

        public Postman postman = new Postman();
        public BiggerPockets biggerPockets = new BiggerPockets();
        public ScrapPurifier scrapPurifier = new ScrapPurifier();
        public List<Upgrade> upgrades = new List<Upgrade>(); // All currently existing upgrades

        private void Start()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;
        }

        public void SetupUpgrades()
        {
            upgrades.Add(postman);
            upgrades.Add(biggerPockets);
            upgrades.Add(scrapPurifier);

            foreach (Upgrade upgrade in upgrades)
            {
                upgrade.Setup();
            }
        }

        private void Update()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;

            if (player.isInsideFactory != playerWasInsideFactory) //For postman Upgrade
            {
                playerWasInsideFactory = player.isInsideFactory;
                postman.UpdateSpeed();
                postman.ToggleWeight(player.isInsideFactory);
            }
        }
    }

    abstract class Upgrade
    {
        int price;
        string name;
        string description;
        int upgradelevel;
        int upgradelevelCap;

        public int Price { get { return price; } set { price = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Description { get { return description; } set { description = value; } }
        public int Upgradelevel { get { return upgradelevel; } set { upgradelevel = value; } }
        public int UpgradelevelCap { get { return upgradelevelCap; } set { upgradelevelCap = value; } }

        abstract public void Setup();
        abstract public void LevelUp();
    }

    class Postman : Upgrade
    {
        bool speedUpgradeApplyed = false;
        bool weightUpgradeApplyed = false;
        PlayerControllerB player;
        float speedOffset = 0;
        float speedOffsetTotal = 0;
        float weightOffset = 0;

        public Postman()
        {
            Price = 500;
            Name = "Postman";
            Description = "Lets you walk faster and carry heavier items while on the surface of the moon";
            UpgradelevelCap = 5;
        }

        public override void Setup()
        {
            //throw new NotImplementedException();
        }

        public void UpdateSpeed(bool updateTotal = true)
        {
            float currentSpeedOffset;
            if (updateTotal)
                currentSpeedOffset = speedOffsetTotal;
            else
                currentSpeedOffset = speedOffset;

            if (player.isInsideFactory && speedUpgradeApplyed)
            {
                player.movementSpeed -= currentSpeedOffset;
                speedUpgradeApplyed = false;
                Debug.Log("MoreUpgrades: Removed Speed upgrade");
            }
            if (!player.isInsideFactory && !speedUpgradeApplyed)
            {
                player.movementSpeed += currentSpeedOffset;
                speedUpgradeApplyed = true;
                Debug.Log("MoreUpgrades: Applyed speed upgrade");
            }
            Debug.Log($"MoreUpgrades: New playerspeed: {player.movementSpeed}");
        }

        public void ToggleWeight(bool isInsideFactory)
        {
            if (isInsideFactory)
                player.carryWeight += weightOffset;
            else
                player.carryWeight -= weightOffset;
        }

        public void UpdateWeightOffset(float upgradeWeightChange, float vanillaWeightChange, bool reduce)
        {
            if (reduce)
                weightOffset -= vanillaWeightChange - upgradeWeightChange;
            else
                weightOffset += vanillaWeightChange - upgradeWeightChange;
        }

        public override void LevelUp()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;

            Upgradelevel++;

            Debug.Log($"MoreUpgrades: Leveling up Postman to level {Upgradelevel}");

            speedOffset = Upgradelevel * 0.5f;
            speedOffsetTotal += speedOffset;
            speedUpgradeApplyed = false;

            // weightOffset = (10f - Upgradelevel) / 10f;
            // weightOffsetTotal += weightOffset;
            // weightUpgradeApplyed = false;

            Price += (int)MathF.Round(Price * .15f);
            Price -= Price % 5;

            UpdateSpeed(false);
            //UpdateWeight(false);
        }
    }

    class BiggerPockets : Upgrade
    {
        public BiggerPockets()
        {
            Price = 750;
            Name = "Bigger Pockets";
            Description = "Gives you one extra inventory slot per level";
            UpgradelevelCap = 3;
        }

        public override void Setup()
        {
            //throw new NotImplementedException();
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .25f);
            Price -= Price % 5;
        }
    }

    class ScrapPurifier : Upgrade
    {
        public ScrapPurifier()
        {
            Price = 1000;
            Name = "Scrap Purifier";
            Description = "Enhances the general Value of all Scraps";
            UpgradelevelCap = 2;
        }
        public override void Setup()
        {
            //throw new NotImplementedException();
        }

        public float UpdateValue()
        {
            float valueMultiplier = 1 + (Upgradelevel * 100f);
            return valueMultiplier;
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .25f);
            Price -= Price % 5;
        }
    }
}