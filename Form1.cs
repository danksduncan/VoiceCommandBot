using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Xml;

namespace VoiceCommandBot
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer s = new SpeechSynthesizer();

        Boolean wake = true;

        String temp;
        String cond;

        Choices list = new Choices();
        public Form1()
        {
            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();
            list.Add(new string[] { "hello", "how are you", "what time is it", "what is today", "open google", "wake", "sleep", "restart", "update", "open word document", "close word document",
            "whats the weather like", "whats the temperature" });

            Grammar gr = new Grammar(new GrammarBuilder(list));

            try
            {
                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_Speechrecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);
            }

            catch { return; }

            s.SelectVoiceByHints(VoiceGender.Female);
            InitializeComponent();
        }

        public String GetWeather(String input)
        {
            String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='city, state')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();
            wData.Load(query);

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                temp = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
                condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                high = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
                low = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value;
                if (input == "temp")
                {
                    return temp;
                }
                if (input == "cond")
                {
                    return condition;
                }
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }

        public static void killProg(String s)
        {
            System.Diagnostics.Process[] procs = null;

            try
            {
                procs = Process.GetProcessesByName(s);
                Process prog = procs[0];

                if (!prog.HasExited)
                {
                    prog.Kill();
                }
            }
            finally
            {
                if (procs != null)
                {
                    foreach(Process p in procs)
                    {
                        p.Dispose();
                    }
                } 
            }
        }

        public void restart()
        {
            Process.Start(@"C:\Users\VBot\VBot.exe");
            Environment.Exit(0);
        }

        public void say(String h)
        {
            s.Speak(h);
            textBox2.AppendText(h + "\n"); 
        }

        private void rec_Speechrecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            if (r == "wake")
            {
                wake = true;
                label3.Text = "State: Awake";
            }
            if (r == "sleep")
            {
                wake = false;
                label3.Text = "State: Sleep mode";
            }

            if (wake == true)
            {
                if (r == "open word document")
                {
                    Process.Start(@"C:\\Program Files\\Microsoft Office\\root\\Office16\\WINWORD.EXE");
                }

                if (r == "close word document")
                {
                    killProg("WINWORD.EXE");
                }

                if (r == "restart" || r == "update")
                {
                    restart();
                }

                //what you say
                if (r == "hello")
                {
                    //what it says
                    say("Hi");
                }

                if (r == "what time is it")
                {
                    say(DateTime.Now.ToString("h:mm tt"));
                }

                if (r == "what is today")
                {
                    say(DateTime.Now.ToString("M/d/yyyy"));
                }

                if (r == "how are you")
                {
                    say("Great, and you?");
                }

                //if (r == "open google")
                //{
                //    Process.Start("http://google.com");
                //}
            }
            textBox1.AppendText(r + "\n");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
