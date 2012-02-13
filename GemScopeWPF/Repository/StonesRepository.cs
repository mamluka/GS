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
using Newtonsoft.Json;
using TagLib;
using File = TagLib.File;

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

        public Stone LoadStoneByFilenameInCurrentFolder(string filename)
        {
            try
            {
                using (var file = TagLib.File.Create(filename))
                {

                    if (file.Tag.Comment == null) return null;

                    var diamond = JsonConvert.DeserializeObject<Diamond>(file.Tag.Comment);

                    if (diamond == null) return null;

                    diamond.FullFilePath = filename;
                    diamond.Filename = Path.GetFileName(filename);



                    return diamond;
                }


            }
            catch (JsonReaderException)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("When tried to load the diamod data from file: " + filename + " an error occured:" + ex.Message);
            }
        }
        public bool CreateOrUpdateStone(BitmapSource bitmap, string filename, List<StoneInfoPart> infoparts)
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

                var diamond = new Diamond()
                                  {
                                      InfoList = infoparts,
                                      MediaType = 1,

                                  };

                using (var file = TagLib.File.Create(filename))
                {
                    file.GetTag(TagTypes.JpegComment, true);
                    file.Mode = File.AccessMode.Write;
                    file.Tag.Comment = JsonConvert.SerializeObject(diamond);

                    file.Save();
                }

                return true;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
              
            }

        }

        public bool CreateOrUpdateStoneMovie(string filename, List<StoneInfoPart> infoparts)
        {

            try
            {
                var diamond = new Diamond()
                {
                    InfoList = infoparts,
                    MediaType = 2

                };

                using (var file = TagLib.File.Create(filename))
                {
                    file.GetTag(TagTypes.JpegComment, true);
                    file.Mode = File.AccessMode.Write;
                    file.Tag.Comment = JsonConvert.SerializeObject(diamond);

                    file.Save();
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
            diamond.Filename = Path.GetFileName(filename);
            diamond.FullFilePath = filename;
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

            if (System.IO.File.Exists(xmlfile))
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
            return System.IO.File.Exists(filename);
        }

    }
}
