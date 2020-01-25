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
using InitiativeTracker.Data.Tray;

namespace InitiativeTracker.View
{
    /// <summary>
    /// Interaction logic for FolderEditor.xaml
    /// </summary>
    public partial class FolderEditor : Window
    {
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register("EditMode", typeof(EditorMode), typeof(FolderEditor));
        public static readonly DependencyProperty ParentEntryProperty = DependencyProperty.Register("ParentEntry", typeof(TreeEntry), typeof(FolderEditor));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(TreeEntry), typeof(FolderEditor));

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

        private FolderEditor(EditorMode mode, TreeEntry parent, TreeEntry target)
        {
            InitializeComponent();
            EditMode = mode;
            ParentEntry = parent;
            Target = target;
        }

        public FolderEditor() : this(EditorMode.Create, null, new TreeEntry("New Folder")) { }
        public FolderEditor(TreeEntry edit) : this(EditorMode.Edit, null, edit) { }
        public FolderEditor(TreeEntry parent, bool create) : this(EditorMode.Create, parent, new TreeEntry("New Folder")) { }

        private void Btn_Save_OnClick(object sender, RoutedEventArgs e)
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
