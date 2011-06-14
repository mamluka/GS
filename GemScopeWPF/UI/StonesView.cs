using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using GemScopeWPF.Repository;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Input;
using WPFMediaKit.DirectShow.Controls;
namespace GemScopeWPF.UI
{
    public class StonesView
    {
        public static ListBox ViewControl { get; set; }
        public static ItemsControl StoneInfoDisplayControl { get; set; }
        public static string CurrentPath { get; set; }

        private static int _sortproperty;
        private static int _sortpropertydirection;
        public static int SortProperty
        {
            get { return _sortproperty; }
            set { 
                _sortproperty = value;
                //LoadImagesToImageView(CurrentPath);
            }
        }


        public static int SortPropertyDirection
        {
            get { return _sortpropertydirection; }
            set { 
                _sortpropertydirection = value;
                //LoadImagesToImageView(CurrentPath);
            }
        }
        
        
   

        static public void InitStonesView() 
        {
            ViewControl.SelectionChanged += new SelectionChangedEventHandler(ViewControl_SelectionChanged);
            SortProperty = 1;
            SortPropertyDirection = 1;
            
        }

       
        public static void FolderBrowser_FolderChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            if (tree.Items.Count > 0)
            {
                TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

                string SelectedImagePath = (string)temp.Tag;
                //show user selected path
                // MessageBox.Show(SelectedImagePath);
                LoadImagesToImageView(SelectedImagePath);
            }
        }
        public static void RefreshView() {
            LoadImagesToImageView(CurrentPath);
        }
        public static void LoadImagesToImageView(string path)
        {
            CurrentPath = path;

            //var files = Directory.GetFiles(path, "*.jpg");

            string[] fnsjpg = Directory.GetFiles(path, "*.jpg");
           // string[] fnswmv = Directory.GetFiles(path, "*.wmv");
            string[] fnswmv = Directory.GetFiles(path, "*.mp4");

            string[] fnsmp4 = Directory.GetFiles(path, "*.wmv");

            List<string> fns = fnsjpg.Union(fnswmv).Union(fnsmp4).ToList<string>();

            StonesRepository rep = new StonesRepository(CurrentPath);

            
            switch (_sortproperty)
            {
                case 1:
                    if (_sortpropertydirection == 1)
                    {
                        fns = fns.OrderBy(m => new FileInfo(m).Name).ToList<string>();
                    }
                    else
                    {
                        fns = fns.OrderByDescending(m => new FileInfo(m).Name).ToList<string>();
                    }
                    
                    break;
                case 2:
                    if (_sortpropertydirection == 1)
                    {
                        fns = fns.OrderBy(m => new FileInfo(m).LastWriteTime).ToList<string>();
                    }
                    else
                    {
                        fns = fns.OrderByDescending(m => new FileInfo(m).LastWriteTime).ToList<string>();
                    }
                    
                    break;
                case 3:
                    if (_sortpropertydirection == 1)
                    {
                        fns = fns.OrderBy(m => new FileInfo(m).Extension).ToList<string>();
                    }
                    else
                    {
                        fns = fns.OrderByDescending(m => new FileInfo(m).Extension).ToList<string>();
                    }
                    
                    break;
                case 4:
                    if (_sortpropertydirection == 1)
                    {
                        fns = fns.OrderBy(m => new FileInfo(m).Length).ToList<string>();
                    }
                    else
                    {
                        fns = fns.OrderByDescending(m => new FileInfo(m).Length).ToList<string>();
                    }
                    
                    break;
                case 5:
                    List<string> fns_withdata = fns.Where(m => rep.LoadStoneByFilenameInCurrentFolder(Path.GetFileName(m)) != null).ToList<string>();
                    List<string> fns_withoutdata = fns.Where(m => rep.LoadStoneByFilenameInCurrentFolder(Path.GetFileName(m)) == null).ToList<string>();
                    if (_sortpropertydirection == 1)
                    {

                        fns_withdata = fns_withdata.OrderBy(m => rep.LoadStoneByFilenameInCurrentFolder(Path.GetFileName(m)).GetInfoByTitle("CaratWeight")).ToList<string>();
                    }
                    else
                    {
                        fns_withdata = fns_withdata.OrderByDescending(m => rep.LoadStoneByFilenameInCurrentFolder(Path.GetFileName(m)).GetInfoByTitle("CaratWeight")).ToList<string>();
                    }

                    fns = fns_withdata.Union(fns_withoutdata).ToList<string>();
                    
                    break;
                default:
                    break;
            }

            

            ViewControl.Items.Clear();

            foreach (var file in fns)
            {
                StackPanel stack = new StackPanel();
                
                Image img = new Image();

                MemoryStream ms = new MemoryStream();
                BitmapImage src = new BitmapImage();

                if (Path.GetExtension(file) == ".jpg")
                {

                    FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    ms.SetLength(stream.Length);
                    stream.Read(ms.GetBuffer(), 0, (int)stream.Length);

                    ms.Flush();
                    stream.Close();

                    

                    src.BeginInit();
                    src.StreamSource = ms;
                    src.EndInit();
                    img.Source = src;
                    
                }
                else if (Path.GetExtension(file) == ".mp4" || Path.GetExtension(file) == ".wmv")
                {
                    //MediaPlayer mp = new MediaPlayer();
                    //mp.ScrubbingEnabled = true;
                    //mp.Open(new Uri(file,UriKind.Absolute));
                    //mp.Play();
                    //mp.MediaOpened += new EventHandler(mp_MediaOpened);
                    //mp.Position = new TimeSpan(0, 0, 2);
                    //RenderTargetBitmap rtb = new RenderTargetBitmap(640, 480, 1/200, 1/200, PixelFormats.Default);
                    //DrawingVisual dv = new DrawingVisual();
                    //DrawingContext dc = dv.RenderOpen();
                    //dc.DrawVideo(mp, new Rect(0, 0, 640, 480));
                    //dc.Close();
                    //rtb.Render(dv);                   
                    //img.Source = BitmapFrame.Create(rtb);
                    //mp.Stop();
                    //mp.Close();

                    //MediaPlayer _mediaPlayer = new MediaPlayer();
                    //_mediaPlayer.ScrubbingEnabled = true;
                    //_mediaPlayer.Open(new Uri(file, UriKind.Absolute));
                    //uint[] framePixels;
                    //uint[] previousFramePixels;

                    //framePixels = new uint[640 * 480];
                    //previousFramePixels = new uint[framePixels.Length];

                    //var drawingVisual = new DrawingVisual();
                    //var renderTargetBitmap = new RenderTargetBitmap(640, 480, 96, 96, PixelFormats.Default);
                    //using (var drawingContext = drawingVisual.RenderOpen())
                    //{
                    //    drawingContext.DrawVideo(_mediaPlayer, new Rect(0, 0, 640, 480));
                    //}
                    //renderTargetBitmap.Render(drawingVisual);

                    //// Copy the pixels to the specified location
                    //renderTargetBitmap.CopyPixels(previousFramePixels, 640 * 4, 0);

                    src.BeginInit();
                    src.UriSource = new Uri(@"/GemScopeWPF;component/Media/movieplaceholder.jpg", UriKind.RelativeOrAbsolute);
                    src.EndInit();

                    img.Source = src;
                 
                    


                    

                }

                
               
                double[] wh = new double[] {100,100};

 
                
                

                img.Width = wh[0];
                img.Height = wh[1];

                
                img.Margin = new Thickness(5);

                

                TextBlock blk1 = new TextBlock();
                if (Path.GetFileNameWithoutExtension(file).Length > 20)
                {
                    blk1.Text = Path.GetFileNameWithoutExtension(file).Substring(0,17)+"...";
                }
                else
                {
                    blk1.Text = Path.GetFileNameWithoutExtension(file);
                }
                
                blk1.Margin = new Thickness(5, 0, 0, 0);
                blk1.TextAlignment = TextAlignment.Center;
                stack.Children.Add(img);
                stack.Children.Add(blk1);

                stack.Tag = Path.GetFileName(file);

                

             //   stack.VerticalAlignment = VerticalAlignment.Top;
              //  stack.HorizontalAlignment = HorizontalAlignment.Left;

               // stack.Width = wh[0]+10;
              //  stack.Height = wh[1] + 10;


                stack.MouseDown += new System.Windows.Input.MouseButtonEventHandler(stack_MouseDown);

                stack.MouseRightButtonDown += new MouseButtonEventHandler(stack_MouseRightButtonDown);


                ViewControl.Items.Add(stack);



            }


        }

