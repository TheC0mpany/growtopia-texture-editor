using System;
using System.IO;
using System.Windows.Forms;

namespace growtopia_texture_editor
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
            textBox1.Text = Helper.GetGrowtopiaPath();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // Check if the selected path contains any .rttex files
                    if (Directory.GetFiles(dialog.SelectedPath, "*.rttex", SearchOption.AllDirectories).Length == 0)
                    {
                        MessageBox.Show("The selected path does not contain any .rttex files. Please select a different path.", "Incorrect Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        textBox1.Text = dialog.SelectedPath;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text.Trim();
            if (Helper.IsValidPath(path))
            {
                Globals.growtopiaPath = path;
                this.Hide();
                Globals.main.Show();
            }
            else
            {
                MessageBox.Show("Path is invalid, try again.");
            }
        }
    }
}
