// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEditor.TestTools.TestRunner.Api;
using TestStatus = UnityEditor.TestTools.TestRunner.Api.TestStatus;

namespace TestHelper.Editor.TestDoubles
{
    /// <summary>
    /// Implemented only <c>ToXml</c> method.
    /// </summary>
    public class FakeTestResultAdaptor : ITestResultAdaptor
    {
        private readonly string _path;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Fake input NUnit3 XML file. This file should not contain XML declaration; <c>TNode</c> does not expect it.</param>
        public FakeTestResultAdaptor(string path)
        {
            _path = path;
        }

        public TNode ToXml()
        {
            var xmlText = File.ReadAllText(_path);
            var xDocument = XDocument.Parse(xmlText);
            Assume.That(xDocument.Declaration, Is.Null,
                "The test input file should not contain XML declaration; TNode does not expect this.");

            return TNode.FromXml(xmlText);
        }

        public ITestAdaptor Test { get { throw new NotImplementedException(); } }
        public string Name { get { throw new NotImplementedException(); } }
        public string FullName { get { throw new NotImplementedException(); } }
        public string ResultState { get { throw new NotImplementedException(); } }
        public TestStatus TestStatus { get { throw new NotImplementedException(); } }
        public double Duration { get { throw new NotImplementedException(); } }
        public DateTime StartTime { get { throw new NotImplementedException(); } }
        public DateTime EndTime { get { throw new NotImplementedException(); } }
        public string Message { get { throw new NotImplementedException(); } }
        public string StackTrace { get { throw new NotImplementedException(); } }
        public int AssertCount { get { throw new NotImplementedException(); } }
        public int FailCount { get { throw new NotImplementedException(); } }
        public int PassCount { get { throw new NotImplementedException(); } }
        public int SkipCount { get { throw new NotImplementedException(); } }
        public int InconclusiveCount { get { throw new NotImplementedException(); } }
        public bool HasChildren { get { throw new NotImplementedException(); } }
        public IEnumerable<ITestResultAdaptor> Children { get { throw new NotImplementedException(); } }
        public string Output { get { throw new NotImplementedException(); } }
    }
}
