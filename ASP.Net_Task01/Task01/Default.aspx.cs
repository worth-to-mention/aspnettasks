﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Task01
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ChangeHeaderButton_Click(object sender, EventArgs e)
        {
            Master.HeaderText = HeaderTextBox.Text;
        }
    }
}