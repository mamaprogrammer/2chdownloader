using Awesomium.Core;
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
            webControl.Source = "http://2ch.hk/b".ToUri();
        }

        private void GetCoociesButton_Click(object sender, RoutedEventArgs e)
        {
            //webControl.Source = "http://2ch.hk/b".ToUri();
            string cookies = webControl.ExecuteJavascriptWithResult("document.headers;");
            Data.CoocieValue = cookies;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
