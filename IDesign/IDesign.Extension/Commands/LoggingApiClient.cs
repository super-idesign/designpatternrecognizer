﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IDesign.Extension.Model;
using Newtonsoft.Json;
using Action = IDesign.Extension.Model.Action;

namespace IDesign.Extension.Commands
{
    public static class LoggingApiClient
    {
        private static readonly HttpClient client = new HttpClient() { BaseAddress = new Uri("https://localhost:5001/api/Logging/") };

        public static async Task<HttpResponseMessage> PostActionAsync(EnvDTE.vsBuildAction vsAction)
        {
            Action action = null;
            switch (vsAction)
            {
                case EnvDTE.vsBuildAction.vsBuildActionBuild:
                    action = new Action("Build");
                    break;
                case EnvDTE.vsBuildAction.vsBuildActionRebuildAll:
                    action = new Action("RebuildAll");
                    break;
                case EnvDTE.vsBuildAction.vsBuildActionClean:
                    action = new Action("Clean");
                    break;
                case EnvDTE.vsBuildAction.vsBuildActionDeploy:
                    action = new Action("Deploy");
                    break;
            }
            string actionJson = JsonConvert.SerializeObject(action);
            var content = new StringContent(actionJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("Actions", content);
            response.EnsureSuccessStatusCode();

            return response;
        }

    }
}
