using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSProject
{
    class Prompt
    {
        private char partiton;
        private string path;
        public char Partition
        {
            get { return partiton; }
        }
        public string Path
        {
            get { return path; }
        }

        public Prompt()
        {
            partiton = 'C';
            path = "";
        }
        public Prompt(char part, string p)
        {
            partiton = Char.ToUpper(part);
            path = p;
        }
        public void BackOneStep()
        {
            if (!path.Equals(""))
            {
                char backSlash = '\\';
                int pos = path.LastIndexOf(backSlash);
                if (pos != -1)
                    path = path.Remove(pos);
                else
                    path = "";
            }
        }
        public void BackAllSteps()
        {
            path = "";
        }
        public void StepInto(string directory)
        {
            path += directory;
        }
        public void ChangePrompt(char part, string p)
        {
            partiton = Char.ToUpper(part);
            path = p;
        }
        public string GetPath()
        {
            return partiton + ":\\" + ((path == "") ? "" : path + "\\");
        }
        public override string ToString()
        {
            return partiton + ":\\" + path + ">";
        }
    }
}
