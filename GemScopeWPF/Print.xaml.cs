using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GemScopeWPF.UI;
using GemScopeWPF.Repository;
namespace GemScopeWPF
{
    /// <summary>
    /// Interaction logic for Print.xaml
    /// </summary>
    public partial class Print : Window
    {
        public Print()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Stone> stones =  StonesView.GetCurrentSelectedStones().Where(m=> m.MediaType==1).ToList<Stone>();

            for (int i = 0; i < stones.Count; i+=2)
            {

                TableRow row = new TableRow();
                TableCell cell1 = new TableCell();

                Paragraph p1 = new Paragraph();

                Image img1 = new Image();
                
                BitmapImage src1 = new BitmapImage();
                src1.BeginInit();
                src1.UriSource = new Uri(stones[i].FullFilePath, UriKind.Absolute);
                src1.EndInit();

                img1.Source = src1;

                TextBlock block1 = new TextBlock();
                
               

                block1.Text = stones[i].CompositeDescription;
                block1.Style = (Style)FindResource("stoneDescription");

                p1.Inlines.Add(img1);
                p1.Inlines.Add(block1);

                cell1.Blocks.Add(p1);

                row.Cells.Add(cell1);
                //check if we add the second cell to the current row

                if (i + 1 < stones.Count)
                {

                    TableCell cell2 = new TableCell();

                    Paragraph p2 = new Paragraph();

                    Image img2 = new Image();
                    
                    BitmapImage src2 = new BitmapImage();
                    src2.BeginInit();
                    src2.UriSource = new Uri(stones[i+1].FullFilePath, UriKind.Absolute);
                    src2.EndInit();

                    img2.Source = src2;

                    TextBlock block2 = new TextBlock();

                    block2.Style = (Style)FindResource("stoneDescription");

                    block2.Text = stones[i + 1].CompositeDescription;

                    p2.Inlines.Add(img2);
                    p2.Inlines.Add(block2);

                    cell2.Blocks.Add(p2);

                    row.Cells.Add(cell2);
                }

                flowRowsContainer.Rows.Add(row);





            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {

                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocument).DocumentPaginator, "Flow Document Print Job");

            }
        }

        
    }
}
