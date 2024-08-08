using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace XboxDevKitRemoteDevicePortalApplication
{
    public partial class Form1 : Form
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private string username;
        private string password;
        private string ipAddress;
        private string port;

        public Form1()
        {
            InitializeComponent();
            LoadConfig();
            InitializeWebView2();
        }

        private void LoadConfig()
        {
            try
            {
                // Path to the XML file (assumes it's in the same directory as the executable)
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string xmlFilePath = Path.Combine(exePath, "config.xml");

                // Load and parse the XML file
                XDocument configDoc = XDocument.Load(xmlFilePath);

                // Extract values from XML
                username = configDoc.Root.Element("Credentials").Element("Username").Value;
                password = configDoc.Root.Element("Credentials").Element("Password").Value;
                ipAddress = configDoc.Root.Element("Server").Element("IPAddress").Value;
                port = configDoc.Root.Element("Server").Element("Port").Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load configuration: " + ex.Message);
            }
        }

        private async void InitializeWebView2()
        {
            webView = new Microsoft.Web.WebView2.WinForms.WebView2
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(webView);
            await webView.EnsureCoreWebView2Async(null);

            webView.CoreWebView2.ServerCertificateErrorDetected += WebView_ServerCertificateErrorDetected;

            // Construct the URL with credentials
            string url = $"https://{username}:{password}@{ipAddress}:{port}/";
            webView.CoreWebView2.Navigate(url);
        }

        private void WebView_ServerCertificateErrorDetected(object sender, CoreWebView2ServerCertificateErrorDetectedEventArgs e)
        {
            e.Action = CoreWebView2ServerCertificateErrorAction.AlwaysAllow;
        }
    }
}
