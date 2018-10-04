using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace CertificateApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            hostTextBox.Text = GetInitialIPAddress();
            hostTextBox.Focus();
        }

        private string GetInitialIPAddress()
        {
            /*
            // Get the local gateway address
            string initialIPAddress = "";

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.Supports(NetworkInterfaceComponent.IPv4) && (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipInterfaceProperties = networkInterface.GetIPProperties();

                    if (ipInterfaceProperties.GatewayAddresses.Count > 0 && ipInterfaceProperties.GatewayAddresses[0].Address.Equals(new IPAddress(0)) == false)
                    {
                        initialIPAddress = ipInterfaceProperties.GatewayAddresses[0].Address.ToString();
                        break;
                    }
                }
            }

            return initialIPAddress;
            */

            return "www.google.com";
        }

        private void HostTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetCertificate(hostTextBox.Text);
            }
        }

        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            GetCertificate(hostTextBox.Text);
        }

        private void GetCertificate(string HostName)
        {
            hostTextBox.IsEnabled = false;
            getButton.IsEnabled = false;
            resultsTextBox.Text = "";

            string text = "";
            try
            {
                string url = "https://" + HostName + "/";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();

                X509Certificate cert = request.ServicePoint.Certificate;
                X509Certificate2 cert2 = new X509Certificate2(cert);

                string issuer = cert2.Issuer;
                string subject = cert2.Subject;
                string from = cert2.GetEffectiveDateString();
                string to = cert2.GetExpirationDateString();

                string serialnumber = cert2.GetSerialNumberString();
                string serialnumberVersion = cert2.Version.ToString();

                string signatureHashName = cert2.SignatureAlgorithm.FriendlyName;
                string signatureHashOid = cert2.SignatureAlgorithm.Value;
                string signature = formatBytes(cert2.GetCertHashString());
                string signatureSize = cert2.GetCertHash().Length.ToString();

                string publickeyAlgorithmName = cert2.PublicKey.Oid.FriendlyName;
                string publickeyAlgorithmOid = cert2.PublicKey.Oid.Value;
                string publickey = formatBytes(cert2.GetPublicKeyString());
                string publickeySize = cert2.PublicKey.EncodedKeyValue.RawData.Length.ToString();

                StringBuilder sb = new StringBuilder();

                sb.Append("Subject: " + subject);
                sb.Append("\r\n\r\n");

                sb.Append("Issuer: " + issuer);
                sb.Append("\r\n\r\n");

                sb.Append("Valid from: " + from);
                sb.Append("\r\n");
                sb.Append("Valid to: " + to);
                sb.Append("\r\n\r\n");

                sb.Append("Serial number: " + serialnumber);
                sb.Append("\r\n");
                sb.Append("Serial number version: " + serialnumberVersion);
                sb.Append("\r\n\r\n");

                sb.Append("Signature algorithm: " + signatureHashName + " ( " + signatureHashOid + " )");
                sb.Append("\r\n");
                sb.Append("Signature: " + signatureSize + " bytes : " + signature);
                sb.Append("\r\n\r\n");

                sb.Append("Public key algorithm: " + publickeyAlgorithmName + " ( " + publickeyAlgorithmOid + " )");
                sb.Append("\r\n");
                sb.Append("Public key: " + publickeySize + " bytes : " + publickey);

                text = sb.ToString(); 
            }
            catch (Exception e)
            {
                text = "EXCEPTION: " + e;
            }

            hostTextBox.IsEnabled = true;
            getButton.IsEnabled = true;
            resultsTextBox.Text = text;
        }

        private string formatBytes(string value)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; ++i)
            {
                sb.Append((i > 0 && i % 2 == 0 ? " " : "") + value[i]);
            }

            return sb.ToString();
        }
    }
}
