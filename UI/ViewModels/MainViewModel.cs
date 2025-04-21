using System;
using System.Collections.Generic;
using System.ComponentModel;
using PathFindingClassDiagram.Helpers;
using System.IO;
using System.Threading.Tasks;
using PathFindingClassDiagram.Services.Interfaces;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace PathFindingClassDiagram.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IFileService _fileService;
        private readonly IExtractorService _extractorService;
        private readonly IDiagramService _diagramService;

        private string _directoryPath = string.Empty;
        private string _threadCount = Environment.ProcessorCount.ToString();
        private bool _useRelationships;
        private string _elapsedTime = string.Empty;
        private string _memoryUsed = string.Empty;
        private string _outputPath = string.Empty;
        private int _progress;
        private bool _isBusy;

        public string DirectoryPath
        {
            get => _directoryPath;
            set
            {
                if (_directoryPath != value)
                {
                    _directoryPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ThreadCount
        {
            get => _threadCount;
            set
            {
                if (_threadCount != value)
                {
                    _threadCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool useRelationships
        {
            get => _useRelationships;
            set
            {
                if (_useRelationships != value)
                {
                    _useRelationships = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                if (_elapsedTime != value)
                {
                    _elapsedTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MemoryUsed
        {
            get => _memoryUsed;
            set
            {
                if (_memoryUsed != value)
                {
                    _memoryUsed = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OutputPath
        {
            get => _outputPath;
            set
            {
                if (_outputPath != value)
                {
                    _outputPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel(
            IFileService fileService,
            IExtractorService extractorService,
            IDiagramService diagramService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _extractorService = extractorService ?? throw new ArgumentNullException(nameof(extractorService));
            _diagramService = diagramService ?? throw new ArgumentNullException(nameof(diagramService));
        }

        public void Reset()
        {
            DirectoryPath = string.Empty;
            ThreadCount = Environment.ProcessorCount.ToString();
            useRelationships = false;
            ElapsedTime = string.Empty;
            MemoryUsed = string.Empty;
            OutputPath = string.Empty;
            Progress = 0;
            IsBusy = false;
        }

        public (bool isValid, string ErrorMessage) ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(DirectoryPath))
            {
                return (false, "Please input the directory path");
            }

            if (string.IsNullOrWhiteSpace(ThreadCount))
            {
                return (false, "Please input number of threads");
            }

            if (!int.TryParse(ThreadCount, out int numOfThreads))
            {
                return (false, "Please input a valid number of threads");
            }

            if (numOfThreads <= 0)
            {
                return (false, "Number of threads must be greater than zero");
            }

            return (true, string.Empty);
        }

        public async Task ProcessAsync(IProgress<(int Completed, int Total)> progress = null)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Progress = 0;

            try
            {
                using (var performanceTracker = new PerformanceTracker())
                {
                    // Get PHP files
                    List<string> phpFiles = _fileService.GetPhpFiles(DirectoryPath);

                    if (phpFiles.Count == 0)
                    {
                        throw new InvalidOperationException("No PHP files found in the specified directory");
                    }

                    // Create a progress reporter that updates the Progress property
                    var progressReporter = new Progress<(int Completed, int Total)>(p =>
                    {
                        Progress = (int)((float)p.Completed / p.Total * 100);
                        progress?.Report(p);
                    });

                    // Extract class diagrams
                    int threadCount = int.Parse(ThreadCount);
                    var (classDiagrams, relationships) = await _extractorService.ExtractClassDiagramAsync(
                        phpFiles,
                        threadCount,
                        progressReporter);

                    // Generate diagram image
                    using (Image diagramImage = _diagramService.GenerateClassDiagram(classDiagrams, relationships, useRelationships))
                    {
                        string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");

                        if (Directory.Exists(outputDirectory))
                        {
                            Directory.Delete(outputDirectory, recursive: true);
                            Directory.CreateDirectory(outputDirectory);
                        }

                        // Save the diagram image
                        _fileService.SaveImageOutput(Path.Combine(outputDirectory, "ClassDiagrams.jpg"), diagramImage);

                        // Update output path
                        OutputPath = Directory.GetCurrentDirectory();
                    }

                    // Update performance metrics
                    ElapsedTime = performanceTracker.ElapsedTimeString;
                    MemoryUsed = performanceTracker.MemoryUsedString;
                }
            }
            finally
            {
                IsBusy = false;
                Progress = 100;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
