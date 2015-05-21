using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSManager
{
    public partial class WCFConnTest : Form
    {
        public string Url;
        public WCFConnTest()
        {
            InitializeComponent();
        }

        private void WCFConnTest_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(Url);
        }

    }
}
