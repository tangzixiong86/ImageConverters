using System;
namespace ImageConverters.Infrastructure
{
    /// <summary>
    ///  将文档转换为图片的基础接口。
    /// </summary> 
    public interface IImageConverter
    {
        /// <summary>
        /// 转换失败时，触发此事件。事件参数：失败的原因（FailCause）
        /// </summary>
        event Action<string> ConvertFailed;
      
        /// <summary>
        /// 转换完成时，触发此事件。
        /// </summary>
        event Action<int,string> ConvertSucceed;
        /// <summary>
        /// 当成功转换一页时，触发此事件。参数：已经完成转换的页数（doneCount） - 总页数（totalCount）
        /// </summary>  
        event Action<int, int> ProgressChanged;

        /// <summary>
        /// 取消转换。
        /// </summary>
        void Cancel();

        /// <summary>
        ///  开始转换。
        /// </summary>
        /// <param name="originFilePath">要被转换的文档的全路径。</param>
        /// <param name="imageOutputDirPath">用于保存转换后的图片的目录的全路径</param>
        void ConvertToImage(string originFilePath, string imageOutputDirPath);
    }
}