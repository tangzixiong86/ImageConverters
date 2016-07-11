using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageConverters.Infrastructure
{
    /// <summary>
    /// 图片转换器工厂。
    /// </summary>
    public class ImageConverterFactory : IImageConverterFactory
    {
        public IImageConverter CreateImageConverter(string extendName)
        {
            if (extendName == ".doc" || extendName == ".docx")
            {
                return new Word2ImageConverter();
            }

            if (extendName == ".pdf")
            {
                return new Pdf2ImageConverter();
            }

            if (extendName == ".ppt")
            {
                return new Ppt2ImageConverter();
            }
            if (extendName == ".pptx")
            {
                return new Pptx2ImageConverter();
            }
            if (extendName == ".xls" || extendName == ".xlsx")
            {
                return new Xls2ImageConverter();
            }
            return null;
        }

        public bool Support(string extendName)
        {
            return extendName == ".doc" || extendName == ".docx"
                || extendName == ".pdf" || extendName == ".ppt"
                || extendName == ".pptx" || extendName == ".xls"
                || extendName == ".xlsx";
        }
    }
}