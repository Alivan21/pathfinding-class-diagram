using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;
using TreeSitter;

namespace PathFindingClassDiagram.Services
{
    public class TreeSitterExtractorService : IExtractorService
    {
        private readonly IFileService _fileService;
        private readonly object _lockObject = new object();

        public TreeSitterExtractorService(IFileService fileService)
        {
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            
            // Get the correct platform directory
            string platform = Environment.Is64BitProcess ? "x64" : "x86";
            
            // Add the directory containing tree-sitter DLLs to the PATH
            string dllPath = Path.Combine(applicationPath, "tree-sitter", platform);
            if (!Directory.Exists(dllPath))
            {
                Directory.CreateDirectory(dllPath);
            }
                
            string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            if (!path.Contains(dllPath))
            {
                Environment.SetEnvironmentVariable("PATH", path + Path.PathSeparator + dllPath);
            }
            
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
                        List<string> extractedInfo = ExtractClassAndMethodNamesWithTreeSitter(filePath);

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

        private List<string> ExtractClassAndMethodNamesWithTreeSitter(string filePath)
        {
            List<string> extractedInfo = new List<string>();

            try
            {
                string fileContent;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    fileContent = reader.ReadToEnd();
                }

                // Create PHP language instance
                using (var language = new Language("php"))
                using (var parser = new Parser(language))
                {
                    // Parse the file content
                    using (var tree = parser.Parse(fileContent))
                    {
                        if (tree?.RootNode != null)
                        {
                            // Extract classes, methods, and attributes
                            ExtractFromNode(tree.RootNode, extractedInfo, fileContent);
                        }
                    }
                }

                // Save output for debugging
                string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                string outputFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(filePath) + "_treesitter_output.txt");
                _fileService.SaveTextOutput(outputFileName, extractedInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tree-sitter parsing error for {filePath}: {ex.Message}");
            }

            return extractedInfo;
        }

