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
using System.Threading;
using System.Windows.Forms;
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

  <body>
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
			btnPrint.SetBounds(90,0,30,30);
			btnPrint.Click += this.btnPrintClick;
			this.Controls.Add(btnPrint);
			this.Controls.SetChildIndex(btnPrint, 0);

			Label version = new Label();
			version.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			version.SetBounds(130, 7, 200, 15);
			version.BackColor = Color.White;
			version.ForeColor = Color.Gray;
			this.Controls.Add(version);
			this.Controls.SetChildIndex(version, 0);
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
						DialogResult result = sfd.ShowDialog();
						if (result == DialogResult.OK) {
						  //获得文件路径
						    String filePathOnSave = sfd.FileName.ToString();
						    File.WriteAllText(filePathOnSave, textOnExit);
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
			  //获得文件路径
			    this.fileName = ofd.FileName.ToString();
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

  <body>
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
			throw new NotImplementedException();
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
	}
}

