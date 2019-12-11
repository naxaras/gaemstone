using System.Runtime.CompilerServices;

namespace gaemstone.Common.Utility
{
	public readonly struct NullableRef<T>
	{
		public static readonly NullableRef<T> Empty = new NullableRef<T>();

		private unsafe readonly void* _ref;

		public bool IsNull { get {
			unsafe { return (_ref == null); }
		} }

		public ref T Reference { get {
			unsafe { return ref Unsafe.AsRef<T>(_ref); }
		} }

		public NullableRef(ref T reference)
			{ unsafe { _ref = Unsafe.AsPointer(ref reference); } }
	}
}