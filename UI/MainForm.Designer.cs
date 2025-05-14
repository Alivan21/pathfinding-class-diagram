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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            this.pathfindingToggle = new System.Windows.Forms.CheckBox();
            this.empty_form_button = new System.Windows.Forms.Button();
            this.generate_button = new System.Windows.Forms.Button();
            this.output_text = new System.Windows.Forms.Label();
            this.output_location = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // file_location_text
            // 
            this.file_location_text.AutoSize = true;
            this.file_location_text.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.file_location_text.Location = new System.Drawing.Point(20, 70);
            this.file_location_text.Name = "file_location_text";
            this.file_location_text.Size = new System.Drawing.Size(151, 23);
            this.file_location_text.TabIndex = 0;
            this.file_location_text.Text = "Input File Location";
            // 
            // directoryBox
            // 
            this.directoryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.directoryBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directoryBox.Location = new System.Drawing.Point(20, 100);
            this.directoryBox.Name = "directoryBox";
            this.directoryBox.ReadOnly = true;
            this.directoryBox.Size = new System.Drawing.Size(535, 27);
            this.directoryBox.TabIndex = 1;
            this.directoryBox.TabStop = false;
            this.directoryBox.Tag = "";
            // 
            // browse_button
            // 
            this.browse_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.browse_button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browse_button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browse_button.Location = new System.Drawing.Point(570, 100);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(70, 27);
            this.browse_button.TabIndex = 2;
            this.browse_button.Text = "Browse";
            this.browse_button.UseVisualStyleBackColor = false;
            // 
            // threads_text
            // 
            this.threads_text.AutoSize = true;
            this.threads_text.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.threads_text.Location = new System.Drawing.Point(20, 150);
            this.threads_text.Name = "threads_text";
            this.threads_text.Size = new System.Drawing.Size(70, 23);
            this.threads_text.TabIndex = 3;
            this.threads_text.Text = "Threads";
            // 
            // stopwatch_text
            // 
            this.stopwatch_text.AutoSize = true;
            this.stopwatch_text.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopwatch_text.Location = new System.Drawing.Point(246, 150);
            this.stopwatch_text.Name = "stopwatch_text";
            this.stopwatch_text.Size = new System.Drawing.Size(89, 23);
            this.stopwatch_text.TabIndex = 4;
            this.stopwatch_text.Text = "Stopwatch";
            // 
            // memory_text
            // 
            this.memory_text.AutoSize = true;
            this.memory_text.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memory_text.Location = new System.Drawing.Point(457, 150);
            this.memory_text.Name = "memory_text";
            this.memory_text.Size = new System.Drawing.Size(73, 23);
            this.memory_text.TabIndex = 5;
            this.memory_text.Text = "Memory";
            // 
            // threads_box
            // 
            this.threads_box.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.threads_box.Location = new System.Drawing.Point(20, 180);
            this.threads_box.Name = "threads_box";
            this.threads_box.Size = new System.Drawing.Size(180, 27);
            this.threads_box.TabIndex = 6;
            // 
            // stopwatch_box
            // 
            this.stopwatch_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stopwatch_box.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopwatch_box.Location = new System.Drawing.Point(246, 180);
            this.stopwatch_box.Name = "stopwatch_box";
            this.stopwatch_box.ReadOnly = true;
            this.stopwatch_box.Size = new System.Drawing.Size(180, 27);
            this.stopwatch_box.TabIndex = 7;
            // 
            // memory_box
            // 
            this.memory_box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.memory_box.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memory_box.Location = new System.Drawing.Point(457, 180);
            this.memory_box.Name = "memory_box";
            this.memory_box.ReadOnly = true;
            this.memory_box.Size = new System.Drawing.Size(180, 27);
            this.memory_box.TabIndex = 8;
            // 
            // relationshipToggle
            // 
            this.relationshipToggle.AutoSize = true;
            this.relationshipToggle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.relationshipToggle.Location = new System.Drawing.Point(20, 230);
            this.relationshipToggle.Name = "relationshipToggle";
            this.relationshipToggle.Size = new System.Drawing.Size(137, 24);
            this.relationshipToggle.TabIndex = 9;
            this.relationshipToggle.Text = "Use relationship";
            this.relationshipToggle.UseVisualStyleBackColor = true;
            // 
            // pathfindingToggle
            // 
            this.pathfindingToggle.AutoSize = true;
            this.pathfindingToggle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathfindingToggle.Location = new System.Drawing.Point(20, 260);
            this.pathfindingToggle.Name = "pathfindingToggle";
            this.pathfindingToggle.Size = new System.Drawing.Size(136, 24);
            this.pathfindingToggle.TabIndex = 10;
            this.pathfindingToggle.Text = "Use pathfinding";
            this.pathfindingToggle.UseVisualStyleBackColor = true;
            // 
            // empty_form_button
            // 
            this.empty_form_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.empty_form_button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.empty_form_button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.empty_form_button.Location = new System.Drawing.Point(412, 397);
            this.empty_form_button.Name = "empty_form_button";
            this.empty_form_button.Size = new System.Drawing.Size(108, 40);
            this.empty_form_button.TabIndex = 12;
            this.empty_form_button.Text = "Empty Form";
            this.empty_form_button.UseVisualStyleBackColor = false;
            // 
            // generate_button
            // 
            this.generate_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.generate_button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.generate_button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generate_button.ForeColor = System.Drawing.Color.White;
            this.generate_button.Location = new System.Drawing.Point(536, 397);
            this.generate_button.Name = "generate_button";
            this.generate_button.Size = new System.Drawing.Size(108, 40);
            this.generate_button.TabIndex = 12;
            this.generate_button.Text = "Generate";
            this.generate_button.UseVisualStyleBackColor = false;
            // 
            // output_text
            // 
            this.output_text.AutoSize = true;
            this.output_text.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.output_text.Location = new System.Drawing.Point(20, 310);
            this.output_text.Name = "output_text";
            this.output_text.Size = new System.Drawing.Size(165, 23);
            this.output_text.TabIndex = 12;
            this.output_text.Text = "Output File Location";
            // 
            // output_location
            // 
            this.output_location.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.output_location.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.output_location.Location = new System.Drawing.Point(20, 340);
            this.output_location.Name = "output_location";
            this.output_location.ReadOnly = true;
            this.output_location.Size = new System.Drawing.Size(620, 27);
            this.output_location.TabIndex = 14;
            this.output_location.Tag = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.titleLabel);
            this.panel1.Controls.Add(this.file_location_text);
            this.panel1.Controls.Add(this.directoryBox);
            this.panel1.Controls.Add(this.browse_button);
            this.panel1.Controls.Add(this.threads_text);
            this.panel1.Controls.Add(this.stopwatch_text);
            this.panel1.Controls.Add(this.memory_text);
            this.panel1.Controls.Add(this.threads_box);
            this.panel1.Controls.Add(this.stopwatch_box);
            this.panel1.Controls.Add(this.memory_box);
            this.panel1.Controls.Add(this.relationshipToggle);
            this.panel1.Controls.Add(this.pathfindingToggle);
            this.panel1.Controls.Add(this.output_text);
            this.panel1.Controls.Add(this.output_location);
            this.panel1.Controls.Add(this.empty_form_button);
            this.panel1.Controls.Add(this.generate_button);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(20);
            this.panel1.Size = new System.Drawing.Size(660, 460);
            this.panel1.TabIndex = 16;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(20, 15);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(406, 41);
            this.titleLabel.TabIndex = 15;
            this.titleLabel.Text = "Laravel Controller Extractor";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(684, 485);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Laravel Controller Extractor";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.CheckBox pathfindingToggle;
        private System.Windows.Forms.Button empty_form_button;
        private System.Windows.Forms.Button generate_button;
        private System.Windows.Forms.Label output_text;
        private System.Windows.Forms.TextBox output_location;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label titleLabel;
    }
}
