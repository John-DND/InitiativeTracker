using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitiativeTracker.Data.Tray
{
    /// <summary>
    /// Same format as EntityStackProvider
    /// </summary>
    public class TreeEntryStackProvider : Notifier
    {
        SortedTreeEntryList _treeEntryStack;

        public SortedTreeEntryList TreeEntryStack
        {
            get { return _treeEntryStack; }
            set { SetProperty(ref _treeEntryStack, value); }
        }

        public static TreeEntryStackProvider Instance;

        public TreeEntryStackProvider()
        {
            _treeEntryStack = new SortedTreeEntryList();
            Instance = this;
        }
    }
}