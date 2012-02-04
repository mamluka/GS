using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GemScopeWPF.Repository
{
    public abstract class Stone
    {
        public List<StoneInfoPart> InfoList { get; set; }
        public string FullFilePath { get; set; }
        public string Filename { get; set; }
        public int MediaType { get; set; }
        public string CompositeDescription { get; set; }

        public Stone()
        {
            InfoList = new List<StoneInfoPart>();

        }
        public string GetInfoByTitle(string title)
        {
            var q = from info in InfoList where info.Title == title select info;

            if (q.SingleOrDefault() != null)
            {
                return q.SingleOrDefault().Value;
            }
            else
            {
                return null; 
            }
            
        }
        

        
    }
}
