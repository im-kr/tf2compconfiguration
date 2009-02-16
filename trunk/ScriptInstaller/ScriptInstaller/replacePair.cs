using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptInstaller
{
    struct ReplacePair
    {
        private string _replaceMe;

        public string ReplaceMe
        {
            get { return _replaceMe; }
            set { _replaceMe = value; }
        }
        private string _newLine;

        public string NewLine
        {
            get { return _newLine; }
            set { _newLine = value; }
        }
    }
}
