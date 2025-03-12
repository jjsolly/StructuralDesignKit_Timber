using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace StructuralDesignKitGH_Core
{
	public class StructuralDesignKitGH_CoreInfo : GH_AssemblyInfo
	{
		public override string Name => "RFEM6_GetData";

		//Return a 24x24 pixel bitmap to represent this GHA library.
		public override Bitmap Icon => null;

		//Return a short string describing the purpose of this GHA library.
		public override string Description => "";

		public override Guid Id => new Guid("8235f0a1-391b-4789-895c-8e8d9840d4c1");

		//Return a string identifying you or your company.
		public override string AuthorName => "";

		//Return a string representing your preferred contact details.
		public override string AuthorContact => "";

		//Return a string representing the version.  This returns the same version as the assembly.
		public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
	}
}