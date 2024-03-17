using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace MoreUpgrades
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        static UpgradeManager upgradeManager;
        public static RaycastHit hit;
        static int firstEmptyItemSlot;

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
            if (upgradeManager == null)
                upgradeManager = GameObject.Find("MoreUpgrades.Upgrademanager").GetComponent<UpgradeManager>();

            if (___currentlyGrabbingObject == null) return;

            if (hit.collider.gameObject.layer != 8 && hit.collider.gameObject.tag == "PhysicsProp")
            {
                if (___currentlyGrabbingObject.grabbable && firstEmptyItemSlot != -1)
                {
                    float weightAddition = Mathf.Clamp(___currentlyGrabbingObject.itemProperties.weight - 1f, 0f, 10f) * (upgradeManager.postman.Upgradelevel / 10f);
                    float originalWeightAdded = Mathf.Clamp(___currentlyGrabbingObject.itemProperties.weight - 1f, 0f, 10f);

                    upgradeManager.postman.UpdateWeightOffset(___currentlyGrabbingObject.itemProperties.weight, false);
                    upgradeManager.postman.ReduceWeight(___currentlyGrabbingObject.itemProperties.weight);
                }
            }
        }

        [HarmonyPatch("SetObjectAsNoLongerHeld")]
        [HarmonyPostfix]
        static void SetObjectAsNoLongerHeldPatch(PlayerControllerB __instance)
        {
            GrabbableObject dropObject = __instance.currentlyHeldObjectServer;

            float weightReduction = Mathf.Clamp(dropObject.itemProperties.weight - 1f, 0f, 10f) * (upgradeManager.postman.Upgradelevel / 10f);
            float originalWeightReduced = Mathf.Clamp(dropObject.itemProperties.weight - 1f, 0f, 10f);

            upgradeManager.postman.UpdateWeightOffset(dropObject.itemProperties.weight, true);
            upgradeManager.postman.AddWeigth(dropObject.itemProperties.weight);
        }

        [HarmonyPatch("FirstEmptyItemSlot")]
        [HarmonyPostfix]
        static void FirstEmptyItemSlotPatch(ref int __result)
        {
            firstEmptyItemSlot = __result;
        }
    }
}
