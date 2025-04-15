using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PathFindingClassDiagram.Services.Interfaces
{
    public interface IExtractorService
    {
        Task<(List<Models.ClassDiagram> ClassDiagrams, List<Models.Relationship> Relationships)> ExtractClassDiagramAsync(
            List<string> filePaths,
            int threadCount,
            IProgress<(int Completed, int Total)> progress = null
            );
    }
}
