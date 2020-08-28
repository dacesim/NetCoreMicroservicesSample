﻿using Infrastructure.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.EventStores.Projection
{
    public abstract class Projection : IProjection
    {
        private readonly Dictionary<Type, Action<object>> Handlers = new Dictionary<Type, Action<object>>();

        public Type[] Handles => Handlers.Keys.ToArray();

        protected virtual void Projects<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            Handlers.Add(typeof(TEvent), @event => action((TEvent)@event));
        }

        public virtual void Handle(IEvent @event)
        {
            Handlers[@event.GetType()](@event);
        }
    }
}
