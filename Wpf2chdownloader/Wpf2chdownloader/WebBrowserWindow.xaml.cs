using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf2chdownloader
{
    /// <summary>
    /// Interaction logic for WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window
    {
        public WebBrowserWindow()
        {
            InitializeComponent();
            (wfhSample.Child as System.Windows.Forms.WebBrowser).ScriptErrorsSuppressed = true;
            (wfhSample.Child as System.Windows.Forms.WebBrowser).Navigate("https://2ch.hk/b/");
        }

        private void GetCoociesButton_Click(object sender, RoutedEventArgs e)
        {
            var test = (wfhSample.Child as System.Windows.Forms.WebBrowser).Document.Cookie;
            Data.CoocieValue = (wfhSample.Child as System.Windows.Forms.WebBrowser).Document.Cookie;
        }
    }
}
