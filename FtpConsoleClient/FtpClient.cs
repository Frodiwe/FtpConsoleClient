using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ftpConsoleClient
{
    // Ftp client class
    // It consists of
    //  - ftp server's uri
    //  - username, password for login
    //  - shell for basic ftp commands
    class FtpClient
    {
        private string ftpUri = "ftp://mozilla.org";
        private FtpWebRequest request;
        private NetworkCredential credentials;




        public FtpClient() { credentials = new NetworkCredential("", ""); }

        public FtpClient(string uri)
        {
            ftpUri = uri == "" || uri == null ? ftpUri : "ftp://" + uri;
            credentials = new NetworkCredential("", "");
        }

        public FtpClient(string uri, string username, string password)
        {
            ftpUri = uri == "" || uri == null ? ftpUri : "ftp://" + uri;
            credentials = new NetworkCredential(username, password);
        }

        public string FtpUri 
        { 
            get { return ftpUri.Substring(6); } 
            set { ftpUri = value == "" || value == null ? ftpUri : value; }
        }

        public void Reloggin(string username, string password)
        {
            credentials.UserName = username;
            credentials.Password = password;
        }


        public void GetCommandsHelp(params string[] consoleArgs)
        {
            using (StreamReader file = new StreamReader("../readme.txt"))
            {
                Console.WriteLine(file.ReadToEnd());
            }
        }


    }
}
