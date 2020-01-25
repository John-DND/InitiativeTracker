using InitiativeTracker.Data.Tray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InitiativeTracker.Data.Util;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace InitiativeTracker.View
{
    /// <summary>
    /// Interaction logic for GeneratorEditor.xaml
    /// </summary>
    public partial class GeneratorEditor : Window
    {
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register("EditMode", typeof(EditorMode), typeof(GeneratorEditor));
        public static readonly DependencyProperty ParentEntryProperty = DependencyProperty.Register("ParentEntry", typeof(TreeEntry), typeof(GeneratorEditor));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(TreeEntry), typeof(GeneratorEditor));

        public EditorMode EditMode
        {
            get { return (EditorMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public TreeEntry ParentEntry
        {
            get { return (TreeEntry)GetValue(ParentEntryProperty); }
            set { SetValue(ParentEntryProperty, value); }
        }

        public TreeEntry Target
        {
            get { return (TreeEntry)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        private GeneratorEditor(EditorMode mode, TreeEntry parent, TreeEntry target)
        {
            InitializeComponent();
            EditMode = mode;
            ParentEntry = parent;
            Target = target;
        }

        public GeneratorEditor() : this(EditorMode.Create, null, new TreeEntry("Name", 0, 0, 0, true, false, false, 0, 0)) { }
        public GeneratorEditor(TreeEntry edit) : this(EditorMode.Edit, null, edit) { }
        public GeneratorEditor(TreeEntry parent, bool create) : this(EditorMode.Create, parent, new TreeEntry("Name", 0, 0, 0, true, false, false, 0, 0)) { }

        private void Btn_DetachImage_Click(object sender, RoutedEventArgs e)
        {
            for (int i = Lsv_Images.SelectedItems.Count - 1; i > -1; i--)
            {
                string imageSource = (string) Lsv_Images.SelectedItems[i];
                BitmapManager.Remove(imageSource);
                Target.ImageSources.Remove(imageSource);
            }
        }

        private void Btn_AttachImage_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Open image files"))
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Image files", ".png,.jpg,.jpeg,.bmp"));
                dialog.Multiselect = true;

                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    foreach (string path in dialog.FileNames)
                    {
                        Target.ImageSources.Add(path);
                    }
                }
            }
        }

        private void GeneratorEditor_OnClosed(object sender, EventArgs e)
        {
            foreach (string path in Target.ImageSources)
            {
                BitmapManager.Remove(path);
            }
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (EditMode == EditorMode.Edit) Close();
            else
            {
                if (ParentEntry != null) ParentEntry.Contents.Add(Target);
                else TreeEntryStackProvider.Instance.TreeEntryStack.Add(Target);
                Close();
            }
        }
    }
}