        private void ExtractFromNode(Node node, List<string> extractedInfo, string sourceCode)
        {
            try
            {
                // Extract class declarations and interface declarations
                if (node.Type == "class_declaration" || node.Type == "interface_declaration")
                {
                    ExtractClassDeclaration(node, extractedInfo, sourceCode);
                }

                // Extract method declarations
                if (node.Type == "method_declaration")
                {
                    ExtractMethodDeclaration(node, extractedInfo, sourceCode);
                }

                // Extract property declarations (PHP class properties)
                if (node.Type == "property_declaration")
                {
                    ExtractPropertyDeclaration(node, extractedInfo, sourceCode);
                }

                // Recursively process child nodes
                foreach (var child in node.Children)
                {
                    ExtractFromNode(child, extractedInfo, sourceCode);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting from node {node.Type}: {ex.Message}");
            }
        }

        private void ExtractClassDeclaration(Node classNode, List<string> extractedInfo, string sourceCode)
        {
            try
            {
                string className = null;
                string inheritance = null;
                string interfaceImplementation = null;
                bool isInterface = classNode.Type == "interface_declaration";

                foreach (var child in classNode.Children)
                {
                    // Get class/interface name
                    if (child.Type == "name" && className == null)
                    {
                        className = child.Text;
                    }

                    // Both classes and interfaces use "base_clause" for their extensions
                    if (child.Type == "base_clause")
                    {
                        foreach (var baseChild in child.Children)
                        {
                            if (baseChild.Type == "name")
                            {
                                inheritance = baseChild.Text;
                                break;
                            }
                        }
                    }

                    // Check for implements clause
                    if (!isInterface && child.Type == "class_interface_clause")
                    {
                        foreach (var interfaceChild in child.Children)
                        {
                            if (interfaceChild.Type == "name")
                            {
                                interfaceImplementation = interfaceChild.Text;
                                break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(className))
                {
                    extractedInfo.Add($"Class: {className}");
                    // Only set isInterface to true if it's an interface declaration, NOT for implementing classes
                    extractedInfo.Add($"IsInterface: {isInterface}");

                    if (!string.IsNullOrEmpty(inheritance))
                    {
                        extractedInfo.Add($"relation: {inheritance} : {(isInterface ? "Interface" : "Inheritance")}");
                    }

                    if (!isInterface && !string.IsNullOrEmpty(interfaceImplementation))
                    {
                        extractedInfo.Add($"relation: {interfaceImplementation} : Interface");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting {(classNode.Type == "interface_declaration" ? "interface" : "class")} declaration: {ex.Message}");
            }
        }

        private void ExtractMethodDeclaration(Node methodNode, List<string> extractedInfo, string sourceCode)
        {
            try
            {
                string visibility = "+"; // default to public
                string methodName = null;
                List<string> parameters = new List<string>();

                foreach (var child in methodNode.Children)
                {
                    // Get visibility modifier
                    if (child.Type == "visibility_modifier")
                    {
                        string modifier = child.Text;
                        switch (modifier)
                        {
                            case "private":
                                visibility = "-";
                                break;
                            case "protected":
                                visibility = "#";
                                break;
                            case "public":
                                visibility = "+";
                                break;
                        }
                    }

                    // Get method name
                    if (child.Type == "name")
                    {
                        methodName = child.Text;
                    }

                    // Get parameters
                    if (child.Type == "formal_parameters")
                    {
                        ExtractParameters(child, parameters);
                    }
                }

                if (!string.IsNullOrEmpty(methodName))
                {
                    string parameterString = string.Join(", ", parameters);
                    extractedInfo.Add($"Method: {visibility} {methodName}({parameterString})");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting method declaration: {ex.Message}");
            }
        }

        private void ExtractPropertyDeclaration(Node propertyNode, List<string> extractedInfo, string sourceCode)
        {
            try
            {
                string visibility = "+"; // default to public
                string propertyName = null;

                foreach (var child in propertyNode.Children)
                {
                    // Get visibility modifier
                    if (child.Type == "visibility_modifier")
                    {
                        string modifier = child.Text;
                        switch (modifier)
                        {
                            case "private":
                                visibility = "-";
                                break;
                            case "protected":
                                visibility = "#";
                                break;
                            case "public":
                                visibility = "+";
                                break;
                        }
                    }

                    // Get property name from property_element
                    if (child.Type == "property_element")
                    {
                        foreach (var propChild in child.Children)
                        {
                            if (propChild.Type == "variable_name")
                            {
                                propertyName = propChild.Text?.TrimStart('$');
                                break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(propertyName))
                {
                    extractedInfo.Add($"Attribute: {visibility} {propertyName}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting property declaration: {ex.Message}");
            }
        }

        private void ExtractParameters(Node parametersNode, List<string> parameters)
        {
            try
            {
                foreach (var child in parametersNode.Children)
                {
                    if (child.Type == "simple_parameter")
                    {
                        foreach (var paramChild in child.Children)
                        {
                            if (paramChild.Type == "variable_name")
                            {
                                string paramName = paramChild.Text?.TrimStart('$');
                                if (!string.IsNullOrEmpty(paramName))
                                {
                                    parameters.Add(paramName);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting parameters: {ex.Message}");
            }
        }

        private ClassDiagram CreateClassDiagram(List<string> extractedInfo, List<Relationship> relationships)
        {
            string className = extractedInfo.FirstOrDefault(info => info.StartsWith("Class:"))?.Substring(7);
            string relation = extractedInfo.FirstOrDefault(info => info.StartsWith("relation:"))?.Substring(10);
            
            // Change this line to read the IsInterface flag directly
            bool isInterface = extractedInfo.Any(info => info.StartsWith("IsInterface:") && info.EndsWith("True"));

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

            return new ClassDiagram(className, attributes, methods, relationships, isInterface);
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
