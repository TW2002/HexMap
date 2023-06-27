using System;
using System.Speech.Synthesis;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechLib;

public class Reader
{
    public String Voice  { get; set; }
    private SpeechSynthesizer reader;

    public Reader()
    {
        reader = new();
        const string msg = "Greetings programs";
        //Console.WriteLine(msg);
        reader.SpeakAsync(msg);
    }

    public void Read(String msg, String voice = null)
    {
        if (voice != null) Voice = voice;

        switch (voice)
        {
            case "Qubot":
                //reader.Voice = ;
                reader.SelectVoiceByHints(VoiceGender.Female , VoiceAge.Teen); // to change VoiceGender and VoiceAge check out those links below
                reader.Volume = 100;  // (0 - 100)
                reader.Rate = 0;     // (-10 - 10)

                break;
        }

        reader.SpeakAsync(msg);
    }

}
