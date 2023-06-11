// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Framework;

namespace TestHelper.Wrappers.UnityEditor
{
    [TestFixture]
    [SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure")]
    public class GameViewSizeWrapperTest
    {
        private readonly GameViewSizeWrapper _sut = GameViewSizeWrapper.CreateInstance(1920, 1080, "Full HD");

        [Test]
        public void CreateInstance_Instantiate()
        {
            Assert.That(_sut.DisplayText(), Is.EqualTo("Full HD (1920x1080)"));
        }

        [Test]
        public void Equals_WrapperObjectIsNull_NotEquals()
        {
            Assert.That(_sut.Equals(null), Is.False);
        }

        [Test]
        public void Equals_InnerObjectIsNull_NotEquals()
        {
            var other = new GameViewSizeWrapper(null);
            Assert.That(_sut.Equals(other), Is.False);
        }

        [Test]
        public void Equals_DifferentType_NotEquals()
        {
            var editorAssembly = Assembly.Load("UnityEditor.dll");
            var gameViewSize = editorAssembly.GetType("UnityEditor.GameViewSize");
            var gameViewSizeType = editorAssembly.GetType("UnityEditor.GameViewSizeType");
            var typeAspectRatio = Enum.Parse(gameViewSizeType, "AspectRatio");
            var gameViewSizeConstructor = gameViewSize.GetConstructor(new[]
            {
                gameViewSizeType, typeof(int), typeof(int), typeof(string)
            });
            var instance = gameViewSizeConstructor?.Invoke(new[] { typeAspectRatio, 1920, 1080, "Full HD" });
            var other = new GameViewSizeWrapper(instance);

            Assume.That(other.DisplayText(), Is.EqualTo("Full HD (1920:1080 Aspect)"));
            Assert.That(_sut.Equals(other), Is.False);
        }

        [Test]
        public void Equals_DifferentWidth_NotEquals()
        {
            var other = GameViewSizeWrapper.CreateInstance(1919, 1080, "Full HD");
            Assert.That(_sut.Equals(other), Is.False);
        }

        [Test]
        public void Equals_DifferentHeight_NotEquals()
        {
            var other = GameViewSizeWrapper.CreateInstance(1920, 1079, "Full HD");
            Assert.That(_sut.Equals(other), Is.False);
        }

        [Test]
        public void Equals_SameSizeButDifferentName_Equals()
        {
            var other = GameViewSizeWrapper.CreateInstance(1920, 1080, "Another name");
            Assert.That(_sut.Equals(other), Is.True);
        }
    }
}
#endif
