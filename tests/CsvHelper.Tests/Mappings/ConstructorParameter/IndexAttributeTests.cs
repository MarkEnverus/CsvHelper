﻿// Copyright 2009-2022 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Tests.Mocks;
using Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelper.Tests.Mappings.ConstructorParameter
{
	
    public class IndexAttributeTests
    {
		[Fact]
		public void AutoMap_WithIndexAttributes_ConfiguresParameterMaps()
		{
			var context = new CsvContext(new CsvConfiguration(CultureInfo.InvariantCulture));
			var map = context.AutoMap<Foo>();

			Assert.Equal(2, map.ParameterMaps.Count);
			Assert.Equal(0, map.ParameterMaps[0].Data.Index);
			Assert.Equal(1, map.ParameterMaps[1].Data.Index);
		}

		[Fact]
		public void GetRecords_WithIndexAttributes_HasHeader_CreatesRecords()
		{
			var parser = new ParserMock
			{
				{ "Id", "Name" },
				{ "1", "one" },
			};
			using (var csv = new CsvReader(parser))
			{
				var records = csv.GetRecords<Foo>().ToList();

				Assert.Single(records);
				Assert.Equal(1, records[0].Id);
				Assert.Equal("one", records[0].Name);
			}
		}

		[Fact]
		public void GetRecords_WithIndexAttributes_NoHeader_CreatesRecords()
		{
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false,
			};
			var parser = new ParserMock(config)
			{
				{ "1", "one" },
			};
			using (var csv = new CsvReader(parser))
			{
				var records = csv.GetRecords<Foo>().ToList();

				Assert.Single(records);
				Assert.Equal(1, records[0].Id);
				Assert.Equal("one", records[0].Name);
			}
		}

		[Fact]
		public void WriteRecords_WithIndexAttributes_DoesntUseParameterMaps()
		{
			var records = new List<Foo>
			{
				new Foo(1, "one"),
			};

			using (var writer = new StringWriter())
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(records);

				var expected = new StringBuilder();
				expected.Append("Id,Name\r\n");
				expected.Append("1,one\r\n");

				Assert.Equal(expected.ToString(), writer.ToString());
			}
		}

		private class Foo
		{
			public int Id { get; private set; }

			public string Name { get; private set; }

			public Foo([Index(0)] int id, [Index(1)] string name)
			{
				Id = id;
				Name = name;
			}
		}
	}
}
