using System;
using System.Collections.Generic;
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

        public virtual Container Register<TService, TInstance>(TInstance? instance) 
            where TService : class
            where TInstance : class, TService
        {
            return new Container<TService, TInstance>(0, instance);
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

    internal class Container<TThisService, TThisInstance> : Container 
        where TThisService : class
        where TThisInstance : class, TThisService
    {
        private readonly object m_SyncRoot = new object();
        private object? m_Instance;

        public Container(int depth, TThisInstance? instance)
        {
            this.Depth = depth;
            this.Next = null;

            m_Instance = instance;
        }

        protected int Depth{get;}

        public override Container Register<TService, TInstance>(TInstance? instance) where TInstance : class
        {
            return new Container<TService, TInstance, TThisService, TThisInstance>(this.Depth + 1, instance, this);
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
            if(m_Instance is not null) return m_Instance;

            // Now be a bit more rigourous
            if(Interlocked.CompareExchange(ref m_Instance, null, null) is object existingInstance) return existingInstance;

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

    internal class Container<TThisService, TThisInstance, TNextService, TNextInstance> : Container<TThisService, TThisInstance>
        where TThisService : class
        where TThisInstance : class, TThisService
        where TNextService : class
        where TNextInstance : class, TNextService
    {
        private readonly Container<TNextService, TNextInstance> m_Next;

        public Container(int depth, TThisInstance? instance, Container<TNextService, TNextInstance> next) : base(depth, instance)
        {
            this.Next = next;
            m_Next = next;
        }

        public override Container Register<TService, TInstance>(TInstance? instance) where TInstance : class
        {
            return new Container<TService, TInstance, TThisService, TThisInstance>(this.Depth + 1, instance, this);
        }

        public override bool TryCreate(Type type, out object? instance)
        {
            var context = new CreationContext(this);
            return DoTryCreate(type, context, out instance);
        }        
    }
}
