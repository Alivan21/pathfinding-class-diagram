using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services
{
    public class ExtractorService : IExtractorService
    {
        private readonly IFileService _fileService;
        private readonly object _lockObject = new object();

        public ExtractorService(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<(List<ClassDiagram> ClassDiagrams, List<Relationship> Relationships)> ExtractClassDiagramsAsync(
            List<string> filePaths,
            int threadCount,
            IProgress<(int Completed, int Total)> progress = null)
        {
            List<ClassDiagram> classDiagrams = new List<ClassDiagram>();
            List<Relationship> relationships = new List<Relationship>();
            int processedFiles = 0;
            int totalFiles = filePaths.Count;

            await Task.Run(() =>
            {
                ParallelOptions options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = threadCount
                };

                Parallel.ForEach(filePaths, options, filePath =>
                {
                    try
                    {
                        List<string> extractedInfo = ExtractClassAndMethodNames(filePath);

                        if (extractedInfo.Count > 0)
                        {
                            ClassDiagram diagram = CreateClassDiagram(extractedInfo, relationships);

                            lock (_lockObject)
                            {
                                classDiagrams.Add(diagram);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error processing file {filePath}: {ex.Message}");
                    }
                    finally
                    {
                        int completed = Interlocked.Increment(ref processedFiles);
                        progress?.Report((completed, totalFiles));
                    }
                });
            });

            return (classDiagrams, relationships);
        }

        private List<string> ExtractClassAndMethodNames(string filePath)
        {
            List<string> extractedInfo = new List<string>();

            string fileContent;
            using (StreamReader reader = new StreamReader(filePath))
            {
                fileContent = reader.ReadToEnd();
            }

            MatchCollection classMatches = Regex.Matches(fileContent, @"class\s+([^\s{]+)\s+(extends|implements)\s+([^\s{]+)\s*{");
            foreach (Match classMatch in classMatches)
            {
                string className = classMatch.Groups[1].Value;
                string relationType = classMatch.Groups[2].Value;
                string relatedClassName = classMatch.Groups[3].Value;
                string relation = (relationType == "extends") ? "Inheritance" : "Interface";

                extractedInfo.Add($"Class: {className}");
                extractedInfo.Add($"relation: {relatedClassName} : {relation}");
            }

            MatchCollection attributeMatches = Regex.Matches(fileContent, @"protected\s+\$(\w+)|private\s+\$(\w+)|public\s+\$(\w+)");
            foreach (Match attributeMatch in attributeMatches)
            {
                string accessModifier = "";
                if (attributeMatch.Groups[1].Success)
                {
                    accessModifier = "#";
                    extractedInfo.Add($"Attribute: {accessModifier} {attributeMatch.Groups[1].Value}");
                }
                else if (attributeMatch.Groups[2].Success)
                {
                    accessModifier = "-";
                    extractedInfo.Add($"Attribute: {accessModifier} {attributeMatch.Groups[2].Value}");
                }
                else if (attributeMatch.Groups[3].Success)
                {
                    accessModifier = "+";
                    extractedInfo.Add($"Attribute: {accessModifier} {attributeMatch.Groups[3].Value}");
                }
            }

            MatchCollection methodMatches = Regex.Matches(fileContent, @"(public|private|protected)\s+function\s+([^\s(]+)\s*\((.*?)\)\s*{");
            foreach (Match methodMatch in methodMatches)
            {
                string accessModifier;
                string modifier = methodMatch.Groups[1].Value;

                if (modifier == "public")
                    accessModifier = "+";
                else if (modifier == "private")
                    accessModifier = "-";
                else if (modifier == "protected")
                    accessModifier = "#";
                else
                    accessModifier = "";

                string methodName = methodMatch.Groups[2].Value;
                string parameters = methodMatch.Groups[3].Value;

                MatchCollection parameterMatches = Regex.Matches(parameters, @"(\$[a-zA-Z_\x7f-\xff][a-zA-Z0-9_\x7f-\xff]*)");
                List<string> methodParams = new List<string>();

                foreach (Match m in parameterMatches)
                {
                    methodParams.Add(m.Groups[1].Value.TrimStart('$'));
                }

                string methodParamsString = string.Join(", ", methodParams);

                extractedInfo.Add($"Method: {accessModifier} {methodName}({methodParamsString})");
            }

            // Save extracted info to file
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(filePath) + "_output.txt");
            _fileService.SaveTextOutput(outputFileName, extractedInfo);

            return extractedInfo;
        }

        /// <summary>
        /// Creates a class diagram from extracted information
        /// </summary>
        private ClassDiagram CreateClassDiagram(List<string> extractedInfo, List<Relationship> relationships)
        {
            string className = extractedInfo.FirstOrDefault(info => info.StartsWith("Class:"))?.Substring(7);
            string relation = extractedInfo.FirstOrDefault(info => info.StartsWith("relation:"))?.Substring(10);

            if (!string.IsNullOrEmpty(relation))
            {
                string[] relationSplit = relation.Split(new[] { " : " }, StringSplitOptions.None);
                if (relationSplit.Length == 2)
                {
                    string relatedClassName = relationSplit[0];
                    string relationType = relationSplit[1];

                    lock (_lockObject)
                    {
                        relationships.Add(new Relationship
                        {
                            SourceClass = className,
                            TargetClass = relatedClassName,
                            Type = (RelationshipType)Enum.Parse(typeof(RelationshipType), relationType)
                        });
                    }
                }
            }

            List<string> attributes = extractedInfo
                .Where(info => info.StartsWith("Attribute:"))
                .Select(info => info.Substring(11))
                .ToList();

            List<string> methods = extractedInfo
                .Where(info => info.StartsWith("Method:"))
                .Select(info => info.Substring(8))
                .ToList();

            return new ClassDiagram(className, attributes, methods, null);
        }

        public Task<(List<ClassDiagram> ClassDiagrams, List<Relationship> Relationships)> ExtractClassDiagramAsync(
            List<string> filePaths,
            int threadCount,
            IProgress<(int Completed, int Total)> progress = null)
        {
            return ExtractClassDiagramsAsync(filePaths, threadCount, progress);
        }
    }
}
