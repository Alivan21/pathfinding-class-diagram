using System;
using System.Windows.Forms;
using PathFindingClassDiagram.Services.Interfaces;
using PathFindingClassDiagram.UI.ViewModels;

namespace PathFindingClassDiagram.UI
{
    public partial class MainForm : Form
    {
        private readonly MainViewModel _viewModel;
        private readonly IFileService _fileService;
        private readonly IExtractorService _extractorService;
        private readonly IDiagramService _diagramService;

        public MainForm(
            IFileService fileService,
            IExtractorService extractorService,
            IDiagramService diagramService)
        {
            InitializeComponent();

            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _extractorService = extractorService ?? throw new ArgumentNullException(nameof(extractorService));
            _diagramService = diagramService ?? throw new ArgumentNullException(nameof(diagramService));

            _viewModel = new MainViewModel(fileService, extractorService, diagramService);

            SetupDataBindings();

            // Register event handlers
            this.Load += MainForm_Load;
            browse_button.Click += Browse_button_Click;
            empty_form_button.Click += Empty_form_button_Click;
            generate_button.Click += Generate_button_Click;
            threads_box.KeyPress += Threads_box_KeyPress;
            pathfindingToggle.CheckedChanged += PathfindingToggle_CheckedChanged;

            // Set initial UI state
            output_text.Visible = false;
            output_location.Visible = false;
        }

        private void SetupDataBindings()
        {
            // Create bindings
            directoryBox.DataBindings.Add("Text", _viewModel, nameof(_viewModel.DirectoryPath), false, DataSourceUpdateMode.OnPropertyChanged);
            threads_box.DataBindings.Add("Text", _viewModel, nameof(_viewModel.ThreadCount), false, DataSourceUpdateMode.OnPropertyChanged);
            relationshipToggle.DataBindings.Add("Checked", _viewModel, nameof(_viewModel.useRelationships), false, DataSourceUpdateMode.OnPropertyChanged);
            pathfindingToggle.DataBindings.Add("Checked", _viewModel, nameof(_viewModel.UsePathfinding), false, DataSourceUpdateMode.OnPropertyChanged);
            stopwatch_box.DataBindings.Add("Text", _viewModel, nameof(_viewModel.ElapsedTime), false, DataSourceUpdateMode.OnPropertyChanged);
            memory_box.DataBindings.Add("Text", _viewModel, nameof(_viewModel.MemoryUsed), false, DataSourceUpdateMode.OnPropertyChanged);
            output_location.DataBindings.Add("Text", _viewModel, nameof(_viewModel.OutputPath), false, DataSourceUpdateMode.OnPropertyChanged);
            progressBar.DataBindings.Add("Value", _viewModel, nameof(_viewModel.Progress), false, DataSourceUpdateMode.OnPropertyChanged);

            // Subscribe to IsBusy property changed
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.IsBusy))
                {
                    generate_button.Enabled = !_viewModel.IsBusy;
                    browse_button.Enabled = !_viewModel.IsBusy;
                    empty_form_button.Enabled = !_viewModel.IsBusy;
                    pathfindingToggle.Enabled = !_viewModel.IsBusy;
                    relationshipToggle.Enabled = !_viewModel.IsBusy;
                    threads_box.Enabled = !_viewModel.IsBusy;
                }
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialize the form
            threads_box.Text = Environment.ProcessorCount.ToString();
        }

        private void Browse_button_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _viewModel.DirectoryPath = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Validates thread box input to only allow digits
        /// </summary>
        private void Threads_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        /// <summary>
        /// Handles pathfinding toggle changed event
        /// </summary>
        private void PathfindingToggle_CheckedChanged(object sender, EventArgs e)
        {
            _viewModel.UsePathfinding = pathfindingToggle.Checked;
        }

        /// <summary>
        /// Handles the empty form button click event
        /// </summary>
        private void Empty_form_button_Click(object sender, EventArgs e)
        {
            _viewModel.Reset();
            output_text.Visible = false;
            output_location.Visible = false;
        }

        /// <summary>
        /// Handles the generate button click event
        /// </summary>
        private async void Generate_button_Click(object sender, EventArgs e)
        {
            // Validate input
            var (isValid, errorMessage) = _viewModel.ValidateInput();
            if (!isValid)
            {
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Process the extraction
                await _viewModel.ProcessAsync();

                // Show output information
                output_text.Visible = true;
                output_location.Visible = true;

                // Reset form title
                this.Text = "Laravel Controller Extractor";

                MessageBox.Show("Extraction completed. Check the output files.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}