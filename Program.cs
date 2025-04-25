using System;
using System.Windows.Forms;
using PathFindingClassDiagram.Services.Interfaces;
using PathFindingClassDiagram.Services;
using PathFindingClassDiagram.UI;

namespace PathFindingClassDiagram
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configure services (simple dependency injection)
            IFileService fileService = new FileService();
            IExtractorService extractorService = new ExtractorService(fileService);
            IDiagramService diagramService = new DiagramService();

            // Create and run the main form with injected services
            Application.Run(new MainForm(fileService, extractorService, diagramService));
        }
    }
}
