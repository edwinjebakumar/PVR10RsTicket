using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace _10PVRService
{
    
    public partial class PVRService : ServiceBase
    {
        private string url;


        public WebBrowser wb;
        private bool flag = false;
        private Thread thread=null;
        private string movieName="";

        public PVRService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logger(DateTime.Now + " PVR Service Started");
                base.OnStart(args);
                this.url = args[0].ToString();
                runBrowserThread();
            }
            catch (Exception ex)
            {
                Logger(ex.Message+" "+DateTime.Now + " PVR Service Starting Error");
            }
            
        }

        protected override void OnStop()
        {
            Logger(DateTime.Now + " PVR Service Stopped1");
            thread.Suspend();
            //Thread.Sleep(3000);
            Logger(DateTime.Now + " PVR Service Stopped2");
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ((sender as WebBrowser).ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                wb = sender as WebBrowser;
                if (wb.Document != null)
                {
                    try
                    {
                        HtmlElementCollection theElementCollection = default(HtmlElementCollection);
                        Thread.Sleep(1000);
                        theElementCollection = wb.Document.GetElementsByTagName("tbody");
                        if (theElementCollection.Count > 0)
                        {
                            HtmlElement curTbodyElement = theElementCollection[theElementCollection.Count - 1];
                            HtmlElementCollection trCollection = curTbodyElement.Children;
                            //Logger(DateTime.Now + " Document Loaded");
                            foreach (HtmlElement curElement1 in trCollection)
                            {
                                HtmlElementCollection seatCollection = curElement1.GetElementsByTagName("input");
                                foreach (HtmlElement item in seatCollection)
                                {
                                    if (item.GetAttribute("disabled") == "False")
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag)
                                    break;
                            }
                        }
                        else
                            Logger(DateTime.Now + " SeatLayout Not Available");
                       
                        if (flag)
                        {
                            Logger(DateTime.Now + " Seats Available!!!!!!!!!!!!!!");

                            const string accountSid = "AC1dc96f7cf13f0940e7e36b56cb17bf0b";
                            const string authToken = "3598f3e20cce6afb52bac039d101349b";
                            TwilioClient.Init(accountSid, authToken);

                            var to = new PhoneNumber("+917639489879");
                            var from = new PhoneNumber("+14159854361");
                            var call = CallResource.Create(to,
                                                           from,
                                                           url: new Uri("http://demo.twilio.com/docs/voice.xml"));
                            //string[] tokens = this.url.Split('/');
                            Regex regex = new Regex(@"\/[^/]*\?");
                            Match match = regex.Match(this.url);
                            if (match.Success)
                              movieName=  match.Value.TrimEnd('?').TrimStart('/').ToString();
                            var message = MessageResource.Create(
                            to,
                            from: new PhoneNumber("+14159854361"),
                            body: "Rs. 10 Ticket is available for: " + movieName);

                            //MessageBox.Show(call.Sid);
                            //MessageBox.Show("Tickets Available");
                            ServiceController sc = new ServiceController(this.ServiceName);
                            sc.Stop();
                        }
                        else
                        {
                            //Logger(DateTime.Now + " No Seats");
                            wb.Navigate(this.url);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger(ex.Message+" "+DateTime.Now + " Could not load document!");
                    }
                }

                else
                    Logger(DateTime.Now + " Document is Empty!");

            }
        }

        public static void Logger(String lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\test.txt", true);
            file.WriteLine(lines);

            file.Close();

        }

        
        public void runBrowserThread()
        {
            thread = new Thread(() => {
                wb = new WebBrowser();
                wb.AllowNavigation = true;
                wb.ScriptErrorsSuppressed = true;
                wb.DocumentCompleted += wb_DocumentCompleted;
                wb.Navigate(this.url);
                Application.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
