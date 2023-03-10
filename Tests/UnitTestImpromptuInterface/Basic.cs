// 
//  Copyright 2010  Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using MarshalByRefProxy;
using System.Dynamic;
using Dynamitey;
using Dynamitey.DynamicObjects;
using MarshalByRefProxy.Optimization;

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
    public class Basic : Helper
    {

        

        [Test]
        public void AnonPropertyTest()
        {
            var tAnon = new { Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid() };

            var tActsLike = tAnon.MarshalByRefAs<ISimpeleClassProps>();


            Assert.AreEqual(tAnon.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);
        }





        [Test]
        public void CacheTest()
        {
            var tAnon = new { Prop1 = "Test 1", Prop2 = 42L, Prop3 = Guid.NewGuid() };
            var tAnon2 = new { Prop1 = "Test 2", Prop2 = 43L, Prop3 = Guid.NewGuid() };

            var tActsLike = tAnon.MarshalByRefAs<ISimpeleClassProps>();
            var tActsLike2 = tAnon2.MarshalByRefAs<ISimpeleClassProps>();

            Assert.AreEqual(tActsLike.GetType(), tActsLike2.GetType());

            Assert.AreEqual(tAnon.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tAnon.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tAnon.Prop3, tActsLike.Prop3);

            Assert.AreEqual(tAnon2.Prop1, tActsLike2.Prop1);
            Assert.AreEqual(tAnon2.Prop2, tActsLike2.Prop2);
            Assert.AreEqual(tAnon2.Prop3, tActsLike2.Prop3);

        }

        [Test]
        public void AnonEqualsTest()
        {
            var tAnon = new { Prop1 = "Test 1", Prop2 = 42L, Prop3 = Guid.NewGuid() };

            var tActsLike = tAnon.MarshalByRefAs<ISimpeleClassProps>();
            var tActsLike2 = tAnon.MarshalByRefAs<ISimpeleClassProps>();

            Assert.AreEqual(tActsLike, tActsLike2);


        }

        [Test]
        public void ExpandoPropertyTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();

            ISimpeleClassProps tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpeleClassProps>(tNew);




            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
        }


        [Test]
        public void ExpandoSEtPropertyTest()
        {

            var prop1 = "Test";
            var prop2 = 42L;
            var prop3 = Guid.NewGuid();

            dynamic tNew = new ExpandoObject();


            ISimpeleSetClassProps tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpeleSetClassProps>(tNew);
            tActsLike.Prop1 = prop1;
            tActsLike.Prop2 = prop2;
            tActsLike.Prop3 = prop3;



            Assert.AreEqual(tNew.Prop1, prop1);
            Assert.AreEqual(tNew.Prop2, prop2);
            Assert.AreEqual(tNew.Prop3, prop3);
        }


        [Test]
        public void ImpromptuConversionPropertyTest()
        {

            dynamic tNew = new Dictionary();
            tNew.Prop1 = "Test";
            tNew.Prop2 = "42";
            tNew.Prop3 = Guid.NewGuid();

            var tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpeleClassProps>(tNew);




            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(42L, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
        }

        #if !SELFRUNNER
        [Test]
        public void AliasTest()
        {
           IDictionary<string,object> expando = new ExpandoObject();

           expando.Add("手伝ってくれますか？", new Func<string,string>(it=> "Of Course!!!"));

           expando.Add("★✪The Best Named Property in World!!☮", "yes");


            var mapped = expando.MarshalByRefAs<ITestAlias>();


            Assert.AreEqual("Of Course!!!", mapped.CanYouHelpMe("hmmm"));
            Assert.AreEqual("yes", mapped.URTrippin);
        }
#endif

        [Test]
        public void DoubleInterfacetest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();
            tNew.ReturnProp = new PropPoco();

            IInheritProp tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IInheritProp>(tNew, null, typeof(ISimpeleClassProps));




            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
            Assert.AreEqual(tNew.ReturnProp, tActsLike.ReturnProp);
        }

        [Test]
        public void NestedInterfacetest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.NameLevel1 = "one";
            tNew.Nested = new ExpandoObject();
            tNew.Nested.NameLevel2 = "two";

            INest tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<INest>(tNew);

            Assert.AreEqual(tNew.NameLevel1, tActsLike.NameLevel1);
            Assert.AreEqual(tNew.Nested.NameLevel2, tActsLike.Nested.NameLevel2);

        }

        [Test]
        public void NonNestedInterfaceTest()
        {
            dynamic tNew = new ExpandoObject();
            dynamic tNew2 = new ExpandoObject();
            tNew.NameLevel1 = "one";
            tNew.Nested = new ExpandoObject();
            tNew.Nested2 = new Func<object>(() => tNew2);

            INonNest tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs(tNew);

            Assert.AreEqual(tNew.NameLevel1, tActsLike.NameLevel1);
            Assert.Throws<RuntimeBinderException>(() => { var tval= tActsLike.Nested; });
            Assert.Throws<RuntimeBinderException>(() =>
                                                      {
                                                          var tval = tActsLike.Nested2();
                                                          ;
                                                      });
        }

        [Test]
        public void PartialNonNestedInterfaceTest()
        {
            dynamic tNew = new ExpandoObject();
            dynamic tNew2 = new ExpandoObject();
            tNew.NameLevel1 = "one";
     
            tNew.Nested = new ExpandoObject();
            tNew.Nested2 = new Func<object>(() => tNew2);
            tNew.Nested.NameLevel2 = "two";

            INonPartialNest tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs(tNew);

            Assert.AreEqual(tNew.NameLevel1, tActsLike.NameLevel1);
            Assert.Throws<RuntimeBinderException>(() =>
            {
                var tVal2 = tActsLike.Nested2();
                ;
            });
            Assert.AreEqual(tNew.Nested.NameLevel2, tActsLike.Nested.NameLevel2);
        }

        [Test]
        public void NestedInterfaceMethodtest()
        {
            dynamic tNew = new ExpandoObject();
            dynamic tNew2 = new ExpandoObject();
            tNew.NameLevel1 = "one";
            tNew.Nested = new Func<object, object, object>((x, y) => tNew2);
            tNew.Nested(1, 2).NameLevel2 = "two";

            INestMeth tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<INestMeth>(tNew);

            Assert.AreEqual(tNew.NameLevel1, tActsLike.NameLevel1);
            Assert.AreEqual(tNew.Nested(1,2).NameLevel2, tActsLike.Nested(1,2).NameLevel2);

        }

        [Test]
        public void DoublePropertyTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            tNew.Prop3 = Guid.NewGuid();
            tNew.ReturnProp = new PropPoco();

            IInheritProp tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IInheritProp>(tNew, null, typeof(IPropPocoProp));




            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            Assert.AreEqual(tNew.Prop2, tActsLike.Prop2);
            Assert.AreEqual(tNew.Prop3, tActsLike.Prop3);
            Assert.AreEqual(tNew.ReturnProp, tActsLike.ReturnProp);
        }

        [Test]
        public void EventPropertyCollisionTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.Event = 3;

            IEventCollisions tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IEventCollisions>(tNew, null, typeof(IEvent));


            Assert.AreEqual(tNew.Event, tActsLike.Event);
        }
        
        [Test]
        public void InterfaceDirectDuplicateTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.StartsWith = new Func<string, bool>(x => true);
            
            ISimpleStringMethod tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpleStringMethod>(tNew, null, typeof(ISimpleStringMethod));


            Assert.AreEqual(tNew.StartsWith("test"), tActsLike.StartsWith("test"));
        }

        [Test]
        public void MethodCollisionTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.StartsWith = new Func<string, bool>(x => true);

            ISimpleStringMethod tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpleStringMethod>(tNew, null, typeof(ISimpleStringMethodCollision));
            Assert.AreEqual(tNew.StartsWith("test"), tActsLike.StartsWith("test"));

            dynamic tNew2 = new ExpandoObject();
            tNew2.StartsWith = new Func<string, int>(x => 5);
            ISimpleStringMethodCollision tActsLike2 = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpleStringMethod>(tNew2, null, typeof(ISimpleStringMethodCollision));

            Assert.AreEqual(tNew2.StartsWith("test"), tActsLike2.StartsWith("test"));
        }

        [Test]
        public void DictIndexTest()
        {


            dynamic tNew = new Dictionary();
            tNew.Prop1 = "Test";
            tNew.Prop2 = "42";
            tNew.Prop3 = Guid.NewGuid();

            IObjectStringIndexer tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IObjectStringIndexer>(tNew);




            Assert.AreEqual(tNew["Prop1"], tActsLike["Prop1"]);
        }

        [Test]
        public void ArrayIndexTest()
        {


            var tNew = new[] { "Test1", "Test2" };

            var tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IStringIntIndexer>(tNew);




            Assert.AreEqual(tNew[1], tActsLike[1]);
        }

        [Test]
        public void AnnonMethodsTest()
        {

            var tNew = new
            {
                Action1 = new Action(Assert.Fail),
                Action2 = new Action<bool>(Assert.IsFalse),
                Action3 = new Func<string>(() => "test"),
            };

            ISimpeleClassMeth tActsLike = tNew.MarshalByRefAs<ISimpeleClassMeth>();



            AssertException<AssertionException>(tActsLike.Action1);
            AssertException<AssertionException>(() => tActsLike.Action2(true));

            Assert.AreEqual("test", tActsLike.Action3());


        }


        [Test]
        public void ExpandoMethodsTest()
        {

            dynamic tNew = new ExpandoObject();
            tNew.Action1 = new Action(Assert.Fail);
            tNew.Action2 = new Action<bool>(Assert.IsFalse);
            tNew.Action3 = new Func<string>(() => "test");

            ISimpeleClassMeth tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<ISimpeleClassMeth>(tNew);



            AssertException<AssertionException>(tActsLike.Action1);
            AssertException<AssertionException>(() => tActsLike.Action2(true));

            Assert.AreEqual("test", tActsLike.Action3());


        }
        [Test]
        public void EventPocoPropertyTest()
        {
            var tPoco = new PocoEvent();
            var tActsLike = tPoco.MarshalByRefAs<IEvent>();
            var tSet = false;
            tActsLike.Event += (obj, args) => tSet = true;

            tActsLike.OnEvent(null, null);
            Assert.AreEqual(true, tSet);

        }


        [Test]
        public void EventPocoPropertyTest2()
        {
            var tPoco = new PocoEvent();
            var tActsLike = tPoco.MarshalByRefAs<IEvent>();
            var tSet = false;
            EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
            tActsLike.Event += tActsLikeOnEvent;
            tActsLike.Event -= tActsLikeOnEvent;
            tActsLike.OnEvent(null, null);
            Assert.AreEqual(false, tSet);

        }

        [Test]
        public void EventDynamicPropertyTest()
        {
            object tPoco = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
            IEvent tActsLike = tPoco.MarshalByRefAs<IEvent>();
            var tSet = false;
            tActsLike.Event += (obj, args) => tSet = true;

            tActsLike.OnEvent(null, null);
            Assert.AreEqual(true, tSet);

        }


        [Test]
        public void EventDynamicPropertyTest2()
        {
            object tPoco = Build.NewObject(Prop2: 3, Event: null, OnEvent: new ThisAction<object, EventArgs>((@this, obj, args) => @this.Event(obj, args)));
            IEvent tActsLike = tPoco.MarshalByRefAs<IEvent>();
            var tSet = false;
            EventHandler<EventArgs> tActsLikeOnEvent = (obj, args) => tSet = true;
            tActsLike.Event += tActsLikeOnEvent;
            tActsLike.Event -= tActsLikeOnEvent;
            tActsLike.OnEvent(null, null);
            Assert.AreEqual(false, tSet);

        }

        [Test]
        public void StringPropertyTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.MarshalByRefAs<ISimpleStringProperty>();


            Assert.AreEqual(tAnon.Length, tActsLike.Length);
        }

        [Test]
        public void StringMethodTest()
        {
            var tAnon = "Test 123";
            var tActsLike = tAnon.MarshalByRefAs<ISimpleStringMethod>();


            Assert.AreEqual(tAnon.StartsWith("Te"), tActsLike.StartsWith("Te"));
        }

        [Test]
        public void DynamicArgMethodTest()
        {
            var tPoco = new PocoNonDynamicArg();
            var tActsLike = tPoco.MarshalByRefAs<IDynamicArg>();

            var tList = new List<string>();

            Assert.AreEqual(1, tActsLike.ReturnIt(1));
            Assert.AreEqual(tList, tActsLike.ReturnIt(tList));
        }



        [Test]
        public void DynamicArgMethodTest2()
        {
            dynamic tPoco = new PocoNonDynamicArg();
            dynamic tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs<IDynamicArg>(tPoco);



            Assert.AreEqual(DynamicArgsHelper(tPoco, new[] { 1, 2, 3 }), tActsLike.Params(1, 2, 3));
            Assert.AreEqual(tPoco.Params("test"), tActsLike.Params("test"));
        }

        private bool DynamicArgsHelper(dynamic obj, params dynamic[] objects)
        {
            return obj.Params(objects);
        }



        [Test]
        public void InformalPropTest()
        {
            dynamic tNew = new ExpandoObject();
            tNew.Prop1 = "Test";
            tNew.Prop2 = 42L;
            var tActsLike = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAsProperties(tNew, new Dictionary<string, Type>() { { "Prop1", typeof(string) } });


            Assert.AreEqual(tNew.Prop1, tActsLike.Prop1);
            AssertException<RuntimeBinderException>(() => { var tTest = tActsLike.Prop2; });
        }




        [Test]
        public void OverloadMethodTest()
        {
            var tPoco = new OverloadingMethPoco();
            var tActsLike = tPoco.MarshalByRefAs<IOverloadingMethod>();

            var tValue = 1;
            Assert.AreEqual("int", tActsLike.Func(tValue));
            Assert.AreEqual("object", tActsLike.Func((object)tValue));
        }

        [Test]
        public void OutMethodTest()
        {
            var tPoco = new MethOutPoco();
            var tActsLike = tPoco.MarshalByRefAs<IMethodOut>();

            string tResult = String.Empty;

            var tOut = tActsLike.Func(out tResult);

            Assert.AreEqual(true, tOut);
            Assert.AreEqual("success", tResult);
        }

        [Test]
        public void OutMethodTest2()
        {
            var tPoco = new GenericMethOutPoco();
            var tActsLike = tPoco.MarshalByRefAs<IMethodOut>();

            string tResult = "success";

            var tOut = tActsLike.Func(out tResult);

            Assert.AreEqual(true, tOut);
            Assert.AreEqual(null, tResult);
        }

        [Test]
        public void OutMethodTest3()
        {
            var tPoco = new GenericMethOutPoco();
            var tActsLike = tPoco.MarshalByRefAs<IMethodOut2>();

            int tResult = 3;

            var tOut = tActsLike.Func(out tResult);

            Assert.AreEqual(true, tOut);
            Assert.AreEqual(0, tResult);
        }

        [Test]
        public void GenericMethodTest()
        {
            dynamic ot = new OtherThing();
            IGenericTest test = MarshalByRefProxy.MarshalByRefProxy.MarshalByRefAs(ot);

            var tResult =test.GetThings<Thing>(Guid.Empty);

            Assert.AreEqual(true, tResult is List<Thing>);

        }



        [Test]
        public void GenericOutMethodTest()
        {
            var tPoco = new GenericMethOutPoco();
            var tActsLike = tPoco.MarshalByRefAs<IGenericMethodOut>();

            int tResult = 3;

            var tOut = tActsLike.Func(out tResult);

            Assert.AreEqual(true, tOut);
            Assert.AreEqual(0, tResult);

            string tResult2 = "success";

            var tOut2 = tActsLike.Func(out tResult2);

            Assert.AreEqual(true, tOut2);
            Assert.AreEqual(null, tResult2);
        }

        [Test]
        public void RefMethodTest()
        {
            var tPoco = new MethRefPoco();
            var tActsLike = tPoco.MarshalByRefAs<IMethodRef>();

            int tResult = 1;

            var tOut = tActsLike.Func(ref tResult);

            Assert.AreEqual(true, tOut);
            Assert.AreEqual(3, tResult);

            int tResult2 = 2;

            tOut = tActsLike.Func(ref tResult2);

            Assert.AreEqual(true, tOut);
            Assert.AreEqual(4, tResult2);
        }
    }
}

