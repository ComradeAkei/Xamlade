using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xamlade.Extensions;
using Xamlade.FunctionalAreas;
using Xamlade.LinkWorkers;

namespace Xamlade.jClasses
{
    public interface JProperties
    {
        //Свойства по категориям
        public Dictionary<string, Dictionary<string, Property>> xPropertiesGroup { get; set; }
        
        // Метод для получения свойства по имени
        public Property GetProperty(string name)
        {
            foreach (var KVP in xPropertiesGroup)
                if (KVP.Value.TryGetValue(name, out var property))
                    return property;
            throw new KeyNotFoundException($"Property '{name}' not found.");
        }

       
        public void SetProperty<T>(string name, T value, string category = "main")
        {
            if(Constants.ExcludedWords.Contains(name)) return;
            if(value is not null)
                xPropertiesGroup[category][name] = new Property(value, value.GetType());
        }
        public void SetProperty<T>(string name, T value, byte flags, string category = "main" )
        {
            xPropertiesGroup[category][name] = new Property(value, value.GetType(), flags);
        }
    
        public void InitProperties()
        {
            xPropertiesGroup = new();
            xPropertiesGroup["main"] = new Dictionary<string, Property>();
            xPropertiesGroup["container"] = new Dictionary<string, Property>();
            var type = this.GetType();
            var props = type.GetProperties()
                .Where(prop => !Constants.ExcludedWords.Contains(prop.Name));
            
            foreach (var prop in props)
            {
                var propValue = prop.GetValue(this);
                if (propValue is null) continue;
                SetProperty(prop.Name, propValue);
            }
            
        }

        // Метод для добавления контейнерных свойств
        public void AddContainerProperties()
        {
            if ((this as JControl)?.jParent is not { } parent) return;
            if(parent.ContainerProperties is null) return;
            foreach (var prop in parent.ContainerProperties)
                SetProperty(prop.Item1, prop.Item2((this as JControl)!),category:"container");
        }

        //  метод для добавления специальных свойств
        protected abstract void AddSpecialProperties();

       
        public void UpdateProperty(string name, object? value)
        {
           
            //ОПТИМИЗИРОВАТЬ!
            SetProperty(name,value);
        }
    }

    // Структура для хранения свойств
    public struct Property
    {
        public object? Value;
        public Type Type;
        public byte Flags;

        public Property( object value, Type? t = null, byte flags = 0)
        {
            if (value is Property p)
                (Value, Type, Flags) = (p.Value, p.Type, p.Flags);
            else 
                (Value, Type, Flags) = (value, t ?? value.GetType(), flags);
        }
    }
}
