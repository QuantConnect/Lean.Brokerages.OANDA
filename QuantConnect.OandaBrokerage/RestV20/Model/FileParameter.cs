/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.IO;

namespace Oanda.RestV20.Model
{
    public struct FileParameter
    {
        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The content stream.
        /// </summary>
        public Stream Writer { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The content length.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// Helper to create a FileParameter from a byte array.
        /// </summary>
        public static FileParameter Create(string name, byte[] data, string filename, string contentType = "application/octet-stream")
        {
            var stream = new MemoryStream(data);
            return new FileParameter
            {
                Name = name,
                Writer = stream,
                FileName = filename,
                ContentType = contentType,
                ContentLength = data.Length
            };
        }

        /// <summary>
        /// Helper to create a FileParameter from a stream.
        /// </summary>
        public static FileParameter Create(string name, Stream stream, string filename, string contentType = "application/octet-stream")
        {
            return new FileParameter
            {
                Name = name,
                Writer = stream,
                FileName = filename,
                ContentType = contentType,
                ContentLength = stream.Length
            };
        }
    }
}