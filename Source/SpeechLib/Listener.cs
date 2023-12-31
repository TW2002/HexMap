﻿using System;
using System.Speech.Recognition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;
using SpeechLib;

namespace SpeechLib;

public class Listener
{
    /// <summary>
    /// A speech recognition event has occurred.
    /// </summary>
    public EventHandler? EventRecognized;
    public delegate void EventHandler(object sender, RecognizedArgs e);

    private Reader reader;
    private bool recognizermuted;
    private RecognizedArgs eArgs = new();
    private SpeechRecognitionEngine recognizer;

    public Listener(Reader r = null)
    {   if (r != null)
        {
            reader = r;
            reader.Started += OnReaderStarted;
            reader.Completed += OnReaderCompleted;
        }


        // Create an in-process speech recognizer for the en-US locale.  
        recognizer = new(new CultureInfo("en-US"));
        AddEvent(recognizer, "Omega");
        AddEvent(recognizer, "Omega", "Omega 42");
        AddEvent(recognizer, "Connect");
        AddEvent(recognizer, "ConnectAll", "Connect All");
        AddEvent(recognizer, "Disconnect", "Connect All");
        AddEvent(recognizer, "DisconnectAll", "Connect All");
        AddEvent(recognizer, "Kill Shadow");
        //AddEvent(recognizer, "");



        // Create and load the exit grammar.  
        //Grammar exitGrammar = new Grammar(new GrammarBuilder("exit"));
        //exitGrammar.Name = "Exit Grammar";
        //recognizer.LoadGrammar(exitGrammar);

        // Create and load the dictation grammar.  
        Grammar dictation = new DictationGrammar();
        dictation.Name = "Dictation Grammar";
        recognizer.LoadGrammar(dictation);

        // Attach event handlers to the recognizer.  
        recognizer.SpeechRecognized +=
          new EventHandler<SpeechRecognizedEventArgs>(
            SpeechRecognizedHandler);
        recognizer.RecognizeCompleted +=
          new EventHandler<RecognizeCompletedEventArgs>(
            RecognizeCompletedHandler);

        // Assign input to the recognizer.  
        recognizer.SetInputToDefaultAudioDevice();

        // Begin asynchronous recognition.  
        //Debug.WriteLine("Starting recognition...");
        //completed = false;
        recognizer.RecognizeAsync(RecognizeMode.Multiple);

    }

    private void OnReaderCompleted(object sender, EventArgs e)
    {
        if (recognizermuted)
        {
            recognizermuted = false;

            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
    }

    private void OnReaderStarted(object sender, EventArgs e)
    {
        if (!recognizermuted)
        {
            recognizermuted = true;
            //((SpeechRecognitionEngine)sender).RecognizeAsyncCancel();
            recognizer.RecognizeAsyncCancel();
        }
    }

    private void AddEvent(SpeechRecognitionEngine r, string name, string text = null, string value = null)
    {
        // Create and load the grammar.  
        if (String.IsNullOrEmpty(text))
        {
            text = name;
        }
        Grammar g = new Grammar(new GrammarBuilder(text));
        g.Name = name;

        if (!String.IsNullOrEmpty(value))
        {
            g.Name += "," + value;    
        }
        r.LoadGrammar(g);
    }

    private void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
    {
        if (!String.IsNullOrEmpty(e.Result.Grammar.Name))
        {
            eArgs.Name = e.Result.Grammar.Name;
            eArgs.Text = e.Result.Text;
            eArgs.Value = "";

            if (EventRecognized != null) EventRecognized(this, eArgs);
        }
        
        //Debug.WriteLine("  Speech recognized:");
        string grammarName = "<not available>";
        if (e.Result.Grammar.Name != null &&
          !e.Result.Grammar.Name.Equals(string.Empty))
        {
            grammarName = e.Result.Grammar.Name;
        }
        //Debug.WriteLine("    {0,-17} - {1}",
        //  grammarName, e.Result.Text);

        if (grammarName.Equals("Exit Grammar"))
        {
            ((SpeechRecognitionEngine)sender).RecognizeAsyncCancel();
        }
    }


    private void RecognizeCompletedHandler(object sender, RecognizeCompletedEventArgs e)
    {
        //Debug.WriteLine("  Recognition completed.");
        //completed = true;
    }
}

public class RecognizedArgs : EventArgs
{
    public String? Name { get; set; }
    public String? Value { get; set; }
    public String? Text { get; set; }

    // Empty constructor .
    public RecognizedArgs() {}
}