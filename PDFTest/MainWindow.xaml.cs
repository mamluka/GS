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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Serialization;
using System.Windows.Xps.Packaging;

namespace PDFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IDocumentPaginatorSource doc = this.flowdoc;
            SaveAsXps(doc);

           
        }

        public static int SaveAsXps(IDocumentPaginatorSource docref)
        {

            object doc;

            doc = docref;

            //FileInfo fileInfo = new FileInfo(fileName);



            //using (FileStream file = fileInfo.OpenRead())
            //{

            //    System.Windows.Markup.ParserContext context = new System.Windows.Markup.ParserContext();

            //    context.BaseUri = new Uri(fileInfo.FullName, UriKind.Absolute);

            //    doc = System.Windows.Markup.XamlReader.Load(file, context);

            //}

            string fileName = @"c:\a.xps";



            if (!(doc is IDocumentPaginatorSource))
            {

                Console.WriteLine("DocumentPaginatorSource expected");

                return -1;

            }



            using (Package container = Package.Open(fileName + ".xps", FileMode.Create))
            {

                using (XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.Maximum))
                {

                    XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);



                    DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;



                    // 8 inch x 6 inch, with half inch margin

                    paginator = new DocumentPaginatorWrapper(paginator, new Size(768, 676), new Size(24, 24));



                    rsm.SaveAsXaml(paginator);

                }

            }



            Console.WriteLine("{0} generated.", fileName + ".xps");



            return 0;

        }
    }
}
