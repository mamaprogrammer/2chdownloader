using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using Wpf2chdownloader.Models;

namespace Wpf2chdownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
    static class Data
    {
        public static string Url { get; set; }
        public static string CoocieValue { get; set; }

    }


    public partial class MainWindow : Window
    {
        BackgroundWorker worker;
        public MainWindow()
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            listMD5Hash = new List<string>();
            ReadMD5List();
            ReadCookies();
            InitializeComponent();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadFiles();
        }

        public string ComputeMD5Checksum(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty).ToLower();
                return result;
            }
        }

        public string downloadDir = "\\DownloadDir";

        private void DownloadFile(Models.threadclass.File item)
        {
            string link = item.path;
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Cookie, Data.CoocieValue);
            string localPath = Directory.GetCurrentDirectory();
            localPath = localPath + "\\DownloadDir" + downloadDir;
            Directory.CreateDirectory(localPath);
            webClient.DownloadFile(new Uri(link), localPath +"\\"+ item.name );
            string s = ComputeMD5Checksum(localPath + "\\" + item.name);
            string test = "";
            if (s != item.md5) test += s + " - " + item.md5;   
        }

        private void DownloadFiles()
        {
            UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(progressBar.SetValue);
            double value = 0;
            var sw = new Stopwatch();
            int downloadsize = 0;
            foreach (var item in fileLIstforDownload)
            {
                if (!listMD5Hash.Contains(item.md5))
                {
                    sw.Start();
                    DownloadFile(item);
                    TimeSpan ts = sw.Elapsed;
                    listMD5Hash.Add(item.md5);
                    downloadsize += item.size;
                    var newTime = new TimeSpan(0, 0, (int)((ts.TotalSeconds * size / downloadsize) - ts.TotalSeconds));
                    string elapsedTime = String.Format("Прошло - {0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                    elapsedTime += String.Format("; Осталось примерно - {0:00}:{1:00}:{2:00}", newTime.Hours, newTime.Minutes, newTime.Seconds);
                    value += item.size;
                    Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => this.timeBlock.Text = elapsedTime));
                    Dispatcher.Invoke(updProgress, new object[] { ProgressBar.ValueProperty, value });
                }
            }
            WriteMD5List();
            value = 0;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.downloadButton.IsEnabled = true));
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.typeFileComboBox.IsEnabled = true));
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.loadButton.IsEnabled = true));
            Dispatcher.Invoke(updProgress, new object[] { ProgressBar.ValueProperty, value });
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => this.timeBlock.Text = ""));
        }

        public void loadThread(Uri url)
        {
            var thread = new Models.threadclass.Rootobject();
            try
            {
                string s = url.AbsoluteUri;
                var wb = new WebClient { Encoding = Encoding.UTF8 };
                wb.Headers.Add(HttpRequestHeader.Cookie,Data.CoocieValue);
                var json = wb.DownloadString(url.AbsoluteUri.Substring(0, s.LastIndexOf(".")) + ".json");
                thread = JsonConvert.DeserializeObject<Models.threadclass.Rootobject>(json);
            }
            catch
            {
            }
            threadForDownload = thread; 
        }

        public List<Models.threadclass.File> parseThread(Models.threadclass.Rootobject thread)
        {
            var fileList = new List<Models.threadclass.File>();
            foreach (var item in thread.threads[0].posts)
            {
                if (item.files.Count() > 0)
                    foreach (var file in item.files)
                    {
                        file.path = "https://2ch.hk/"  + thread.Board +"/"+ file.path;
                        fileList.Add(file);
                    }   
            }
            return fileList;
       }

        public string getFileType (string path)
        {
            return path.Substring(path.IndexOf("."));
        }

        public string getCountType ()
        {
            string s = "Найдено всего : ";
            foreach (var item in allfileTypeCount)
            {
                s += item.ToString(); 
            }
            s += "\nНайдено нового : ";
            foreach (var item in fileTypeCount)
            {
                s += item.ToString();
            }
            return s;
        }

        public List<Models.threadclass.File> fileList;
        public List<Models.threadclass.File> allfileList;
        public List<Models.threadclass.File> fileLIstforDownload;
        public Dictionary<string, int> fileTypeCount;
        public Dictionary<string, int> allfileTypeCount;
        public Models.threadclass.Rootobject threadForDownload;
        public Models.boardclass.Rootobject boardForDownload;
        public string Cookies;
        public int size;

        public List<string> listMD5Hash;

        public void filldict(ref Dictionary<string, int> dict, List<Models.threadclass.File> list)
        {
            dict = new Dictionary<string, int>();
            foreach (var item in list)
            {
                if (!dict.ContainsKey(getFileType(item.name)))
                {
                    dict.Add(getFileType(item.name), 1);
                }
                else
                {
                    dict[getFileType(item.name)] += 1;
                }
            }
        }

        public void filltypeFileComboBox ( )
        {
            foreach (var item in fileList)
            {
                if (!typeFileComboBox.Items.Contains(getFileType(item.name)))
                {
                    typeFileComboBox.Items.Add(getFileType(item.name));
                }
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Uri url;
            try
            {
                url = new Uri(inputUrlBox.Text);
                loadThread(url);
                WriteCookies();
                allfileList = parseThread(threadForDownload);
                fileList = new List<Models.threadclass.File>();
                foreach (var item in allfileList)
                {
                    if (!listMD5Hash.Contains(item.md5))
                    {
                        fileList.Add(item);
                    }
                }
                typeFileComboBox.Items.Clear();
                typeFileComboBox.Items.Add("Все");
                typeFileComboBox.SelectedItem = "Все";
                filldict(ref fileTypeCount, fileList);
                filldict(ref allfileTypeCount, allfileList);
                filltypeFileComboBox();
                infoBlock.Text = getCountType();
                downloadButton.IsEnabled = true;
                typeFileComboBox.IsEnabled = true;
                md5Button.IsEnabled = false;
            }
            catch
            {
            }
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            fileLIstforDownload = new List<Models.threadclass.File>();
            int count = 0;
            downloadButton.IsEnabled = false;
            typeFileComboBox.IsEnabled = false;
            loadButton.IsEnabled = false;
            md5Button.IsEnabled = false;
            downloadDir = "\\" + threadForDownload.current_thread;
            if ((string)typeFileComboBox.SelectedValue == "Все")
            {
                foreach (var item in fileList)
                {
                    count += item.size;
                }
                fileLIstforDownload = fileList;
            }
            else
            {
                foreach (var item in fileList)
                {
                    if (getFileType(item.name) == (string)typeFileComboBox.SelectedValue)
                    {
                        fileLIstforDownload.Add(item);
                        count += item.size;
                    }
                }
            }
            progressBar.Maximum = count;
            size = count;
            worker.RunWorkerAsync();
        }

        public void ReadCookies()
        {
            string path = "cookies.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(string));
            StreamReader reader = new StreamReader(path);
            Data.CoocieValue = (string)serializer.Deserialize(reader);
            reader.Close();
        }

        public void WriteCookies()
        {
            string path = "cookies.xml";
            var writer = new XmlSerializer(typeof(string));
            var wfile = new StreamWriter(path);
            writer.Serialize(wfile, Data.CoocieValue);
            wfile.Close();
        }

        public void ReadMD5List()
        {
            listMD5Hash = new List<string>();
            string path = "listmd5.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            StreamReader reader = new StreamReader(path);
            listMD5Hash = (List<string>)serializer.Deserialize(reader);
            reader.Close();
        }

        public void WriteMD5List()
        {
            string path = "listmd5.xml";
            var writer = new XmlSerializer(typeof(List<string>));
            var wfile = new StreamWriter(path);
            writer.Serialize(wfile, listMD5Hash);
            wfile.Close();
        }


        private void md5Button_Click(object sender, RoutedEventArgs e)
        {
            var allFiles = Directory.GetFiles(Directory.GetCurrentDirectory()+ "\\DownloadDir", "*.*", SearchOption.AllDirectories);
            foreach (var item in allFiles)
            {
                listMD5Hash.Add(ComputeMD5Checksum(item));
            }
            WriteMD5List();
        }

        private void openWebBrowserbutton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserWindow f = new WebBrowserWindow(); // создаем
            if (inputUrlBlock.Text != null) Data.Url = inputUrlBlock.Text;
            f.ShowDialog(); // показываем
            
        }

        public void loadBoard(Uri url)
        {
            var thread = new Models.boardclass.Rootobject();

            try
            {
                string s = url.AbsoluteUri;
                var wb = new WebClient { Encoding = Encoding.UTF8 };
                wb.Headers.Add(HttpRequestHeader.Cookie, Data.CoocieValue);
                var json = wb.DownloadString(url.AbsoluteUri.Substring(0, s.LastIndexOf(".")) + ".json");
                thread = JsonConvert.DeserializeObject<Models.boardclass.Rootobject>(json);
            }
            catch
            {
            }
            boardForDownload = thread;
        }

        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var time = origin.AddSeconds(timestamp);
            return time.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
        }

        private void findeButton_Click(object sender, RoutedEventArgs e)
        {
            Uri url;
            var boardUrl = new List<string>();
            var boardLetter = boardTextBox.Text; 
            url = new Uri("https://2ch.hk/"+ boardLetter + "/catalog.json");
            loadBoard(url);
            string[] regularString = regularTextBox.Text.Split(',');
            string comment;
            foreach (var item in boardForDownload.threads)
            {
                bool f = false;
                foreach (var str in regularString)
                {
                    if (item.comment.ToLower().Contains(str)) f = true;
                }
                if (item.comment.Length > 130) comment = item.comment.Substring(0, 130) + "...";
                else comment = item.comment;
                if (f) boardUrl.Add("https://2ch.hk/b/res/"+ item.num + ".html  | create - " + ConvertFromUnixTimestamp (item.timestamp)  + "  |  lasthit - " + ConvertFromUnixTimestamp(item.lasthit) +
                   "\n" + comment  + "\n ------------------------------------------------------ \n");  
            }
            findeTextBox.Text = "";
            foreach (var item in boardUrl)
            {
                findeTextBox.Text += item + "\n";
            }

        }
    }
}