        static void stack_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
           // MessageBox.Show((string)((StackPanel)sender).Tag);


        }

        static void mp_MediaOpened(object sender, EventArgs e)
        {
            MessageBox.Show("started");
        }

        static void stack_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                StonesView.EditStoneItem();
            }
        }

       

        static void ViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ( ((ListBox)sender).SelectedItem != null )
            {
                string filename = (string)((StackPanel)((ListBox)sender).SelectedItem).Tag;

                StonesRepository rep = new StonesRepository(CurrentPath);

                Stone stone = rep.LoadStoneByFilenameInCurrentFolder(filename);

                if (stone != null)
                {
                    StoneInfoDisplayControl.ItemsSource = stone.InfoList;
                }
                else
                {
                    StoneInfoDisplayControl.ItemsSource = new List<StoneInfoPart>();
                }
            }

           
            


        }
        static public string GetCurrentSelectedStoneFilename()
        {
            if (ViewControl.SelectedItem != null)
            {
                string filename = (string)((StackPanel)(ViewControl).SelectedItem).Tag;
                string file = Path.Combine(CurrentPath, filename);

                return file;
            }
            return String.Empty;
        }
        static public List<string> GetCurrentSelectedStoneFilenames()
        {
            List<string> list = new List<string>();

            if (ViewControl.SelectedItems.Count > 0)
            {
                foreach (var item in ViewControl.SelectedItems)
                {
                   
                    string filename = (string)((StackPanel)(item)).Tag;
                    string file = Path.Combine(CurrentPath, filename);


                    if (!String.IsNullOrEmpty(filename))
                    {
                        list.Add(file);
                    }
                }
                return list;
            }
            else
            {
                return list;
            }
        }
        static public Stone GetCurrentSelectedStone()
        {
            if (ViewControl.SelectedItem != null)
            {
                StonesRepository rep = new StonesRepository(CurrentPath);
                //TODO move this to  a property getter
                string filename = (string)((StackPanel)(ViewControl).SelectedItem).Tag;

                Stone stone = rep.LoadStoneByFilenameInCurrentFolder(filename);

                if (stone != null)
                {
                    return stone;
                }
                else
                {
                    return rep.CreateAnewStoneSkeleton(filename);

                }


            }

            return null;
        }
        static public List<Stone> GetCurrentSelectedStones()
        {
            List<Stone> list = new List<Stone>();

            if (ViewControl.SelectedItems.Count > 0)
            {
                foreach (var item in ViewControl.SelectedItems)
                {
                    StonesRepository rep = new StonesRepository(CurrentPath);
                    //TODO move this to  a property getter
                    string filename = (string)((StackPanel)(item)).Tag;

                    Stone stone = rep.LoadStoneByFilenameInCurrentFolder(filename);

                    if (stone != null)
                    {
                        list.Add(stone);
                    }
                }
                return list;
            }
            else
            {
                return list;
            }
        }

        static public void OpenRenameDialogAndPreformRename(string file)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string ext = Path.GetExtension(file);
            string newfilename = Microsoft.VisualBasic.Interaction.InputBox("Enter a new filename for the current stone", "Rename the stone", filename);

            if (!String.IsNullOrWhiteSpace(newfilename))
            {
                File.Move(file, Path.Combine(CurrentPath, newfilename + ext));

                StonesRepository rep = new StonesRepository(CurrentPath);
                rep.RenameStone(Path.GetFileName(file),newfilename + ext);

                RefreshView();
            }

        }

        static public void PreformDeleteOfStone(string file)
        {
            //string filename = Path.GetFileNameWithoutExtension(file);
          //  string ext = Path.GetExtension(file);
       //     string newfilename = Microsoft.VisualBasic.Interaction.InputBox("Enter a new filename for the current stone", "Rename the stone", filename);

            if (File.Exists(file))
            {
                File.Delete(file);

                StonesRepository rep = new StonesRepository(CurrentPath);
                rep.DeleteStone(Path.GetFileName(file));

                RefreshView();
                
            }

        }

        static public void PreformSaveAsOfStone(string file)
        {
            SaveFileDialog savedlg = new SaveFileDialog();
            savedlg.Filter = "JPeg Image|*.jpg";
            savedlg.Title = "Save a stone image";
            savedlg.ShowDialog();
            savedlg.DefaultExt = "jpg";
            if (!String.IsNullOrEmpty(savedlg.FileName))
            {

                File.Copy(file, savedlg.FileName);

            }
        }

        static public void EditStoneItem()
        {
            Stone stone = StonesView.GetCurrentSelectedStone();



            if (stone != null)
            {

                if (stone.MediaType == 1)
                {

                    NewStone newstone = new NewStone();
                    newstone.CurrentStone = stone;
                    newstone.EditMode = true;
                    newstone.FolderUponCaptureEvent = StonesView.CurrentPath;
                    newstone.ShowDialog();
                }
                else if (stone.MediaType == 2)
                {
                    NewStomeMovie newstonemovie = new NewStomeMovie();
                    newstonemovie.CurrentStone = stone;
                    newstonemovie.EditMode = true;
                    newstonemovie.FolderUponCaptureEvent = StonesView.CurrentPath;
                    newstonemovie.ShowDialog();
                }
            }
        }

       
    }
}
