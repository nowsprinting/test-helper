// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using NUnit.Framework;

namespace TestHelper.Wrappers.UnityEditor
{
    [TestFixture]
    public class GameViewWrapperTest
    {
        private readonly GameViewWrapper _sut = GameViewWrapper.GetWindow();

        [Test]
        public void GetWindow()
        {
            Assert.That(_sut, Is.Not.Null);
        }

        [Test]
        public void SelectedSizeIndex()
        {
            var beforeSelectedIndex = _sut.SelectedSizeIndex();

            _sut.SelectedSizeIndex(0);
            Assert.That(_sut.SelectedSizeIndex(), Is.EqualTo(0));

            _sut.SelectedSizeIndex(beforeSelectedIndex);
        }
    }
}
#endif
