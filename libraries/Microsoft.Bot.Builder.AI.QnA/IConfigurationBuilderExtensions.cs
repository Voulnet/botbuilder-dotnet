﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Bot.Builder.AI.QnA
{
    /// <summary>
    /// Extension methods for QnA.
    /// </summary>
    public static class IConfigurationBuilderExtensions
    {
        /// <summary>
        /// Setup configuration to utilize the settings file generated by bf qnamaker:build.
        /// </summary>
        /// <remarks>
        /// This will pick up --environment as the environment to target.  If environment is Development it will use the name of the logged in user.
        /// This will pick up --root as the root folder to run in.  
        /// </remarks>
        /// <param name="builder">Configuration builder to modify.</param>
        /// <param name="botRoot">Root folder for bot assets.</param>
        /// <param name="qnaRegion">Language region for QnA, e.g, en-us.</param>
        /// <param name="environment">Running enviroment, e.g, development or alias.</param>
        /// <returns>Modified configuration builder.</returns>
        public static IConfigurationBuilder UseQnAMakerSettings(this IConfigurationBuilder builder, string botRoot, string qnaRegion, string environment)
        {
            var settings = new Dictionary<string, string>();
            settings["BotRoot"] = botRoot;
            builder.AddInMemoryCollection(settings);
            if (environment == "Development")
            {
                environment = Environment.UserName;
            }

            var di = new DirectoryInfo(botRoot);
 
            foreach (var file in di.GetFiles($"qnamaker.settings.{environment.ToLowerInvariant()}.{qnaRegion}.json", SearchOption.AllDirectories))
            {
                var relative = file.FullName.Substring(di.FullName.Length);
                if (!relative.Contains("bin\\") && !relative.Contains("obj\\"))
                {
                    builder.AddJsonFile(file.FullName, optional: false, reloadOnChange: true);
                }
            }

            return builder;
        }
    }
}
