namespace PAN_IDENTIFIER
{
    public partial class Form1 : Form
    {
        private static string folderPath = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderPath = GetXmlFileDirectory().ToString();
            label1.Text = folderPath;
        }

        private string GetXmlFileDirectory()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();
            button2.Enabled = false;
            //check if the user selected a folder
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                string folderPath = folderBrowserDialog.SelectedPath;

                //get a list of XML files in the selected folder
                string[] xmlFiles = Directory.GetFiles(folderPath, "*.xml");

                if (xmlFiles.Length == 0)
                {
                    MessageBox.Show("No XML files found in the selected directory.");
                    return "No path selected";
                }
                button2.Enabled = true;
                return folderPath;
            }
            else
            {
                return "No path selected";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.startTime = DateTime.Now;
            Program.panIdentify(folderPath);
        }

    }
}
