using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EZ_Builder;
using EZ_Builder.Config.Sub;

namespace AimlBotter {

  public partial class FormMain : EZ_Builder.UCForms.FormPluginMaster {

    readonly string _EXECUTOR_NAME = "AimlBotter";

    public AIMLbot.Bot _bot;
    AIMLbot.User _user;

    public FormMain() {

      InitializeComponent();
    }

    public override void SetConfiguration(PluginV1 cf) {

      cf.STORAGE.AddIfNotExist(ConfigTitles.RESPONSE_VARIABLE, "$BotResponse");
      cf.STORAGE.AddIfNotExist(ConfigTitles.RESPONSE_SCRIPT, string.Empty);
      cf.STORAGE.AddIfNotExist(ConfigTitles.RESPONSE_XML, string.Empty);
      cf.STORAGE.AddIfNotExist(ConfigTitles.CONFIG_ROOT_FOLDER, EZ_Builder.Common.CombinePath(EZ_Builder.Constants.PLUGINS_FOLDER, cf._pluginGUID));

      EZ_Builder.Scripting.VariableManager.SetVariable(cf.STORAGE[ConfigTitles.RESPONSE_VARIABLE].ToString(), string.Empty);

      if (_bot == null) {

        try {

          string configRootFolder = cf.STORAGE[ConfigTitles.CONFIG_ROOT_FOLDER].ToString();

          string aimlFolder = EZ_Builder.Common.CombinePath(configRootFolder, "aiml");

          string configFolder = EZ_Builder.Common.CombinePath(configRootFolder, "config");

          string settingsFile = EZ_Builder.Common.CombinePath(configRootFolder, "config", "Settings.xml");

          EZ_Builder.Invokers.SetAppendText(tbLog, false, "Loading AIM configuration... ");

          _bot = new AIMLbot.Bot();
          _bot.PathToAIML = aimlFolder;
          _bot.PathToConfigFiles = configFolder;
          _bot.loadSettings(settingsFile);
          _bot.loadAIMLFromFiles();
          _bot.isAcceptingUserInput = true;
          _bot.WrittenToLog += _bot_WrittenToLog;

          _user = new AIMLbot.User("DJ", _bot);

          EZ_Builder.Invokers.SetAppendText(tbLog, true, "Success!");
        } catch (Exception ex) {

          EZ_Builder.Invokers.SetAppendText(tbLog, true, ex.ToString());
        }
      }

      base.SetConfiguration(cf);
    }

    private void _bot_WrittenToLog() {

      EZ_Builder.Invokers.SetAppendText(tbLog, true, _bot.LastLogMessage);
    }

    public override object[] GetSupportedControlCommands() {

      return new object[] {
        string.Format("{0}, [phrase]", EZ_Builder.Scripting.ControlCommands.SetPhrase)
      };
    }

    public override void SendCommand(string windowCommand, params string[] values) {

      if (windowCommand.Equals(EZ_Builder.Scripting.ControlCommands.SetPhrase, StringComparison.InvariantCultureIgnoreCase)) {

        tbLog.Invoke(new Action(() => {

          textBox1.Text = values[0];

          button1_Click_1(null, null);
        }));
      } else {

        base.SendCommand(windowCommand, values);
      }
    }

    private void button1_Click_1(object sender, EventArgs e) {

      textBox1.Text = textBox1.Text.Trim();

      if (textBox1.Text == string.Empty)
        return;

      AIMLbot.Request req = new AIMLbot.Request(textBox1.Text, _user, _bot);

      tbLog.AppendText("You> " + textBox1.Text);
      tbLog.AppendText(Environment.NewLine);

      AIMLbot.Result r = _bot.Chat(req);

      string unfilteredResp = r.Output;

      string resp = string.Empty;
      string[] cmds = Common.GetTextBetween(unfilteredResp, out resp, '[', ']');

      resp = resp.Replace("\r", string.Empty);
      resp = resp.Replace("\n", string.Empty);
      resp = EZ_Builder.Scripting.VariableManager.SubsituteWithValues(resp);
      resp = resp.Replace("\"", "");
      resp = Regex.Replace(resp, @"\s+", " ");

      tbLog.AppendText("Bot> " + resp);
      tbLog.AppendText(Environment.NewLine);

      EZ_Builder.Scripting.VariableManager.SetVariable(
        _cf.STORAGE[ConfigTitles.RESPONSE_VARIABLE].ToString(),
        resp);

      if (_cf.STORAGE[ConfigTitles.RESPONSE_SCRIPT].ToString() != string.Empty)
        EZ_Builder.Scripting.EZScriptManager.GetExecutor(_EXECUTOR_NAME).StartScriptASync(_cf.STORAGE[ConfigTitles.RESPONSE_SCRIPT].ToString());

      foreach (string cmd in cmds) {

        Invokers.SetAppendText(tbLog, true, 50, "Exec: {0}", cmd);

        string response = EZ_Builder.Scripting.EZScriptManager.GetExecutor(_EXECUTOR_NAME + "_embedded").ExecuteScriptSingleLine(cmd);

        if (response != string.Empty)
          Invokers.SetAppendText(tbLog, true, 50, response);
      }

      textBox1.Clear();
    }

    private void ucConfigurationButton1_Click(object sender, EventArgs e) {

      using (FormOptions form = new FormOptions()) {

        form.SetConfiguration(_cf, this);

        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

          _bot = null;
          _user = null;

          SetConfiguration(form.GetConfiguration());
        }
      }
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {

      EZ_Builder.Scripting.EZScriptManager.RemoveExecutor(_EXECUTOR_NAME);
    }
  }
}
