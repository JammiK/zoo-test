using System;
using JG.Data.Entities.Interfaces.Attributes;
using JG.Data.Entities.Interfaces.Contexts.Common;
using JG.Data.Entities.Interfaces.Factories.Contexts.Common;

namespace JG.Data.Entities.Factories
{
    public abstract class ContextFactoryBase<T, TS> : IContextFactory where T : IContext
    {
        public virtual bool IgnoreSave { get; }
        public Type GetContextType => typeof(T);
        public Type GetContextSaveType => typeof(TS);
        public virtual ContextLifeScope LifeScope => ContextLifeScope.Global;

        public bool CheckAvailableSave(object contextSave)
        {
            return contextSave is TS;
        }

        public IContext CreateContext()
        {
            return CreateContextInternal();
        }

        public object CreateSave(IContext context)
        {
            if (context is T tContext)
            {
                return CreateSaveInternal(tContext);
            }
            
            throw new Exception($"Unable to cast {context.GetType()} to {typeof(T)}");
        }

        public IContext CreateContext(object contextSave)
        {
            if (contextSave is TS tsContextSave)
            {
                return CreateContextInternal(tsContextSave);
            }
            
            throw new Exception($"Unable to cast {contextSave.GetType()} to {typeof(T)}");
        }

        protected abstract T CreateContextInternal();
        protected abstract TS CreateSaveInternal(T tContext);
        protected abstract T CreateContextInternal(TS contextSave);
    }
}
