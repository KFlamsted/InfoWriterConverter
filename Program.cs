using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace InfoWriterConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("InfoWriter OBS Plugin Output Converter");
            Console.WriteLine("=====================================");

            string inputFile = "input.txt";
            string outputFile = "output.txt";

            // Parse command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-i":
                    case "--input":
                        if (i + 1 < args.Length)
                        {
                            inputFile = args[i + 1];
                            i++; // Skip the next argument as it's the value
                        }
                        else
                        {
                            Console.WriteLine("✗ Error: -i flag requires a file path.");
                            ShowUsage();
                            Environment.Exit(1);
                        }
                        break;
                    case "-o":
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputFile = args[i + 1];
                            i++; // Skip the next argument as it's the value
                        }
                        else
                        {
                            Console.WriteLine("✗ Error: -o flag requires a file path.");
                            ShowUsage();
                            Environment.Exit(1);
                        }
                        break;
                    case "-h":
                    case "--help":
                        ShowUsage();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine($"✗ Error: Unknown argument '{args[i]}'");
                        ShowUsage();
                        Environment.Exit(1);
                        break;
                }
            }

            try
            {
                ConvertInfoWriterOutput(inputFile, outputFile);
                Console.WriteLine($"✓ Successfully converted '{inputFile}' to '{outputFile}'");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"✗ Error: Input file '{inputFile}' not found.");
                ShowUsage();
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  InfoWriterConverter [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -i, --input <file>    Input file path (default: input.txt)");
            Console.WriteLine("  -o, --output <file>   Output file path (default: output.txt)");
            Console.WriteLine("  -h, --help           Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  InfoWriterConverter");
            Console.WriteLine("  InfoWriterConverter -i my-notes.txt -o converted.txt");
            Console.WriteLine("  InfoWriterConverter --input recording.txt --output chapters.txt");
        }

        static void ConvertInfoWriterOutput(string inputFile, string outputFile)
        {
            var lines = File.ReadAllLines(inputFile);
            var outputLines = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                string currentLine = lines[i].Trim();

                // Skip empty lines and lines that don't start with EVENT: or HOTKEY:
                if (string.IsNullOrWhiteSpace(currentLine) || 
                    (!currentLine.StartsWith("EVENT:") && !currentLine.StartsWith("HOTKEY:")))
                {
                    continue;
                }

                // Check if we have a timestamp line following this event/hotkey line
                if (i + 1 >= lines.Length)
                {
                    Console.WriteLine($"Warning: Found {currentLine} without timestamp on line {i + 1}");
                    continue;
                }

                string timestampLine = lines[i + 1].Trim();
                
                // Extract timestamp (format: "0:MM:SS Record Time Marker...")
                var timestampMatch = Regex.Match(timestampLine, @"^(\d+:\d{2}:\d{2})");
                if (!timestampMatch.Success)
                {
                    Console.WriteLine($"Warning: Could not parse timestamp from line {i + 2}: {timestampLine}");
                    continue;
                }

                string timestamp = timestampMatch.Groups[1].Value;

                // Handle different types of events
                if (currentLine.StartsWith("EVENT:"))
                {
                    if (currentLine.Contains("START RECORDING") || currentLine.Contains("STOP RECORDING"))
                    {
                        // Skip START/STOP RECORDING events entirely
                        i += 2; // Skip the timestamp line and potential empty line
                        continue;
                    }
                    else if (currentLine.Contains("RECORDING PAUSED"))
                    {
                        outputLines.Add($"{timestamp} RECORDING PAUSED");
                        i += 2; // Skip the timestamp line and potential empty line
                        continue;
                    }
                    else if (currentLine.Contains("RECORDING RESUMED"))
                    {
                        outputLines.Add($"{timestamp} RECORDING RESUMED");
                        i += 2; // Skip the timestamp line and potential empty line
                        continue;
                    }
                }

                // Handle HOTKEY events
                if (currentLine.StartsWith("HOTKEY:"))
                {
                    // Extract note key between "HOTKEY:" and "@"
                    var noteKeyMatch = Regex.Match(currentLine, @"HOTKEY:([^@]+)@");
                    if (noteKeyMatch.Success)
                    {
                        string noteKey = noteKeyMatch.Groups[1].Value.Trim();
                        outputLines.Add($"{timestamp} {noteKey}");
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not parse note key from line {i + 1}: {currentLine}");
                    }
                }

                // Skip the timestamp line (we've already processed it)
                i++;
                
                // Skip the next empty line if it exists
                if (i + 1 < lines.Length && string.IsNullOrWhiteSpace(lines[i + 1]))
                {
                    i++;
                }
            }

            File.WriteAllLines(outputFile, outputLines);
        }
    }
}
