using System;

namespace JG.Core.Installers.Containers
{
    public class TypeBindingConfig
    {
        public Type Type;
        public bool Lazy;
        public bool IsSceneObject;
        public bool SingleInstance = true;
    }
}