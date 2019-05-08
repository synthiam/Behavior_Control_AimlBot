using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EZ_Builder.Config.Sub;

namespace AimlBotter {

  public partial class FormOptions : Form {

    PluginV1 _cf = new PluginV1();
    FormMain _formMain;

    public FormOptions() {

      InitializeComponent();
    }

    public void SetConfiguration(PluginV1 cf, FormMain formMain) {

      _cf = cf;
      _formMain = formMain;

      tbResponseVariable.Text = cf.STORAGE[ConfigTitles.RESPONSE_VARIABLE].ToString();

      ucScriptResponse.Value = cf.STORAGE[ConfigTitles.RESPONSE_SCRIPT].ToString();
      ucScriptResponse.XML = cf.STORAGE[ConfigTitles.RESPONSE_XML].ToString();

      lblConfigFolder.Text = cf.STORAGE[ConfigTitles.CONFIG_ROOT_FOLDER].ToString();
    }

    public PluginV1 GetConfiguration() {

      _cf.STORAGE[ConfigTitles.RESPONSE_VARIABLE] = tbResponseVariable.Text;

      _cf.STORAGE[ConfigTitles.RESPONSE_SCRIPT] = ucScriptResponse.Value;
      _cf.STORAGE[ConfigTitles.RESPONSE_XML] = ucScriptResponse.XML;

      _cf.STORAGE[ConfigTitles.CONFIG_ROOT_FOLDER] = lblConfigFolder.Text;

      return _cf;
    }

    private void button2_Click(object sender, EventArgs e) {

      DialogResult = DialogResult.Cancel;
    }

    void save() {

      var sc = new EZ_Builder.Scripting.SyntaxCheck();

      tbResponseVariable.Text = tbResponseVariable.Text.Trim();

      if (!tbResponseVariable.Text.StartsWith("$") && tbResponseVariable.Text.Length < 2) {

        MessageBox.Show("Variable must start with a $");

        return;
      }

      DialogResult = DialogResult.OK;
    }

    private void button1_Click(object sender, EventArgs e) {

      _formMain._bot = null;

      save();
    }

    private void button3_Click(object sender, EventArgs e) {

      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() {
        FileName = lblConfigFolder.Text,
        UseShellExecute = true,
        Verb = "open"
      });
    }

    private void button4_Click(object sender, EventArgs e) {

      save();
    }

    private void button5_Click(object sender, EventArgs e) {

      textBox1.Text = textBox1.Text.Trim();

      if (textBox1.Text == string.Empty)
        return;

      string aimlFolder = EZ_Builder.Common.CombinePath(lblConfigFolder.Text, "aiml");

      List<string> _files = new List<string>();

      foreach (string file in Directory.GetFiles(aimlFolder, "*.aiml")) {

        string contents = File.ReadAllText(file);

        if (contents.ToLower().Contains(textBox1.Text.ToLower()))
          _files.Add(file);
      }

      if (_files.Count > 3)
        if (MessageBox.Show(string.Format("Your search appears in {0} AIML files. Do you wish to open them all? That's a lot of windows!", _files.Count), "Whoa!", MessageBoxButtons.YesNo) != DialogResult.Yes)
          return;

      if (_files.Count == 0) {

        MessageBox.Show("Your search text was not found in any AIML files.");

        return;
      }

      foreach (string file in _files)
        System.Diagnostics.Process.Start(file);
    }

    private void button4_Click_1(object sender, EventArgs e) {

      string editorBinary = EZ_Builder.Common.CombinePath(
        EZ_Builder.Constants.PLUGINS_FOLDER,
        _cf._pluginGUID,
        "GaitoBotEditor",
        "GaitoBotEditor.exe");

      System.Diagnostics.Process.Start(editorBinary);
    }

    private void btnChangeConfigFolder_Click(object sender, EventArgs e) {

      using (var ofd = new FolderBrowserDialog()) {

        ofd.SelectedPath = lblConfigFolder.Text;

        if (ofd.ShowDialog() != DialogResult.OK)
          return;

        lblConfigFolder.Text = ofd.SelectedPath;
      }
    }
  }
}
