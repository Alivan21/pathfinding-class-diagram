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

            // Set initial UI state
            output_text.Visible = false;
            file_location_output.Visible = false;
            output_location.Visible = false;
        }

        private void SetupDataBindings()
        {
            // Create bindings
            directoryBox.DataBindings.Add("Text", _viewModel, nameof(_viewModel.DirectoryPath), false, DataSourceUpdateMode.OnPropertyChanged);
            threads_box.DataBindings.Add("Text", _viewModel, nameof(_viewModel.ThreadCount), false, DataSourceUpdateMode.OnPropertyChanged);
            relationshipToggle.DataBindings.Add("Checked", _viewModel, nameof(_viewModel.useRelationships), false, DataSourceUpdateMode.OnPropertyChanged);
            stopwatch_box.DataBindings.Add("Text", _viewModel, nameof(_viewModel.ElapsedTime), false, DataSourceUpdateMode.OnPropertyChanged);
            memory_box.DataBindings.Add("Text", _viewModel, nameof(_viewModel.MemoryUsed), false, DataSourceUpdateMode.OnPropertyChanged);
            output_location.DataBindings.Add("Text", _viewModel, nameof(_viewModel.OutputPath), false, DataSourceUpdateMode.OnPropertyChanged);

            // Subscribe to IsBusy property changed
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.IsBusy))
                {
                    generate_button.Enabled = !_viewModel.IsBusy;
                    browse_button.Enabled = !_viewModel.IsBusy;
                    empty_form_button.Enabled = !_viewModel.IsBusy;
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
        /// Handles the empty form button click event
        /// </summary>
        private void Empty_form_button_Click(object sender, EventArgs e)
        {
            _viewModel.Reset();
            output_text.Visible = false;
            file_location_output.Visible = false;
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
                // Create a progress reporter
                var progress = new Progress<(int Completed, int Total)>(p =>
                {
                    // Update progress bar or status
                    if (p.Total > 0)
                    {
                        // You could add a progress bar to the UI
                        // progressBar.Value = (int)((float)p.Completed / p.Total * 100);
                    }
                });

                // Process the extraction
                await _viewModel.ProcessAsync(progress);

                // Show output information
                output_text.Visible = true;
                file_location_output.Visible = true;
                output_location.Visible = true;

                MessageBox.Show("Extraction completed. Check the output files.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
