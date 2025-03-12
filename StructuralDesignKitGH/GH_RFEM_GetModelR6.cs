using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using StructuralDesignKitLibrary.RFEM;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;

namespace StructuralDesignKitGH
{
	public class GH_RFEM_GetModelR6 : GH_Component
	{
		/// <summary>
		/// Initializes a new instance of the MyComponent1 class.
		/// </summary>
		public GH_RFEM_GetModelR6()
		  : base("Get RFEM6 model", "Model",
			  "Get the active model in RFEM 6",
			  "SDK", "RFEM 6")
		{
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddBooleanParameter("Run", "Run", "Boolean value to get the RFEM model",GH_ParamAccess.item);
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddGenericParameter("Model", "Model", "Model", GH_ParamAccess.item);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			bool run = false;
			DA.GetData(0, ref run);
			ModelClient model = null;


			if (run)
			{
				RFEM6_Utilities RFEM = new RFEM6_Utilities();
				model = RFEM.GetActiveModel();
			}
			DA.SetData(0, model);
		}

		/// <summary>
		/// Provides an Icon for the component.
		/// </summary>
		protected override System.Drawing.Bitmap Icon
		{
			get
			{
				//You can add image files to your project resources and access them like this:
				// return Resources.IconForThisComponent;
				return null;
			}
		}

		/// <summary>
		/// Gets the unique ID for this component. Do not change this ID after release.
		/// </summary>
		public override Guid ComponentGuid
		{
			get { return new Guid("6FBB10F0-5AF9-4E87-AC98-6B34C6DB97EA"); }
		}
	}
}