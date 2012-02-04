using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GemScopeWPF.Repository
{
    public class BindingTest
    {
        private string m_Title;

        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }
        private List<string> m_List;
        

        public List<string> List
        {
            get { return m_List; }
            set { m_List = value; }
        }
        private string m_Image;

        public string Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }
        
        
        public BindingTest()
        {
            Title = "this is a test";

            List = new List<string>();
            List.Add("this");

            List.Add("is");
            List.Add("a");
            List.Add("test");

            Image = @"C:\Users\maMLUka\Documents\DJV\test1.jpg";
        }
        
    }
}
