using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using System.Configuration;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GemScopeWPF.Utils;

namespace GemScopeWPF.Repository
{

    public class  StonesRepository
    {
        public List<StoneInfoPart> StoneInfo { get; set; }
        public bool IsExtendedStoneInfoExists { get; set; }
        public string StoneRepXMLFilenameWithoutPath { get; set; }
        public string XmlFile { get; set; }
        public string XmlFilePath { get; set; }
        private XDocument Xdoc { get; set; }
        /// <summary>
        /// Build a new rep object
        /// </summary>
        /// <param name="path">The path of the folder that we are looking at</param>
        public StonesRepository(string path)
        {
            StoneInfo = new List<StoneInfoPart>();
            IsExtendedStoneInfoExists = false;



            StoneRepXMLFilenameWithoutPath = SettingsManager.ReadSetting("StoneRepXMLFilename");

            //inforce default
            if (String.IsNullOrWhiteSpace(StoneRepXMLFilenameWithoutPath))
            {
                StoneRepXMLFilenameWithoutPath = "stonerep.xml";
            }
            
            XmlFile = Path.Combine(path,StoneRepXMLFilenameWithoutPath);
            XmlFilePath = path;

            this.IsExtendedStoneInfoPresent(path);

            if (IsExtendedStoneInfoExists)
            {
                Xdoc = XDocument.Load(XmlFile);
            }
            else
            {
                Xdoc = null;
            }



            
        }
        /// <summary>
        /// Loads the stone using the filename in the current folder
        /// </summary>
        /// <param name="file">The name of the file in the folder without the path</param>
        /// <returns></returns>
        public Stone LoadStoneByFilenameInCurrentFolder(string file)
        {
            if (IsExtendedStoneInfoExists)
            {
                var q = from s in Xdoc.Root.Elements("stone")
                        where s.Attribute("filename").Value == file
                        select s;

                if (q.SingleOrDefault() != null)
                {
                    Diamond diamond = new Diamond();

                    var infoparts = from info in q.SingleOrDefault().Elements("info")
                                    select new StoneInfoPart
                                    {
                                        Title = info.Attribute("title").Value,
                                        Value = info.Value,
                                        TitleForReport=info.Attribute("titleforreport").Value
                                    };
                    

                    foreach (var infopart in infoparts)
                    {
                        diamond.InfoList.Add(infopart);
                    }

                    diamond.Filename = file;
                    diamond.FullFilePath = Path.Combine(this.XmlFilePath, file);
                    diamond.MediaType =Convert.ToInt32(q.SingleOrDefault().Attribute("mediatype").Value);

//                   var weight = infoparts.Where(m=> m.Title == "CaratWeight").SingleOrDefault<StoneInfoPart>().Value;
//                   var type = infoparts.Where(m => m.Title == "StoneType").SingleOrDefault<StoneInfoPart>().Value;
//                    var color = infoparts.Where(m=> m.Title == "StoneColor").SingleOrDefault<StoneInfoPart>().Value;
//                    var clarity = infoparts.Where(m=> m.Title == "StoneClarity").SingleOrDefault<StoneInfoPart>().Value;
//
//
//
//                    diamond.CompositeDescription = "A " + weight + " Ct. " + color + "/" + clarity + " " + type.ToLower() + " diamond.";


                    return diamond;



                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public bool CreateANewStone(BitmapSource bitmap, string filename, List<StoneInfoPart> infoparts)
        {

            try
            {
                //save the Jpeg file
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.QualityLevel = 100;

                FileStream fstream = new FileStream(filename, FileMode.Create);
                encoder.Save(fstream);
                fstream.Close();
                fstream.Dispose();
                //TODO in the future may support other types
                string type = "diamond";

                if (!IsExtendedStoneInfoExists)
                {
                    this.CreateEmptyStoneRepXMLFile(XmlFilePath);
                }

                if (this.LoadStoneByFilenameInCurrentFolder(Path.GetFileName(filename)) == null)
                {

                    Xdoc.Root.Add(
                        new XElement("stone",
                            new XAttribute("type", type),
                            new XAttribute("mediatype", 1),
                            new XAttribute("filename", Path.GetFileName(filename)),
                            new XAttribute("createdate", DateTime.Today.Ticks),
                            from infopart in infoparts
                            select new XElement("info",
                                new XAttribute("title", infopart.Title),
                                 new XAttribute("titleforreport", infopart.TitleForReport),
                                new XElement("value", new XCData(infopart.Value)))
                                ));


                    Xdoc.Save(XmlFile);
                }
                else
                {
                    this.UpdateStone(filename, infoparts);
                }

                return true;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
              
            }

        }

        public bool CreateANewStoneMovie(string filename, List<StoneInfoPart> infoparts)
        {

            try
            {
                //save the Jpeg file
                
                //TODO in the future may support other types
                string type = "diamond";

                if (!IsExtendedStoneInfoExists)
                {
                    this.CreateEmptyStoneRepXMLFile(XmlFilePath);
                }
                if (this.LoadStoneByFilenameInCurrentFolder(Path.GetFileName(filename)) == null)
                {
                    Xdoc.Root.Add(
                        new XElement("stone",
                            new XAttribute("type", type),
                            new XAttribute("mediatype", 2),
                            new XAttribute("filename", Path.GetFileName(filename)),
                            new XAttribute("createdate", DateTime.Today.Ticks),
                            from infopart in infoparts
                            select new XElement("info",
                                new XAttribute("title", infopart.Title),
                                new XAttribute("titleforreport", infopart.TitleForReport),
                                new XElement("value", new XCData(infopart.Value)))
                                ));


                    Xdoc.Save(XmlFile);
                }
                else
                {
                    this.UpdateStone(filename, infoparts);
                }

                return true;


            }
            catch (Exception)
            {

                return false;

            }

        }
        public Stone CreateAnewStoneSkeleton(string filename)
        {
            Diamond diamond = new Diamond();
            diamond.Filename = filename;
            diamond.FullFilePath = Path.Combine(this.XmlFilePath, filename);
            if (Path.GetExtension(filename) == ".jpg")
            {
                diamond.MediaType = 1;
            }
            else
            {
                diamond.MediaType = 2;
            }
            

            return diamond;

        }
        public bool IsStoneExistsInRep(string filename)
        {
            if (IsExtendedStoneInfoExists)
            {
                var q = Xdoc.Root.Elements("stone").Where(m => m.Attribute("filename").Value == filename).SingleOrDefault();
                if (q != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public bool UpdateStone(string filename, List<StoneInfoPart> infoparts)
        {

            try
            {

                if (!IsExtendedStoneInfoExists)
                {
                    this.CreateEmptyStoneRepXMLFile(XmlFilePath);
                }

                var q = Xdoc.Root.Elements("stone").Where(m => m.Attribute("filename").Value == Path.GetFileName(filename)).SingleOrDefault();

                if (q != null)
                {
                    q.Elements("info").Remove();
                    q.Add(from infopart in infoparts
                          select new XElement("info",
                              new XAttribute("title", infopart.Title),
                              new XAttribute("titleforreport", infopart.TitleForReport),
                              new XElement("value", new XCData(infopart.Value)))


                            );
                }


                Xdoc.Save(XmlFile);

                return true;


            }
            catch (Exception)
            {

                return false;

            }

        }

        public bool DeleteStone(string filename, bool deleteondisk = false)
        {

            if (IsExtendedStoneInfoExists)
            {
                try
                {
                    Xdoc.Root.Elements("stone").Where(m => m.Attribute("filename").Value == filename).Remove();
                    Xdoc.Save(XmlFile);
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
                
            }
            else
            {
                return false;
            }
        }

        public bool RenameStone(string oldfilename,string newfilename)
        {

            if (IsExtendedStoneInfoExists)
            {
                try
                {
                    
                    Xdoc.Root.Elements("stone").Where(m => m.Attribute("filename").Value == oldfilename).Single().Elements("info").Where(m => m.Attribute("title").Value == "Filename").Single().Element("value").ReplaceWith(new XElement("value", new XCData(Path.GetFileNameWithoutExtension(newfilename))));
                    Xdoc.Root.Elements("stone").Where(m => m.Attribute("filename").Value == oldfilename).Single<XElement>().Attribute("filename").Value = newfilename;
                    Xdoc.Save(XmlFile);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        

            
        private void CreateEmptyStoneRepXMLFile(string path)
        {

            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("StoneRep"), new XElement("stones"));

            xdoc.Save(XmlFile);

            Xdoc = XDocument.Load(XmlFile);
        }
        private bool IsExtendedStoneInfoPresent(string path)
        {
          
            string xmlfile = Path.Combine(path, this.StoneRepXMLFilenameWithoutPath);

            if (File.Exists(xmlfile))
            {
                IsExtendedStoneInfoExists = true;
            } else 
            {
                IsExtendedStoneInfoExists = false;
            }

            

            return false;
        }

        public bool IsStoneExists(string filename)
        {
            return File.Exists(filename);
        }

    }
}
