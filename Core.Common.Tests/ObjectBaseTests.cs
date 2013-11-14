using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Common.Tests
{
    [TestClass]
    public class ObjectBaseTests
    {
        [TestMethod]
        public void test_clean_property_change()
        {
            TestClass objTest = new TestClass();
            bool propertyChanged = false;

            objTest.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CleanProp")
                    propertyChanged = true;
            };

            objTest.CleanProp = "test value";

            Assert.IsTrue(propertyChanged, "Changing CleanProp should have set the notiication flag to true.");
        }

        [TestMethod]
        public void test_dirty_property_change()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsDirty, "Object should be clean.");

            objTest.DirtyProp = "test value";

            Assert.IsTrue(objTest.IsDirty, "Object should be dirty.");
        }

        [TestMethod]
        public void test_property_change_single_subscription()
        {
            TestClass objTest = new TestClass();
            int changeCounter = 0;
            PropertyChangedEventHandler handler1 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });
            PropertyChangedEventHandler handler2 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });

            objTest.PropertyChanged += handler1;
            objTest.PropertyChanged += handler1; // should not duplicate
            objTest.PropertyChanged += handler1; // should not duplicate
            objTest.PropertyChanged += handler2;
            objTest.PropertyChanged += handler2; // should not duplicate

            objTest.CleanProp = "test value";

            Assert.IsTrue(changeCounter == 2, "Property change notification should only have been called once.");
        }

        [TestMethod]
        public void test_property_change_dual_syntax()
        {
            TestClass objTest = new TestClass();
            bool propertyChanged = false;

            objTest.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CleanProp" || e.PropertyName == "StringProp")
                    propertyChanged = true;
            };

            objTest.CleanProp = "test value";

            Assert.IsTrue(propertyChanged, "Changing CleanProp should have set the notiication flag to true.");

            propertyChanged = false;
            objTest.StringProp = "test value";

            Assert.IsTrue(propertyChanged, "Changing StringProp should have set the notiication flag to true.");
        }

        [TestMethod]
        public void test_child_dirty_tracking()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsAnythingDirty(), "Nothing in the object graph should be dirty.");

            objTest.Child.ChildName = "test value";

            Assert.IsTrue(objTest.IsAnythingDirty(), "The object graph should be dirty.");

            objTest.CleanAll();

            Assert.IsFalse(objTest.IsAnythingDirty(), "Nothing in the object graph should be dirty.");
        }

        [TestMethod]
        public void test_dirty_object_aggregating()
        {
            TestClass objTest = new TestClass();

            List<IDirtyCapable> dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 0, "There should be no dirty object returned.");

            objTest.Child.ChildName = "test value";
            dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 1, "There should be one dirty object.");

            objTest.DirtyProp = "test value";
            dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 2, "There should be two dirty object.");

            objTest.CleanAll();
            dirtyObjects = objTest.GetDirtyObjects();

            Assert.IsTrue(dirtyObjects.Count == 0, "There should be no dirty object returned.");
        }

        [TestMethod]
        public void test_object_validation()
        {
            TestClass objTest = new TestClass();

            Assert.IsFalse(objTest.IsValid, "Object should not be valid as one its rules should be broken.");

            objTest.StringProp = "Some value";

            Assert.IsTrue(objTest.IsValid, "Object should be valid as its property has been fixed.");
        }
    }
}
