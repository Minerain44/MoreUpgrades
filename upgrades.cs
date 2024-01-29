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
    class UpgradeManger : MonoBehaviour
    {
        PlayerControllerB player;
        bool playerWasInsideFactory = false;

        static Postman Postman = new Postman();
        static BiggerPockets BiggerPockets = new BiggerPockets();
        public List<Upgrade> upgrades = new List<Upgrade>() // All currently existing upgrades
        {
            Postman,
            BiggerPockets
        };

        private void Start()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;

            foreach (Upgrade upgrade in upgrades)
            {
                upgrade.Setup();
            }
        }

        private void Update()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;

            if (player.isInsideFactory != playerWasInsideFactory) //For Postman Upgrade
            {
                playerWasInsideFactory = player.isInsideFactory;
                Postman.UpdateSpeed();
                Postman.UpdateWeight();
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
        bool strenghUpgradeApplyed = false;
        PlayerControllerB player;
        float speedOffset = 0;
        float weightOffset = 0;
        public Postman()
        {
            Price = 500;
            Name = "Postman";
            Description = "Lets you walk faster while on the surface of the moon";
            UpgradelevelCap = 5;
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }

        public void UpdateSpeed()
        {
            if (player.isInsideFactory && speedUpgradeApplyed)
            {
                player.movementSpeed -= speedOffset;
                speedUpgradeApplyed = false;
                Debug.Log("MoreUpgrades: Removed Speed upgrade");
            }
            if (!player.isInsideFactory && !speedUpgradeApplyed)
            {
                player.movementSpeed += speedOffset;
                speedUpgradeApplyed = true;
                Debug.Log("MoreUpgrades: Applyed speed upgrade");
            }
            Debug.Log($"MoreUpgrades: New playerspeed: {player.movementSpeed}");
        }

        public void UpdateWeight()
        {
            if (player.isInsideFactory && strenghUpgradeApplyed)
            {
                player.carryWeight *= weightOffset;
                strenghUpgradeApplyed = false;
                Debug.Log("MoreUpgrades: Removed Weight upgrade");
            }
            if (!player.isInsideFactory && !strenghUpgradeApplyed)
            {
                player.carryWeight /= weightOffset;
                strenghUpgradeApplyed = true;
                Debug.Log("MoreUpgrades: Applyed Weight upgrade");
            }
            Debug.Log($"MoreUpgrades: New Weight: {player.carryWeight}");
        }

        public override void LevelUp()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;

            speedOffset += Upgradelevel * 0.5f;
            weightOffset += (10 - Upgradelevel) / 10;

            Upgradelevel++;
            Price += (int)MathF.Round(Price * .15f);
            Price -= Price % 5;
            speedUpgradeApplyed = false;
            UpdateSpeed();
            UpdateWeight();
            Debug.Log($"MoreUpgrades: Leveling up Postman to level {Upgradelevel}");
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
            throw new NotImplementedException();
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .25f);
            Price -= Price % 5;
        }
    }
}