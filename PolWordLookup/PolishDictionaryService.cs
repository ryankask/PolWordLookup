using Gtk;
using HtmlAgilityPack;
using Nini.Config;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace PolWordLookup
{
    public delegate void SetDefinitionDelegate(String definition);

    public class PolishDictionaryService
    {
        private String LookupURL;
        private String XPathExpression;
        private Boolean IsReady = false;

        public PolishDictionaryService()
        {
            try {
                IniConfigSource settings = new IniConfigSource("settings.ini");
                LookupURL = settings.Configs["Dictionary"].Get("url");
                XPathExpression = settings.Configs["Dictionary"].Get("xpath-expression");

                if (LookupURL != null && XPathExpression != null) {
                    IsReady = true;
                }
            } catch (FileNotFoundException) {
                // Don't do anything
            }
        }

        public Boolean Ready()
        {
             return IsReady;
        }

        public void Lookup(String word, SetDefinitionDelegate setDefinition)
        {
            Application.Invoke(delegate {
                setDefinition("Loading...");
            });

            String URL = String.Format(this.LookupURL, word);
            HttpWebRequest request = HttpWebRequest.Create(URL) as HttpWebRequest;
            request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";

            request.BeginGetResponse(result => {
                FinishLookup(result, request, setDefinition);
            }, null);
        }

        private void FinishLookup(IAsyncResult result, HttpWebRequest request,
                                  SetDefinitionDelegate setDefinition)
        {
            string content;

            using (HttpWebResponse response = request.EndGetResponse(result) as HttpWebResponse) {
                Encoding responseEncoding = Encoding.GetEncoding(response.CharacterSet);

                using (StreamReader reader = new StreamReader(response.GetResponseStream(),
                                                              responseEncoding)) {
                    content = reader.ReadToEnd();
                }
            }

            Application.Invoke(delegate {
                setDefinition(ExtractDefinition(content));
            });
        }

        private String ExtractDefinition(String HTML)
        {
            HtmlDocument lookupDoc = new HtmlDocument();
            lookupDoc.LoadHtml(HTML);

            HtmlNode definitionNode = lookupDoc.DocumentNode.SelectSingleNode(XPathExpression);

            if (definitionNode == null) {
                return "Not found.";
            }

            return definitionNode.InnerText;
        }
    }
}