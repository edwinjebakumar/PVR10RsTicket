using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace _10PVR
{
    public partial class Form1 : Form
    {
        private ServiceController service;
        private string _url = "";
        private bool flag = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                service = new ServiceController("PVRService");
                this._url = textBox1.Text;
                string[] args = new[] { textBox1.Text };
                if (service.Status == ServiceControllerStatus.Stopped)
                    service.Start(args);
                else
                {
                    service.Stop();
                    var timeout = new TimeSpan(0, 0, 59); // 59seconds
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    if (service.Status == ServiceControllerStatus.Stopped)
                        service.Start(args);
                    else
                        MessageBox.Show("Service: " + service.ServiceName + " unable to Start/Stop", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                flag = true;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if(!flag)
                    MessageBox.Show(this.FindForm(),"You can now Close the application", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

    }
}
