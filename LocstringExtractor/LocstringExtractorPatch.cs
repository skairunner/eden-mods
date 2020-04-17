using System;
using System.IO;
using HarmonyLib;
using I2.Loc;
using UnityEngine;

namespace LocstringExtractor
{
    public static class LocstringExtractorPatch
    {
        [HarmonyPatch(typeof(Player), "Start")]
        public static class Player_Start
        {
            public static void Postfix()
            {
                var userdir = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                var output = File.OpenWrite(Path.Combine(userdir, "loc.txt"));
                var writer = new StreamWriter(output);
                // just ransack the entire localization file
                foreach (var term in LocalizationManager.GetTermsList())
                {
                    var translation = LocalizationManager.GetTermData(term).GetTranslation(0);
                    writer.WriteLine($"{term}\t{translation}");
                }

                output.Flush();
                output.Close();
            }
        }
    }
}
