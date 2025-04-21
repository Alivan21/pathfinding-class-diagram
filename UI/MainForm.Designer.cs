namespace PathFindingClassDiagram.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.file_location_text = new System.Windows.Forms.Label();
            this.directoryBox = new System.Windows.Forms.TextBox();
            this.browse_button = new System.Windows.Forms.Button();
            this.threads_text = new System.Windows.Forms.Label();
            this.stopwatch_text = new System.Windows.Forms.Label();
            this.memory_text = new System.Windows.Forms.Label();
            this.threads_box = new System.Windows.Forms.TextBox();
            this.stopwatch_box = new System.Windows.Forms.TextBox();
            this.memory_box = new System.Windows.Forms.TextBox();
            this.relationshipToggle = new System.Windows.Forms.CheckBox();
            this.empty_form_button = new System.Windows.Forms.Button();
            this.generate_button = new System.Windows.Forms.Button();
            this.output_text = new System.Windows.Forms.Label();
            this.file_location_output = new System.Windows.Forms.Label();
            this.output_location = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // file_location_text
            // 
            this.file_location_text.AutoSize = true;
            this.file_location_text.Location = new System.Drawing.Point(24, 45);
            this.file_location_text.Name = "file_location_text";
            this.file_location_text.Size = new System.Drawing.Size(83, 16);
            this.file_location_text.TabIndex = 0;
            this.file_location_text.Text = "File Location";
            // 
            // directoryBox
            // 
            this.directoryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.directoryBox.Location = new System.Drawing.Point(141, 47);
            this.directoryBox.Name = "directoryBox";
            this.directoryBox.ReadOnly = true;
            this.directoryBox.Size = new System.Drawing.Size(335, 22);
            this.directoryBox.TabIndex = 1;
            this.directoryBox.TabStop = false;
            this.directoryBox.Tag = "";
            // 
            // browse_button
            // 
            this.browse_button.Location = new System.Drawing.Point(489, 45);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(152, 24);
            this.browse_button.TabIndex = 2;
            this.browse_button.Text = "Browse";
            this.browse_button.UseVisualStyleBackColor = true;
            // 
            // threads_text
            // 
            this.threads_text.AutoSize = true;
            this.threads_text.Location = new System.Drawing.Point(153, 120);
            this.threads_text.Name = "threads_text";
            this.threads_text.Size = new System.Drawing.Size(58, 16);
            this.threads_text.TabIndex = 3;
            this.threads_text.Text = "Threads";
            // 
            // stopwatch_text
            // 
            this.stopwatch_text.AutoSize = true;
            this.stopwatch_text.Location = new System.Drawing.Point(367, 120);
            this.stopwatch_text.Name = "stopwatch_text";
            this.stopwatch_text.Size = new System.Drawing.Size(69, 16);
            this.stopwatch_text.TabIndex = 4;
            this.stopwatch_text.Text = "Stopwatch";
            // 
            // memory_text
            // 
            this.memory_text.AutoSize = true;
            this.memory_text.Location = new System.Drawing.Point(545, 120);
            this.memory_text.Name = "memory_text";
            this.memory_text.Size = new System.Drawing.Size(56, 16);
            this.memory_text.TabIndex = 5;
            this.memory_text.Text = "Memory";
            // 
            // threads_box
            // 
            this.threads_box.Location = new System.Drawing.Point(40, 158);
            this.threads_box.Name = "threads_box";
            this.threads_box.Size = new System.Drawing.Size(294, 22);
            this.threads_box.TabIndex = 6;
            // 
            // stopwatch_box
            // 
            this.stopwatch_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stopwatch_box.Location = new System.Drawing.Point(340, 158);
            this.stopwatch_box.Name = "stopwatch_box";
            this.stopwatch_box.ReadOnly = true;
            this.stopwatch_box.Size = new System.Drawing.Size(136, 22);
            this.stopwatch_box.TabIndex = 7;
            // 
            // memory_box
            // 
            this.memory_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.memory_box.Location = new System.Drawing.Point(489, 158);
            this.memory_box.Name = "memory_box";
            this.memory_box.ReadOnly = true;
            this.memory_box.Size = new System.Drawing.Size(152, 22);
            this.memory_box.TabIndex = 8;
            // 
            // relationshipToggle
            // 
            this.relationshipToggle.AutoSize = true;
            this.relationshipToggle.Location = new System.Drawing.Point(40, 217);
            this.relationshipToggle.Name = "relationshipToggle";
            this.relationshipToggle.Size = new System.Drawing.Size(129, 20);
            this.relationshipToggle.TabIndex = 9;
            this.relationshipToggle.Text = "use Relationship";
            this.relationshipToggle.UseVisualStyleBackColor = true;
            // 
            // empty_form_button
            // 
            this.empty_form_button.Location = new System.Drawing.Point(324, 245);
            this.empty_form_button.Name = "empty_form_button";
            this.empty_form_button.Size = new System.Drawing.Size(152, 52);
            this.empty_form_button.TabIndex = 10;
            this.empty_form_button.Text = "Empty Form";
            this.empty_form_button.UseVisualStyleBackColor = true;
            // 
            // generate_button
            // 
            this.generate_button.Location = new System.Drawing.Point(489, 245);
            this.generate_button.Name = "generate_button";
            this.generate_button.Size = new System.Drawing.Size(152, 52);
            this.generate_button.TabIndex = 11;
            this.generate_button.Text = "Generate";
            this.generate_button.UseVisualStyleBackColor = true;
            // 
            // output_text
            // 
            this.output_text.AutoSize = true;
            this.output_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.output_text.Location = new System.Drawing.Point(291, 324);
            this.output_text.Name = "output_text";
            this.output_text.Size = new System.Drawing.Size(77, 25);
            this.output_text.TabIndex = 12;
            this.output_text.Text = "Output";
            this.output_text.Visible = false;
            // 
            // file_location_output
            // 
            this.file_location_output.AutoSize = true;
            this.file_location_output.Location = new System.Drawing.Point(24, 378);
            this.file_location_output.Name = "file_location_output";
            this.file_location_output.Size = new System.Drawing.Size(83, 16);
            this.file_location_output.TabIndex = 13;
            this.file_location_output.Text = "File Location";
            this.file_location_output.Visible = false;
            // 
            // output_location
            // 
            this.output_location.Location = new System.Drawing.Point(141, 378);
            this.output_location.Name = "output_location";
            this.output_location.ReadOnly = true;
            this.output_location.Size = new System.Drawing.Size(500, 22);
            this.output_location.TabIndex = 14;
            this.output_location.Tag = "";
            this.output_location.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 450);
            this.Controls.Add(this.output_location);
            this.Controls.Add(this.file_location_output);
            this.Controls.Add(this.output_text);
            this.Controls.Add(this.generate_button);
            this.Controls.Add(this.empty_form_button);
            this.Controls.Add(this.relationshipToggle);
            this.Controls.Add(this.memory_box);
            this.Controls.Add(this.stopwatch_box);
            this.Controls.Add(this.threads_box);
            this.Controls.Add(this.memory_text);
            this.Controls.Add(this.stopwatch_text);
            this.Controls.Add(this.threads_text);
            this.Controls.Add(this.browse_button);
            this.Controls.Add(this.directoryBox);
            this.Controls.Add(this.file_location_text);
            this.Name = "MainForm";
            this.Text = "Laravel Controller Extractor";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label file_location_text;
        private System.Windows.Forms.TextBox directoryBox;
        private System.Windows.Forms.Button browse_button;
        private System.Windows.Forms.Label threads_text;
        private System.Windows.Forms.Label stopwatch_text;
        private System.Windows.Forms.Label memory_text;
        private System.Windows.Forms.TextBox threads_box;
        private System.Windows.Forms.TextBox stopwatch_box;
        private System.Windows.Forms.TextBox memory_box;
        private System.Windows.Forms.CheckBox relationshipToggle;
        private System.Windows.Forms.Button empty_form_button;
        private System.Windows.Forms.Button generate_button;
        private System.Windows.Forms.Label output_text;
        private System.Windows.Forms.Label file_location_output;
        private System.Windows.Forms.TextBox output_location;
    }
}

