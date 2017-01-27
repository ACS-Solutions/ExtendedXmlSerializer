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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.ElementModel.Names;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
	public class ElementSource : IAlteration<IElements>
	{
		readonly INames _names;
		readonly ICollectionItemTypeLocator _locator;
		readonly IAddDelegates _add;
		readonly ISpecification<TypeInfo> _specification;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public ElementSource(INames names, ICollectionItemTypeLocator locator, IAddDelegates add, IEnumerable<TypeInfo> types,
		                     ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
			: this(names, locator, add, new Specification(types), property, field) {}

		public ElementSource(INames names, ICollectionItemTypeLocator locator, IAddDelegates add, ISpecification<TypeInfo> specification,
		                     ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_names = names;
			_locator = locator;
			_add = add;
			_specification = specification;
			_property = property;
			_field = field;
		}

		IEnumerable<IOption<TypeInfo, IElement>> CreateOptions(IElements parameter)
		{
			var members = new ElementMembers(new MemberElementSelector(parameter, _add), _property, _field);
			var guarded = new RecursionGuardedElementMembers(members);
			yield return new ElementOption(_specification, _names);
			yield return new DictionaryElementOption(parameter, _names, guarded);
			yield return new ArrayElementOption(parameter, _names, _locator);
			yield return new CollectionElementOption(parameter, _names, guarded, _locator);
			yield return new MemberedElementOption(_names, guarded);
			yield return new ElementOption(_names);
		}


		public IElements Get(IElements parameter) => new Elements(CreateOptions(parameter).ToArray());

		sealed class Elements : Selector<TypeInfo, IElement>, IElements
		{
			public Elements(params IOption<TypeInfo, IElement>[] options) : base(options) {}
		}

		sealed class Specification : AnySpecification<TypeInfo>
		{
			public Specification(IEnumerable<TypeInfo> known, params TypeInfo[] except)
				: base(
					new AnySpecification<TypeInfo>(new ContainsSpecification<TypeInfo>(known.Except(except).ToArray()),
					                               IsAssignableSpecification<Enum>.Default)) {}
		}
	}
}