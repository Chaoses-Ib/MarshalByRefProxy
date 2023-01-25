using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MarshalByRefProxy
{
    public class MarshalByRefAsCaster : DynamicObject
    {
        public object Target { get; }
#if NETFRAMEWORK
        private System.Security.PermissionSet _permissions;
#endif
        private List<Type> _interfaceTypes;

        public override bool TryConvert(System.Dynamic.ConvertBinder binder, out object result)
        {
            result = null;

            if (binder.Type.IsInterface)
            {
                _interfaceTypes.Insert(0, binder.Type);
#if NETFRAMEWORK
                result = MarshalByRefProxy.DynamicMarshalByRefAs(Target, _permissions, _interfaceTypes.ToArray());
#else
                result = MarshalByRefProxy.DynamicMarshalByRefAs(Target, _interfaceTypes.ToArray());
#endif
                return true;
            }

            if(binder.Type.IsInstanceOfType(Target))
            {
                result = Target;
            }

            return false;
        }


        public MarshalByRefAsCaster(object target,
            IEnumerable<Type> types
#if NETFRAMEWORK
            , System.Security.PermissionSet permissions=null
#endif 
            )
        {
            Target = target;
#if NETFRAMEWORK
            _permissions = permissions;
#endif 
            _interfaceTypes = types.ToList();
        }

    }
}
