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

namespace VoiceCommandBot
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer s = new SpeechSynthesizer();
        Choices list = new Choices();
        public Form1()
        {
            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();
            list.Add(new string[] { "hello", "how are you", "what time is it" });
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
            s.Speak("Hello, My name is Voice Bot");
            InitializeComponent();
        }

        public void say(String h)
        {
            s.Speak(h);

        }

        private void rec_Speechrecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            //what you say
            if (r == "hello")
            {
                //what it says
                say("Hi");
            }

            if(r == "what time is it")
            {
                say(DateTime.Now.ToString("h:mm tt"));
            }

            if (r == "how are you")
            {
                say("Great, and you?");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
