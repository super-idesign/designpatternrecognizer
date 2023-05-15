﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using PatternPal.Extension.Grpc;
using EnvDTE;
using EnvDTE80;
using PatternPal.Protos;
using System.Threading;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;


namespace PatternPal.Extension.Commands
{
    /// <summary>
    /// A static class which is responsible for subscribing logged events.
    /// </summary>
    public static class SubscribeEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static PatternPalExtensionPackage _package;

        private static DTE _dte;

        private static Solution _dteSolution;

        private static string _sessionId;

        private static string _pathToUserDataFolder;

        private static CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes the preparation for the subscription of the logged events. 
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="package"> The PatternPal package itself. </param>
        public static void Initialize(
            DTE dte,
            PatternPalExtensionPackage package, CancellationToken cancellationToken)
        {
            _dte = dte;
            ThreadHelper.ThrowIfNotOnUIThread();
            _package = package;
            _pathToUserDataFolder = Path.Combine(_package.UserLocalDataPath.ToString(), "Extensions", "Team PatternPal",
                "PatternPal.Extension", "UserData");
            _cancellationToken = cancellationToken;
            SaveSubjectId();


            // These events are not handled with an event listener, and thus need to be checked separately whether logging is enabled.
            if (_package.DoLogData)
            {
                OnSessionStart();
                OnProjectOpen();
            }
        }

