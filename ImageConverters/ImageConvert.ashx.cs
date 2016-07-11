using ImageConverters.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageConverters
{
    /// <summary>
    /// ImageConvert 的摘要说明
    /// </summary>
    public class ImageConvert : IHttpHandler
    {
        private string storageDir = "/Common/eWebEditor/UploadFile";
        public void ProcessRequest(HttpContext context)
        {
          
            bool isSuccess = true;
            string errMessage = String.Empty;
            List<string> imageFilePath = new List<string>();
            try
            {
                int year = DateTime.Now.Year;
                string pathDir = context.Server.MapPath(storageDir) + "\\" + year;
                if (!Directory.Exists(pathDir))
                {
                    Directory.CreateDirectory(pathDir);
                }
                HttpPostedFile httpFile = context.Request.Files[0];
                //获取要保存的文件信息
                string filerealname = httpFile.FileName;
                ReportLog.WriteLog(filerealname);
                //获得文件扩展名
                string fileExt = Path.GetExtension(filerealname).ToLower();
                ReportLog.WriteLog(fileExt);
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff")  + fileExt;

                switch (fileExt)
                {
                    case ".doc":
                    case ".docx":
                    case ".pdf":
                    case ".ppt":
                    case ".pptx":
                    case ".xls":
                    case ".xlsx":
                        httpFile.SaveAs(pathDir + "\\" + fileName);
                        ImageConverterFactory factory = new ImageConverterFactory();
                        IImageConverter converter = factory.CreateImageConverter(fileExt);
                        converter.ConvertSucceed += (pageCount, imageFileExt) =>
                        {
                            for (int i = 1; i <= pageCount; i++)
                            {
                                imageFilePath.Add(storageDir+"/" + year+"/" + fileName.TrimEnd(fileExt.ToArray()) +"_" + i.ToString("000") + "." + imageFileExt);
                            }
                        };
                        converter.ConvertFailed += (msg) =>
                        {
                            errMessage = msg;
                            isSuccess = false;
                        };
                        converter.ConvertToImage(pathDir + "\\" + fileName, pathDir);
                        if (File.Exists(pathDir + "\\" + fileName))
                        {
                            File.Delete(pathDir + "\\" + fileName);
                        }
                        break;
                    default:
                        httpFile.SaveAs(pathDir + "\\" + fileName);
                        imageFilePath.Add(String.Format(storageDir+"/{0}/" + fileName,year));
                        break;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                errMessage = "堆栈信息:" + ex.StackTrace + " 错误信息:" + ex.Message;
            }
            JObject json = new JObject(
                new JProperty("success", isSuccess),
                new JProperty("msg", errMessage),
                new JProperty("images",
                        new JArray(imageFilePath.ToArray()))
                );
            //在兼容模式下必须注释些行，否则IE会自动弹出下载框下载结果。
            //context.Response.ContentType = "application/json";
            context.Response.Write(json.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}