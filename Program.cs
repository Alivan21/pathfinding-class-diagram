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

            try
            {
                // Configure services (simple dependency injection)
                IFileService fileService = new FileService();
                IExtractorService extractorService = new ExtractorService(fileService);

                // Create new DiagramService with default pathfinding enabled
                IDiagramService diagramService = new DiagramService(true);

                // Create and run the main form with injected services
                Application.Run(new MainForm(fileService, extractorService, diagramService));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
