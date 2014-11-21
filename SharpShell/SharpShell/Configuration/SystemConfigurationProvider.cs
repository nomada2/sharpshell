﻿using System;
using System.Security.AccessControl;
using System.Threading;
using Microsoft.Win32;

namespace SharpShell.Configuration
{
    /// <summary>
    /// Represents SharpShell System Configuration on the local machine.
    /// This class can be used to read and write the system configuration.
    /// </summary>
    public static class SystemConfigurationProvider
    {
        /// <summary>
        /// The system configuration data itself.
        /// </summary>
        private static readonly Lazy<SystemConfiguration> systemConfiguration;

        /// <summary>
        /// Initializes the <see cref="SystemConfigurationProvider"/> class.
        /// </summary>
        static SystemConfigurationProvider()
        {
            //  Load configuration lazily when needed.
            systemConfiguration = new Lazy<SystemConfiguration>(LoadConfiguration, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns>The system configuration.</returns>
        private static SystemConfiguration LoadConfiguration()
        {
            //  Create the configuration object with the default configuration 
            //  which will be returned if no config is present in the system.
            var config = new SystemConfiguration
            {
                IsConfigurationPresent = false,
                LoggingMode = LoggingMode.Disabled
            };

            //  Open the SharpShell configuration key.

            //  Open the local machine.
            using (var localMachineBaseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
            {
                //  Open the SharpShell Key.
                using (var sharpShellKey = localMachineBaseKey.OpenSubKey(@"Software\SharpShell", 
                    RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey))
                {
                    //  If we don't have the key, return the default config.
                    if (sharpShellKey == null)
                        return config;

                    //  Load the config.
                    config.IsConfigurationPresent = true;
                    config.LoggingMode = (LoggingMode)sharpShellKey.GetValue("LoggingMode", LoggingMode.Disabled);
                    config.LogPath = (string)sharpShellKey.GetValue("LogPath", null);
                }
            }

            //  Return the config.
            return config;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public static SystemConfiguration Configuration
        {
            get { return systemConfiguration.Value; }
        }
    }
}
