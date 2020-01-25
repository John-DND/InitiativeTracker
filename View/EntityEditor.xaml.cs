using InitiativeTracker.Data.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using InitiativeTracker.Data.Util;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;

namespace InitiativeTracker.View
{
    public enum EditorMode
    {
        Edit,
        Create
    }

    /// <summary>
    /// Interaction logic for EntityEditor.xaml
    /// </summary>
    public partial class EntityEditor : Window
    {
        /*
         * DependencyProperties are just fancy properties that can be set in XAML
         * and raise PropertyChanged events.
         */
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register("EditMode", typeof(EditorMode), typeof(EntityEditor));
        public static readonly DependencyProperty ParentEntryProperty = DependencyProperty.Register("ParentEntry", typeof(TreeEntry), typeof(EntityEditor));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Entity), typeof(EntityEditor));

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

        public Entity Target
        {
            get { return (Entity)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        private EntityEditor(EditorMode mode, TreeEntry parent, Entity target)
        {
            InitializeComponent();
            EditMode = mode;
            ParentEntry = parent;
            Target = target;
        }

        public EntityEditor(Entity edit) : this(EditorMode.Edit, null, edit) { }
        public EntityEditor(TreeEntry parent) : this(EditorMode.Create, parent, new Entity("Name", 0, 0, 0)) { }
        public EntityEditor() : this(EditorMode.Create, null, new Entity("Name", 0, 0, 0)) { }

        private void Btn_AttachImage_OnClick(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Open image file"))
            {
                dialog.Filters.Add(new CommonFileDialogFilter("Image files", ".png,.jpg,.jpeg,.bmp"));

                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    Target.ImageSource = dialog.FileName;
                }
            }
        }

        private void Btn_ClearImage_OnClick(object sender, RoutedEventArgs e)
        {
            Target.ImageSource = null;
        }

        private void Btn_Save_OnClick(object sender, RoutedEventArgs e)
        {
            if (EditMode == EditorMode.Edit) Close();
            else
            {
                if (ParentEntry != null) ParentEntry.Contents.Add(new TreeEntry(Target));
                else TreeEntryStackProvider.Instance.TreeEntryStack.Add(new TreeEntry(Target));
                Close();
            }
        }

        private void EntityEditor_OnClosed(object sender, EventArgs e)
        {
            Target.CurrentHealth = Target.MaxHealth;
        }
    }
}
