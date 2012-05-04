using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        private IDictionary<Type, object> _instantiatedServices;

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }

                return _instance;
            }
        }

        private ServiceLocator()
        {
            _instantiatedServices = new Dictionary<Type, object>();
        }

        public T GetService<T>()
        {
            if (_instantiatedServices.ContainsKey(typeof(T)))
            {
                return (T)_instantiatedServices[typeof(T)];
            }
            else
            {
                throw new ApplicationException("cannot find the key");
            }
        }

        public void AddService(Type t, object o)
        {
            if (_instantiatedServices.ContainsKey(t))
            {
                _instantiatedServices[t] = o;
            }
            else
            {
                _instantiatedServices.Add(t, o);
            }
        }
    }
}
