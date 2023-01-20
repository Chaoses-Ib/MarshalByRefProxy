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
using System.Linq;
using System.Runtime.Serialization;
using Dynamitey.DynamicObjects;
using MarshalByRefProxy.Optimization;

namespace MarshalByRefProxy.Build
{
    /// <summary>
    /// This interface can be used to define your own custom proxy if you preload it.
    /// </summary>
    /// <remarks>
    /// Advanced usage only! This is required as well as <see cref="MarshalByRefAsProxyAttribute"></see>
    /// </remarks>
    public interface IMarshalByRefAsProxyInitialize : IMarshalByRefAsProxy
    {
        ///<summary>
        /// Method used to Initialize Proxy
        ///</summary>
        ///<param name="original"></param>
        ///<param name="interfaces"></param>
        ///<param name="informalInterface"></param>
        void Initialize(dynamic original, IEnumerable<Type> interfaces =null, IDictionary<string, Type> informalInterface = null);
    }


    /// <summary>
    /// Base class of Emited Proxies
    /// </summary>
    public abstract class MarshalByRefAsProxy : MarshalByRefObject, IMarshalByRefAsProxyInitialize, ISerializable
    {
        /// <summary>
        /// Returns the proxied object
        /// </summary>
        /// <value></value>
        private dynamic MarshalByRefProxyOriginal { get; set; }

        dynamic IMarshalByRefAsProxy.Original { get { return MarshalByRefProxyOriginal; } }

        private bool _init = false;

        /// <summary>
        /// Method used to Initialize Proxy
        /// </summary>
        /// <param name="original"></param>
        /// <param name="interfaces"></param>
        /// <param name="informalInterface"></param>
        void IMarshalByRefAsProxyInitialize.Initialize(dynamic original, IEnumerable<Type> interfaces, IDictionary<string, Type> informalInterface)
        {
            if(((object)original) == null)
                throw new ArgumentNullException("original", "Can't proxy a Null value");

            if (_init)
                throw new MethodAccessException("Initialize should not be called twice!");
            _init = true;
            MarshalByRefProxyOriginal = original;



            var dynamicObj = MarshalByRefProxyOriginal as IEquivalentType;
            if (dynamicObj != null)
            {
                if (interfaces != null)
                {
                    var aggreType = AggreType.MakeTypeAppendable(dynamicObj);

                    foreach (var type in interfaces)
                    {
                        aggreType.AddType(type);

                    }
                }
                if (informalInterface != null)
                {
                    var aggreType = AggreType.MakeTypeAppendable(dynamicObj);
                    aggreType.AddType(new PropretySpecType(informalInterface));
                }
            }
        }



        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(MarshalByRefProxyOriginal, obj)) return true;
            if (!(obj is MarshalByRefAsProxy)) return MarshalByRefProxyOriginal.Equals(obj);
            return Equals((MarshalByRefAsProxy) obj);
        }

        /// <summary>
        /// MarshalByRef proxy should be equivalent to the objects they proxy
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(MarshalByRefAsProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(MarshalByRefProxyOriginal, other.MarshalByRefProxyOriginal)) return true;
            return Equals(other.MarshalByRefProxyOriginal, MarshalByRefProxyOriginal);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return MarshalByRefProxyOriginal.GetHashCode();
        }

#if !SILVERLIGHT

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(MarshalByRefAsProxySerializationHelper));
			
		    var tCustomAttr =
                GetType().GetCustomAttributes(typeof (MarshalByRefAsProxyAttribute), false).OfType<MarshalByRefAsProxyAttribute>().
                    FirstOrDefault();
			
				
            info.AddValue("Context",
                          tCustomAttr == null 
                          ? null
                          : tCustomAttr.Context,typeof(Type));
			
			
			if(Util.IsMono){
				info.AddValue("MonoInterfaces",
                          tCustomAttr == null 
                          ? null
                          : tCustomAttr.Interfaces.Select(it=>it.AssemblyQualifiedName).ToArray(),typeof(string[]));
			}else{
            	info.AddValue("Interfaces",
                          tCustomAttr == null 
                          ? null
                          : tCustomAttr.Interfaces,typeof(Type[]));
			}


            info.AddValue("Original", (object)MarshalByRefProxyOriginal);

        }
#endif

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return MarshalByRefProxyOriginal.ToString();
        }
    }
}
