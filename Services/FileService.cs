using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services
{
    public class FileService : IFileService
    {
        public List<string> GetPhpFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*.php", SearchOption.AllDirectories).ToList();
        }

        public void SaveTextOutput(string outputPath, List<string> content)
        {
            string directory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                foreach (string line in content)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public void SaveImageOutput(string outputPath, Image image)
        {
            string directory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            image.Save(outputPath, ImageFormat.Jpeg);
        }
    }
}
