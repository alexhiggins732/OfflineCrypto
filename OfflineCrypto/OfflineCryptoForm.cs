using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OfflineCrypto
{
    public partial class OfflineCryptoForm : Form
    {
        public OfflineCryptoForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public int KeySize => (int)nmKeySize.Value;
        public string GetXml(RSA rsa, bool includePrivate)
        {
            RSAParameters rsaKeyInfo = rsa.ExportParameters(includePrivate);
            string publicPrivateKeyXML = rsa.ToXmlString(includePrivate);
            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(publicPrivateKeyXML);

            using (var ms = new MemoryStream())
            using (var writer = new XmlTextWriter(ms, Encoding.Unicode))
            {
                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                doc.WriteContentTo(writer);
                writer.Flush();
                ms.Flush();
                ms.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(ms);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                return formattedXml;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //Generate a public/private key pair.  
            RSA rsa = RSA.Create();
            rsa.KeySize = KeySize;
            //Save the public key information to an RSAParameters structure.  

            txtKey.Text = GetXml(rsa, false);
            txtPrivate.Text = GetXml(rsa, true);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFile.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnAesEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAesPlain.Text))
            {

                MessageBox.Show("Enter an AES Password of at least 32 characters");
                return;
            }
            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                rsa.FromXmlString(txtKey.Text);
                var data = System.Text.Encoding.UTF8.GetBytes(txtAesPlain.Text);
                var encryptedData = rsa.Encrypt(data, true);
                var base64Encrypted = Convert.ToBase64String(encryptedData);
                txtAesBase64.Text = base64Encrypted;
            }
        }

        private void btnAesDecrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrivate.Text))
            {
                MessageBox.Show("Please enter private key RSA xml", "Error");
                return;
            }
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(txtPrivate.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing private key xml: {ex.Message}", "Error");
            }
            try
            {
                var bytes = Convert.FromBase64String(txtAesBase64.Text.Trim());
                using (var rsa = new RSACryptoServiceProvider(KeySize))
                {
                    rsa.FromXmlString(txtPrivate.Text);
                    var decrypted = rsa.Decrypt(bytes, true);
                    var plain = System.Text.Encoding.UTF8.GetString(decrypted);
                    txtAesPlain.Text = plain;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }


        }

        byte[] Key = new byte[] { };
        byte[] IV = new byte[] { };

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtFile.Text))
            {
                MessageBox.Show("Please select a valid file");
                return;
            }


            AES_Encrypt(txtFile.Text, txtAesPlain.Text);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txtFile.Text))
            {
                MessageBox.Show("Please select a valid file");
                return;
            }


            AES_Decrypt(txtFile.Text, txtAesPlain.Text);
        }


        private void AES_Encrypt(string inputFile, string password)
        {
            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            byte[] salt = GenerateRandomSalt();


            var fi = new FileInfo(inputFile);
            var dir = fi.Directory;
            var filename = Path.GetFileNameWithoutExtension(fi.Name);
            var dest = Path.Combine(dir.FullName, $"{filename}-{DateTime.Now.ToFileTime()}{fi.Extension}.aes");
            //create output file name
            FileStream fsCrypt = new FileStream(dest, FileMode.Create);

            //convert password string to byte arrray
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;

            //write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                    cs.Write(buffer, 0, read);
                }

                //close up
                fsIn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
                //Debug.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }
            MessageBox.Show($"Encrypted file {dest}", "Done");

        }

        private void AES_Decrypt(string inputFile, string password)
        {
            //todo:
            // - create error message on wrong password
            // - on cancel: close and delete file
            // - on wrong password: close and delete file!
            // - create a better filen name
            // - could be check md5 hash on the files but it make this slow

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            var fi = new FileInfo(inputFile);
            var dir = fi.Directory;
            var filename = Path.GetFileNameWithoutExtension(fi.Name);
            var fileNameRoot = Path.GetFileNameWithoutExtension(filename);
            var dest = Path.Combine(dir.FullName, $"{filename}");

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(dest, FileMode.Create);

            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
                //Debug.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("Error: " + ex.Message);
                MessageBox.Show(ex.Message, "Error");
                return;
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
                //Debug.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }
            MessageBox.Show($"Decrypted file {dest}", "Done");
        }

        public static byte[] GenerateRandomSalt()
        {
            //Source: http://www.dotnetperls.com/rngcryptoserviceprovider
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Ten iterations.
                for (int i = 0; i < 10; i++)
                {
                    // Fill buffer.
                    rng.GetBytes(data);
                }
            }
            return data;
        }


    }

}
