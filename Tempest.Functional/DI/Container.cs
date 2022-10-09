using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tempest.Functional.DI
{
    public class Container
    {
        public static readonly Container Empty = new Container();

        internal Container()
        {
        }

        internal Container? Next{get; init;}

        /// <summary>
        /// Registers a singleton that will be created on demand
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public Container Register<TService, TInstance>() where TService : class where TInstance : class, TService
        {
            return DoRegister<TService, TInstance>(Lifetime.Singleton, null);
        }

        /// <summary>
        /// Registers a singleton instance
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public Container Register<TService, TInstance>(TInstance instance) where TService : class where TInstance : class, TService
        {
            return DoRegister<TService, TInstance>(Lifetime.Singleton, instance);
        }

        /// <summary>
        /// Registers an instance with the specified lifetime
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public Container Register<TService, TInstance>(Lifetime lifetime) where TService : class where TInstance : class, TService
        {
            return DoRegister<TService, TInstance>(lifetime, null);
        }

        /// <summary>
        /// Registers an instance with the specified lifetime
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="lifetime"></param>
        /// <param name="instance">The </param>
        /// <returns></returns>
        protected virtual Container DoRegister<TService, TInstance>(Lifetime lifetime, TInstance? instance) where TService : class where TInstance : class, TService
        {
            return new Container<TService, TInstance>(0, lifetime, instance);
        }

        public TService Get<TService>() where TService : class
        {
            if(TryCreate(typeof(TService), out var instance))
            {
                return (TService)instance!;
            }

            throw new Exception("could not create");
        }

        public virtual bool TryCreate(Type type, out object? instance)
        {
            instance = null;
            return false;
        }

        internal virtual bool DoTryCreate(Type type, CreationContext context, out object? instance)
        {
            instance = null;
            return false;
        }

        internal virtual object? TryMakeObject(Type type, CreationContext context)
        {
            return null;
        }
    }

    public class Container<TThisService, TThisInstance> : Container 
        where TThisService : class
        where TThisInstance : class, TThisService
    {
        private readonly object m_SyncRoot = new object();
        
        private object? m_Instance;
        private Lifetime m_Lifetime;

        public Container(int depth, Lifetime lifetime,  TThisInstance? instance)
        {
            this.Depth = depth;
            m_Lifetime = lifetime;

            this.Next = null;

            m_Instance = instance;
        }

        protected int Depth{get;}

        protected override Container DoRegister<TService, TInstance>(Lifetime lifetime, TInstance? instance) where TInstance : class
        {
            return new Container<TService, TInstance, Container<TThisService, TThisInstance>>(this.Depth + 1, lifetime, instance, this);
        }

        public override bool TryCreate(Type type, out object? instance)
        {
            var context = new CreationContext(this);
            return DoTryCreate(type, context, out instance);
        }

        internal override bool DoTryCreate(Type type, CreationContext context, out object? instance)
        {
            for(var next = this.Next; next is not null; next = next.Next)
            {
                instance = next.TryMakeObject(type, context);
                if(instance is not null)
                {
                    return true;
                }
            }

            instance = null;
            return false;
        }

        internal override object? TryMakeObject(Type type, CreationContext context)
        {
            if(typeof(TThisInstance) != type) return null;

            // Try a lock free check first
            if(m_Lifetime == Lifetime.Singleton && m_Instance is not null) return m_Instance;

            // Now be a bit more rigourous
            if(m_Lifetime == Lifetime.Singleton && Interlocked.CompareExchange(ref m_Instance, null, null) is object existingInstance) return existingInstance;

            using(context.Enter(this.Depth))
            {
                var (ctor, parameters) = DetermineConstructor(typeof(TThisInstance));
                if(ctor is null) throw new Exception("no public constructor available");

                var arguments = new object?[parameters.Count];

                for(int i = 0; i < parameters.Count; i++)
                {
                    var parameter = parameters[i];

                    if(context.Root.DoTryCreate(parameter.ParameterType, context, out var argument))
                    {
                        arguments[i] = argument;
                    }
                    else
                    {
                        throw new Exception("could not create argument type");
                    }
                }


                if(m_Lifetime == Lifetime.Transient)
                {
                    return ctor.Invoke(arguments);
                }

                lock(m_SyncRoot)
                {
                    if(m_Instance is null)
                    {
                        m_Instance = ctor.Invoke(arguments);
                    }

                    return m_Instance;
                }
            }
        }

        /// <summary>
        /// Works out which constructor to call
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private (ConstructorInfo? Constructor, IReadOnlyList<ParameterInfo> Parameters) DetermineConstructor(Type type)
        {
            ConstructorInfo? longestConstructor = null;
            IReadOnlyList<ParameterInfo> longestParameters = Array.Empty<ParameterInfo>();

            var constructors = type.GetConstructors();
            if(constructors is null || constructors.Length == 0) return (longestConstructor, longestParameters);

            for(int i = 0; i < constructors.Length; i++)
            {
                var ctor = constructors[i];

                var parameters = ctor.GetParameters();
                if(parameters.Length > longestParameters.Count || longestConstructor is null)
                {
                    longestConstructor = ctor;
                    longestParameters = parameters;
                }
            }

            return (longestConstructor, longestParameters);
        }
    }

    internal class Container<TThisService, TThisInstance, TNext> : Container<TThisService, TThisInstance>
        where TThisService : class
        where TThisInstance : class, TThisService
    {
        private readonly Container m_Next;

        public Container(int depth, Lifetime lifetime,  TThisInstance? instance, Container next) : base(depth, lifetime, instance)
        {
            this.Next = next;
            m_Next = next;
        }

        protected override Container DoRegister<TService, TInstance>(Lifetime lifetime, TInstance? instance) where TInstance : class
        {
            return new Container<TService, TInstance, Container<TThisService, TThisInstance, TNext>>(this.Depth + 1, lifetime, instance, this);
        }

        public override bool TryCreate(Type type, out object? instance)
        {
            var context = new CreationContext(this);
            return DoTryCreate(type, context, out instance);
        }        
    }
}
