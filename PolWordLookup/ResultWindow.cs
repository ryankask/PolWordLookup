using System;
using Gtk;

namespace PolWordLookup
{
    public partial class ResultWindow : Gtk.Window
    {
        public ResultWindow () : 
                base(Gtk.WindowType.Toplevel)
        {
            this.Build ();
        }
        
        public void SetWord(String word)
        {
            resultForWordLbl.LabelProp = word;
            Title = String.Format("Results for \"{0}\"", word);
        }
        
        public void SetDefinition(String definition)
        {
            definitionTxt.Buffer.Text = definition;
        }
    }
}

