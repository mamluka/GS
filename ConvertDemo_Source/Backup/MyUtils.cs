using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ConvertDemo
{
    class MyUtils
    {
        public static Control GetSelectedControlFromGroupBox(GroupBox group)
        {
            for (int i = 0; i < group.Controls.Count; i++)
            {
                if (group.Controls[i] is RadioButton)
                {
                    RadioButton rb = (RadioButton)group.Controls[i];
                    if (rb.Checked)
                    {
                        return rb;
                    }
                }
                else if (group.Controls[i] is CheckBox)
                {
                    CheckBox cb = (CheckBox)group.Controls[i];
                    if (cb.Checked)
                    {
                        return cb;
                    }
                }
            }

            return null;
        }

        public static int StrToIntWithDefValue(string strInt, int def)
        {
            if (strInt == null)
            {
                return def;
            }

            int result = int.Parse(strInt);
            return result > 0 ? result : def;
        }
    }
}
