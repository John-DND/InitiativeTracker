using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InitiativeTracker.Data.Stack;
using InitiativeTracker.Data.Tray;
using InitiativeTracker.Data.Util;
using InitiativeTracker.Data.XML;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace InitiativeTracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //utility fields
        SortedEntityList _initiativeStack;
        SortedTreeEntryList _treeEntryList;

        //used by drag and drop code
        TreeEntry _draggedItem;
        Point _lastMouseDown;

        //also used by drag and drop code
        const double DRAG_TOLERANCE = 20;

        static PlayerDisplay _display;

        public MainWindow()
        {
            InitializeComponent();
            _initiativeStack = EntityStackProvider.Instance.InitiativeStack;
            _treeEntryList = TreeEntryStackProvider.Instance.TreeEntryStack;

            _display = new PlayerDisplay();
            _display.Show();
        }

        /// <summary>
        /// Called when the user releases a dragged entity.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trv_EntityTray_OnDrop(object sender, DragEventArgs e)
        {
            TreeEntry targetItem = (TreeEntry)((DependencyObject)e.OriginalSource).FindVisualParentOfType<TreeViewItem>()?.DataContext;
            _draggedItem.MoveTo(targetItem);

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        /// <summary>
        /// Self-explanatory. Used to save the item being dragged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trv_EntityTray_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(Trv_EntityTray);

                /*
                 * this basically just ensures that we can't start a drag and drop operation unless the user moves the cursor more than
                 * DRAG_TOLERANCE pixels, determined by taxicab distance but ignoring the smaller component
                 */
                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > DRAG_TOLERANCE) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > DRAG_TOLERANCE))
                {
                    DependencyObject source = (DependencyObject)e.OriginalSource;
                    TreeViewItem item = source.FindVisualParentOfType<TreeViewItem>();
                    if (item != null || source.GetType().Equals(typeof(TreeViewItem)))
                    {
                        _draggedItem = (TreeEntry) item.DataContext;
                        if (_draggedItem != null) DragDrop.DoDragDrop(Trv_EntityTray, _draggedItem, DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the user first presses a mouse button. Saves the position of the
        /// mouse and is used by OnMouseMove to determine how far the user has dragged
        /// the cursor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trv_Entities_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) _lastMouseDown = e.GetPosition(Trv_EntityTray);
        }

        private void Ctm_PushTreeEntity_OnClick(object sender, RoutedEventArgs e)
        {
            TreeEntry entry = (TreeEntry)((MenuItem)sender).DataContext;

            if (_initiativeStack.Count == 0)
            {
                /*
                 * when pushing entities to an empty stack, we should set the CurrentTurn
                 * to 0 manually after the push has occured.
                 */
                entry.Push();
                EntityStackProvider.Instance.SetTurn(0);
            }
            else entry.Push();
        }

        private void Ctm_EditTreeEntity_OnClick(object sender, RoutedEventArgs e)
        {
            EntityEditor editorWindow = new EntityEditor(((TreeEntry)((MenuItem)sender).DataContext).AttachedEntity);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_DeleteTreeEntry_OnClick(object sender, RoutedEventArgs e)
        {
            TreeEntry entry = (TreeEntry)((MenuItem)sender).DataContext;
            entry.Delete();
        }

        private void Ctm_DeleteContents_OnClick(object sender, RoutedEventArgs e)
        {
            TreeEntry entity = (TreeEntry)((MenuItem)sender).DataContext;
            entity.DeleteChildren();
        }

        private void Ctm_DeleteStackEntity_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = Lsv_InitiativeStack.SelectedItems.Count - 1; i > -1; i--)
            {
                Entity entity = (Entity)Lsv_InitiativeStack.SelectedItems[i];
                entity.Delete();
            }
        }

        private void Ctm_PullStackEntity_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = Lsv_InitiativeStack.SelectedItems.Count - 1; i > -1; i--)
            {
                Entity entity = (Entity)Lsv_InitiativeStack.SelectedItems[i];
                entity.Pull();
            }
        }

        private void Ctm_NewEntity_OnClick(object sender, RoutedEventArgs e)
        {
            EntityEditor editorWindow = new EntityEditor();
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_TrayPushAll_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = _treeEntryList.Count - 1; i > -1; i--)
            {
                _treeEntryList[i].Push();
            }
            EntityStackProvider.Instance.SetTurn(0);
        }

        private void Ctm_TrayFolderNewEntity(object sender, RoutedEventArgs e)
        {
            EntityEditor editorWindow = new EntityEditor((TreeEntry)((MenuItem)sender).DataContext);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_TreeViewDeleteAll_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = _treeEntryList.Count - 1; i > -1; i--)
            {
                _treeEntryList[i].Delete();
            }
        }

        private void Ctm_TreeViewNewFolder_OnClick(object sender, RoutedEventArgs e)
        {
            FolderEditor editorWindow = new FolderEditor();
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_TrayFolderNewFolder(object sender, RoutedEventArgs e)
        {
            FolderEditor editorWindow = new FolderEditor((TreeEntry)((MenuItem)sender).DataContext, true);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_TrayFolderEdit_OnClick(object sender, RoutedEventArgs e)
        {
            FolderEditor editorWindow = new FolderEditor((TreeEntry)((MenuItem)sender).DataContext);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_InitiativeStackPullAll_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = _initiativeStack.Count - 1; i > -1; i--)
            {
                _initiativeStack[i].Pull();
            }
        }

        private void Ctm_InitiativeStackDeleteAll_OnClick(object sender, RoutedEventArgs e)
        {
            for (int i = _initiativeStack.Count - 1; i > -1; i--)
            {
                _initiativeStack[i].Delete();
            }
        }

        private void Mni_SaveSelected_OnClick(object sender, RoutedEventArgs e)
        {
            SaveLoadDialog loadDialog = new SaveLoadDialog();
            loadDialog.Owner = this;
            loadDialog.ShowDialog();
        }

        private void Mni_SaveAll_OnClick(object sender, RoutedEventArgs e)
        {
            using (CommonSaveFileDialog dialog = new CommonSaveFileDialog("Save entities"))
            {
                dialog.DefaultExtension = ".xml";
                dialog.Filters.Add(new CommonFileDialogFilter("XML file", ".xml"));

                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    SerializationManager.Save(dialog.FileName, new SerializableGroup(TreeEntryStackProvider.Instance.TreeEntryStack));
                }
            }
        }

        private void Mni_LoadAll_OnClick(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Load entities"))
            {
                dialog.Filters.Add(new CommonFileDialogFilter("XML files", ".xml"));
                dialog.Multiselect = true;
                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    foreach (string fileName in dialog.FileNames)
                    {
                        SerializationManager.LoadToTray(fileName);
                    }
                }
            }
        }

        private void Mni_LoadSelected_OnClick(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Load entities"))
            {
                dialog.Filters.Add(new CommonFileDialogFilter("XML files", ".xml"));
                dialog.Multiselect = true;

                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    SortedTreeEntryList allEntities = new SortedTreeEntryList();
                    foreach (string fileName in dialog.FileNames)
                    {
                        SortedTreeEntryList fileEntities = SerializationManager.LoadToList(fileName);
                        foreach (TreeEntry subFileName in fileEntities)
                        {
                            allEntities.Add(subFileName);
                        }
                    }

                    if (allEntities != null)
                    {
                        SaveLoadDialog saveLoadDialog = new SaveLoadDialog(allEntities);
                        saveLoadDialog.Owner = this;
                        saveLoadDialog.ShowDialog();
                    }
                }
            }
        }

        private void Btn_Previous_OnClick(object sender, RoutedEventArgs e)
        {
            EntityStackProvider.Instance.RetractTurn();
        }

        private void Btn_Next_OnClick(object sender, RoutedEventArgs e)
        {
            EntityStackProvider.Instance.AdvanceTurn();
        }

        private void Chb_IsHidden_IsChecked(object sender, RoutedEventArgs e)
        {
            EntityStackProvider.Instance.Update();
        }

        private void Ctm_TrayFolderNewGenerator_OnClick(object sender, RoutedEventArgs e)
        {
            GeneratorEditor editorWindow = new GeneratorEditor((TreeEntry)((MenuItem)sender).DataContext, true);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_NewGenerator_OnClick(object sender, RoutedEventArgs e)
        {
            GeneratorEditor editorWindow = new GeneratorEditor();
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_TrayGeneratorEdit_OnClick(object sender, RoutedEventArgs e)
        {
            GeneratorEditor editorWindow = new GeneratorEditor((TreeEntry)((MenuItem)sender).DataContext);
            editorWindow.Owner = this;
            editorWindow.ShowDialog();
        }

        private void Ctm_ExecuteGenerator_OnClick(object sender, RoutedEventArgs e)
        {
            TreeEntry generator = (TreeEntry) ((MenuItem) sender).DataContext;
            GeneratorCountDialog dialog = new GeneratorCountDialog();
            dialog.Owner = this;
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                generator.Generate(dialog.EntityCount);
            }
        }

        private void Ctm_ExecuteAndPush_OnClick(object sender, RoutedEventArgs e)
        {
            TreeEntry generator = (TreeEntry)((MenuItem)sender).DataContext;
            GeneratorCountDialog dialog = new GeneratorCountDialog();
            dialog.Owner = this;
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                bool shouldSetTurn = _initiativeStack.Count == 0;
                generator.Generate(dialog.EntityCount);
                for (int i = generator.Contents.Count - 1; i > -1; i--)
                {
                    generator.Contents[i].Push();
                } 

                if(shouldSetTurn) EntityStackProvider.Instance.SetTurn(0);
            }
        }

        private void Ctm_EditStackEntity_OnClick(object sender, RoutedEventArgs e)
        {
            EditHealthDialog dialog = new EditHealthDialog((Entity)((MenuItem)sender).DataContext);
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MainWindow_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                _display.ToggleFullscreen();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _display.Close();
        }

        private void Ctm_ConvertEntities_OnClick(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog("Convert entities"))
            {
                dialog.Filters.Add(new CommonFileDialogFilter("XML files", ".xml"));
                dialog.Multiselect = true;
                if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    foreach (string file in dialog.FileNames)
                    {
                        SerializationManager.ConvertFromLegacy(file);
                    }
                }
            }
        }
    }
}