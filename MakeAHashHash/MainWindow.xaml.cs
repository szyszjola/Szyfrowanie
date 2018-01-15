using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace MakeAHashHash
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

        #region MD5

        private void ButtonHash_Click(object sender, RoutedEventArgs e)
        {
            OutputTekst.Text = PobierzMd5Hash(InputTekst.Text);
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                ButtonHash_Click(this, new RoutedEventArgs());
            }
        }

        static string PobierzMd5Hash(string wejscie)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] wejscieHash = md5.ComputeHash(Encoding.ASCII.GetBytes(wejscie));
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < wejscieHash.Length; i++)
                {
                    stringBuilder.Append(wejscieHash[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        static bool WeryfikujMd5Hash(string wejscie, string hash)
        {
            string wejscieHash = PobierzMd5Hash(wejscie);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(wejscieHash, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PorownanieButton_Click(object sender, RoutedEventArgs e)
        {
            if (WeryfikujMd5Hash(InputTekst.Text, OutputTekst.Text))
            {
                MessageBox.Show("Ciągi są zgodne");
            }
            else
                MessageBox.Show("To nie jest ten sam wyraz!");
        }
        #endregion



        private void SzyfrujDESButton_Click(object sender, RoutedEventArgs e)
        {
            SymmetricAlgorithm sa = new TripleDESCryptoServiceProvider();
            byte[] encryptMessage = Encrypt(sa, DESInputTekst.Text);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in encryptMessage)
            {
                sb.Append(b);
            }
            DESOutputTekst.Text = sb.ToString();

        }

        #region DES

        private static byte[] key;
        private static byte[] vector;
        string klucz = "y16gyg495g23r72h"; //klucz AES 16byte/128bits
        string IV = "rh84fhfs37uhsduq"; //vector AES 16byte/128bits

        private byte[] Encrypt(SymmetricAlgorithm symmetricAlgorithm, string tekst) // metoda szyfrująca
        {
            //key = System.Text.Encoding.UTF8.GetBytes(klucz);
            //vector = System.Text.Encoding.UTF8.GetBytes(IV); // do uzyskania tego samego szyfrowania trzeba zastosowac to
            //inaczej będzie za każdym razem inny wynik
            ICryptoTransform enkryptor = symmetricAlgorithm.CreateEncryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV);
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, enkryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(tekst);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        private string Descypt(SymmetricAlgorithm symmetricAlgorithm, byte[] msg)
        {
            ICryptoTransform decryptor = symmetricAlgorithm.CreateDecryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV);
            using (var msDecrypt = new MemoryStream(msg))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        private void OdszyfrujButton_Click(object sender, RoutedEventArgs e)
        {
            DESOutputTekst.Clear();
        }
    }
}
