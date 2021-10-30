using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhatsappTracking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public static ChromeDriver driver { get; set; }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static async void StartDriver(string proxy)
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions chromeOptions = new ChromeOptions();
            if (!string.IsNullOrEmpty(proxy))
            {
                chromeOptions.AddArgument("--proxy-server="+proxy);
            }
            chromeOptions.AddArgument("disable-infobars");
            chromeOptions.AddArgument("--disable-web-security");
            chromeOptions.AddArgument("--allow-running-insecure-content");
            driver = new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromMinutes(3.0));
            await Task.Delay(2000);
        }


        public bool IsTestElementPresent(By element)
        {
            try
            {
                driver.FindElement(element);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter the number");
                    return;
                }
                StartDriver(null);

                driver.Navigate().GoToUrl("https://web.whatsapp.com");
                await Task.Delay(10000);
                driver.Navigate().GoToUrl("https://api.whatsapp.com/send?phone=" + textBox1.Text);
                await Task.Delay(3000);
                driver.FindElement(By.CssSelector("#action-button")).Click();
                await Task.Delay(2000);
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
                driver.SwitchTo().Alert().Accept();
                await Task.Delay(2000);
            });
            timer1.Enabled = true;
        }

        bool status = true;
        public void Control()
        {
            try
            {
                if (IsTestElementPresent(By.CssSelector("#main > header > div._3V5x5 > div._3Q3ui.i1XSV")))
                {
                    if (IsTestElementPresent(By.CssSelector("#main > header > div._3V5x5 > div._3Q3ui.i1XSV > span")))
                    {
                        if (status)
                        {
                            listView1.Items.Add(textBox1.Text + " is login - " + DateTime.Now);
                            status = false;
                        }
                    }
                }
                else
                {
                    if (!status)
                    {
                        listView1.Items.Add(textBox1.Text + " is logout - " + DateTime.Now);
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                listView1.Items.Add(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"WPTracking.txt";
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (var item in listView1.Items)
                    {
                        sw.WriteLine(item.ToString());
                    }
                    sw.Close();
                    MessageBox.Show("Successfully Saved!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Control();
        }
    }
}
