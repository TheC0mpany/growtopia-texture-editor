using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace growtopia_texture_editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void PopulateTreeView(string path, TreeNode parentNode, string searchText = "")
        {
            try
            {
                if (searchText == "")
                {
                    string[] directories = Directory.GetDirectories(path);
                    string[] files = Directory.GetFiles(path);
                    foreach (string directory in directories)
                    {
                        if (Directory.GetFiles(directory, "*.rttex").Length > 0) // Check if directory contains .rttex files
                        {
                            TreeNode node = new TreeNode(Path.GetFileName(directory))
                            {
                                Tag = directory
                            };
                            PopulateTreeView(directory, node);
                            parentNode.Nodes.Add(node);
                        }
                        else
                        {
                            TreeNode node = new TreeNode(Path.GetFileName(directory));
                            PopulateTreeView(directory, node);
                            if (node.Nodes.Count > 0)
                                parentNode.Nodes.Add(node);
                        }
                    }
                    foreach (string file in files)
                    {
                        if (Path.GetExtension(file) == ".rttex")
                        {
                            TreeNode node = new TreeNode(Path.GetFileName(file))
                            {
                                Tag = file
                            };
                            parentNode.Nodes.Add(node);
                        }
                    }
                    error.Text = "";
                }
                else
                {
                    string[] directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Where(d => Path.GetFileName(d).Contains(searchText)).ToArray();
                    string[] files = Directory.GetFiles(path, "*.rttex", SearchOption.AllDirectories).Where(f => Path.GetFileName(f).Contains(searchText)).ToArray();
                    foreach (string directory in directories)
                    {
                        if (Directory.GetFiles(directory, "*.rttex").Length > 0)
                        {
                            TreeNode node = new TreeNode(Path.GetFileName(directory))
                            {
                                Tag = directory
                            };
                            PopulateTreeView(directory, node, searchText);
                            parentNode.Nodes.Add(node);
                        }
                        else
                        {
                            TreeNode node = new TreeNode(Path.GetFileName(directory));
                            PopulateTreeView(directory, node, searchText);
                            if (node.Nodes.Count > 0)
                            {
                                parentNode.Nodes.Add(node);
                            }
                        }
                    }
                    foreach (string file in files)
                    {
                        TreeNode node = new TreeNode(Path.GetFileName(file))
                        {
                            Tag = file
                        };
                        parentNode.Nodes.Add(node);
                    }

                    error.Text = "";
                }
            }
            catch (Exception ex)
            {
                error.Text = "Error: " + ex.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetTreeView();
        }

        string currentOpenedFile = "";

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentOpenedFile))
                Helper.OpenFileLocation(currentOpenedFile);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Tag != null && File.Exists(e.Node.Tag.ToString()))
                {
                    string filePath = e.Node.Tag.ToString();
                    string convertedFilePath = Path.Combine(Helper.exeDirPath, "cache", Path.GetFileNameWithoutExtension(filePath) + ".png");

                    if (File.Exists(convertedFilePath))
                    {
                        pictureBox1.Image = Image.FromFile(convertedFilePath);
                        currentOpenedFile = convertedFilePath;
                        return;
                    }
                    label1.Text = "Editing file: " + filePath;
                    if (Path.GetExtension(filePath) == ".rttex" && Helper.ConvertRTPACKFile(filePath, Path.Combine(Helper.exeDirPath, "cache")))
                    {
                        pictureBox1.Image = Image.FromFile(convertedFilePath);
                        currentOpenedFile = convertedFilePath;
                    }
                }
                error.Text = "";
            }
            catch (Exception ex)
            {
                error.Text = "Error: " + ex.Message;
            }
        }
        private static void DeleteFilesRecursive(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            DirectoryInfo[] subdirs = di.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                DeleteFilesRecursive(subdir.FullName);
            }
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch { }
            }
        }
        private void clearCacheFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            GC.Collect();
            DeleteFilesRecursive("cache");
        }
        private void ExpandNodes(TreeNode node)
        {
            node.Expand();
            foreach (TreeNode childNode in node.Nodes)
            {
                ExpandNodes(childNode);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();
            if (searchText == "")
            {
                treeView1.Nodes.Clear();
                TreeNode newRootNode = new TreeNode(Globals.growtopiaPath)
                {
                    Tag = Globals.growtopiaPath
                };
                PopulateTreeView(Globals.growtopiaPath, newRootNode);
                newRootNode.Text = "Growtopia";
                treeView1.Nodes.Add(newRootNode);
            }
            else
            {
                treeView1.Nodes.Clear();
                TreeNode newRootNode = new TreeNode(Globals.growtopiaPath)
                {
                    Tag = Globals.growtopiaPath
                };
                PopulateTreeView(Globals.growtopiaPath, newRootNode, searchText);
                newRootNode.Text = "Search Results";
                treeView1.Nodes.Add(newRootNode);
                foreach (TreeNode node in treeView1.Nodes)
                {
                    ExpandNodes(node);
                }
            }
        }
        private void ResetTreeView()
        {
            treeView1.Nodes.Clear();
            TreeNode rootNode = new TreeNode(Globals.growtopiaPath)
            {
                Tag = Globals.growtopiaPath
            };
            PopulateTreeView(Globals.growtopiaPath, rootNode);
            treeView1.Nodes.Add(rootNode);
            treeView1.Nodes[0].Text = "Growtopia";
        }
        private void ConvertAllRTPACKFiles(string directoryPath)
        {
            string[] directories = Directory.GetDirectories(directoryPath);
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string filePath in files)
            {
                if (Path.GetExtension(filePath) == ".rttex")
                {
                    string convertedFilePath = Path.Combine(Helper.exeDirPath, "cache", Path.GetFileNameWithoutExtension(filePath) + ".png");

                    if (!File.Exists(convertedFilePath))
                    {
                        try
                        {
                            Helper.ConvertRTPACKFile(filePath, Path.Combine(Helper.exeDirPath, "cache"));
                        }
                        catch { }
                    }
                }
            }
            foreach (string directory in directories)
            {
                GC.Collect();
                ConvertAllRTPACKFiles(directory);
            }
        }

        private void convertAllRTTEXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertAllRTPACKFiles(Globals.growtopiaPath);
            MessageBox.Show("All RTTEX files converted.");
        }

        private void openGrowtopiaFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Globals.growtopiaPath);
        }

        private void openCacheFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("cache");
        }
    }
}