/*
 * 由SharpDevelop创建。
 * 用户： Admin
 * 日期: 23-02-06 周一
 * 时间: 23:17
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
 
 // TODO Replace the buttons into icons
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using RegistryUtils;
// using Windows.UI.ViewManagement;
using QQ2564874169.Miniblink;

namespace MiniblinkSD
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public partial class Form1 : Form
	{
		MiniblinkBrowser browser = new MiniblinkBrowser();
		WebBrowser tmpPrint = new WebBrowser();
		string fileName = "";
		string fileContent = "";
		bool requireSwitch = false;
		// UISettings uiSettings = new UISettings();
		RegistryMonitor colorStyle = new RegistryMonitor(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");
		int colorStyleEx = (Int32)Registry.CurrentUser
			 .OpenSubKey("Software")
	         .OpenSubKey("Microsoft")
	         .OpenSubKey("Windows")
	         .OpenSubKey("CurrentVersion")
	         .OpenSubKey("Themes")
	         .OpenSubKey("Personalize")
			.GetValue("AppsUseLightTheme");
		
		public Form1()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		public Form1(string str)
		{
			this.fileName = str;
			this.fileContent = File.ReadAllText(str);
			InitializeComponent();
		}

		void Form1Load(object sender, EventArgs e)
		{
			// MessageBox.Show((this.fileName != "").ToString());
			
			this.panel1.Controls.Add(browser);
			browser.SetBounds(0,0,this.panel1.Width,this.panel1.Height);
			browser.Dock = DockStyle.Fill;
			browser.KeyDown += shortcutKeys;
			browser.AllowDrop = true;
			browser.DragOver += browser_DragOver;
			browser.DragDrop += browser_DragDrop; // miniblink not implemented drag drop yet
			// browser.ShowDevTools();
			// browser.LoadUri("https://www.runoob.com/try/try.php?filename=tryhtml_intro");
			if (this.fileName != "") {
				this.Text = this.fileName + " - Miniblink Markdown Vditor";
			} else {
				this.Text = "untitled.md - Miniblink Markdown Vditor";
			}
			
			browser.LoadHtml(@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Title</title>
    <link rel=""stylesheet"" href=""dist/index.css"" />
    <script src=""dist/index.min.js""></script>
  </head>

  <body style=""background-color: " + (colorStyleEx == 1 ? "white" : "#222233") + @";"">
    <div id=""vditor"" class=""vditor"" style=""margin-top: 30px;""></div>
  </body>
</html>
<script>
  var initContent;
  
  const initVditor = (language) => {
    window.vditor = new Vditor(""vditor"", {
      cdn: ""."",//关键
      lang: language,
      height: window.innerHeight - 40,
      resize: {
	  	enable: true
	  },
	  theme: """ + (colorStyleEx == 1 ? "classic" : "dark") + @""",
	  toolbar: [""emoji"",""headings"",""bold"",""italic"",""strike"",""link"",""|"",""list"",""ordered-list"",""check"",""outdent"",""indent"",""|"",""quote"",""line"",""code"",""inline-code"",""insert-before"",""insert-after"",""table"",""|"",""undo"",""redo"",""|"",""edit-mode"",{
                name: ""more"",
                toolbar: [
                    ""both"",
                    ""code-theme"",
                    ""content-theme"",
                    ""outline"",
                    ""preview"",
                    ""devtools"",
                    ""info"",
                    ""help"",
                ],
            }],
      counter: {
      	enable: true
      },
      preview: {
        theme: {
          path: ""dist/css/content-theme"",//关键
        },
      },
      hint: {
        emojiPath: ""dist/images/emoji"",//关键
      },
      cache: {
        enable: false,
      },
      value: `" + (this.fileName != "" ? this.fileContent.Replace("\\", "\\\\").Replace("`", "\\`") : "") + @"`,
      after: function() {
      	initContent = window.vditor.getValue();
      	document.getElementById('vditorContentTheme').href='dist/css/content-theme/" + (colorStyleEx == 1 ? "light" : "dark") + @".css';
      }
    });
  };
  initVditor(""en_US"");
  window.setLang = (language) => {
    window.vditor.destroy();
    initVditor(language);
  };
</script>
");
			
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			ToolTip t = new ToolTip();
			
			Button btnOpen = new Button();
			// btnOpen.Text = "Open...";
			t.SetToolTip(btnOpen, "Open... (Ctrl+O)");
			MemoryStream mStream = new MemoryStream();
			Icon icon = (Icon)(resources.GetObject("open"));
			icon.Save(mStream);
			btnOpen.Image = Image.FromStream(mStream);
			btnOpen.BackColor = colorStyleEx == 1 ? SystemColors.Control : SystemColors.ControlDarkDark;
			btnOpen.SetBounds(0,0,30,30);
			btnOpen.Click += this.btnOpenClick;
			this.Controls.Add(btnOpen);
			this.Controls.SetChildIndex(btnOpen, 0);
			
			Button btnSave = new Button();
			// btnSave.Text = "Save";
			t.SetToolTip(btnSave, "Save (Ctrl+S)");
			mStream = new MemoryStream();
			icon = (Icon)(resources.GetObject("save"));
			icon.Save(mStream);
			btnSave.Image = Image.FromStream(mStream);
			btnSave.BackColor = colorStyleEx == 1 ? SystemColors.Control : SystemColors.ControlDarkDark;
			btnSave.SetBounds(30,0,30,30);
			btnSave.Click += this.btnSaveClick;
			this.Controls.Add(btnSave);
			this.Controls.SetChildIndex(btnSave, 0);
			
			Button btnSaveAs = new Button();
			t.SetToolTip(btnSaveAs, "Save As... (Ctrl+Shift+S)");
			// btnSaveAs.Text = "Save As...";
			mStream = new MemoryStream();
			icon = (Icon)(resources.GetObject("saveAs"));
			icon.Save(mStream);
			btnSaveAs.Image = Image.FromStream(mStream);
			btnSaveAs.BackColor = colorStyleEx == 1 ? SystemColors.Control : SystemColors.ControlDarkDark;
			btnSaveAs.SetBounds(60,0,30,30);
			btnSaveAs.Click += this.btnSaveAsClick;
			this.Controls.Add(btnSaveAs);
			this.Controls.SetChildIndex(btnSaveAs, 0); 

			Button btnPrint = new Button();
			t.SetToolTip(btnPrint, "Print... (Ctrl+P)");
			// btnPrint.Text = "Print...";
			mStream = new MemoryStream();
			icon = (Icon)(resources.GetObject("print"));
			icon.Save(mStream);
			btnPrint.Image = Image.FromStream(mStream);
			btnPrint.BackColor = colorStyleEx == 1 ? SystemColors.Control : SystemColors.ControlDarkDark;
			btnPrint.SetBounds(90,0,30,30);
			btnPrint.Click += this.btnPrintClick;
			this.Controls.Add(btnPrint);
			this.Controls.SetChildIndex(btnPrint, 0);
			
			Button btnSwitch = new Button();
			t.SetToolTip(btnSwitch, "Switch Color Theme of Dark/Light");
			mStream = new MemoryStream();
			icon = (Icon)(resources.GetObject(colorStyleEx == 1 ? "light" : "dark"));
			icon.Save(mStream);
			btnSwitch.Image = Image.FromStream(mStream);
			btnSwitch.BackColor = colorStyleEx == 1 ? SystemColors.Control : SystemColors.ControlDarkDark;
			btnSwitch.SetBounds(120,0,30,30);
			btnSwitch.Click += this.switchTheme;
			this.Controls.Add(btnSwitch);
			this.Controls.SetChildIndex(btnSwitch, 0);

			Label version = new Label();
			version.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			version.SetBounds(160, 7, 200, 15);
			version.BackColor = colorStyleEx == 1 ? Color.White : Color.FromArgb(0x22, 0x22, 0x33);
			version.ForeColor = Color.Gray;
			this.Controls.Add(version);
			this.Controls.SetChildIndex(version, 0);
			
			colorStyle.RegChanged += colorStyle_RegChanged;
			// TODO monitor system color scheme
			colorStyle.Start();
			this.Activated += checkTheme;
		}
		
		void Form1ResizeBegin(object sender, EventArgs e)
		{
			// browser.SetBounds(0,0,this.Width,this.Height);
		}
		void Form1ResizeEnd(object sender, EventArgs e)
		{
			// browser.SetBounds(0,0,this.Width,this.Height);
		}
		void Form1FormClosing(object sender, FormClosingEventArgs e)
		{
			String textOnExit = (String) this.browser.RunJs("return window.vditor.getValue();");
			String textOnInit = (String) this.browser.RunJs("return initContent;");
			// MessageBox.Show(textOnExit.Length.ToString());
			// MessageBox.Show(textOnInit.Length.ToString());
			// MessageBox.Show((textOnExit == textOnInit).ToString());
			if (textOnExit != textOnInit) {
				DialogResult d = MessageBox.Show("Do you want to save your work? \n\nThere are unsaved changes in \"" + (this.fileName != "" ? this.fileName : "untitled.md") + "\".", "Miniblink Markdown Vditor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (d == DialogResult.Yes) {
					if (this.fileName != "") {
						// MessageBox.Show("Simulate overwritting");
						File.WriteAllText(this.fileName, textOnExit);
					} else {
						SaveFileDialog sfd = new SaveFileDialog();
						sfd.Filter = "Markdown Files (*.md)|*.md|All files(*.*)|*.*";
						sfd.FileName = "untitled";
						sfd.DefaultExt = "md";
						sfd.AddExtension = true;
						sfd.RestoreDirectory = true;
						sfd.Title = "Save";
						DialogResult result = sfd.ShowDialog();
						if (result == DialogResult.OK) {
						  //获得文件路径
						    String filePathOnSave = sfd.FileName.ToString();
						    File.WriteAllText(filePathOnSave, textOnExit);
						} else {
							e.Cancel = true;
						}
					}
				} else if (d == DialogResult.Cancel) {
					e.Cancel = true;
				}
			}
			// MessageBox.Show(s);
		}
		
		void btnOpenClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Markdown Files (*.md)|*.md|All files(*.*)|*.*";
			ofd.DefaultExt = "md";
			ofd.AddExtension = true;
			ofd.RestoreDirectory = true;
			DialogResult result = ofd.ShowDialog();
			if (result == DialogResult.OK) {
				openFile(ofd.FileName);
			}
		}
				
		void btnSaveClick(object sender, EventArgs e)
		{
			String textOnExit = (String) this.browser.RunJs("return window.vditor.getValue();");
			if (this.fileName != "") {
				// MessageBox.Show("Simulate overwritting");
				File.WriteAllText(this.fileName, textOnExit);
			} else {
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Markdown Files (*.md)|*.md|All files(*.*)|*.*";
				sfd.FileName = "untitled";
				sfd.DefaultExt = "md";
				sfd.AddExtension = true;
				sfd.RestoreDirectory = true;
				sfd.Title = "Save";
				DialogResult result = sfd.ShowDialog();
				if (result == DialogResult.OK) {
				  //获得文件路径
				    String filePathOnSave = sfd.FileName.ToString();
				    File.WriteAllText(filePathOnSave, textOnExit);
				}
			}
		}

		void btnSaveAsClick(object sender, EventArgs e)
		{
			String textOnExit = (String) this.browser.RunJs("return window.vditor.getValue();");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Markdown Files (*.md)|*.md|All files(*.*)|*.*";
			sfd.FileName = this.fileName != "" ? this.fileName.Substring(this.fileName.LastIndexOf("\\") + 1) : "untitled";
			sfd.DefaultExt = "md";
			sfd.AddExtension = true;
			sfd.RestoreDirectory = true;
			sfd.Title = "Save As";
			DialogResult result = sfd.ShowDialog();
			if (result == DialogResult.OK) {
			  //获得文件路径
			    String filePathOnSave = sfd.FileName.ToString();
			    File.WriteAllText(filePathOnSave, textOnExit);
			}
		}
		
		void shortcutKeys(object sender, KeyEventArgs e) {
			if (e.Control && e.KeyCode == Keys.O) {
				this.btnOpenClick(sender, e);
			}
			if (e.Control && e.KeyCode == Keys.S) {
				this.btnSaveClick(sender, e);
			}
			if (e.Control && e.Shift && e.KeyCode == Keys.S) {
				this.btnSaveAsClick(sender, e);
			}
			if (e.Control && e.KeyCode == Keys.P) {
				this.btnPrintClick(sender, e);
			}
		}

		void browser_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if ((files.Length == 1 && files[0] == fileName) || files.Length > 1) {
				return;
			} else {
				String textOnExit = (String) this.browser.RunJs("return window.vditor.getValue();");
				String textOnInit = (String) this.browser.RunJs("return initContent;");
				// MessageBox.Show(textOnExit.Length.ToString());
				// MessageBox.Show(textOnInit.Length.ToString());
				// MessageBox.Show((textOnExit == textOnInit).ToString());
				if (textOnExit != textOnInit) {
					DialogResult d = MessageBox.Show("Do you want to save your work? \n\nThere are unsaved changes in \"" + (this.fileName != "" ? this.fileName : "untitled.md") + "\".", "Miniblink Markdown Vditor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (d == DialogResult.Yes) {
						if (this.fileName != "") {
							// MessageBox.Show("Simulate overwritting");
							File.WriteAllText(this.fileName, textOnExit);
						} else {
							SaveFileDialog sfd = new SaveFileDialog();
							sfd.Filter = "Markdown Files (*.md)|*.md|All files(*.*)|*.*";
							sfd.FileName = "untitled";
							sfd.DefaultExt = "md";
							sfd.AddExtension = true;
							sfd.RestoreDirectory = true;
							sfd.Title = "Save";
							DialogResult result = sfd.ShowDialog();
							if (result == DialogResult.OK) {
							  //获得文件路径
							    String filePathOnSave = sfd.FileName.ToString();
							    File.WriteAllText(filePathOnSave, textOnExit);
							} else {
								return;
							}
						}
					} else if (d == DialogResult.Cancel) {
						return;
					}
				}
			}
			if (files.Length == 1 && Path.GetExtension(files[0]).ToLower() == ".md") {
				openFile(files[0]);
			}
		}

		void browser_DragOver(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files.Length > 1) {
				e.Effect = DragDropEffects.None;
				return;
			}
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && Path.GetExtension(files[0]).ToLower() == ".md")
		    {
		        e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
		void btnPrintClick(object sender, EventArgs e)
		{
			String html = (String) this.browser.RunJs("return window.vditor.getHTML();");
			tmpPrint.DocumentText = html;
			tmpPrint.DocumentCompleted += tmpPrint_DocumentCompleted;
		}

		void tmpPrint_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			tmpPrint.ShowPrintDialog();
		}

		void colorStyle_RegChanged(object sender, EventArgs e)
		{
			int colorStyleExNew = (Int32)Registry.CurrentUser
			 .OpenSubKey("Software")
	         .OpenSubKey("Microsoft")
	         .OpenSubKey("Windows")
	         .OpenSubKey("CurrentVersion")
	         .OpenSubKey("Themes")
	         .OpenSubKey("Personalize")
			 .GetValue("AppsUseLightTheme");
			if (colorStyleExNew != colorStyleEx) {
				requireSwitch = true;
				switch (colorStyleExNew) {
					case 0: // dark
						// miniblink does not support cross thread calling interfaces
						// browser.RunJs("window.editor.setTheme('dark');");
						break;
					case 1: // light
						// browser.RunJs("window.editor.setTheme('classic');");
						break;
					default:
						break;
				}
				// colorStyleEx = colorStyleExNew;
			}
		}

		void switchTheme(object sender, EventArgs e)
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			MemoryStream mStream = new MemoryStream();
			Icon icon = (Icon)(resources.GetObject("dark"));
			switch (colorStyleEx) {
				case 1: // light to dark
					this.Controls[1].BackColor = SystemColors.ControlDarkDark;
					mStream = new MemoryStream();
					icon = (Icon)(resources.GetObject("dark"));
					icon.Save(mStream);
					((Button)this.Controls[1]).Image = Image.FromStream(mStream);
					this.Controls[2].BackColor = SystemColors.ControlDarkDark;
					this.Controls[3].BackColor = SystemColors.ControlDarkDark;
					this.Controls[4].BackColor = SystemColors.ControlDarkDark;
					this.Controls[5].BackColor = SystemColors.ControlDarkDark;
					this.Controls[0].BackColor = Color.FromArgb(0x22, 0x22, 0x33);
					browser.RunJs("window.vditor.setTheme('dark');");
					browser.RunJs("document.getElementById('vditorContentTheme').href='dist/css/content-theme/dark.css';");
					browser.RunJs("document.body.style.backgroundColor='#222233';");
					this.colorStyleEx = 0;
					break;
				case 0: // dark to light
					this.Controls[1].BackColor = SystemColors.Control;
					mStream = new MemoryStream();
					icon = (Icon)(resources.GetObject("light"));
					icon.Save(mStream);
					((Button)this.Controls[1]).Image = Image.FromStream(mStream);
					this.Controls[2].BackColor = SystemColors.Control;
					this.Controls[3].BackColor = SystemColors.Control;
					this.Controls[4].BackColor = SystemColors.Control;
					this.Controls[5].BackColor = SystemColors.Control;
					this.Controls[0].BackColor = Color.White;
					browser.RunJs("window.vditor.setTheme('classic');");
					browser.RunJs("document.getElementById('vditorContentTheme').href='dist/css/content-theme/light.css';");
					browser.RunJs("document.body.style.backgroundColor='white';");
					this.colorStyleEx = 1;
					break;
				default:
					break;
			}
		}

		void checkTheme(object sender, EventArgs e)
		{
			// MessageBox.Show("theme changing");
			if (requireSwitch) {
				requireSwitch = false;
				int colorStyleExNew = (Int32)Registry.CurrentUser
				 .OpenSubKey("Software")
		         .OpenSubKey("Microsoft")
		         .OpenSubKey("Windows")
		         .OpenSubKey("CurrentVersion")
		         .OpenSubKey("Themes")
		         .OpenSubKey("Personalize")
				 .GetValue("AppsUseLightTheme");
				if (colorStyleExNew != colorStyleEx) {
					requireSwitch = true;
					switchTheme(sender, e);
					colorStyleEx = colorStyleExNew;
				}
			}
		}
		
		void openFile(String fileNameToOpen) {
			//获得文件路径
		    this.fileName = fileNameToOpen;
		    this.fileContent = File.ReadAllText(this.fileName);
		    this.Text = this.fileName + " - Miniblink Markdown Vditor";
		    browser.LoadHtml(@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Title</title>
    <link rel=""stylesheet"" href=""dist/index.css"" />
    <script src=""dist/index.min.js""></script>
  </head>

  <body style=""background-color: " + (colorStyleEx == 1 ? "white" : "#222233") + @";"">
    <div id=""vditor"" class=""vditor"" style=""margin-top: 30px;""></div>
  </body>
</html>
<script>
  var initContent;
  
  const initVditor = (language) => {
    window.vditor = new Vditor(""vditor"", {
      cdn: ""."",//关键
      lang: language,
      height: window.innerHeight - 40,
      resize: {
	  	enable: true
	  },
	  theme: """ + (colorStyleEx == 1 ? "classic" : "dark") + @""",
	  toolbar: [""emoji"",""headings"",""bold"",""italic"",""strike"",""link"",""|"",""list"",""ordered-list"",""check"",""outdent"",""indent"",""|"",""quote"",""line"",""code"",""inline-code"",""insert-before"",""insert-after"",""table"",""|"",""undo"",""redo"",""|"",""edit-mode"",{
                name: ""more"",
                toolbar: [
                    ""both"",
                    ""code-theme"",
                    ""content-theme"",
                    ""outline"",
                    ""preview"",
                    ""devtools"",
                    ""info"",
                    ""help"",
                ],
            }],
      counter: {
      	enable: true
      },
      preview: {
        theme: {
          path: ""dist/css/content-theme"",//关键
        },
      },
      hint: {
        emojiPath: ""dist/images/emoji"",//关键
      },
      cache: {
        enable: false,
      },
      value: `" + (this.fileName != "" ? this.fileContent.Replace("\\", "\\\\").Replace("`", "\\`") : "") + @"`,
      after: function() {
      	initContent = window.vditor.getValue();
      	document.getElementById('vditorContentTheme').href='dist/css/content-theme/" + (colorStyleEx == 1 ? "light" : "dark") + @".css';
      }
    });
  };
  initVditor(""en_US"");
  window.setLang = (language) => {
    window.vditor.destroy();
    initVditor(language);
  };
</script>
");
		}
	}
}

