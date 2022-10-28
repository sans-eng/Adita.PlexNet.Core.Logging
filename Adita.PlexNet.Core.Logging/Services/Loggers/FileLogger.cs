//MIT License

//Copyright (c) 2022 Adita

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Adita.PlexNet.Core.Logging
{
    /// <summary>
    /// Represents a file logger that write log to a file daily to diferent file.
    /// </summary>
    public sealed class FileLogger : ILogger
    {
        #region Private fields
        private readonly string _name;
        private readonly FileLoggerOptions _option;
        #endregion Private fields

        #region Constructors
        /// <summary>
        /// Initialize a new instance of <see cref="FileLogger"/> using specified <paramref name="option"/>.
        /// </summary>
        /// <param name="name">The category name of the logger.</param>
        /// <param name="option">A <see cref="FileLoggerOptions"/> to use.</param>
        public FileLogger(string name, FileLoggerOptions option)
        {
            _name = name;
            _option = option;
        }
        /// <summary>
        /// Initialize a new instance of <see cref="FileLogger"/> using specified <paramref name="option"/>.
        /// </summary>
        /// <param name="name">The category name of the logger.</param>
        /// <param name="option">A <see cref="FileLoggerOptions"/> to use.</param>
        public FileLogger(string name, IOptions<FileLoggerOptions> option)
        {
            _name = name;
            _option = option.Value;
        }
        #endregion Constructors

        #region Public methods
        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => default!;
        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Critical => false,
                LogLevel.Debug => false,
                LogLevel.Trace => false,
                LogLevel.None => false,
                _ => true,
            };
        }
        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (string.IsNullOrWhiteSpace(_option.Directory))
            {
                throw new ArgumentException("Invalid logger directory name.");
            }

            try
            {
                string fileName = string.IsNullOrWhiteSpace(_option.FileNamePrefix) ? $"{DateTime.Now:dd-MM-yyyy}.log" :
                    $"{_option.FileNamePrefix} {DateTime.Now:dd-MM-yyyy}.log";
                string path = Path.Combine(_option.Directory, fileName);

                using (TextWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine($"{DateTime.Now:dd-MM-yyyy HH:mm:ss:fff} [{eventId.Id}] [{logLevel}] ({_name}) {formatter(state, exception)}");
                }
            }
            catch (Exception ex)
            {
                throw new AggregateException("Unable to write log.", ex);
            }
        }
        #endregion Public methods
    }
}
