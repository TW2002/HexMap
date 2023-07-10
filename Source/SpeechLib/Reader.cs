using System;
using System.Speech.Synthesis;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechLib;

public class Reader
{
    public EventHandler? Started;
    public EventHandler? Completed;
    public delegate void EventHandler(object sender, EventArgs e);
    private int readercount; 


    public String Voice  { get; set; }
    private SpeechSynthesizer reader;

    public Reader()
    {
        reader = new();
        reader.SpeakCompleted += OnCompleted;
    }

    private void OnCompleted(object? sender, SpeakCompletedEventArgs e)
    {
        if ((--readercount == 0) && (Completed != null))
            Completed(this, new EventArgs());
    }

    public void Read(String msg, String voice = null)
    {
        if ((readercount++ == 0) && (Started != null)) Started(this, new EventArgs());
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
