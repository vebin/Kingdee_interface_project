using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AutoReportDataToBank
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //定时器中断
        private void timer_click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
           
            if (now.Hour.ToString() == hour.Text.Trim() && now.Minute.ToString() == minute.Text.Trim() )
            {
                try
                {
                    HttpHelper get = new HttpHelper();
                    string result = get.Get("http://118.122.122.35:9081/api/report?orgNum=100");
                    if(result == "{\"code\":\"200\"}")
                    {
                        listBox1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + "执行成功");
                    }
                }
                catch
                {

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = false;
        }
    }
}
