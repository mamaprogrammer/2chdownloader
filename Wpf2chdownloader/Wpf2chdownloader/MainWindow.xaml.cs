﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
using Wpf2chdownloader.Models;

namespace Wpf2chdownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
    public partial class MainWindow : Window
    {
        BackgroundWorker worker;
        public MainWindow()
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            InitializeComponent();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadFiles();
        }


        private void DownloadFile(Models.File item)
        {
            string link = item.path;
            WebClient webClient = new WebClient();
            string localPath = Directory.GetCurrentDirectory();
            localPath = localPath + "\\DownloadDir";
            Directory.CreateDirectory(localPath);
            webClient.DownloadFile(new Uri(link), localPath +"\\"+ item.name );
        }



        private void DownloadFiles()
        {
            UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(progressBar.SetValue);
            double value = 0;
            foreach (var item in fileLIstforDownload)
            {

                DownloadFile(item);
                Dispatcher.Invoke(updProgress, new object[] { ProgressBar.ValueProperty, ++value });
            }

        }

        public Rootobject loadThread(Uri url)
        {
            var thread = new Rootobject();
            try
            {
                string s = url.AbsoluteUri;
                var json = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(url.AbsoluteUri.Substring(0, s.LastIndexOf(".")) + ".json");
                thread = JsonConvert.DeserializeObject<Rootobject>(json);
            }
            catch
            {
            }
            return thread; 
        }

        public List<Models.File> parseThread(Rootobject thread)
        {
            //var test = thread.threads[0].posts;
            var fileList = new List<Models.File>();
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

        public string getCountType (Dictionary<string, int> dict)
        {
            string s = "Найдено : ";
            foreach (var item in dict)
            {
                s += item.ToString(); 
            }
            return s;
        }


        public List<Models.File> fileList;
        public List<Models.File> fileLIstforDownload;
        public Dictionary<string, int> fileTypeCount;
        
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Uri url;
            var thread = new Rootobject();
            try
            {
                url = new Uri(inputUrlBox.Text);
                thread = loadThread(url);
                fileList  = parseThread(thread);
                typeFileComboBox.Items.Clear();
                typeFileComboBox.Items.Add("Все");
                typeFileComboBox.SelectedItem = "Все";
                fileTypeCount = new Dictionary<string, int>();
                foreach (var item in fileList)
                {
                    if (!typeFileComboBox.Items.Contains(getFileType(item.name)))
                    {
                        fileTypeCount.Add( getFileType(item.name), 1 ); 
                        typeFileComboBox.Items.Add(getFileType(item.name));
                    } else
                    {
                        fileTypeCount[getFileType(item.name)] += 1;
                    }
                      
                }
                infoBlock.Text = getCountType(fileTypeCount);
                downloadButton.IsEnabled = true;
            }
            catch
            {
            }
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            fileLIstforDownload = new List<Models.File>();
            int count = 0;
            if ((string)typeFileComboBox.SelectedValue == "Все")
            {
                foreach (var item in fileTypeCount)
                {
                    count += item.Value;
                }
                fileLIstforDownload = fileList;
            }
            else
            {
                count = fileTypeCount[(string)typeFileComboBox.SelectedValue];
                foreach (var item in fileList)
                {
                    if (getFileType(item.name) == (string)typeFileComboBox.SelectedValue) fileLIstforDownload.Add(item);
                }
            }
            // || ((string)typeFileComboBox.SelectedValue) == typeFileComboBox.Text) 
            progressBar.Maximum = count;
            worker.RunWorkerAsync();
        }
    }
}
