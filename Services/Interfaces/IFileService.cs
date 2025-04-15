using System.Collections.Generic;
using System.Drawing;

namespace PathFindingClassDiagram.Services.Interfaces
{
    public interface IFileService
    {
        List<string> GetPhpFiles(string directoryPath);
        void SaveTextOutput(string outputPath, List<string> content);

        void SaveImageOutput(string outputPath, Image image);
    }
}
