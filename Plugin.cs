using System;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using LovenseConnectAPI;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace GorillaTag_Lovense
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    internal class Plugin : BaseUnityPlugin // Thank you plague for making the amazing code for this to be possible (https://github.com/MistressPlague/LovenseConnectCSharp/blob/main/LovenseConnectAPI/Main.cs)
    {
        // this prob doesnt work but if it does cool ig. made it at 1 am
        private static string toyID;
        // private ConfigEntry<int> intensityConfig;

        void Awake()
        {
            // GorillaTagger.OnPlayerSpawned(OnGameInitialized);
            HarmonyPatches.ApplyHarmonyPatches();
            _ = AssignToy();

            // intensity controls that ill finish later (im very lazy)
            /*intensityConfig = Config.Bind("Settings", "VibrationIntensity", 5,
                "Intensity of toy vibration");*/

            Debug.Log("Lovense thing works. have fun with your buttplug or whatever");
        }

        private async System.Threading.Tasks.Task AssignToy()
        {
            List<Main.LovenseToy> toys = await Main.GetToys("http://127.0.0.1:30010");

            if (toys != null && toys.Count > 0)
            {
                toyID = toys[0].ToyID;
                Debug.Log($"Assigned Lovense Toy: {toys[0].ToyNickName} ({toyID})");
            }
            else
            {
                Debug.Log("No Lovense toys found!");
            }
        }

        public static void Vibrate(int amount, int duration)
        {
            if (!string.IsNullOrEmpty(toyID))
            {
                _ = Main.VibrateToyDuration("http://127.0.0.1:30010", toyID, amount, duration);
            }
        }

        // not so sure about this part, ill make it work later when i figure it out
        /*[HarmonyPatch(typeof(GorillaTagger), "UpdateColor", new Type[] { typeof(float), typeof(float), typeof(float) })]
        public class lovense_UpdateColor
        {
            [HarmonyPostfix]
            public static void Postfix(GorillaTagger __instance)
            {

            }
        }

        [HarmonyPatch(typeof(GorillaTagger), "ApplyStatusEffect", new Type[] { typeof(GorillaTagger.StatusEffect), typeof(float) })]
        public class lovense_ApplyStatusEffect
        {
            [HarmonyPostfix]
            public static void Postfix(GorillaTagger __instance, GorillaTagger.StatusEffect newStatus, float duration)
            {

            }
        }*/

        [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "IsHandTouching", new Type[] { typeof(bool) })]
        public class lovense_HandTap
        {
            [HarmonyPostfix]
            public static void Postfix(bool forLeftHand, bool __result)
            {
                if (!__result) return;
                Vibrate(5, 300);
            }
        }
    }
}