using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Models
{
    public class LoggingConfig
    {
        /// <summary>
        /// 日志级别 (Trace, Debug, Information, Warning, Error, Critical)
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 是否启用控制台输出
        /// </summary>
        public bool EnableConsole { get; set; }

        /// <summary>
        /// 是否启用文件输出
        /// </summary>
        public bool EnableFile { get; set; }

        /// <summary>
        /// 日志文件最大大小（MB）
        /// </summary>
        public int MaxFileSizeMB { get; set; }

        /// <summary>
        /// 保留日志文件数量
        /// </summary>
        public int RetainedFileCount { get; set; }
    }

}
