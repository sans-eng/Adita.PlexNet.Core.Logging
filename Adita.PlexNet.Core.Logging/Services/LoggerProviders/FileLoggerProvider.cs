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
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace Adita.PlexNet.Core.Logging
{
    /// <summary>
    /// Represents a <see cref="FileLogger"/> provider.
    /// </summary>
    [UnsupportedOSPlatform("browser")]
    [ProviderAlias("FileLogger")]
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        #region Private fields
        private bool _disposed;
        private readonly IDisposable _onChangeToken;
        private FileLoggerOptions _options;
        private readonly ConcurrentDictionary<string, ILogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);
        #endregion Private fields

        #region Constructors
        /// <summary>
        /// Initialize a new instance of <see cref="FileLoggerProvider"/> using specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">An <see cref="FileLoggerOptions"/> to include.</param>
        public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
        {
            _options = options.CurrentValue;
            _onChangeToken = options.OnChange(updatesOptions => _options = updatesOptions);
        }
        #endregion Constructors

        #region Public methods
        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new FileLogger(name, _options));
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion Public methods

        #region Private methods
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _loggers.Clear();
                _onChangeToken.Dispose();
                _loggers.Clear();
            }
            _disposed = true;
        }
        #endregion Private methods
    }
}
