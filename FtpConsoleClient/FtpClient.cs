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

        // attributes of items received from ftp LIST method (Unix: ls -l)
        // only for Apache? servers, IIS returns data in other format (probably?)
        private enum ItemAttributes
        {
            TypeMod = 0,
            RefCount = 1,
            OwnerName = 2,
            GroupName = 3,
            Size = 4,
            MonthStamp = 5,
            DayStamp = 6,
            TimeOrYearStamp = 7,
            Name = 8,
        }


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


        public void DownloadAndSafeFile(string[] consoleArgs)
        {
            // this function takes not less than 2 params
            if (consoleArgs.Length < 2)
            {
                Console.WriteLine("\tYou must specify file name and the path to safe!");
                return;
            }

            request = CreateFtpRequest(WebRequestMethods.Ftp.DownloadFile, ftpUri + "/" + consoleArgs[0]);
            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: Couldn't find file {0} at {1}\nShow full message? (y/n) ", consoleArgs[0], ftpUri);

                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                return;
            }

            using (response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        try
                        {
                            using (StreamWriter file = new StreamWriter(consoleArgs[1] + consoleArgs[0]))
                            {
                                // safe file to specified directory
                                file.Write(reader.ReadToEnd());
                                Console.Write("Download complete, status {0}", response.StatusDescription);
                                Console.Write("File {0} successfully saved at {1}\n\n", consoleArgs[0], consoleArgs[1]);
                            }
                        }
                        catch (System.IO.DirectoryNotFoundException)
                        {
                            Console.Write("Directory {0} not found!\n\n", consoleArgs[1] + consoleArgs[0]);
                        }
                    }
                }
            }
        }

        public void DownloadAndShowFile(string[] consoleArgs)
        {
            // this func takes 1 param only too
            try
            {
                request = CreateFtpRequest(WebRequestMethods.Ftp.DownloadFile, ftpUri + "/" + consoleArgs[0]);
            }
            catch (System.IndexOutOfRangeException)
            {
                Console.WriteLine("\tYou must specify file name!");
                return;
            }

            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: Couldn't find file {0} at {1}\nShow full message? (y/n) ", consoleArgs[0], ftpUri);

                // if expception user can view one's messsage by typing 'y'
                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                return;
            }

            using (response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        // shows downloaded file on the screen
                        Console.WriteLine(reader.ReadToEnd());
                        Console.WriteLine("Download complete, status {0}", response.StatusDescription);
                    }
                }
            }
        }

        public void GetCommandsHelp(params string[] consoleArgs)
        {
            using (StreamReader file = new StreamReader("../readme.txt"))
            {
                Console.WriteLine(file.ReadToEnd());
            }
        }

        public void GetListDerictory(params string[] consoleArgs)
        {
            request = CreateFtpRequest(WebRequestMethods.Ftp.ListDirectoryDetails, ftpUri);
            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: The remote name ftp://{0} couldn't be resolved\nShow full message? (y/n) ", ftpUri);

                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                return;
            }

            using (response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        // dictionary for this func keys
                        Dictionary<string, ItemAttributes> arguments = new Dictionary<string, ItemAttributes>();
                        // separate response on lines
                        string[] directoryItemsNotSeparated = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        // then separate lines on items and place into a string array
                        string[,] directoryItems = new string[directoryItemsNotSeparated.Length, 9];

                        // add keys to dictionary
                        arguments.Add("-ds", ItemAttributes.DayStamp);
                        arguments.Add("-ms", ItemAttributes.MonthStamp);
                        arguments.Add("-gn", ItemAttributes.GroupName);
                        arguments.Add("-n", ItemAttributes.Name);
                        arguments.Add("-on", ItemAttributes.OwnerName);
                        arguments.Add("-rc", ItemAttributes.RefCount);
                        arguments.Add("-s", ItemAttributes.Size);
                        arguments.Add("-tys", ItemAttributes.TimeOrYearStamp);
                        arguments.Add("-tm", ItemAttributes.TypeMod);

                        // finally separate items on attributes
                        for (int i = 0; i < directoryItems.GetLength(0); i++)
                        {
                            int j = 0;
                            string[] directoryItem = directoryItemsNotSeparated[i].Split(new char[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string attribute in directoryItem)
                                directoryItems[i, j++] = attribute;
                        }

                        // display specified attributes
                        if (0 == consoleArgs.Length || (1 == consoleArgs.Length && !arguments.ContainsKey(consoleArgs[0])))
                        {
                            Console.Write("Last modified\tSize\tName\n\n");
                            for (int i = 0; i < directoryItems.GetLength(0); i++)
                            {
                                Console.Write("{0} {1} {2}\t{3}\t{4}", directoryItems[i, (int)ItemAttributes.DayStamp],
                                                                       directoryItems[i, (int)ItemAttributes.MonthStamp],
                                                                       directoryItems[i, (int)ItemAttributes.TimeOrYearStamp],
                                                                       directoryItems[i, (int)ItemAttributes.Size],
                                                                       directoryItems[i, (int)ItemAttributes.Name]);
                                Console.WriteLine(directoryItems[i, (int)ItemAttributes.TypeMod][0] == '-' ? "" : "/");
                            }
                        }
                        else
                        {
                            for (int i = 0; i < directoryItems.GetLength(0); i++)
                            {
                                for (int j = 0; j < consoleArgs.Length; j++)
                                    if (arguments.ContainsKey(consoleArgs[j]))
                                        Console.Write("{0}\t", directoryItems[i, (int)arguments[consoleArgs[j]]]);
                                Console.WriteLine();
                            }
                        }
                        Console.WriteLine("\nDirectory List Complete, status {0}", response.StatusDescription);
                    }
                }
            }
        }
    }
}
