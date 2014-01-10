using System;

namespace Com.BaseLibrary.Entity
{
	public class ReferencedEntityAttribute : Attribute
	{

		public ReferencedEntityAttribute(Type type)
		{
			this.Type = type;
		}
        public ReferencedEntityAttribute()
        {
           
        }
		
		public Type Type { get; set; }
		public string Prefix { get; set; }
		public string ConditionalProperty { get; set; }
	}
}
