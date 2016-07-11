using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;


namespace ImageConverters.Infrastructure
{
    public class Xls2ImageConverter : IImageConverter
    {
        private Pdf2ImageConverter pdf2ImageConverter;
        public event Action<int, int> ProgressChanged;
        public event Action<int, string> ConvertSucceed;
        public event Action<string> ConvertFailed;

        public void Cancel()
        {
            if (this.pdf2ImageConverter != null)
            {
                this.pdf2ImageConverter.Cancel();
            }
        }

        public void ConvertToImage(string originFilePath, string imageOutputDirPath)
        {
            ConvertToImage(originFilePath, imageOutputDirPath, 0, 0, 200);
        }

        /// <summary>
        /// 将pdf文档转换为图片的方法      
        /// </summary>
        /// <param name="originFilePath">ppt文件路径</param>
        /// <param name="imageOutputDirPath">图片输出路径，如果为空，默认值为pdf所在路径</param>       
        /// <param name="startPageNum">从PDF文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换，如果为0，默认值为pdf总页数</param>       
        /// <param name="resolution">设置图片的像素，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        private void ConvertToImage(string originFilePath, string imageOutputDirPath, int startPageNum, int endPageNum, int resolution)
        {
            try
            {
                Aspose.Cells.Workbook doc = null;
                string fileExtension = Path.GetExtension(originFilePath).ToLower();
                if (fileExtension == ".xls")
                {
                    doc=new Aspose.Cells.Workbook(originFilePath);
                }
                else
                {
                    //Createing and XLSX LoadOptions object
                    Aspose.Cells.LoadOptions loadOptions = new Aspose.Cells.LoadOptions(Aspose.Cells.LoadFormat.Xlsx);

                    //Creating an Workbook object with 2007 xlsx file path and the loadOptions object
                    doc =new Aspose.Cells.Workbook(originFilePath, loadOptions);
                }

                if (doc == null)
                {
                    throw new Exception("ppt文件无效或者ppt文件被加密！");
                }

                if (imageOutputDirPath.Trim().Length == 0)
                {
                    imageOutputDirPath = Path.GetDirectoryName(originFilePath);
                }

                if (!Directory.Exists(imageOutputDirPath))
                {
                    Directory.CreateDirectory(imageOutputDirPath);
                }
                //先将ppt转换为pdf临时文件

                string tmpPdfPath = originFilePath.TrimEnd(fileExtension.ToCharArray()) + ".pdf";
                doc.Save(tmpPdfPath, Aspose.Cells.SaveFormat.Pdf);

                //再将pdf转换为图片
                Pdf2ImageConverter converter = new Pdf2ImageConverter();
                converter.ConvertFailed += (msg) =>
                {
                    if (this.ConvertFailed != null)
                    {
                        this.ConvertFailed(msg);
                    }
                };
                converter.ConvertSucceed += (pageCount, fileExt) =>
                {
                    if (this.ConvertSucceed != null)
                    {
                        this.ConvertSucceed(pageCount, fileExt);
                    }
                };
                converter.ProgressChanged += (done, total) =>
                {
                    if (this.ProgressChanged != null)
                    {
                        this.ProgressChanged(done, total);
                    }
                };
                converter.ConvertToImage(tmpPdfPath, imageOutputDirPath);

                //删除pdf临时文件
                File.Delete(originFilePath);

            }
            catch (Exception ex)
            {
                if (this.ConvertFailed != null)
                {
                    this.ConvertFailed("堆栈信息：" + ex.StackTrace + " 错误信息：" + ex.Message);
                }
            }

            this.pdf2ImageConverter = null;
        }
    }
}
