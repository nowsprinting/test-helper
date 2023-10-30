// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using NUnit.Framework;

namespace TestHelper.RuntimeInternals.Wrappers.UnityEditor
{
    [TestFixture]
    public class GameViewSizesWrapperTest
    {
        private readonly GameViewSizesWrapper _sut = GameViewSizesWrapper.CreateInstance();

        [Test]
        public void CreateInstance_Instantiate()
        {
            Assert.That(_sut, Is.Not.Null);
        }

        [Test]
        public void CurrentGroup_GetGameViewSizeGroupInstance()
        {
            var actual = _sut.CurrentGroup();
            Assert.That(actual, Is.Not.Null);
        }
    }
}
#endif
