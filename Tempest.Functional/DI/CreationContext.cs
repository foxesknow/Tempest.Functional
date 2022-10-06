using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tempest.Functional.DI
{
    internal class CreationContext
    {
        private readonly HashSet<int> m_Depths = new();

        public CreationContext(Container root)
        {
            this.Root = root;
        }

        public Container Root{get;}

        public DepthLock Enter(int level)
        {
            if(m_Depths.Add(level)) 
            {
                return new (level, this);
            }
            else
            {
                throw new Exception("recursive construction!");
            }
        }

        public readonly ref struct DepthLock
        {
            private readonly int m_Level;
            private readonly CreationContext m_Context;

            public DepthLock(int level, CreationContext context)
            {
                m_Level = level;
                m_Context = context;
            }

            public void Dispose()
            {
                m_Context.m_Depths.Remove(m_Level);
            }
        }
    }
}
