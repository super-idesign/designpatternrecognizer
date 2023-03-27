﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternPal.Extension.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Grpc.Core;
using PatternPal.Extension.Grpc;
using StreamJsonRpc;
using EnvDTE;
using PatternPal.Extension.UserControls;
using PatternPal.Extension.ViewModels;
using EnvDTE80;
using PatternPal.Protos;

namespace PatternPal.Extension
{
    public static class SubscribeBuildEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static PatternPalExtensionPackage package;

        public static void Initialize(EnvDTE.DTE dte, PatternPalExtensionPackage package)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
            SubscribeBuildEvents.package = package;
        }

        private static async void BuildEvents_OnBuildDone(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            if (package.DoLogData)
            {
                try { await LoggingApiClient.PostActionAsync(Action); }
                catch (Exception ex) { }

                //Request to service
                LogBuildEventRequest request = new LogBuildEventRequest { SubjectId = "iets" };

                LoggingService.LoggingServiceClient client =
                    new LoggingService.LoggingServiceClient(GrpcChannelHelper.Channel);
                  LogBuildEventResponse response = client.LogBuildEvent(request);
                   
                
            
            }
        }
    }
}
