﻿using BepInEx;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace MoreUpgrades
{
    public class SaveController
    {
        UpgradeManager upgradeManager;
        SaveFile saveFile;

        string GetSaveFile()
        {
            string currentSaveFile = GameNetworkManager.Instance.currentSaveFileName;
            return Path.Combine(GetSaveDir(), $"{currentSaveFile}.json");
        }

        string GetSaveDir()
        {
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(assemblyLocation, "Saves");
        }

        void CreateSaveFile()
        {
            if(!Directory.Exists(GetSaveDir()))
                Directory.CreateDirectory(GetSaveDir());
                
            if(!File.Exists(GetSaveFile()))
                File.Create(GetSaveFile()).Dispose();
        }

        public void SaveToFile()
        {
            if(upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();
            
            Debug.Log($"MoreUpgrades: Saving to file: {GetSaveFile()}");
            
            CreateSaveFile();
            PrepareSaveFile();

            StreamWriter writer = new StreamWriter(GetSaveFile());
            writer.Write(saveFile.ToJson());
            writer.Close();
        }

        void PrepareSaveFile()
        {
            saveFile = new SaveFile(
                upgradeManager.postman.Upgradelevel,
                upgradeManager.biggerPockets.Upgradelevel,
                upgradeManager.scrapPurifier.Upgradelevel,
                upgradeManager.scrapMagnet.Upgradelevel,
                upgradeManager.weatherCleaner.Upgradelevel
            );
        }

        public void LoadFromSave()
        {
            if(upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();
            
            StreamReader reader = new StreamReader(GetSaveFile());
            string json = reader.ReadLine();
            reader.Close();

            saveFile = new SaveFile();
            JsonUtility.FromJsonOverwrite(json, saveFile);

            upgradeManager.postman.Upgradelevel = saveFile.postmanLevel;
            upgradeManager.biggerPockets.Upgradelevel = saveFile.biggerPocketsLevel;
            upgradeManager.scrapPurifier.Upgradelevel = saveFile.scrapPurifierLevel;
            upgradeManager.scrapMagnet.Upgradelevel = saveFile.scrapMagnetLevel;
            upgradeManager.weatherCleaner.Upgradelevel = saveFile.weatherCleanerLevel;
        }        
    }

    public class SaveFile : MonoBehaviour
    {
        public int postmanLevel;
        public int biggerPocketsLevel;
        public int scrapPurifierLevel;
        public int scrapMagnetLevel;
        public int weatherCleanerLevel;

        public SaveFile() { }
        
        public SaveFile(int postmanLevel, int biggerPocketsLevel, int scrapPurifierLevel, int scrapMagnetLevel, int weatherCleanerLevel)
        {
            this.postmanLevel = postmanLevel;
            this.biggerPocketsLevel = biggerPocketsLevel;
            this.scrapPurifierLevel = scrapPurifierLevel;
            this.scrapMagnetLevel = scrapMagnetLevel;
            this.weatherCleanerLevel = weatherCleanerLevel;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [HarmonyPatch(typeof(DeleteFileButton))]
    class DeleteFileButtonPatch
    {
        [HarmonyPatch("DeleteFile")]
        [HarmonyPostfix]
        static void DeleteFilePatch(DeleteFileButton __instance)
        {
            if(__instance.fileToDelete < 0 || __instance.fileToDelete > 2) return;
            string fileToDelete = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves", $"LCSaveFile{__instance.fileToDelete + 1}.json");
            
            Debug.Log($"MoreUpgrades: Deleting file: {__instance.fileToDelete + 1}");
            Debug.Log($"MoreUpgrades: Path to file: {fileToDelete}");

            if(File.Exists(fileToDelete))File.Delete(fileToDelete);
        }
    }
}