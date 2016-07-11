using System;
using System.IO;



namespace ImageConverters.Infrastructure
{
	public class ReportLog
	{
        private static  string _ConsoleUrl = "C:\\";

		/// <summary>
		/// 添加操作日志
		/// </summary>
		/// <param name="strErr">错误信息</param>
		public static void WriteLog(string strErr)
		{
			try
			{
				StreamWriter sw = null;
				string path = _ConsoleUrl + "\\log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
				if (!File.Exists(path)) 
				{
					if(!Directory.Exists(_ConsoleUrl + "\\log"))
						Directory.CreateDirectory(_ConsoleUrl + "\\log");
					sw = File.CreateText(path);
					sw.WriteLine("--------------------------------------------------------");
					sw.WriteLine("----------------------日志----------------------");
					sw.WriteLine("--------------------------------------------------------");
					sw.WriteLine(" ");
					sw.Close();
				}

				sw =File.AppendText(path); 
				strErr ="【" + DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") + "】	" + strErr;
				sw.WriteLine(strErr);
				sw.Close();
			}
			catch(Exception er)
			{
                throw new Exception(er.Message + er.StackTrace);
               
            }
		}
	}
}
