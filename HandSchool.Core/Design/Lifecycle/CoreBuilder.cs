using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using HandSchool.Internals;
using HandSchool.Views;

namespace HandSchool.Design.Lifecycle
{
    public class Core : ContainerBuilder, ILifetimeScope
    {
        ILifetimeScope _root, _current;

        ILifetimeScope CurrentLifeScope => _current ?? _root;

        public PlatformBase Platform { get; private set; }
        
        public Dictionary<string, Type> Registar { get; }
        
        public Core()
        {
            Registar = new Dictionary<string, Type>();
        }

        public Core UsePlatform(PlatformBase implementedPlatform)
        {
            this.RegisterInstance(implementedPlatform);
            Platform = implementedPlatform;
            return this;
        }

        public Core UseSchool(SchoolBuilder sb)
        {
            if (_current != null)
                _current.Dispose();
            _current = sb.FromParent(_root);
            return this;
        }

        public Core BuildRoot()
        {
            _root = Build();
            return this;
        }

        #region IContainer

        IDisposer ILifetimeScope.Disposer => CurrentLifeScope.Disposer;

        object ILifetimeScope.Tag => CurrentLifeScope.Tag;

        IComponentRegistry IComponentContext.ComponentRegistry => CurrentLifeScope.ComponentRegistry;

        event EventHandler<LifetimeScopeBeginningEventArgs> ILifetimeScope.ChildLifetimeScopeBeginning
        {
            add => CurrentLifeScope.ChildLifetimeScopeBeginning += value;
            remove => CurrentLifeScope.ChildLifetimeScopeBeginning -= value;
        }

        event EventHandler<LifetimeScopeEndingEventArgs> ILifetimeScope.CurrentScopeEnding
        {
            add => CurrentLifeScope.CurrentScopeEnding += value;
            remove => CurrentLifeScope.CurrentScopeEnding -= value;
        }

        event EventHandler<ResolveOperationBeginningEventArgs> ILifetimeScope.ResolveOperationBeginning
        {
            add => CurrentLifeScope.ResolveOperationBeginning += value;
            remove => CurrentLifeScope.ResolveOperationBeginning -= value;
        }

        ILifetimeScope ILifetimeScope.BeginLifetimeScope()
        {
            return CurrentLifeScope.BeginLifetimeScope();
        }

        ILifetimeScope ILifetimeScope.BeginLifetimeScope(object tag)
        {
            return CurrentLifeScope.BeginLifetimeScope(tag);
        }

        ILifetimeScope ILifetimeScope.BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return CurrentLifeScope.BeginLifetimeScope(configurationAction);
        }

        ILifetimeScope ILifetimeScope.BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return CurrentLifeScope.BeginLifetimeScope(tag, configurationAction);
        }

        void IDisposable.Dispose()
        {
            CurrentLifeScope.Dispose();
        }

        object IComponentContext.ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return CurrentLifeScope.ResolveComponent(registration, parameters);
        }

        #endregion
    }
}
