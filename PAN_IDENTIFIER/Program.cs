using PAN_IDENTIFIER.Models;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PAN_IDENTIFIER
{
    internal static class Program
    {
        public static DateTime startTime = DateTime.Now;
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        public static void panIdentify(string path)
        {
            string[] pathArray = path.Split('\\');
            string folderName = pathArray[pathArray.Length - 2]; //-- D:\ExamplePath\2010\PROCESSED
            folderName = Regex.Replace(folderName, @"([a-zA-Z]):", "$1$1");
            folderName += ".txt";
            //if(!File.Exists(path))
            //{
            //    MessageBox.Show("An Error Occured: File Directory cannot be found");
            //    return;
            //}

            List<JournalInfoModel> journalList = new List<JournalInfoModel>();
            string[] xmlFiles = Directory.GetFiles(path, "*.xml");

            foreach (string xmlFile in xmlFiles)
            {
                Console.WriteLine($"Processing file: {xmlFile}");
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(xmlFile);
                XDocument doc = XDocument.Load(xmlFile);
                XElement? journalMeta = doc.Descendants("journal-meta").FirstOrDefault();
                if (doc != null && journalMeta != null)
                {
                    JournalInfoModel currentJournal = new JournalInfoModel();
                    XElement? journalTitleElement = journalMeta.Descendants("journal-title-group")
                                                   .Descendants("journal-title")
                                                   .FirstOrDefault();

                    XElement? printIssnElement = journalMeta.Descendants("issn")
                                                    .FirstOrDefault(e => e.Attribute("publication-format")?.Value == "print");

                    XElement? electronicIssnElement = journalMeta.Descendants("issn")
                                                         .FirstOrDefault(e => e.Attribute("publication-format")?.Value == "electronic");

                    currentJournal.PAN = fileNameWithoutExtension;
                    currentJournal.JOURNAL_TITLE = journalTitleElement?.Value ?? " ";
                    currentJournal.ISSN = printIssnElement?.Value ?? " ";
                    currentJournal.eISSN = electronicIssnElement?.Value ?? " ";

                    journalList.Add(currentJournal);
                }
                else
                {
                    MessageBox.Show($"Failed to load XML document: {xmlFile}");
                }
            }
            if (journalList.Count > 0)
            {
                //define the path for the output file
                string outputPath = @"C:\PAN_Identification_Output\";
                if(!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                //create/overwrite the output file
                using (StreamWriter writer = new StreamWriter(outputPath+folderName))
                {
                    //header
                    writer.WriteLine("PAN\tJournal Title\tPrint ISSN\teISSN");

                    //write each journalInfo to the file with tab-separated values
                    foreach (var journalInfo in journalList)
                    {
                        writer.WriteLine($"{journalInfo.PAN}\t{journalInfo.JOURNAL_TITLE}\t{journalInfo.ISSN}\t{journalInfo.eISSN}");
                    }
                }
                DateTime timeAfter = DateTime.Now;
                TimeSpan elapsedTime = timeAfter - startTime;

                string elapsedTimeString = elapsedTimeString = $"{elapsedTime.ToString(@"hh\:mm\:ss")}.{elapsedTime.Milliseconds.ToString("000")}";

                MessageBox.Show($"Journal information has been saved to {outputPath + folderName}, Processing time is {elapsedTimeString}");

                Application.Exit();
            }
        }
    }
}