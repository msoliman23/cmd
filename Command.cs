using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace OSProject
{
    class Command
    {
        private string name;
        private int numberOfParameters;
        private List<string> parameters;
        Prompt prompt;
        public string Name
        {
            get { return name; }
        }
        public int NumberParams
        {
            get { return numberOfParameters; }
        }
        public List<string> Parameters
        {
            get { return parameters; }
        }
        public Command(Prompt pmt)
        {
            name = "";
            parameters = new List<string>();
            numberOfParameters = 0;
            prompt = pmt;            /////////////////////////////////////////////////////
        }

        public void Parse(string cmd)
        {
            cmd = cmd.Trim();
            cmd = cmd.ToLower();
            if (cmd.Length == 2 && cmd[1].Equals(':')) //C:
            {
                name = "";
                //type = CommandType.CD;
                numberOfParameters = 0;
                if (Directory.Exists(cmd))
                {
                    prompt.ChangePrompt(cmd[0], "");
                }
                else
                    Console.WriteLine("This drive wasn't found");
            }
            else if (cmd.Equals("exit"))
            {
                name = "exit";
                //type = CommandType.EXIT;
                numberOfParameters = 0;
            }
            else if (cmd.StartsWith("cd"))
            {
                parseCD(cmd);
            }
            else if (cmd.StartsWith("dir"))
            {
                parseDIR(cmd);
            }
            //
            else if (cmd.StartsWith("md"))
            {
                parseMD(cmd);
            }
            else if (cmd.StartsWith("remove"))
            {
                parseRemove(cmd);
            }
            else if (cmd.StartsWith("del"))
            {
                parseDel(cmd);
            }
            else if (cmd.StartsWith("move"))
            {
                parseMOVE(cmd);
            }
            else if (cmd.StartsWith("ren"))
            {
                parseREN(cmd);
            }
            else if (cmd.StartsWith("help"))
            {
                parseHelp(cmd);
            }
            else if (cmd.StartsWith("cls"))
            {
                Console.Clear();
            }
            else if (cmd.StartsWith("type"))
            {
                parseType(cmd);
            }
            else if (cmd.StartsWith("attrib"))
            {
                parseAttrib(cmd);
            }
            else if (cmd.StartsWith("xcopy"))
            {
                parseXCopy(cmd);
            }
            else if (cmd.StartsWith("find"))
            {
                parseFind(cmd);
            }
            else if (cmd.StartsWith("sort"))
            {
                parseSort(cmd);
            }
            else if (cmd.StartsWith("comp"))
            {
                parseComp(cmd);
            }
            else
                throw new InvalidCommand("\'" + cmd + "\' is not recognized as internal or external command, \noperable program or batch file.");
        }

        private void parseCD(string cmd)
        {

            name = "cd";
            //type = CommandType.CD;
            cmd = cmd.Substring(2); //cd .. --> .. start from index 2 ((( cd 1\2\3 --> 1\2\3 )))
            cmd = cmd.Trim();
            numberOfParameters = 1;
            if (cmd.Contains(' '))
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }
            else if (cmd == "/?")
                helpDir("cd");
            else if (cmd.Contains(':'))
            {
                if (Directory.Exists(cmd))
                    Console.WriteLine(cmd);
                else
                    Console.WriteLine("The system cannot find the path specified.");
            }

            else if (cmd.Equals(".."))
            {
                prompt.BackOneStep();
            }
            else if (cmd.Equals("\\"))
            {
                prompt.BackAllSteps();
            }
            else if ((cmd[0] == '\\' && cmd[1] == '\\') || (cmd.Length > 1 && cmd[0] == '/' && cmd[1] == '/') || (cmd.Length == 1 && cmd[0] == '/'))
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
            else
            {
                //Console.WriteLine("2");
                if (Directory.Exists(cmd))
                {
                    //Console.WriteLine("3");
                    try
                    {
                        Console.WriteLine("====");
                        if (char.IsLetter(cmd[3]))
                        {
                            ////////////////////////////////////////////////////////////////////////////////////////
                            Console.WriteLine("____" + cmd[0] + "++++++++" + cmd.Substring(3));
                            prompt.ChangePrompt(cmd[0], cmd.Substring(3));
                        }
                        else
                            Console.WriteLine("The system cannot find the path specified.==");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error1");
                    }
                }
                else if (Directory.Exists(prompt.GetPath() + cmd)) // not all pth.. just folder names
                {
                    //Console.WriteLine("4");
                    try
                    {
                        prompt.ChangePrompt(prompt.Partition, (prompt.GetPath() + cmd).Substring(3));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error2");
                    }
                }
                else
                    throw new InvalidCommand("The system cannot find the path specified.");
            }
        }

        private void parseDel(string cmd)
        {
            name = "REMOVE";
            //type = CommandType.REMOVE;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            //string path;
            if (numberOfParameters > 2 || numberOfParameters == 0)
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }
            else if (numberOfParameters == 2) //be 2 for options
            {
                delDir(tokens[1], tokens[2]);
            }
            else
                delDir(tokens[1]);

        }
        private void delDir(string path)
        {

            //Console.WriteLine("___________show"+prompt.GetPath() +"_____"+path);
            if (path.Length > 2 && path[0] != '/')
            {
                Console.Write("Are you sure (Y/N)?  ");
                string ch = Console.ReadLine();
                if (ch.Length == 1)
                {
                    if (ch[0] == 'y' || ch[0] == 'Y')
                    {
                        try
                        {
                            FileAttributes attributes = File.GetAttributes(path);
                            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                Console.WriteLine("File can't be deleted .. it is Read-Only");
                                Console.WriteLine("Press /f to delete this file .. any key to skip");
                                string st = Console.ReadLine();
                                if (st.ToLower() == "/f")
                                    delDir(path, "/f");

                            }
                            else
                            {
                                if (path[1] != ':')     //not all path .. file wanted 
                                {
                                    if (File.Exists(prompt.GetPath() + path))
                                    {
                                        File.Delete(prompt.GetPath() + path);
                                        Console.WriteLine("file was deleted");
                                    }
                                    else
                                        Console.WriteLine("file isn't found");
                                }
                                else                    //all path
                                {
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                        Console.WriteLine("file was deleted");
                                    }
                                    else
                                        Console.WriteLine("file isn't found");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("The process failed: {0}", e.Message);
                        }
                    }
                    else if (ch[0] == 'N' || ch[0] == 'n')
                    {

                    }
                    else
                        delDir(path);
                }
                else
                    delDir(path);
            }
            else
            {
                if (path == "/?")
                    helpDir("del");
                else
                {
                    Console.WriteLine("The system cannot find the path specified.");
                }
            }

        }
        private void delDir(string path1, string path2)
        {
            if (!File.Exists(path1))
            {
                Console.WriteLine("file isn't found");
            }
            else
            {
                if (path2.ToLower() == "/f")
                {
                    FileAttributes attributes = File.GetAttributes(path1);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        // R-Only to R/W file.
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(path1, attributes);
                    }
                    delDir(path1);
                }
            }
        }

        private void parseFind(string cmd)
        {
            name = "find";
            //type = CommandType.FIND;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            // string path;
            if (numberOfParameters != 2 && numberOfParameters != 1 && numberOfParameters != 3)
            {
                throw new InvalidCommand("The Syntax of the command is incorrect.");
            }
            else if (numberOfParameters == 1)
            {
                if (tokens[1] == "/?")
                    helpDir("find");
                else
                    Console.WriteLine("The Syntax of the command is incorrect.");
            }
            else if (numberOfParameters == 2)
            {
                if (File.Exists(tokens[2]))
                    findDir(tokens[1], tokens[2]);
                else
                    Console.WriteLine("File isn't found");
            }
            else if (numberOfParameters == 3)
            {
                if (File.Exists(tokens[2]))
                    findDir(tokens[1], tokens[2], tokens[3]);
                else
                    Console.WriteLine("File isn't found");
            }
            else
                Console.WriteLine("The Syntax of the command is incorrect.");

        }
        private void findDir(string srch, string path)
        {
            FileAttributes attributes = File.GetAttributes(path);

            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                Console.WriteLine("File is hidden");
            }
            else
            {
                // Open the file to read from.
                string[] readText = File.ReadAllLines(path);

                string dirName = path.Substring(path.LastIndexOf('\\') + 1);
                Console.WriteLine("------------" + dirName);
                for (int i = 0; i < readText.Length; i++)
                {
                    if (readText[i].Contains(srch))
                    {
                        Console.WriteLine(readText[i]);
                    }
                }
            }
        }
        private void findDir(string srch, string path1, string path2)
        {
            if (path2.ToLower() == "/r")
            {
                FileAttributes attributes = File.GetAttributes(path1);

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    Console.WriteLine("File is hidden");
                }
                else
                {
                    // Open the file to read from.
                    string[] readText = File.ReadAllLines(path1);

                    string dirName = path1.Substring(path1.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-15}\t", dirName);

                    Console.WriteLine("------------" + path1.ToString());
                    for (int i = 0; i < readText.Length; i++)
                    {
                        if (!readText[i].Contains(srch))
                        {
                            Console.WriteLine(readText[i]);
                        }
                    }
                }
            }
            else if (path2.ToLower() == "/n")
            {
                FileAttributes attributes = File.GetAttributes(path1);

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    Console.WriteLine("File is hidden");
                }
                else
                {
                    // Open the file to read from.
                    string[] readText = File.ReadAllLines(path1);

                    string dirName = path1.Substring(path1.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-15}\t", dirName);

                    Console.WriteLine("------------" + path1.ToString());
                    for (int i = 0; i < readText.Length; i++)
                    {
                        int count = i;
                        if (readText[i].Contains(srch))
                        {
                            Console.WriteLine((++count) + "_" + readText[i]);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("The Syntax of the command is incorrect.");
            }
        }

        private void parseREN(string cmd)
        {
            name = "ren";
            //type = CommandType.REN;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters == 2)
            {
                renDir(tokens[1], tokens[2]);
            }

            else if (numberOfParameters == 1)
            {
                if (tokens[1] == "/?")
                    helpDir("ren");
                else
                    Console.WriteLine("The Syntax of the command is incorrect.");
            }
            else
            {
                throw new InvalidCommand("The Syntax of the command is incorrect.");
            }

        }
        private void renDir(string path1, string path2)
        {
            try
            {

                string s1 = path1.Substring(0, path1.LastIndexOf("\\"));
                string s2 = path2.Substring(0, path2.LastIndexOf("\\"));
                if (s1 == s2)
                {
                    if (File.Exists(path1))
                    {
                        File.Move(path1, path2);
                        Console.WriteLine("File is renamed successfully");
                    }
                    else
                        Console.WriteLine("file isn't found");
                }
                else
                    Console.WriteLine("The system cannot find the path specified2.");

            }
            catch (Exception e)
            {
                throw new InvalidCommand("The system cannot find the path specified22.");
            }
        }

        private void parseMOVE(string cmd)
        {
            name = "move";
            //type = CommandType.MOVE;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            // string path;
            if (numberOfParameters != 2 && numberOfParameters != 1)
            {
                throw new InvalidCommand("The Syntax of the command is incorrect.");
            }
            else if (numberOfParameters == 1)
            {
                if (tokens[1] == "/?")
                    helpDir("move");
                else
                    Console.WriteLine("The Syntax of the command is incorrect.");
            }
            else
            {
                moveDir(tokens[1], tokens[2]);
            }


        }
        private void moveDir(string path1, string path2)
        {
            try
            {
                int count1 = 0, count2 = 0;
                for (int i = 0; i < path1.Length; i++)  //count \\ in path (if != )--> if 
                    if (path1[i] == '\\')
                        count1++;
                for (int j = 0; j < path2.Length; j++)
                    if (path2[j] == '\\')
                        count2++;
                //Console.WriteLine(path1 + "__________" + path2);
                if (count1 != count2)                       // move d:\1\1.txt d:\2 (fileName written in 1 path
                {
                    string[] subpath1 = path1.Split('\\');
                    if (File.Exists(path1))
                    {
                        if (!File.Exists(path2 + '\\' + subpath1[subpath1.Length - 1]))
                        {
                            File.Move(path1, path2 + '\\' + subpath1[subpath1.Length - 1]); ;
                            Console.WriteLine("File was moved successfully");
                        }
                        else
                            Console.WriteLine("File is already exists");    // file 1 is found in folder 2
                    }
                    else
                        Console.WriteLine("file isn't found");
                }
                else                                        // move d:\1\1.txt d:\2\1.txt (fileName must written in 2 path
                {
                    if (File.Exists(path1))
                    {
                        if (!File.Exists(path2))
                        {
                            File.Move(path1, path2);
                            Console.WriteLine("File was moved successfully");
                        }
                        else
                            Console.WriteLine("File is already exists");        // file 1 is found in folder 2
                    }
                    else
                        Console.WriteLine("The system cannot find the path specified.");
                }
            }
            catch (Exception e)
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }

        }

        private void parseRemove(string cmd)
        {
            name = "DEL";
            //type = CommandType.DEL;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            //string path;
            if (numberOfParameters > 1)
            {
                throw new InvalidCommand("The system cannot find the path specified1.");
            }
            else if (numberOfParameters == 0)
            {
                removeDir(prompt.Partition + ":\\" + prompt.Path);  //C :\\Intel
            }
            else
                removeDir(tokens[1]);

        }
        private void removeDir(string path)
        {
            if (path.Length > 2)
            {
                if (path.Length > 1)
                {
                    try
                    {
                        if (path[1] == ':')
                        {
                            if (Directory.Exists(path) && path.Length > 3)
                            {
                                Directory.Delete(path, true);

                                bool directoryExists = Directory.Exists(path);

                                Console.WriteLine("folder is deleted");
                            }
                            else
                                Console.WriteLine("Folder isn't found");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("The system cannot find the path specified.");
                    }
                }
                else
                    Console.WriteLine("Invaled syntax.. Write all folder's path");
            }
            else
            {
                if (path == "/?")
                    helpDir("remove");
                else
                    Console.WriteLine("The system cannot find the path specified.");
            }
        }

        private void parseDIR(string cmd)
        {
            name = "dir";
            //type = CommandType.DIR;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters > 2)
            {
                throw new InvalidCommand("The system cannot find the path specified1.");
            }
            else if (numberOfParameters == 0)
            {
                printDir(prompt.Partition + ":\\" + prompt.Path);  //C :\\Intel
            }
            else if (numberOfParameters == 2)
            {
                if (Directory.Exists(tokens[1]))
                {
                    printDir(tokens[1], tokens[2]);
                }
                else if (Directory.Exists(prompt.GetPath() + tokens[1]))
                {
                    printDir(prompt.GetPath() + tokens[1], tokens[2]);
                }
                else
                    throw new InvalidCommand("The system cannot find the path specified2.");
            }
            else
            {
                if (Directory.Exists(tokens[1]) || tokens[1].ToLower() == "/w" || tokens[1].ToLower() == "/p" || tokens[1] == "/?")
                {
                    printDir(tokens[1]);
                }
                else if (Directory.Exists(prompt.GetPath() + tokens[1]))
                {
                    printDir(prompt.GetPath() + tokens[1]);
                }
                else
                    throw new InvalidCommand("The system cannot find the path specified.");
            }
        }
        private void printDir(string path)
        {


            if (path == "/p")
            {
                string[] subdirectories;
                string[] subfiles;
                subdirectories = Directory.GetDirectories(prompt.GetPath());
                subfiles = Directory.GetFiles(prompt.GetPath());

                int counter = 1;
                foreach (string sd in subdirectories)
                {
                    string dirName = sd.Substring(sd.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-30}\t", dirName);
                    Console.WriteLine(sb.ToString() + "    time.." + Directory.GetLastWriteTime(sd));
                    if (counter % 5 == 0)
                    {
                        Console.WriteLine("Press any key to continue ...");
                        Console.ReadLine();
                    }
                    counter++;
                }

            }
            else if (path == "/w")
            {
                string[] subdirectories;
                string[] subfiles;
                subdirectories = Directory.GetDirectories(prompt.GetPath());
                subfiles = Directory.GetFiles(prompt.GetPath());

                int c = 1;
                foreach (string sd in subdirectories)
                {
                    string dirName = sd.Substring(sd.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-20}\t", dirName);
                    Console.Write(sb.ToString());
                    if (c % 3 == 0)
                        Console.WriteLine();
                    c++;
                }
                Console.WriteLine();
            }
            else if (path == "/?")
            {
                helpDir("dir");
            }
            else
            {
                string[] subdirectories;
                string[] subfiles;
                subdirectories = Directory.GetDirectories(path);
                subfiles = Directory.GetFiles(path);
                foreach (string sd in subdirectories)
                {
                    string dirName = sd.Substring(sd.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-30}\t", dirName);
                    Console.WriteLine(sb.ToString() + "    time.." + Directory.GetLastWriteTime(sd));
                }
                foreach (string sf in subfiles)
                {
                    string fileName = sf.Substring(sf.LastIndexOf('\\') + 1);
                    Console.WriteLine(fileName);
                }
                Console.WriteLine("\t\t" + subfiles.Length + " File(s)");
                Console.WriteLine("\t\t" + subdirectories.Length + " Dir(s)");
            }
        }
        private void printDir(string path1, string path2)
        {
            int count = 0;
            bool bo = true;
            for (int i = 0; i < path1.Length; i++)
            {
                if (path1[i] == '\\')
                    count++;
            }
            //if (count == 0)
            //path1 += '\\';
            try
            {
                // Only get files that begin with the letter "c."
                string[] dirs = Directory.GetFiles(path1, path2);
                Console.WriteLine("Directory of " + path1);
                foreach (string sd in dirs)
                {
                    string dirName = sd.Substring(sd.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-15}\t", dirName);
                    Console.WriteLine(sb.ToString() + "         " + Directory.GetLastWriteTime(sd));
                }
                int b = path1.LastIndexOf('\\');
                if (b != -1)
                {
                    path1 = path1.Substring(0, path1.LastIndexOf('\\'));
                }
                Console.WriteLine();
                if (b != -1)
                {
                    printDir(path1, path2);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }

        private void parseType(string cmd)
        {
            name = "type";
            //type = CommandType.TYPE;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters > 1)
            {
                throw new InvalidCommand("The system cannot find the path specified2.");
            }
            else if (numberOfParameters == 0)
            {
                printDir(prompt.Partition + ":\\" + prompt.Path);  //C :\\Intel
            }
            else
            {
                typeDir(tokens[1]);
            }
        }
        private void typeDir(string path)
        {
            if (path.Length > 2)
            {
                // This text is added only once to the file.
                if (File.Exists(path))
                {
                    string[] readText = File.ReadAllLines(path);

                    foreach (string s in readText)
                    {
                        Console.WriteLine(s);
                    }
                }
                else
                    Console.WriteLine("file isn't found");
            }
            else
            {
                if (path == "/?")
                    helpDir("type");
                else
                    Console.WriteLine("The system cannot find the path specified3.");
            }
        }

        private void parseSort(string cmd)
        {
            name = "sort";
            //type = CommandType.SORT;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            //string path;
            if (numberOfParameters > 2)
            {
                throw new InvalidCommand("The system cannot find the path specified1.");
            }
            else if (numberOfParameters == 2)
            {
                sortDir(tokens[1], tokens[2]);
            }
            else if (numberOfParameters == 0)
            {
                removeDir(prompt.Partition + ":\\" + prompt.Path);  //C :\\Intel
            }
            else
            {
                if (tokens[1].Length == 2 && tokens[1] == "/?")
                {
                    helpDir("sort");
                }

                else if (tokens[1].Length == 1)
                {
                    Console.WriteLine("The system cannot find the path specified.");
                }
                else
                {
                    if (tokens[1][1] == ':')
                        sortDir(tokens[1]);

                    else
                        Console.WriteLine("The system cannot find the path specified.");
                }
            }
        }
        private void sortDir(string path)
        {
            if (!File.Exists(path))
            {
                //   Create a file to write to.
                string createText = Environment.NewLine;
                File.WriteAllText(path, createText);
                Console.WriteLine("file is not found");
            }
            // Open the file to read from.
            string[] readText = File.ReadAllLines(path);

            string dirName = path.Substring(path.LastIndexOf('\\') + 1);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0,-15}\t", dirName);

            Array.Sort(readText);
            foreach (string s in readText)
            {
                Console.WriteLine(s);
            }

        }
        private void sortDir(string path1, string path2)
        {
            if (path2.ToLower() == "/r")
            {
                if (!File.Exists(path1))
                {
                    //   Create a file to write to.
                    string createText = Environment.NewLine;
                    File.WriteAllText(path1, createText);
                    Console.WriteLine("file is not found");
                }
                // Open the file to read from.
                string[] readText = File.ReadAllLines(path1);

                string dirName = path1.Substring(path1.LastIndexOf('\\') + 1);
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0,-15}\t", dirName);

                Array.Sort(readText);
                Array.Reverse(readText);
                foreach (string s in readText)
                {
                    Console.WriteLine(s);
                }
            }
            else if (path2.ToLower() == "/n")
            {
                if (!File.Exists(path1))
                {
                    //   Create a file to write to.
                    string createText = Environment.NewLine;
                    File.WriteAllText(path1, createText);
                    Console.WriteLine("file is not found");
                }
                // Open the file to read from.
                string[] readText = File.ReadAllLines(path1);

                string dirName = path1.Substring(path1.LastIndexOf('\\') + 1);
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0,-15}\t", dirName);

                Array.Sort(readText);
                for (int i = 0; i < readText.Length; i++)
                {
                    int num = i;
                    Console.WriteLine((++num)+".."+readText[i]);
                }

            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }

        private void parseComp(string cmd)
        {
            name = "comp";
            //type = CommandType.COMP;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters > 2)
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }
            else if (numberOfParameters == 0)
            {
                printDir(prompt.Partition + ":\\" + prompt.Path);  //C :\\Intel
            }
            else if (numberOfParameters == 1)
            {
                if (tokens[1] == "/?")
                    helpDir("comp");
                else
                    Console.WriteLine("The system cannot find the path specified.");
            }
            else
            {
                compDir(tokens[1], tokens[2]);
            }
        }
        private void compDir(string path1, string path2)
        {
            if (File.Exists(path1) && File.Exists(path2))
            {
                FileInfo f1 = new FileInfo(path1);
                FileInfo f2 = new FileInfo(path2);
                long sz1, sz2;
                sz1 = f1.Length;
                sz2 = f2.Length;
                if (sz1 == sz2)
                {
                    Console.WriteLine("Files are the same size .. size " + sz2 + " KB");
                }
                else
                {
                    string sub1 = path1.Substring(path1.LastIndexOf('\\') + 1);
                    Console.WriteLine(sub1 + "  ");
                    Console.WriteLine("     size: " + sz1 + " KB");

                    string sub2 = path2.Substring(path1.LastIndexOf('\\') + 1);
                    Console.WriteLine(sub2.ToString() + "  ");
                    Console.WriteLine("     size: " + sz2 + " KB");
                }
            }
            else
            {
                if (!File.Exists(path1))
                    Console.WriteLine("First file isn't found");
                if (!File.Exists(path2))
                    Console.WriteLine("Second file isn't found");
            }

        }

        private void parseMD(string cmd)
        {
            name = "md";
            //type = CommandType.MD;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters > 1)
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }
            else if (numberOfParameters == 0)
            {
                printDir(prompt.Partition + ":\\" + prompt.Path);  //C :\\Intel
            }
            else
            {
                mdDir(tokens[1]);
            }
        }
        private void mdDir(string path)
        {
            if (path != "/?")
            {
                if (path.Length > 2)
                {
                    if (path[1] != ':')
                    {
                        try
                        {
                            Directory.CreateDirectory(prompt.GetPath() + path);
                            Console.WriteLine("Folder is created successfully");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("The system cannot find the path specified.");
                        }
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                            Console.WriteLine("Folder is created successfully");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("The system cannot find the path specified.");
                        }
                    }
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(prompt.GetPath() + path);
                        Console.WriteLine("Folder is created successfully");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("The system cannot find the path specified.");
                    }
                }
            }
            else
            {
                helpDir("md");
            }

        }

        private void parseAttrib(string cmd)
        {
            name = "attrib";
            //type = CommandType.ATTRIB;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters > 2)
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }
            else if (numberOfParameters == 1)
            {
                attribDir(tokens[1]);
            }
            else if (numberOfParameters == 2)
            {
                if (tokens[2][0] == '+' || tokens[2][0] == '-')
                {
                    attribDir(tokens[1], tokens[2]);
                }
                else
                    Console.WriteLine("The system cannot find the path specified.");
            }
            else
                Console.WriteLine("The system cannot find the path specified.");
        }
        private void attribDir(string path1)
        {
            if (path1.Length > 2)
            {
                if (File.Exists(path1))
                {
                    FileAttributes attributes = File.GetAttributes(path1);

                    if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)  //hedden
                        Console.Write("H    ");
                    else
                        Console.Write("     ");

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) //readOnly
                        Console.Write("R    ");
                    else
                        Console.Write("     ");

                    if ((attributes & FileAttributes.Archive) == FileAttributes.Archive) //Archieved
                        Console.Write("A    ");
                    else
                        Console.Write("     ");

                    string dirName = path1.Substring(path1.LastIndexOf('\\') + 1);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0,-30}\t", dirName);
                    Console.WriteLine(sb.ToString());
                }
                else
                {
                    Console.WriteLine("File isn't found");
                }
            }
            else
            {
                if (path1 == "/?")
                    helpDir("attrib");
                else
                    Console.WriteLine("The system cannot find the path specified.");
            }
        }
        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
        private void attribDir(string path1, string path2)
        {
            // Create the file if it does not exist.
            if (File.Exists(path1))
            {
                FileAttributes attributes = File.GetAttributes(path1);

                //hidden or not
                if (path2.ToLower() == "-h")
                {
                    if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        // Show the file.
                        attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                        File.SetAttributes(path1, attributes);
                        Console.WriteLine("file is no longer hidden successfully.");
                    }
                    else
                        Console.WriteLine("file is already shown");
                }
                else if (path2.ToLower() == "+h")
                {
                    if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        // Hide the file.
                        File.SetAttributes(path1, File.GetAttributes(path1) | FileAttributes.Hidden);
                        Console.WriteLine("file is now hidden successfully.");
                    }
                    else
                        Console.WriteLine("file is already hedden");
                }

                //readonly or read and write
                if (path2.ToLower() == "-r")
                {
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        // Show the file.
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(path1, attributes);
                        Console.WriteLine("file is now read-write successfully.");
                    }
                    else
                        Console.WriteLine("file is already read-write");
                }
                else if (path2.ToLower() == "+r")
                {
                    if ((attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
                    {
                        // Hide the file.
                        File.SetAttributes(path1, File.GetAttributes(path1) | FileAttributes.ReadOnly);
                        Console.WriteLine("file is now read-only successfully.");
                    }
                    else
                        Console.WriteLine("file is already read-only");
                }

                //Archiev or not
                if (path2.ToLower() == "-a")
                {
                    if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        // Show the file.
                        attributes = RemoveAttribute(attributes, FileAttributes.Archive);
                        File.SetAttributes(path1, attributes);
                        Console.WriteLine("file is now not archieved successfully.");
                    }
                    else
                        Console.WriteLine("file isn't archieved");
                }
                else if (path2.ToLower() == "+a")
                {
                    if ((attributes & FileAttributes.Archive) != FileAttributes.Archive)
                    {
                        // Hide the file.
                        File.SetAttributes(path1, File.GetAttributes(path1) | FileAttributes.Archive);
                        Console.WriteLine("file is now archieved successfully.");
                    }
                    else
                        Console.WriteLine("file is already archieved");
                }
            }
            else
                Console.WriteLine("File isn't found");
        }

        private void parseXCopy(string cmd)
        {
            name = "xcopy";
            //type = CommandType.XCOPY;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;
            if (numberOfParameters == 1)
            {
                if (tokens[1] == "/?")
                {
                    helpDir("xcopy");
                }
                else if (tokens[1][1] == ':')
                {
                    xCopyDir(tokens[1], prompt.Partition + ":\\" + prompt.Path);
                }
                else
                    Console.WriteLine("The system cannot find the path specified.");
            }
            else if (numberOfParameters == 2)
            {
                xCopyDir(tokens[1], tokens[2]);
            }
            //else if (numberOfParameters == 3)
            //{
            //    xCopyDir(tokens[1], tokens[2], tokens[3]);
            //}
            else if (numberOfParameters == 0)
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }

        }
        private void xCopyDir(string path1, string path2)
        {

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //Give the name as Xcopy
            startInfo.FileName = "xcopy";
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = "\"" + path1 + "\"" + " " + "\"" + path2 + "\"" /*+ @" /e /y /I"*/;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void parseHelp(string cmd)
        {
            name = "help";
            //type = CommandType.HELP;
            string[] tokens = cmd.Split(' ');
            numberOfParameters = tokens.Length - 1;

            if (numberOfParameters > 2)
            {
                throw new InvalidCommand("The system cannot find the path specified.");
            }
            else if (numberOfParameters == 0)
            {
                helpDir();
            }
            else if (numberOfParameters == 1)
            {
                helpDir(tokens[1]);
            }
        }
        private void helpDir()
        {

            Console.WriteLine("ATTRIB         Displays or changes file attributes.");
            Console.WriteLine("CD             Displays the name of or changes the current directory.");
            Console.WriteLine("CLS            Clears the screen.");
            Console.WriteLine("COMP           Compares the contents of two files or sets of files.");
            Console.WriteLine("DEL            Deletes one or more files.");
            Console.WriteLine("DIR            Displays a list of files and subdirectories in a directory.");
            Console.WriteLine("EXIT           Quits the CMD.EXE program (command interpreter).");
            Console.WriteLine("FIND           Searches for a text string in a file or files.");
            Console.WriteLine("HELP           Provides Help information for Windows commands.");
            Console.WriteLine("MD             Creates a directory.");
            Console.WriteLine("MOVE           Moves one or more files from one directory to another directory.");
            Console.WriteLine("REN            Renames a file or files.");
            Console.WriteLine("REMOVE         Permanently delete folder");
            Console.WriteLine("SORT           Sorts input.");
            Console.WriteLine("TYPE           Displays the contents of a text file.");
            Console.WriteLine("XCOPY          Copies files and directory trees.");

        }
        private void helpDir(string path)
        {
            if (path.ToLower() == "dir")
                Console.WriteLine("Displays a list of files and subdirectories in a directory.");

            else if (path.ToLower() == "md")
                Console.WriteLine("Creates a directory.");

            else if (path.ToLower() == "cd")
                Console.WriteLine("Displays the name of or changes the current directory.");

            else if (path.ToLower() == "type")
                Console.WriteLine("Displays the contents of a text file.");

            else if (path.ToLower() == "remove")
                Console.WriteLine("Permanently delete folder.");

            else if (path.ToLower() == "del")
                Console.WriteLine("Deletes one files.");

            else if (path.ToLower() == "exit")
                Console.WriteLine("Quits the CMD.EXE program (command interpreter).");

            else if (path.ToLower() == "help")
                Console.WriteLine("Provides Help information for Windows commands.");

            else if (path.ToLower() == "move")
                Console.WriteLine("Moves one or more files from one directory to another directory.");

            else if (path.ToLower() == "ren")
                Console.WriteLine("Renames a file.");

            else if (path.ToLower() == "type")
                Console.WriteLine("Displays the contents of a text file.");

            else if (path.ToLower() == "attrib")
                Console.WriteLine("Displays or changes file attributes.");

            else if (path.ToLower() == "comp")
                Console.WriteLine("Compares the contents of two files or sets of files.");

            else if (path.ToLower() == "sort")
                Console.WriteLine("Sorts input.");

            else if (path.ToLower() == "xcopy")
                Console.WriteLine("Copies files and directory trees.");

            else if (path.ToLower() == "find")
                Console.WriteLine("Searches for a text string in a file or files.");

            else if (path.ToLower() == "/?")
                helpDir("help");
        }

    }
}

