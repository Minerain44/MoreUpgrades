using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Unity.Netcode;

namespace MoreUpgrades
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        static UpgradeManger upgradeManger;
        public static RaycastHit hit;

        static int firstEmptyItemSlot;
        // [HarmonyPatch("Start")]
        // [HarmonyPostfix]
        // static void StartPatch()
        // {
        //     upgradeManger = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManger>();
        // }

        [HarmonyPatch("BeginGrabObject")]
        [HarmonyPrefix]
        static void BeginGrabObjectPrePatch(PlayerControllerB __instance, ref int ___interactableObjectsMask)
        {
            Ray interactRay = new Ray(__instance.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            Physics.Raycast(interactRay, out hit, __instance.grabDistance, ___interactableObjectsMask);
        }

        [HarmonyPatch("BeginGrabObject")]
        [HarmonyPostfix]
        static void BeginGrabObjectPostPatch(PlayerControllerB __instance, ref GrabbableObject ___currentlyGrabbingObject, ref int ___interactableObjectsMask)
        {
            if (upgradeManger == null)
                upgradeManger = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManger>();

            if (___currentlyGrabbingObject == null) return;

            // Ray interactRay = new Ray(__instance.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            // Physics.Raycast(interactRay, out hit, __instance.grabDistance, ___interactableObjectsMask);// && hit.collider.gameObject.layer != 8 && hit.collider.tag == "PhysicsProp"

            Debug.Log("---------------------------------Raycast Info----------------------------------------------------");
            Debug.Log($"MoreUpgrades: Hit Raycast: {hit}");
            Debug.Log($"MoreUpgrades: Object Raycast: {hit.collider.gameObject}");
            Debug.Log($"MoreUpgrades: Object Raycast layer: {hit.collider.gameObject.layer} Target: not 8");
            Debug.Log($"MoreUpgrades: Object Raycast Tag: {hit.collider.tag} Target: 'PhysicsProp'");
            Debug.Log("----------------------------------Object Info----------------------------------------------------");
            Debug.Log($"MoreUpgrades: Object: {___currentlyGrabbingObject}");
            Debug.Log($"MoreUpgrades: Object Layer: {___currentlyGrabbingObject.gameObject.layer} Tag: {___currentlyGrabbingObject.gameObject.tag}"); // Object layer = 6, ne
            Debug.Log("------------------------------------End Info-----------------------------------------------------");

            if (hit.collider.gameObject.layer != 8 && hit.collider.gameObject.tag == "PhysicsProp")
            {
                if (___currentlyGrabbingObject.grabbable && firstEmptyItemSlot != -1)
                {
                    float weightReduction = Mathf.Clamp(___currentlyGrabbingObject.itemProperties.weight - 1f, 0f, 10f) * (upgradeManger.postman.Upgradelevel / 10f);
                    float originalWeightAdded = Mathf.Clamp(___currentlyGrabbingObject.itemProperties.weight - 1f, 0f, 10f);

                    Debug.Log($"MoreUpgrades: Adding weight from picking up an object");
                    // Debug.Log($"MoreUpgrades: Curent Carried Weight: {__instance.carryWeight}");
                    // Debug.Log($"MoreUpgrades: Object: {___currentlyGrabbingObject}");
                    // Debug.Log($"MoreUpgrades: Total Object Weight: {___currentlyGrabbingObject.itemProperties.weight}");
                    // Debug.Log($"MoreUpgrades: Added Object Weight: {originalWeightAdded}");
                    // Debug.Log($"MoreUpgrades: Weight Reduction: {weightReduction}");

                    weightReduction = Mathf.Clamp(___currentlyGrabbingObject.itemProperties.weight - 1f, 0f, 10f) * (upgradeManger.postman.Upgradelevel / 10f);
                    __instance.carryWeight -= weightReduction;
                }
            }
        }

        [HarmonyPatch("SetObjectAsNoLongerHeld")]
        [HarmonyPostfix]
        static void SetObjectAsNoLongerHeldPatch(PlayerControllerB __instance)
        {
            GrabbableObject dropObject = __instance.currentlyHeldObjectServer;

            float weightAddition = Mathf.Clamp(dropObject.itemProperties.weight - 1f, 0f, 10f) * (upgradeManger.postman.Upgradelevel / 10f);
            float originalWeightAdded = Mathf.Clamp(dropObject.itemProperties.weight - 1f, 0f, 10f);

            Debug.Log($"MoreUpgrades: Removing weight from dropping an object");
            // Debug.Log($"MoreUpgrades: Curent Carried Weight: {__instance.carryWeight}");
            // Debug.Log($"MoreUpgrades: Object: {dropObject}");
            // Debug.Log($"MoreUpgrades: Total Object Weight: {dropObject.itemProperties.weight}");
            // Debug.Log($"MoreUpgrades: Removed Object Weight: {originalWeightAdded}");
            // Debug.Log($"MoreUpgrades: Weight Addition: {weightAddition}");

            __instance.carryWeight += weightAddition;
        }

        [HarmonyPatch("FirstEmptyItemSlot")]
        [HarmonyPostfix]
        static void FirstEmptyItemSlotPatch(ref int __result)
        {
            firstEmptyItemSlot = __result;
        }
    }
}
