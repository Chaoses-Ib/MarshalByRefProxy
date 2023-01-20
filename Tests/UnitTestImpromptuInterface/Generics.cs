﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MarshalByRefProxy;

#if !SELFRUNNER
using NUnit.Framework;
#endif


#if SILVERLIGHT
namespace UnitTestImpromptuInterface.Silverlight
#else
namespace UnitTestImpromptuInterface
#endif
{
  [TestFixture]
    public class Generics : Helper
    {
        [Test]
        public void TestGenericMeth()
        {
       
            GenericMethHelper(3, "3");
            GenericMethHelper(4, "4");
            GenericMethHelper(true, "True");

            GenericMethHelper2(3);
            GenericMethHelper2(4);
            GenericMethHelper2(true);
            GenericMethHelper2("test'");
        }

        private void GenericMethHelper<T>(T param, string expected)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action = new Func<T, string>(it => it.ToString());
            IGenericMeth tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IGenericMeth>(tNew);

            Assert.AreEqual(expected, tActsLike.Action(param));
        }

        private void GenericMethHelper2<T>(T param)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action2 = new Func<T, T>(it => it);
            IGenericMeth tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IGenericMeth>(tNew);

            Assert.AreEqual(param, tActsLike.Action2(param));
        }


        [Test]
        public void TestGenericType()
        {

            GenericHelper(3, "3");
            GenericHelper(4, "4");
            GenericHelper(true, "True");
        }

        private void GenericHelper<T>(T param, string expected)
        {
            dynamic tNew = new ExpandoObject();
            tNew.Funct = new Func<T, string>(it => it.ToString());
            IGenericType<T> tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IGenericType<T>>(tNew);

            Assert.AreEqual(expected, tActsLike.Funct(param));
        }

       [Test]
        public void TestGenericTypeConstraints()
        {

            var tObj = new Object();
            GenericHelperConstraints(tObj, tObj.ToString());
        }

        private void GenericHelperConstraints<T>(T param, string expected) where T : class
        {
            dynamic tNew = new ExpandoObject();
            tNew.Funct = new Func<T, string>(it => it.ToString());
            var tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IGenericTypeConstraints<T>>(tNew);

            Assert.AreEqual(expected, tActsLike.Funct(param));
        }



        [Test]
        public void TestConstraintsMethGeneric()
        {
            var tObj = new Object();
            GenericMethConstraintsHelper(tObj, tObj.ToString());
            var tTest = Tuple.Create(new Uri(@"http://google.com"));
            GenericMethConstraintsHelper2(tTest, tTest.ToString());
        }

        private void GenericMethConstraintsHelper<T>(T param, string expected) where T : class
        {
            dynamic tNew = new ExpandoObject();
            tNew.Action = new Func<T, string>(it => it.ToString());
            var tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IGenericMethWithConstraints>(tNew);

            Assert.AreEqual(expected, tActsLike.Action(param));
        }

        private void GenericMethConstraintsHelper2<T>(T param, string expected) where T : IComparable

        {
            dynamic tNew = new ExpandoObject();
            tNew.Action2 = new Func<T, string>(it => it.ToString());
            var tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IGenericMethWithConstraints>(tNew);

            Assert.AreEqual(expected, tActsLike.Action2(param));
        }
    }
}
