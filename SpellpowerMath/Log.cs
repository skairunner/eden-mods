using System;
using System.IO;
using UnityEngine;

namespace Clarity
{
    public static class Log
    {
        private static StreamWriter log;

        private static string _logFileLocation;
        public static string LogFileLocation
        {
            get => _logFileLocation;
            set
            {
                _logFileLocation = value;
                log = File.CreateText(value);
            }
        }

        public static void Initialize()
        {
            if (log == null)
            {
                var ass = System.Reflection.Assembly.GetEntryAssembly();
                var location = ass == null ? @"C:\coding\eden-mods" : ass.Location;
                LogFileLocation = Path.Combine(location, "output.log");
            }
        }

        public static void WriteLine(string line)
        {
            if (log == null)
            {
                Initialize();
            }

            var date = DateTime.Now.ToString();
            log.WriteLine($"[{date}] {line}");
            log.Flush();
        }
    }
}