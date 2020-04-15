using System;
using HarmonyLib;

namespace Clarity
{
    public static class ClarityPatch
    {
        [HarmonyPatch(typeof(Player), "Start")]
        public static class Player_Start
        {
            public static void Postfix()
            {
                Log.Initialize();
                Log.WriteLine("Hello World");
            }
        }
    }
}
