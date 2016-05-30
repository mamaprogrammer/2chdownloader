using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        

        public List<File> parseThread(Rootobject thread)
        {
            //var test = thread.threads[0].posts;
            var fileList = new List<File>();
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


        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Uri url;
            var thread = new Rootobject();
            try
            {
                url = new Uri(InputUrlBox.Text);
                thread = loadThread(url);
                var fileLIst  = parseThread(thread);
                typeFileComboBox.Items.Clear();
                typeFileComboBox.Items.Add("Все");
                typeFileComboBox.SelectedItem = "Все";
                var typeList = new List<string>();
                foreach (var item in fileLIst)
                {
                    if (!typeFileComboBox.Items.Contains(item.type + " - " + item.name.Substring(item.name.IndexOf("."))))
                    {
                       // typeList.Add(item.type);
                        typeFileComboBox.Items.Add(item.type +" - " + item.name.Substring(item.name.IndexOf(".")));
                    }   
                }
            }
            catch
            {
            }
        }
    }
}
