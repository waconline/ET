﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
	[BsonIgnoreExtraElements]
	public partial class Entity : ComponentWithId
	{
		[BsonElement]
		[BsonIgnoreIfNull]
		private HashSet<Component> components;

		[BsonIgnore]
		private Dictionary<Type, Component> componentDict;

		protected Entity()
		{
			this.components = new HashSet<Component>();
			this.componentDict = new Dictionary<Type, Component>();
		}

		protected Entity(long id): base(id)
		{
			this.components = new HashSet<Component>();
			this.componentDict = new Dictionary<Type, Component>();
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			foreach (Component component in this.GetComponents())
			{
				try
				{
					component.Dispose();
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}

			this.components.Clear();
			this.componentDict.Clear();
		}

		public Component AddComponent(Type type)
		{
			Component component = ComponentFactory.CreateWithParent(type, this);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
			}

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K>() where K : Component, new()
		{
			K component = ComponentFactory.CreateWithParent<K>(this);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K, P1>(P1 p1) where K : Component, new()
		{
			K component = ComponentFactory.CreateWithParent<K, P1>(this, p1);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}
			
			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K, P1, P2>(P1 p1, P2 p2) where K : Component, new()
		{
			K component = ComponentFactory.CreateWithParent<K, P1, P2>(this, p1, p2);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}
			
			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : Component, new()
		{
			K component = ComponentFactory.CreateWithParent<K, P1, P2, P3>(this, p1, p2, p3);

			if (this.componentDict.ContainsKey(component.GetType()))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}
			this.componentDict.Add(component.GetType(), component);
			return component;
		}

		public void RemoveComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof(K), out component))
			{
				return;
			}

			this.components.Remove(component);
			this.componentDict.Remove(typeof(K));

			component.Dispose();
		}

		public void RemoveComponent(Type type)
		{
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return;
			}

			this.components?.Remove(component);
			this.componentDict.Remove(type);

			component.Dispose();
		}

		public K GetComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof(K), out component))
			{
				return default(K);
			}
			return (K)component;
		}

		public Component GetComponent(Type type)
		{
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return null;
			}
			return component;
		}

		public Component[] GetComponents()
		{
			return this.componentDict.Values.ToArray();
		}

		public override void BeginInit()
		{
			this.components = new HashSet<Component>();
			this.componentDict = new Dictionary<Type, Component>();
		}

		public override void EndInit()
		{
			try
			{
				this.InstanceId = IdGenerater.GenerateId();

				this.componentDict.Clear();

				if (this.components != null)
				{
					foreach (Component component in this.components)
					{
						component.Parent = this;
						this.componentDict.Add(component.GetType(), component);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
}