using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services
{
    /// <summary>
    /// Hybrid extractor service that tries Tree-sitter first and falls back to regex if Tree-sitter fails
    /// </summary>
    public class HybridExtractorService : IExtractorService
    {
        private readonly ExtractorService _regexService;
        private readonly IFileService _fileService;
        private readonly bool _treeSitterInitialized;
        private TreeSitterExtractorService _treeSitterService;

        public HybridExtractorService(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _regexService = new ExtractorService(fileService);
            
            try
            {
                string platform = Environment.Is64BitProcess ? "x64" : "x86";
                string treeSitterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tree-sitter", platform);
                
                if (!Directory.Exists(treeSitterPath))
                {
                    throw new DirectoryNotFoundException($"Tree-sitter {platform} directory not found");
                }
                
                if (!File.Exists(Path.Combine(treeSitterPath, "tree-sitter.dll")))
                {
                    throw new FileNotFoundException($"Tree-sitter {platform} DLL not found");
                }

                _treeSitterService = new TreeSitterExtractorService(fileService);
                _treeSitterInitialized = true;
                Debug.WriteLine($"Tree-sitter {platform} initialized successfully");
            }
            catch (Exception ex)
            {
                _treeSitterInitialized = false;
                Debug.WriteLine($"Tree-sitter initialization failed: {ex.Message}. Will use regex-based extraction instead.");
            }
        }

        public async Task<(List<ClassDiagram> ClassDiagrams, List<Relationship> Relationships)> ExtractClassDiagramsAsync(
            List<string> filePaths,
            int threadCount,
            IProgress<(int Completed, int Total)> progress = null)
        {
            // If Tree-sitter wasn't initialized, go straight to regex
            if (!_treeSitterInitialized)
            {
                Debug.WriteLine("Using regex-based extraction as Tree-sitter is not available");
                return await _regexService.ExtractClassDiagramsAsync(filePaths, threadCount, progress);
            }

            try
            {
                Debug.WriteLine("Attempting extraction with Tree-sitter...");
                // Try Tree-sitter first
                var result = await _treeSitterService.ExtractClassDiagramsAsync(filePaths, threadCount, progress);

                // Check if we got meaningful results
                if (result.ClassDiagrams?.Count > 0)
                {
                    Debug.WriteLine($"Tree-sitter extraction successful: {result.ClassDiagrams.Count} classes found");
                    return result;
                }
                else
                {
                    Debug.WriteLine("Tree-sitter returned no results, falling back to regex...");
                    return await _regexService.ExtractClassDiagramsAsync(filePaths, threadCount, progress);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Tree-sitter failed: {ex.Message}, falling back to regex...");
                // Fall back to regex if Tree-sitter fails
                return await _regexService.ExtractClassDiagramsAsync(filePaths, threadCount, progress);
            }
        }

        public async Task<(List<ClassDiagram> ClassDiagrams, List<Relationship> Relationships)> ExtractClassDiagramAsync(
            List<string> filePaths,
            int threadCount,
            IProgress<(int Completed, int Total)> progress = null)
        {
            return await ExtractClassDiagramsAsync(filePaths, threadCount, progress);
        }
    }
}