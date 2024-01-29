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
                //Postman.UpdateWeight();
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
        float weightOffsetTotal = 0;
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

        public void UpdateWeight(bool updateTotal = true)
        {
            return;

            /*
            Disabled because it is causing issues with speed, because weight is set below one
            To fix the issue, the weight needs to be updated when entering / leaving the factory and when changeing the carried items while outside of the factory

            possible functions to patch (PlayerControllerB)

            SetObjectAsNoLongerHeld()
                - is being called when an item is droppen (i think)
                - has a dropObject with a weight value attached

                - used: carryWeight -= Mathf.Clamp(dropObject.itemProperties.weight - 1f, 0f, 10f);

            BeginGrabObject()
                - is being called once the client starts grabbing an object
                - has a currentlyGrabbingObject with a weight value attached

                - used: carryWeight += Mathf.Clamp(currentlyGrabbingObject.itemProperties.weight - 1f, 0f, 10f);
            */

            float currentWeightOffset;
            if (updateTotal)
                currentWeightOffset = weightOffsetTotal;
            else
                currentWeightOffset = weightOffset;

            if (player.isInsideFactory && weightUpgradeApplyed)
            {
                player.carryWeight /= currentWeightOffset;
                weightUpgradeApplyed = false;
                Debug.Log("MoreUpgrades: Removed Weight upgrade");
            }
            if (!player.isInsideFactory && !weightUpgradeApplyed)
            {
                player.carryWeight *= currentWeightOffset;
                weightUpgradeApplyed = true;
                Debug.Log("MoreUpgrades: Applyed Weight upgrade");
            }
            Debug.Log($"MoreUpgrades: New Weight: {player.carryWeight}");
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

            weightOffset = (10f - Upgradelevel) / 10f;
            weightOffsetTotal += weightOffset;
            //weightUpgradeApplyed = false;

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