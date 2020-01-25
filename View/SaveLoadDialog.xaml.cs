using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using InitiativeTracker.Data.Tray;
using InitiativeTracker.Data.XML;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace InitiativeTracker.View
{
    /// <summary>
    /// Interaction logic for SaveLoadDialog.xaml
    /// </summary>
    public partial class SaveLoadDialog : Window
    {
        public enum Mode
        {
            Save,
            Load
        }

        public static readonly DependencyProperty DialogModeProperty = DependencyProperty.Register("DialogMode", typeof(Mode), typeof(SaveLoadDialog));
        public static readonly DependencyProperty TreeEntriesProperty = DependencyProperty.Register("TreeEntries", typeof(SortedTreeEntryList), typeof(SaveLoadDialog));

        public Mode DialogMode
        {
            get { return (Mode)GetValue(DialogModeProperty); }
            set { SetValue(DialogModeProperty, value); }
        }

        public SortedTreeEntryList TreeEntries
        {
            get { return (SortedTreeEntryList)GetValue(TreeEntriesProperty); }
            set { SetValue(TreeEntriesProperty, value); }
        }

        public SaveLoadDialog()
        {
            InitializeComponent();
            DialogMode = Mode.Save;
            TreeEntries = TreeEntryStackProvider.Instance.TreeEntryStack;
        }

        public SaveLoadDialog(SortedTreeEntryList entries)
        {
            InitializeComponent();
            TreeEntries = entries;
            DialogMode = Mode.Load;
        }

        /// <summary>
        /// Reset the entire tray to default CanSave value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveDialog_OnClosing(object sender, CancelEventArgs e)
        {
            if (DialogMode == Mode.Save)
            {
                foreach (TreeEntry entry in TreeEntryStackProvider.Instance.TreeEntryStack)
                {
                    if (!entry.CanSave) entry.CanSave = true;
                }
            }
        }

        private void Btn_SaveLoad_OnClick(object sender, RoutedEventArgs e)
        {
            if (DialogMode == Mode.Save)
            {
                using (CommonSaveFileDialog dialog = new CommonSaveFileDialog("Save entities"))
                {
                    dialog.DefaultExtension = ".xml";
                    dialog.Filters.Add(new CommonFileDialogFilter("XML file", ".xml"));

                    if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                    {
                        SerializationManager.Save(dialog.FileName, new SerializableGroup(TreeEntries));
                        Close();
                    }
                }
            }
            else
            {
                //in the case of Mode.Load, the dialog has already been displayed before the dialog was created
                foreach (TreeEntry entry in TreeEntries)
                {
                    if (entry.CanSave) TreeEntryStackProvider.Instance.TreeEntryStack.Add(entry);
                }
                Close();
            }
        }
    }
}
