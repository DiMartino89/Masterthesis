using System.IO;
using UnityEngine;

namespace CSharpCompiler
{
    public class UnityLogTextWriter : TextWriter
    {
        TMPro.TextMeshProUGUI _errorMessageText;

        public UnityLogTextWriter() : base()
        {
            _errorMessageText = GameObject.FindGameObjectWithTag("ErrorMessage").GetComponent<TMPro.TextMeshProUGUI>();
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.ASCII; }
        }
        public override void Write(string value)
        {
            Debug.Log(value);

            _errorMessageText.text = value;
        }
    }
}