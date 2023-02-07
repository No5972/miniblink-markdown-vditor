/*
 * 由SharpDevelop创建。
 * 用户： Admin
 * 日期: 23-02-06 周一
 * 时间: 23:14
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Windows.Forms;

namespace MiniblinkSD
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (args.Length > 0) {
				Application.Run(new Form1(args[0]));
			} else {
				Application.Run(new Form1());
			}
		}
		
	}
}
