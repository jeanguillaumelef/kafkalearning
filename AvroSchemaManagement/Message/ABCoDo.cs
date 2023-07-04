// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.7.7.5
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Message
{
    using global::Avro;
	using global::Avro.Specific;
	
	public partial class ABCoDo : ISpecificRecord
	{
		public static Schema _SCHEMA = Schema.Parse("{\"type\":\"record\",\"name\":\"ABCoDo\",\"namespace\":\"Message\",\"fields\":[{\"name\":\"A\",\"typ" +
				"e\":\"int\"},{\"name\":\"B\",\"type\":\"int\"},{\"name\":\"C\",\"type\":[\"null\",\"int\"]},{\"name\":\"" +
				"D\",\"type\":[\"null\",\"int\"]}]}");
		private int _A;
		private int _B;
		private System.Nullable<int> _C;
		private System.Nullable<int> _D;
		public virtual Schema Schema
		{
			get
			{
				return ABCoDo._SCHEMA;
			}
		}
		public int A
		{
			get
			{
				return this._A;
			}
			set
			{
				this._A = value;
			}
		}
		public int B
		{
			get
			{
				return this._B;
			}
			set
			{
				this._B = value;
			}
		}
		public System.Nullable<int> C
		{
			get
			{
				return this._C;
			}
			set
			{
				this._C = value;
			}
		}
		public System.Nullable<int> D
		{
			get
			{
				return this._D;
			}
			set
			{
				this._D = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.A;
			case 1: return this.B;
			case 2: return this.C;
			case 3: return this.D;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.A = (System.Int32)fieldValue; break;
			case 1: this.B = (System.Int32)fieldValue; break;
			case 2: this.C = (System.Nullable<int>)fieldValue; break;
			case 3: this.D = (System.Nullable<int>)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}