        /// <summary>
        /// Subscribes the event handlers for logging data.
        /// </summary>
        public static async Task SubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            _dteSolution = _dte.Solution;
            // Code that interacts with UI elements goes here
            _dte.Events.BuildEvents.OnBuildDone += OnCompileDone;
            _dte.Events.SolutionEvents.Opened += OnProjectOpen;
            _dte.Events.SolutionEvents.BeforeClosing += OnProjectClose;
            _dte.Events.DebuggerEvents.OnEnterDesignMode += OnRunProgram; // TODO: Not triggering...
        }


        /// <summary>
        /// Unsubscribes the event handlers for logging data.
        /// </summary>
        public static async Task UnsubscribeEventHandlersAsync()
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync(_cancellationToken);
            _dte.Events.BuildEvents.OnBuildDone -= OnCompileDone;
            _dte.Events.SolutionEvents.BeforeClosing -= OnProjectClose;
        }


        #region Events

        /// <summary>
        /// The event handler for handling the Compile Event. The given parameters are part of the event listener input and among other things necessary to give the right output message.
        /// </summary>
        /// <param name="Scope"></param>
        /// <param name="Action"></param>
        private static void OnCompileDone(
            vsBuildScope Scope,
            vsBuildAction Action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string outputMessage;
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                outputMessage = string.Format("Build {0} with errors. See the output window for details.",
                    Action.ToString());

                // As the compilation led to an error, a separate  log is sent with the compile error diagnostics
                // and the specific code section in which the compilation error occurred
            }
            else
            {
                outputMessage = string.Format("Build {0} succeeded.", Action.ToString());
            }

            LogEventRequest request = CreateStandardLog();
            string pathSolutionFullName = _dte.Solution.FullName;
            string pathSolutionFile = _dte.Solution.FileName;
            // Distinguish a sln file or just a csproj file to be opened
            if (pathSolutionFullName == "")
            {
                // A csproj file was opened
                Array startupProjects = (Array)_dte.Solution.SolutionBuild.StartupProjects;
                request.CodeStateSection = (string)startupProjects.GetValue(0);
            }
            else
            {
                string pathSolutionDirectory = Path.GetDirectoryName(_dte.Solution.FullName);

                request.CodeStateSection = GetRelativePath(pathSolutionDirectory, pathSolutionFile);
            }

            request.EventType = EventType.EvtCompile;
            request.CompileResult = outputMessage;

            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);

            // When the compilation was an error, a Compile Error log needs to be send.
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                Window window = _dte.Windows.Item(WindowKinds.vsWindowKindErrorList);
                ErrorList errorListWindow = (ErrorList)window.Selection;

                for (int i = 1; i <= errorListWindow.ErrorItems.Count; i++)
                {
                    ErrorItem errorItem = errorListWindow.ErrorItems.Item(i);
                    string errorType = errorItem.ErrorLevel.ToString();
                    string errorMessage = errorItem.Description;
                    string errorSourceLocation = String.Concat("Text:" + errorItem.Line);

                    // The relative path of the source file for the error is required for the ProgSnap format
                    string projectFolderName =
                        Path.GetDirectoryName(_dte.Solution.Projects.Item(errorItem.Project).FullName);
                    string codeStateSection = GetRelativePath(projectFolderName, errorItem.FileName);

                    OnCompileError(request, errorType, errorMessage, errorSourceLocation, codeStateSection);
                }
            }
        }

        /// <summary>
        /// The event handler for handling the Session.Start Event. When a new session starts, a (new) sessionID is generated .
        /// </summary>
        private static void OnSessionStart()
        {
            _sessionId = Guid.NewGuid().ToString();

            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionStart;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        /// <summary>
        /// The event handler for handling the Session.End Event. This method needs to be called from the package logic, hence the given internal modifier.
        /// </summary>
        internal static void OnSessionEnd()
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtSessionEnd;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        /// <summary>
        /// The event handler for handling the Compile.Error Event. This is strictly a complementary event to the regular Compile event for each separate error.
        /// </summary>
        private static void OnCompileError(LogEventRequest parent, string compileMessagetype, string compileMessageData,
            string sourceLocation, string codeStateSection)
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtCompileError;
            request.ParentEventId = parent.EventId;
            request.CompileMessageType = compileMessagetype;
            request.CompileMessageData = compileMessageData;
            request.SourceLocation = sourceLocation;
            request.CodeStateSection = codeStateSection;
            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
        }

        /// <summary>
        /// The event handler for handling the Project.Open Event.
        /// </summary>
        private static void OnProjectOpen()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtProjectOpen;
            LogEachProject(request);
        }

        /// <summary>
        /// The event handler for handling the Project.Close Event.
        /// </summary>
        private static void OnProjectClose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtProjectClose;
            LogEachProject(request);
        }

        /// <summary>
        /// The event handler for handling the Run.Program Event.
        /// </summary>
        private static void OnRunProgram(dbgEventReason reason)
        {
            LogEventRequest request = CreateStandardLog();
            request.EventType = EventType.EvtRunProgram;

            if (reason == dbgEventReason.dbgEventReasonExceptionThrown ||
                reason == dbgEventReason.dbgEventReasonExceptionNotHandled)
            {
                request.ExecutionResult = ExecutionResult.ExtError;
            }

            LogProviderService.LogProviderServiceClient client =
                new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
            LogEventResponse response = client.LogEvent(request);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
        }

        #endregion

        /// <summary>
        /// Creates a standard log format with set fields that are always generated. Consequently, it is used by all other specific logs.
        /// </summary>
        /// <returns>The standard log format.</returns>
        private static LogEventRequest CreateStandardLog()
        {
            return new LogEventRequest
            {
                EventId = Guid.NewGuid().ToString(),
                SubjectId = GetSubjectId(),
                SessionId = _sessionId
            };
        }

        /// <summary>
        /// Saves the SubjectId of the user, if not set already, as a GUID.
        /// It creates a folder and a file for this at the UserLocalDataPath in the PatternPal Extension folder as this place is unique per user.
        /// </summary>
        private static void SaveSubjectId()
        {
            // A SubjectID is only ever generated once per user. If the directory already exists, the SubjectID was already set.
            if (Directory.Exists(_pathToUserDataFolder))
            {
                return;
            }

            Directory.CreateDirectory(_pathToUserDataFolder);
            string fileName = "subjectid.txt";
            string filePath = Path.Combine(_pathToUserDataFolder, fileName);
            string fileContents = Guid.NewGuid().ToString();
            File.WriteAllText(filePath, fileContents);
        }

        /// <summary>
        /// Reads the SubjectID from a local file.
        /// </summary>
        /// <returns>The SubjectID - It returns the contents of the local file.</returns>
        private static string GetSubjectId()
        {
            // A SubjectID is only ever generated once per user. If the directory already exists, the SubjectID was already set.

            string fileName = "subjectid.txt";
            string filePath = Path.Combine(_pathToUserDataFolder, fileName);
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Gets the relative path when given an absolute path and a filename.
        /// </summary>
        /// <param name="relativeTo"> The absolute path to the root folder of the solution or project</param>
        /// <param name="path">The absolute path to the specific file</param>
        /// <returns></returns>
        public static string GetRelativePath(string relativeTo, string path)
        {
            Uri uri = new Uri(relativeTo);
            string rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString())
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
            {
                rel = $".{Path.DirectorySeparatorChar}{rel}";
            }

            return rel;
        }

        /// <summary>
        /// Cycles through all active projects and log the given event for each of these projects.
        /// </summary>
        private static void LogEachProject(LogEventRequest request)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Projects projects = _dte.Solution.Projects;

            foreach (Project project in projects)
            {
                if (project == null)
                {
                    continue;
                }

                request.ProjectId = project.FullName;

                LogProviderService.LogProviderServiceClient client =
                    new LogProviderService.LogProviderServiceClient(GrpcHelper.Channel);
                LogEventResponse response = client.LogEvent(request);
            }
        }

    }
}
