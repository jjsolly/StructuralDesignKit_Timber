using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StructuralDesignKitLibrary.RFEM;
using System;
using System.Collections.Generic;

using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;


namespace StructuralDesignKitGH_Core
{
	public class GetData : GH_Component
	{
		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public GetData()
		  : base("GetData", "Nickname",
			"Description",
			"SDK", "RFEM6")
		{
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddBooleanParameter("Run", "Run", "Boolean value to get the RFEM model", GH_ParamAccess.item);

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
		/// <param name="DA">The DA object can be used to retrieve data from input parameters and 
		/// to store data in output parameters.</param>
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
		/// Provides an Icon for every component that will be visible in the User Interface.
		/// Icons need to be 24x24 pixels.
		/// You can add image files to your project resources and access them like this:
		/// return Resources.IconForThisComponent;
		/// </summary>
		protected override System.Drawing.Bitmap Icon => null;

		/// <summary>
		/// Each component must have a unique Guid to identify it. 
		/// It is vital this Guid doesn't change otherwise old ghx files 
		/// that use the old ID will partially fail during loading.
		/// </summary>
		public override Guid ComponentGuid => new Guid("cf20ef61-f014-4776-b675-2f02e0fbe0ae");
	}
}