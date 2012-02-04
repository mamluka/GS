using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace GemScopeWPF.UI
{
    public class FolderBrowser
    {
        private object dummyNode = null;
        public string SelectedImagePath { get; set; }
        public TreeView FolderControl { get; set; }

        public string CurrentPath { get; set; }
        private static FolderBrowser Singelton { get; set; }

        public static FolderBrowser GetInstance()
        {
            if (Singelton == null)
            {
                Singelton = new FolderBrowser();
                return Singelton;
            }
            else
            {
                return Singelton;
            }
        }

        public void SetFolderControl(TreeView tree) 
        {
            this.FolderControl = tree;
        }

       
        public void Init(string path)
        {

            TreeViewItem item = new TreeViewItem();
            item.Header = Path.GetFileName(path);
            if (String.IsNullOrWhiteSpace((string)item.Header))
            {
                item.Header = Path.GetPathRoot(path);
            }
            item.Tag = path;
            item.FontWeight = FontWeights.Normal;
            item.Items.Add(dummyNode);
            item.Expanded += new RoutedEventHandler(folder_Expanded);
            item.IsExpanded = true;
            FolderControl.Items.Add(item);
            this.CurrentPath = path;

            ((TreeViewItem)FolderControl.Items[0]).IsSelected = true;


        }
        public void ReloadAfterChanges()
        {
            //FolderControl.Items.Clear();

        }
        public void ChangeHomeFolder(string path)
        {
            //TODO fix this depandancy
            if (this.FolderControl != null)
            {
                this.FolderControl.Items.Clear();
                this.Init(path);
            }
        }
        public void AddItemClickEvents()
        {
            this.FolderControl.SelectedItemChanged += foldersItem_SelectedItemChanged;

            this.FolderControl.SelectedItemChanged += StonesView.FolderBrowser_FolderChange;
            
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            

            TreeView tree = (TreeView)sender;
            if (tree.Items.Count > 0)
            {
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);
                this.CurrentPath = (string)temp.Tag;
            }

            //temp.Background =
        }
       
        public void CreateNewSubFolderUnderCurrentFolder() 
        {
            string foldername = Microsoft.VisualBasic.Interaction.InputBox("Enter a new subfolder name", "Create a subfolder...","subfolder");
            string newpath = Path.Combine(this.CurrentPath, foldername);
            Directory.CreateDirectory(newpath);

            FolderBrowser fb = FolderBrowser.GetInstance();
            

            TreeViewItem item = new TreeViewItem();
            item.Header = Path.GetFileName(newpath);
            item.Tag = newpath;
            item.FontWeight = FontWeights.Normal;
            
            item.Expanded += new RoutedEventHandler(fb.folder_Expanded);
           
            ((TreeViewItem)fb.FolderControl.SelectedItem).Items.Add(item);
            


        }
    }
}
