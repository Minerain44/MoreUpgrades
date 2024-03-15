using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace MoreUpgrades
{
    class UpgradeManager : MonoBehaviour
    {
        PlayerControllerB player;
        bool playerWasInsideFactory = false;

        public Postman postman = new Postman();
        public BiggerPockets biggerPockets = new BiggerPockets();
        public ScrapPurifier scrapPurifier = new ScrapPurifier();
        public ScrapMagnet scrapMagnet = new ScrapMagnet();
        public WeatherCleaner weatherCleaner = new WeatherCleaner();
        public List<Upgrade> upgrades = new List<Upgrade>(); // All currently existing upgrades

        private void Start()
        {
            if (player == null)
                player = GameNetworkManager.Instance.localPlayerController;
        }

        public void CreateUpgrades()
        {
            upgrades.Add(postman);
            upgrades.Add(biggerPockets);
            upgrades.Add(scrapPurifier);
            upgrades.Add(scrapMagnet);
            upgrades.Add(weatherCleaner);
        }

        public void SetupUpgrades()
        {
            foreach (Upgrade upgrade in upgrades)
            {
                upgrade.Setup();
            }
        }

        private void Update()
        {
            if (player == null)
            {
                player = GameNetworkManager.Instance.localPlayerController;
                if (player != null) SetupUpgrades(); // Only call it once the player has spawned since some upgrades also need to get the player
            }

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
        bool onetimeUse = false;

        public int Price { get { return price; } set { price = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Description { get { return description; } set { description = value; } }
        public int Upgradelevel { get { return upgradelevel; } set { upgradelevel = value; } }
        public int UpgradelevelCap { get { return upgradelevelCap; } set { upgradelevelCap = value; } }
        public bool OnetimeUse { get { return onetimeUse; } set { onetimeUse = value; } }

        abstract public void Setup();
        abstract public void LevelUp();
    }

    class Postman : Upgrade
    {
        bool speedUpgradeApplyed = false;
        PlayerControllerB player;
        float speedOffset = 0;
        float speedOffsetTotal = 0;
        float weightOffset = 0;

        public Postman()
        {
            Price = 500;
            Name = "Postman";
            Description = "This is a suit attachment created by The Company to allow you to move more quickly and carry more weight. For some currently unknown reason, this device is deactivated while inside of a building. The Company promised to repair it, once you finished work."; // "Lets you walk faster and carry heavier items while on the surface of the moon";
            UpgradelevelCap = 5;
        }

        public override void Setup()
        {
            player = GameNetworkManager.Instance.localPlayerController;
            if (player == null)
                Debug.LogError($"MoreUpgrades: No Player found! Some core game functionalitys (eg. Dropping Items) will not work!");
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

        public void ReduceWeight(float objectWeight)
        {
            objectWeight = (float)Mathf.Round(objectWeight * 100) / 100f;
            float weightMultiplier = 0;
            if (!player.isInsideFactory) { weightMultiplier = Upgradelevel / 10f; }
            float weight = Mathf.Clamp(objectWeight - 1f, 0f, 10f) * weightMultiplier;

            player.carryWeight -= weight;

            Debug.Log($"MoreUpgrades: New Player Weight: {player.carryWeight}");
        }

        public void AddWeigth(float objectWeight)
        {
            objectWeight = (float)Mathf.Round(objectWeight * 100) / 100f;
            float weightMultiplier = 0;
            if (!player.isInsideFactory) { weightMultiplier = Upgradelevel / 10f; }
            float weight = Mathf.Clamp(objectWeight - 1f, 0f, 10f) * weightMultiplier;

            player.carryWeight += weight;

            Debug.Log($"MoreUpgrades: New Player Weight: {player.carryWeight}");
        }

        public void ToggleWeight(bool isInsideFactory)
        {
            if (isInsideFactory)
                player.carryWeight += weightOffset;
            else
                player.carryWeight -= weightOffset;
        }

        public void UpdateWeightOffset(float vanillaWeightChange, bool reduce)
        {
            vanillaWeightChange -= 1f;
            float upgradeWeightChange = (float)Mathf.Clamp(vanillaWeightChange, 0f, 10f) * (Upgradelevel / 10f);

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

            speedOffset = Upgradelevel * 0.35f;
            speedOffsetTotal += speedOffset;
            speedUpgradeApplyed = false;

            Price += (int)MathF.Round(Price * .15f);
            Price -= Price % 5;

            UpdateSpeed(false);
        }
    }

    class BiggerPockets : Upgrade
    {
        public BiggerPockets()
        {
            Price = 750;
            Name = "Bigger Pockets";
            Description = "A type of suit that was created long ago to carry around more items. It's a little dusty, but it could come in handy when roaming around the moons collecting scrap."; // "Gives you one extra inventory slot per level";
            UpgradelevelCap = 2;
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
            Price = 1500;
            Name = "Scrap Purifier";
            Description = "All of the factories had little robots roaming around the facility in order to keep everything neat and tidy. These robots could still be used to polish the scrap lying around in order to increase its value. Although they won't work if anyone is in the facility."; // "Increases the general Value of all Scraps";
            UpgradelevelCap = 2;
        }
        public override void Setup()
        {
            //throw new NotImplementedException();
        }

        public float UpdateValue()
        {
            float valueMultiplier = 1 + (Upgradelevel * .5f);
            return valueMultiplier;
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .25f);
            Price -= Price % 5;
        }
    }

    class ScrapMagnet : Upgrade
    {
        public ScrapMagnet()
        {
            Price = 1000;
            Name = "Scrap Magnet";
            Description = "Calms The Entity to increase the number of Scrap"; // "Increases the amount of scrap that spawns in a moon";
            UpgradelevelCap = 2;
        }
        public override void Setup()
        {
            //throw new NotImplementedException();
        }

        public float UpdateValue()
        {
            float valueMultiplier = 1 + (Upgradelevel * .4f); //debug value Actual value is ~ 0.4
            valueMultiplier = 100f;
            return valueMultiplier;
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            Price += (int)MathF.Round(Price * .25f);
            Price -= Price % 5;
        }
    }

    class WeatherCleaner : Upgrade
    {
        StartOfRound startOfRound;

        public StartOfRound StartOfRound { get => startOfRound; set => startOfRound = value; }

        public WeatherCleaner()
        {
            Price = 400;
            Name = "Weather Cleaner";
            Description = "There is some sort of device installed on this ship that can alter the weather on moons across the galaxy, but of course it requires a premium subscription to work"; // "Clears all weather effects";
            UpgradelevelCap = 1;
            OnetimeUse = true;
        }
        public override void Setup()
        {
            //throw new NotImplementedException();
        }

        public void ClearWeather()
        {
            Debug.Log("MoreUpgrades: Setting Planets Weather");
            for (int i = 0; i < startOfRound.levels.Length; i++)
            {
                startOfRound.levels[i].currentWeather = LevelWeatherType.None;
            }
        }

        public override void LevelUp()
        {
            Upgradelevel++;
            ClearWeather();
        }
    }
}