﻿#region Copyright(c) Alexey Sadomov, Vladimir Timashkov. All Rights Reserved.
// -----------------------------------------------------------------------------
// Copyright(c) 2010 Alexey Sadomov, Vladimir Timashkov. All Rights Reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//   1. No Trademark License - Microsoft Public License (Ms-PL) does not grant you rights to use
//      authors names, logos, or trademarks.
//   2. If you distribute any portion of the software, you must retain all copyright,
//      patent, trademark, and attribution notices that are present in the software.
//   3. If you distribute any portion of the software in source code form, you may do
//      so only under this license by including a complete copy of Microsoft Public License (Ms-PL)
//      with your distribution. If you distribute any portion of the software in compiled
//      or object code form, you may only do so under a license that complies with
//      Microsoft Public License (Ms-PL).
//   4. The names of the authors may not be used to endorse or promote products
//      derived from this software without specific prior written permission.
//
// The software is licensed "as-is." You bear the risk of using it. The authors
// give no express warranties, guarantees or conditions. You may have additional consumer
// rights under your local laws which this license cannot change. To the extent permitted
// under your local laws, the authors exclude the implied warranties of merchantability,
// fitness for a particular purpose and non-infringement.
// -----------------------------------------------------------------------------
#endregion

using CamlexNET.Impl.Operands;
using CamlexNET.Impl.Operations.DateRangesOverlap;
using CamlexNET.Impl.ReverseEngeneering;
using CamlexNET.Impl.ReverseEngeneering.Caml.Analyzers;
using CamlexNET.Impl.ReverseEngeneering.Caml.Factories;
using CamlexNET.Interfaces.ReverseEngeneering;
using NUnit.Framework;
using Rhino.Mocks;

namespace CamlexNET.UnitTests.ReverseEngeneering.Analyzers.TestBase
{
    internal class ReDateRangesOverlapAnalyzerTest
    {
        [Test]
        public void test_WHEN_xml_is_null_THEN_expression_is_not_valid()
        {
            var analyzer = new ReDateRangesOverlapAnalyzer(null, null);
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_neither_field_refs_nor_value_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), null);
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_field_refs_specified_and_value_not_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef Name=\"StartField\" />" +
                "    <FieldRef Name=\"StopField\" />" +
                "    <FieldRef Name=\"RecurrenceField\" />" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), null);
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_field_ref_not_specified_and_value_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <Value Type=\"DateTime\"><Now/></Value>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), null);
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_not_enough_field_refs_specified_and_value_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef Name=\"StartField\" />" +
                "    <FieldRef Name=\"StopField\" />" +
                "    <Value Type=\"DateTime\"><Now/></Value>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), null);
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_field_refs_and_text_value_specified_THEN_expression_is_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef Name=\"StartField\" />" +
                "    <FieldRef Name=\"StopField\" />" +
                "    <FieldRef Name=\"RecurrenceField\" />" +
                "    <Value Type=\"DateTime\"><Now/></Value>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), new ReOperandBuilderFromCaml());
            Assert.IsTrue(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_field_ref_without_name_attribute_and_text_value_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef />" +
                "    <FieldRef Name=\"StopField\" />" +
                "    <FieldRef Name=\"RecurrenceField\" />" +
                "    <Value Type=\"DateTime\"><Now/></Value>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), new ReOperandBuilderFromCaml());
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_field_refs_and_text_value_without_type_attribute_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef Name=\"StartField\" />" +
                "    <FieldRef Name=\"StopField\" />" +
                "    <FieldRef Name=\"RecurrenceField\" />" +
                "    <Value><Now/></Value>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), new ReOperandBuilderFromCaml());
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        public void test_WHEN_field_refs_and_incorrect_text_value_specified_THEN_expression_is_not_valid()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef Name=\"StartField\" />" +
                "    <FieldRef Name=\"StopField\" />" +
                "    <FieldRef Name=\"RecurrenceField\" />" +
                "    <Value Type=\"DateTime\">test</Value>" +
                "</DateRangesOverlap>");

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), new ReOperandBuilderFromCaml());
            Assert.IsFalse(analyzer.IsValid());
        }

        [Test]
        [ExpectedException(typeof(CamlAnalysisException))]
        public void test_WHEN_expression_is_not_valid_THEN_exception_is_thrown()
        {
            var analyzer = new ReDateRangesOverlapAnalyzer(null, null);
            analyzer.GetOperation();
        }

        [Test]
        public void test_WHEN_expression_is_valid_THEN_operation_is_returned()
        {
            var xml = string.Format(
                "<DateRangesOverlap>" +
                "    <FieldRef Name=\"Title\" />" +
                "    <FieldRef Name=\"Title\" />" +
                "    <FieldRef Name=\"Title\" />" +
                "    <Value Type=\"DateTime\"><Now/></Value>" +
                "</DateRangesOverlap>");

            var b = MockRepository.GenerateStub<IReOperandBuilder>();
            b.Stub(c => c.CreateFieldRefOperand(null)).Return(new FieldRefOperand("Title")).IgnoreArguments();
            b.Stub(c => c.CreateValueOperand(null, false)).Return(new DateTimeValueOperand("Now", false)).IgnoreArguments();

            var analyzer = new ReDateRangesOverlapAnalyzer(XmlHelper.Get(xml), b);
            var operation = analyzer.GetOperation();
            Assert.IsInstanceOf<DateRangesOverlapOperation>(operation);
            var operationT = (DateRangesOverlapOperation)operation;
            Assert.That(operationT.ToExpression().ToString(), Is.EqualTo(
                "DateRangesOverlap(x.get_Item(\"Title\"), x.get_Item(\"Title\"), x.get_Item(\"Title\"), Convert(Convert(Camlex.Now)))"));
        }
    }
}