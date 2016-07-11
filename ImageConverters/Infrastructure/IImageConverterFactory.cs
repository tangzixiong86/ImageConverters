namespace ImageConverters.Infrastructure
{ 
    /// <summary>
    /// 图片转换器工厂。
    /// </summary>
    public interface IImageConverterFactory
    {
        /// <summary>
        /// 根据文档的扩展名，创建对应的图片转换器。
        /// </summary>
        /// <param name="extendName">全小写的扩展名。如“.docx”，“.pdf”等</param>
        /// <returns>如果不支持对应的文件类型，则返回null。</returns>
        IImageConverter CreateImageConverter(string extendName);   
        /// <summary>
        /// 是否支持目标类型的文档。
        /// </summary>
        /// <param name="extendName">全小写的扩展名。如“.docx”，“.pdf”等</param>
        /// <returns>是否支持目标类型的文档。</returns>
        bool Support(string extendName);
    }
}