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
using System.Collections.Generic;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Legacy;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
    class AdditionalTypeConverters : ITypeConverters
    {
        public static AdditionalTypeConverters Default { get; } = new AdditionalTypeConverters();
        AdditionalTypeConverters() : this(ElementTypes.Default) {}

        private readonly IElementTypes _elementTypes;

        public AdditionalTypeConverters(IElementTypes elementTypes)
        {
            _elementTypes = elementTypes;
        }

        public IEnumerable<ITypeConverter> Get(IConverter parameter)
        {
            yield return new DictionaryTypeConverter(_elementTypes, parameter);
            yield return new ArrayTypeConverter(_elementTypes, parameter);
            yield return new EnumerableTypeConverter(_elementTypes, parameter);
            yield return new InstanceTypeConverter(_elementTypes, parameter);
        }

        class DictionaryTypeConverter : TypeConverter
        {
            public DictionaryTypeConverter(IElementTypes elementTypes, IConverter converter)
                : base(IsAssignableSpecification<IDictionary>.Default,
                       new DictionaryReader(elementTypes, converter), new DictionaryBodyWriter(converter)) {}
        }

        class EnumerableTypeConverter : TypeConverter
        {
            public EnumerableTypeConverter(IElementTypes elementTypes, IConverter converter)
                : base(
                    IsEnumerableTypeSpecification.Default,
                    new ListReader(elementTypes, converter),
                    new EnumerableBodyWriter(converter)
                ) {}
        }

        class InstanceTypeConverter : TypeConverter
        {
            public InstanceTypeConverter(IElementTypes elementTypes, IConverter converter)
                : this(
                    elementTypes,
                    new InstanceMembers(new MemberFactory(elementTypes, converter,
                                                          new EnumeratingReader(elementTypes, converter))),
                    Activators.Default) {}

            public InstanceTypeConverter(IElementTypes elementTypes, IInstanceMembers members, IActivators activators)
                : base(
                    IsActivatedTypeSpecification.Default, new InstanceBodyReader(members, elementTypes, activators),
                    new TypeEmittingWriter(new InstanceBodyWriter(members))) {}
        }
    }
}