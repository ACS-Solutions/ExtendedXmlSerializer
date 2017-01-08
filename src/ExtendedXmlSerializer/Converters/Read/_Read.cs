// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Xml.Linq;
using ExtendedXmlSerialization.Converters.Legacy;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Converters.Read
{
    class LegacyDeserializer : IDeserializer
    {
        private readonly Typed? _type;
        private readonly IReader _reader;

        public LegacyDeserializer(ISerializationToolsFactory tools, Type type)
            : this(new LegacyRootConverters(tools).Get(tools), type) {}

        public LegacyDeserializer(IReader reader, Typed? type = null)
        {
            _reader = reader;
            _type = type;
        }

        public object Deserialize(Stream stream)
        {
            var text = new StreamReader(stream).ReadToEnd();
            var root = XDocument.Parse(text).Root;
            var result = _reader.Read(root, _type);
            return result;
        }
    }

    public interface IReader<out T> : IReader
    {
        new T Read(XElement element, Typed? hint = null);
    }

    public abstract class ReaderBase<T> : IReader<T>
    {
        object IReader.Read(XElement element, Typed? hint) => Read(element, hint);

        public abstract T Read(XElement element, Typed? hint = null);
    }
}