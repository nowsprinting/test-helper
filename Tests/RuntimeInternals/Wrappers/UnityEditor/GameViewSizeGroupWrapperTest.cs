// Copyright (c) 2023 Koji Hasegawa.
// This software is released under the MIT License.

#if UNITY_EDITOR
using NUnit.Framework;

namespace TestHelper.RuntimeInternals.Wrappers.UnityEditor
{
    [TestFixture]
    public class GameViewSizeGroupWrapperTest
    {
        private readonly GameViewSizeGroupWrapper _sut = GameViewSizesWrapper.CreateInstance().CurrentGroup();
        private readonly GameViewSizeWrapper _addSize = GameViewSizeWrapper.CreateInstance(2, 1, "Test Added");

        [Test]
        public void CreateInstance_Instantiate()
        {
            Assert.That(_sut, Is.Not.Null);
        }

        [Test]
        public void GetTotalCount_GotCount()
        {
            Assert.That(_sut.GetTotalCount(), Is.GreaterThan(0));
        }

        [Test]
        public void GetGameViewSize_GotGameViewSize()
        {
            var actual = _sut.GetGameViewSize(0);
            actual.GetInnerInstance();
            Assert.That(actual.GetInnerInstance(), Is.Not.Null);
        }

        [Test, Order(0)]
        public void AddCustomSize_Added()
        {
            var beforeCount = _sut.GetTotalCount();
            _sut.AddCustomSize(_addSize);

            var afterCount = _sut.GetTotalCount();
            Assert.That(afterCount, Is.EqualTo(beforeCount + 1));
        }

        [Test, Order(1)]
        public void IndexOf_FoundAddedSize()
        {
            var index = _sut.IndexOf(_addSize);
            Assert.That(index, Is.EqualTo(_sut.GetTotalCount() - 1));
        }

        [Test]
        public void IndexOf_NotFound()
        {
            var notExistSize = GameViewSizeWrapper.CreateInstance(20, 10, "Not Exist");
            var index = _sut.IndexOf(notExistSize);
            Assert.That(index, Is.EqualTo(-1));
        }

        [Test, Order(2)]
        public void RemoveCustomSize_Removed()
        {
            var beforeCount = _sut.GetTotalCount();
            var index = _sut.IndexOf(_addSize);
            _sut.RemoveCustomSize(index);

            var afterCount = _sut.GetTotalCount();
            Assert.That(afterCount, Is.EqualTo(beforeCount - 1));
        }
    }
}
#endif
