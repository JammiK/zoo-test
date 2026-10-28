using System;
using System.Collections.Generic;

namespace JG.Core.Installers.Containers
{
    public class DiContainerConfig
    {
        public readonly List<TypeBindingConfig> Bindings = new List<TypeBindingConfig>();

        public void SelfBind<T>(bool lazy = false, bool singleInstance = true)
        {
            var typeBinding = new TypeBindingConfig()
            {
                Type = typeof(T),
                Lazy = lazy,
                SingleInstance = singleInstance
            };
            Bindings.Add(typeBinding);
        }

        public void SelfBind(Type type, bool lazy = false, bool singleInstance = true)
        {
            var typeBinding = new TypeBindingConfig()
            {
                Type = type,
                Lazy = lazy,
                SingleInstance = singleInstance
            };
            
            Bindings.Add(typeBinding);
        }
        
        
        /// <summary>
        /// T - type of new registration
        /// TU - type of old registration
        /// </summary>
        /// <param name="lazy"></param>
        /// <param name="singleInstance"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        public void OverrideSelfBind<T, TU>(bool lazy = false, bool singleInstance = true)
        {
            var removedAmount = Bindings.RemoveAll(binding => binding.Type == typeof(TU));
            if (removedAmount == 0)
            {
                throw new Exception($"Can't override binding for {typeof(TU)}");
            }
            
            var typeBinding = new TypeBindingConfig()
            {
                Type = typeof(T),
                Lazy = lazy,
                SingleInstance = singleInstance,
            };
            Bindings.Add(typeBinding);
        }
        
        public void OverrideSelfBind(Type type, Type oldType, bool lazy = false, bool singleInstance = true)
        {
            var removedAmount = Bindings.RemoveAll(binding => binding.Type == oldType);
            if (removedAmount == 0)
            {
                throw new Exception($"Can't override binding for {oldType}");
            }
            
            var typeBinding = new TypeBindingConfig()
            {
                Type = type,
                Lazy = lazy,
                SingleInstance = singleInstance,
            };
            Bindings.Add(typeBinding);
        }
        
        public void ViewBind<T>()
        {
            var typeBinding = new TypeBindingConfig()
            {
                Type = typeof(T),
                IsSceneObject = true
            };
            Bindings.Add(typeBinding);
        }
    }
}