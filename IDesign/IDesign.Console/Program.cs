﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDesign.Core;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;
using NDesk.Options;

namespace IDesign.ConsoleApp
{
    internal class Program
    {
        /// <summary>
        ///     Prints the parses commandline input and starts the runner
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        private static void Main(string[] args)
        {
            var designPatternsList = RecognizerRunner.designPatterns;
            var showHelp = false;
            var selectedFiles = new List<string>();
            var selectedPatterns = new List<DesignPattern>();
            var fileManager = new FileManager();
            var recognizerRunner = new RecognizerRunner();

            if (args.Length <= 0)
            {
                Console.WriteLine("No arguments or files specified please confront --help");
                Console.ReadKey();
                return;
            }

            var options = new OptionSet
            {
                {"h|help", "shows this message and exit", v => showHelp = v != null}
            };

            //Add design patterns as specifiable option
            foreach (var pattern in designPatternsList)
                options.Add(pattern.Name, "includes " + pattern.Name, v => selectedPatterns.Add(pattern));

            var arguments = options.Parse(args);

            if (showHelp)
            {
                ShowHelp(options);
                Console.ReadKey();
                return;
            }

            selectedFiles = (from a in arguments where a.EndsWith(".cs") && a.Length > 3 select a).ToList();

            foreach (var arg in arguments)
                if (Directory.Exists(arg))
                    selectedFiles.AddRange(fileManager.GetAllCsFilesFromDirectory(arg));

            if (selectedFiles.Count == 0)
            {
                Console.WriteLine("No files specified!");
                Console.ReadKey();
                return;
            }

            //When no specific pattern is chosen, select all
            if (selectedPatterns.Count == 0) selectedPatterns = designPatternsList;

            Console.WriteLine("Selected files:");

            foreach (var file in selectedFiles) Console.WriteLine(" - " + file);

            Console.WriteLine("\nSelected patterns:");

            foreach (var pattern in selectedPatterns) Console.WriteLine(" - " + pattern.Name);

            recognizerRunner.OnProgressUpdate += (sender, progress) =>
                DrawTextProgressBar(progress.Status, progress.CurrentPercentage, 100);

            recognizerRunner.CreateGraph(selectedFiles);
            var results = recognizerRunner.Run(selectedPatterns);

            PrintResults(results);

            Console.ReadKey();
        }

        /// <summary>
        ///     Prints a message on how to use this program and all possible options
        /// </summary>
        /// <param name="options">All commandline options</param>
        private static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: idesign [INPUT] [OPTIONS]");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        ///     Prints results of RecognizerRunner.Run
        /// </summary>
        /// <param name="results">A List of RecognitionResult</param>
        private static void PrintResults(List<RecognitionResult> results)
        {
            Console.WriteLine("\nResults:");

            for (var i = 0; i < results.Count; i++)
            {
                Console.Write($"{i}) {results[i].EntityNode.GetName()} | {results[i].Pattern.Name}: ");

                PrintScore(results[i].Result.GetScore());

                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var result in results[i].Result.GetResults())
                    PrintResult(result, 1);

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void PrintResult(ICheckResult result, int depth)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var symbol = "X";

            if (result.GetFeedbackType() == FeedbackType.SemiCorrect)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                symbol = "-";
            }

            if (result.GetFeedbackType() == FeedbackType.Correct)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                symbol = "✓";
            }

            Console.WriteLine(new string('\t', depth) + symbol + $" {result.GetMessage()}");

            foreach (var child in result.GetChildFeedback())
            {
                PrintResult(child, depth + 1);
            } 
        }

        /// <summary>
        ///     Prints the score with a color depending on the score
        /// </summary>
        /// <param name="score"></param>
        private static void PrintScore(int score)
        {
            if (score <= 33)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (score <= 66)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(score);

            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void DrawTextProgressBar(string stepDescription, int progress, int total)
        {
            int totalChunks = 30;

            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = totalChunks + 1;
            Console.Write("]"); //end
            Console.CursorLeft = 1;

            double pctComplete = Convert.ToDouble(progress) / total;
            int numChunksComplete = Convert.ToInt16(totalChunks * pctComplete);

            //draw completed chunks
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("".PadRight(numChunksComplete));

            //draw incomplete chunks
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("".PadRight(totalChunks - numChunksComplete));

            //draw totals
            Console.CursorLeft = totalChunks + 5;
            Console.BackgroundColor = ConsoleColor.Black;

            string output = progress.ToString() + " of " + total.ToString();
            Console.Write(output.PadRight(15) + stepDescription); //pad the output so when changing from 3 to 4 digits we avoid text shifting
        }
    }
}
