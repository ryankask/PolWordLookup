using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
    private PolWordLookup.ResultWindow resultWin;
    private PolWordLookup.PolishDictionaryService dictionary;
    
    public MainWindow(): base (Gtk.WindowType.Toplevel)
    {
        Build();

        dictionary = new PolWordLookup.PolishDictionaryService();

        if (!dictionary.Ready()) {
            DisplayError("No settings file found or settings missing or wrong. Quitting.");
            Destroy();
        }
    }
    
    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnLookupBtnClicked(object sender, System.EventArgs e)
    {
        if (wordEntry.Text == "") {
            DisplayError("Enter a word.");
            return;                                            
        }
        
        if (resultWin == null) {
            resultWin = new PolWordLookup.ResultWindow();
        } else if (!resultWin.Visible) {
            resultWin.Destroy();
            resultWin = new PolWordLookup.ResultWindow();
        } else {
            resultWin.Present();
        }
        
        String word = wordEntry.Text.Trim();
        resultWin.SetWord(word);
        dictionary.Lookup(word, resultWin.SetDefinition);
    }

    private void DisplayError(String errorMessage)
    {
        MessageDialog errorDialog = new MessageDialog(
            this,
            DialogFlags.DestroyWithParent,
            MessageType.Error,
            ButtonsType.Ok,
            errorMessage
        );
        errorDialog.Run();
        errorDialog.Destroy();
    }
}
