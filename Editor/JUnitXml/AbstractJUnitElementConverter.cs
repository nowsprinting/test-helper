// Copyright (c) 2023-2024 Koji Hasegawa.
// This software is released under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using NUnit.Framework.Interfaces;

namespace TestHelper.Editor.JUnitXml
{
    /// <summary>
    /// Abstract class of converting element of NUnit3 test result to JUnit XML format (legacy) element.
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
    internal abstract class AbstractJUnitElementConverter
    {
        // JUnit XML format (legacy) element/attribute names.
        protected const string JUnitElementTestsuites = "testsuites";
        protected const string JUnitElementTestsuite = "testsuite";
        protected const string JUnitElementProperties = "properties";
        protected const string JUnitElementProperty = "property";
        protected const string JUnitElementSystemOut = "system-out";
        protected const string JUnitElementTestcase = "testcase";
        protected const string JUnitElementFailure = "failure";
        protected const string JUnitAttributeName = "name";
        protected const string JUnitAttributeDisabled = "disabled";
        protected const string JUnitAttributeErrors = "errors";
        protected const string JUnitAttributeFailures = "failures";
        protected const string JUnitAttributeTests = "tests";
        protected const string JUnitAttributeTime = "time";
        protected const string JUnitAttributeID = "id";
        protected const string JUnitAttributeSkipped = "skipped";
        protected const string JUnitAttributeTimestamp = "timestamp";
        protected const string JUnitAttributeValue = "value";
        protected const string JUnitAttributeClassname = "classname";
        protected const string JUnitAttributeAssertions = "assertions";
        protected const string JUnitAttributeStatus = "status";
        protected const string JUnitAttributeMessage = "message";
        protected const string JUnitAttributeType = "type";

        // JUnit XML format (legacy) element attributes and elements.
        protected string ID => _id;
        protected string Name => _name;
        protected string ClassName => _classname;
        protected int Disabled => _skipped;
        protected int Skipped => _skipped;
        protected static int Errors => 0;
        protected int Failures => _failed + _inconclusive;
        protected int Tests => _passed + _failed + _inconclusive + _skipped;
        protected int Assertions => _asserts;
        protected double Time => _duration; // seconds
        protected string Timestamp => _starttime;
        protected string Status => _result;
        protected bool IsTestCaseSkipped => _result == "Skipped"; // test-case has not skipped attribute
        protected bool IsTestCaseFailed => _result == "Failed"; // test-case has not failures attribute
        protected bool IsTestCaseInconclusive => _result == "Inconclusive"; // test-case has not inconclusive attribute
        protected List<( string, string)> Properties => _properties;
        protected string Reason => _reason;
        protected (string, string) Failure => _failure; // message, stack-trace
        protected string SystemOut => _output;

        // NUnit3 test result element names.
        internal const string NUnitTestRun = "test-run";
        internal const string NUnitTestSuite = "test-suite";
        internal const string NUnitTestCase = "test-case";

        // NUnit3 test result element attributes and elements.
        private readonly string _id;
        private readonly string _name;
        private readonly string _classname;
        private readonly string _result;
        private readonly int _passed;
        private readonly int _failed;
        private readonly int _inconclusive;
        private readonly int _skipped;
        private readonly int _asserts;
        private readonly string _starttime;
        private readonly double _duration; // seconds
        private readonly List<( string, string)> _properties = new List<(string, string)>(); // name, value
        private string _reason;
        private (string, string) _failure; // message, stack-trace
        private string _output;

        /// <summary>
        /// Constructor.
        /// Parse NUnit3 test result elements and store them.
        /// </summary>
        /// <param name="node">NUnit3 test result elements to be converted.</param>
        protected AbstractJUnitElementConverter(TNode node)
        {
            this._id = node.Attributes["id"];
            this._result = node.Attributes["result"];
            this._asserts = System.Convert.ToInt32(node.Attributes["asserts"]);
            this._starttime = node.Attributes["start-time"];
            this._duration = System.Convert.ToDouble(node.Attributes["duration"]);

            switch (node.Name)
            {
                case NUnitTestRun:
                case NUnitTestSuite:
                    this._passed = System.Convert.ToInt32(node.Attributes["passed"]);
                    this._failed = System.Convert.ToInt32(node.Attributes["failed"]);
                    this._inconclusive = System.Convert.ToInt32(node.Attributes["inconclusive"]);
                    this._skipped = System.Convert.ToInt32(node.Attributes["skipped"]);
                    break;
                case NUnitTestCase:
                    this._passed = 0;
                    this._failed = 0;
                    this._inconclusive = 0;
                    this._skipped = 0;
                    break;
            }

            switch (node.Name)
            {
                case NUnitTestSuite:
                case NUnitTestCase:
                    this._name = node.Attributes["name"];
                    this._classname = node.Attributes["classname"];

                    node.ChildNodes.ForEach(child =>
                    {
                        if (child.Name == "properties")
                        {
                            child.ChildNodes.ForEach(property =>
                            {
                                this._properties.Add((property.Attributes["name"], property.Attributes["value"]));
                            });
                        }

                        if (child.Name == "reason")
                        {
                            child.ChildNodes.ForEach(grandchild =>
                            {
                                if (grandchild.Name == "message")
                                {
                                    this._reason = grandchild.Value.Trim();
                                }
                            });
                        }

                        if (child.Name == "failure")
                        {
                            child.ChildNodes.ForEach(grandchild =>
                            {
                                switch (grandchild.Name)
                                {
                                    case "message":
                                        this._failure.Item1 = grandchild.Value.Trim();
                                        break;
                                    case "stack-trace":
                                        this._failure.Item2 = grandchild.Value.Trim();
                                        break;
                                }
                            });
                        }

                        if (child.Name == "output")
                        {
                            this._output = child.Value.Trim();
                        }
                    });
                    break;
                case NUnitTestRun:
                    this._name = string.Empty;
                    this._classname = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// Convert to JUnit XML format (legacy) element.
        /// </summary>
        /// <returns></returns>
        public abstract XElement ToJUnitElement();
    }
}
