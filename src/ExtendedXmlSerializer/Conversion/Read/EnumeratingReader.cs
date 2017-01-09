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

using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class EnumeratingReader : ReaderBase<IEnumerable>, IEnumeratingReader
    {
        private readonly ITypes _types;
        private readonly IReader _reader;

        public EnumeratingReader(ITypes types, IReader reader)
        {
            _types = types;
            _reader = reader;
        }

        public override IEnumerable Read(XElement element, Typed? hint = null)
        {
            var elementType = hint.GetValueOrDefault().Info;
            foreach (var child in element.Elements())
            {
                var itemType = _types.Get(child)?.GetTypeInfo() ?? elementType;
                var item = _reader.Read(child, itemType);
                yield return item;
            }
        }
    }
}