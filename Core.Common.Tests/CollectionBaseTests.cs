using System;
using System.Collections.Generic;
using Core.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Common.Tests
{
    [TestClass]
    public class CollectionBaseTests
    {
        [TestMethod]
        public void test_collection_item_collection_and_property_change_notification()
        {
            TestList objList = new TestList();
            TestClass objTest = new TestClass();
            bool collectionChanged = false;
            bool propertyChanged = false;

            objList.CollectionChanged += (s, e) => collectionChanged = true;

            objList.Add(objTest);

            Assert.IsTrue(collectionChanged, "Collection change should have fired.");

            objList.ItemPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "DirtyProp")
                    propertyChanged = true;
            };

            objTest.DirtyProp = "test value";

            Assert.IsTrue(propertyChanged, "Item property change should have fired.");
        }

        [TestMethod]
        public void test_collection_dirtiness()
        {
            TestList objList = new TestList();
            TestClass objTest = new TestClass();

            objList.Add(objTest);

            objTest.DirtyProp = "test value";

            Assert.IsTrue(objList.IsDirty, "Collection should be dirty.");
        }

        [TestMethod]
        public void test_collection_property_dirtyness()
        {
            TestClass objTest = new TestClass();
            TestChild objChild = new TestChild();

            objTest.Children.Add(objChild);

            Assert.IsFalse(objTest.IsAnythingDirty(), "Nothing in the object graph should be dirty.");

            objChild.ChildName = "test value";

            Assert.IsTrue(objTest.IsAnythingDirty(), "The test object should be reflecting dirtiness within.");
        }

        [TestMethod]
        public void test_dirty_collection_aggregating()
        {
            TestClass objTest = new TestClass();
            TestChild objChild = new TestChild();

            List<IDirtyCapable> dirtyObjects = objTest.GetDirtyObjects();

            objTest.Children.Add(objChild);

            Assert.IsTrue(dirtyObjects.Count == 0, "There should be no dirty object returned.");

            objChild.ChildName = "test value";
            dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 1, "There should be one dirty object.");

            objTest.DirtyProp = "test value";
            dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 2, "There should be two dirty object.");

            objTest.CleanAll();
            dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 0, "There should be no dirty object returned.");
        }
    }
}
