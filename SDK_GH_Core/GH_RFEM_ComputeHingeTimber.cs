using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using SDK = StructuralDesignKitLibrary;
using StructuralDesignKitLibrary.RFEM;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
using System.Linq;
using StructuralDesignKitGH_Core.Helpers;
using System.Windows.Forms.VisualStyles;



namespace StructuralDesignKitGH_Core
{
	public class GH_RFEM_ComputeHingeTimber : GH_Component
	{
		/// <summary>
		/// Initializes a new instance of the GH_RFEM_ComputeHingeTimber class.
		/// </summary>
		public GH_RFEM_ComputeHingeTimber()
		  : base("GetTimberHinge", "Get_T_Hinge",
			  "Retrieve the surface hinges from the RFEM model",
			  "SDK", "RFEM6")
		{
		}

		

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddGenericParameter("RFEM Model", "model", "RFEM model obtained from the getModel component", GH_ParamAccess.item);
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddGenericParameter("HingeObject", "H", "Hinge object to pass on following SDK component", GH_ParamAccess.list);
			pManager.AddLineParameter("outline", "outline", "outline", GH_ParamAccess.list);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			//input
			ModelClient model = null;

			
			//output
			List<Line> OutputLines = new List<Line>();


			//input definition
			List<GH_Helper_HingeResults> results = new List<GH_Helper_HingeResults>();
			List<GH_Helper_HingeResults> hingeResults = new List<GH_Helper_HingeResults>();


			DA.GetData(0, ref model);


			# region Create Hinge objects
			try
			{



				//get object info
				var listLinesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_LINE);
				var listSurfacesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_SURFACE);
				var listHingesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_LINE_HINGE);
				var listNodesID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_NODE);

				List<node> nodes = new List<node>();
				List<line> lines = new List<line>();
				List<surface> surfaces = new List<surface>();
				List<line_hinge> hinges = new List<line_hinge>();

				foreach (var item in listNodesID)
				{
					nodes.Add(model.get_node(item.no));
				}

				foreach (var item in listLinesID)
				{
					lines.Add(model.get_line(item.no));
				}

				foreach (var item in listSurfacesID)
				{
					surfaces.Add(model.get_surface(item.no));
				}

				foreach (var item in listHingesID)
				{
					hinges.Add(model.get_line_hinge(item.no));
				}




				List<SurfaceEdge> SurfaceEdgesList = new List<SurfaceEdge>();

				foreach (surface srf in surfaces)
				{

					List<Curve> crvs = new List<Curve>();
					List<Point3d> pts = new List<Point3d>();
					foreach (int i in srf.boundary_lines)
					{
						line L = lines.Where(p => p.no == i).First();

						Line RhinoLine = GH_Helper_HingeResults.ConvertLineRFEM_ToRhino(L, nodes);
						crvs.Add(RhinoLine.ToNurbsCurve());

						node startNode = nodes.Where(p => p.no == L.definition_nodes[0]).First();
						node EndNode = nodes.Where(p => p.no == L.definition_nodes[1]).First();

						Point3d startPoint = new Point3d(startNode.global_coordinate_1, startNode.global_coordinate_2, startNode.global_coordinate_3);
						Point3d endPoint = new Point3d(EndNode.global_coordinate_1, EndNode.global_coordinate_2, EndNode.global_coordinate_3);

						if (!pts.Contains(startPoint)) pts.Add(startPoint);
						if (!pts.Contains(endPoint)) pts.Add(endPoint);

					}

					Curve joinedEdge = Curve.JoinCurves(crvs)[0];

					Brep surface = Brep.CreatePlanarBreps(joinedEdge, 0.00001)[0];

					PolylineCurve plCrv = (PolylineCurve)joinedEdge;

					Curve crv = plCrv.ToPolyline().GetSegments()[0].ToNurbsCurve();
					Point3d mid = crv.PointAtNormalizedLength(0.5);
					double t;
					crv.ClosestPoint(mid, out t);
					//crv.FrameAt(t,out plane);
					double[] d = { 0, t };

					Plane plane = crv.GetPerpendicularFrames(d)[1];

					Vector3d V1 = plane.YAxis;
					V1 *= 0.1;

					Transform T1 = Transform.Translation(V1);


					Point3d pt1 = plane.Origin;


					pt1.Transform(T1);



					if(surface.Faces[0].DuplicateFace(false).ClosestPoint(pt1).DistanceTo(pt1)>0.05)
					{
						joinedEdge.Reverse();
					}



					List<Curve> curves  = new List<Curve>();
					foreach(Line segment in plCrv.ToPolyline().GetSegments())
					{
						curves.Add(segment.ToNurbsCurve());
					}
					SurfaceEdgesList.Add(new SurfaceEdge(srf.no, curves));

				}


				foreach (line_hinge hinge in hinges)
				{
					hingeResults = GH_Helper_HingeResults.CreateHinges(hinge);

					foreach (GH_Helper_HingeResults h in hingeResults)
					{
						h.UpdateLine(lines, nodes, SurfaceEdgesList);
					}
				}


				bool modelHasAnyResults = model.has_any_results();

				if (modelHasAnyResults)
				{

					List<object_location> locationsList = new List<object_location>();

					foreach (var item in listHingesID)
					{

						object_location loc = new object_location();
						loc.no = item.no;
						loc.type = object_types.E_OBJECT_TYPE_LINE_HINGE;
						locationsList.Add(loc);
					}
					object_location[] locations = locationsList.ToArray();






					//Get load combinations ULS

					var LoadCombiID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_LOAD_COMBINATION);
					var designSituationID = model.get_all_object_numbers_by_type(object_types.E_OBJECT_TYPE_DESIGN_SITUATION);

					List<int> designSituationULS = new List<int>();

					List<load_combination> ULSLoadCombinations = new List<load_combination>();

					foreach (var item in designSituationID)
					{
						design_situation D = model.get_design_situation(item.no);
						if (D.design_situation_type == "DESIGN_SITUATION_TYPE_STR_PERMANENT_AND_TRANSIENT_6_10") designSituationULS.Add(item.no);
					}


					foreach (var item in LoadCombiID)
					{
						load_combination combi = model.get_load_combination(item.no);
						if (designSituationULS.Contains(combi.design_situation)) ULSLoadCombinations.Add(combi);
					}



					//Update forces in hinge results

					foreach (load_combination lc in ULSLoadCombinations)
					{
						if (model.has_results(case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION, lc.no))
						{

							var hingeForces = model.get_results_for_line_hinges_forces(case_object_types.E_OBJECT_TYPE_LOAD_COMBINATION, lc.no, locations);


							List<line_hinges_forces_row> cleanedHF = new List<line_hinges_forces_row>();


							//Removes the last rows of each line hinge values where info such as Extreme values, average and so on are stored
							for (int i = 0; i < hingeForces.Length; i++)
							{
								if (hingeForces[i].description == "Extremes") i += 10;
								if (hingeForces[i].description == "Average")
								{
									cleanedHF.Add(hingeForces[i]);
									i += 2;
								}
								else if (hingeForces[i].description == "Total max/min values with corresponding values")
								{
									break;
								}

								else cleanedHF.Add(hingeForces[i]);
							}


							//Populate the result objects
							foreach (var result in cleanedHF)
							{
								GH_Helper_HingeResults hinge = hingeResults.Where(p => p.SurfaceID == result.row.surface_no).Where(p => p.LineID == result.row.line_no).First();

								if (!hinge.PositionParams.Contains(result.row.location)) hinge.PositionParams.Add(result.row.location);

								if (hinge.LC.Contains(lc.no))
								{
									int index = hinge.LC.IndexOf(lc.no);

									if (result.description == "Average")
									{
										hinge.N_Average.Add(result.row.line_hinge_force_n);
										hinge.Vy_Average.Add(result.row.line_hinge_force_v_y);
										hinge.Vz_Average.Add(result.row.line_hinge_force_v_z);
										hinge.N_Vy_Combined_Average.Add(Math.Sqrt(Math.Pow(hinge.N_Average[index], 2) + Math.Pow(hinge.Vy_Average[index], 2)));
										hinge.Mx_Average.Add(result.row.line_hinge_moment_m_x);
									}
									else
									{
										
										hinge.N[index].Add(result.row.line_hinge_force_n);
										hinge.Vy[index].Add(result.row.line_hinge_force_v_y);
										hinge.Vz[index].Add(result.row.line_hinge_force_v_z);
										hinge.N_Vy_Combined[index].Add(Math.Sqrt(Math.Pow(hinge.N[index].Last(), 2) + Math.Pow(hinge.Vy[index].Last(), 2)));
										hinge.Mx[index].Add(result.row.line_hinge_moment_m_x);
									}
									
								}
								else
								{
									hinge.LC.Add(lc.no);
									hinge.LC_Names.Add(lc.name);
									//Need a funciton to retrieve the load duration
									//hinge.loadDuration.Add(lc.load_duration
									int index = hinge.LC.IndexOf(lc.no);
									hinge.N.Add(new List<double>());
									hinge.Vy.Add(new List<double>());
									hinge.Vz.Add(new List<double>());
									hinge.N_Vy_Combined.Add(new List<double>());
									hinge.Mx.Add(new List<double>());





									hinge.N[index].Add(result.row.line_hinge_force_n);
									hinge.Vy[index].Add(result.row.line_hinge_force_v_y);
									hinge.Vz[index].Add(result.row.line_hinge_force_v_z);
									hinge.N_Vy_Combined[index].Add(Math.Sqrt(Math.Pow(hinge.N[index].Last(), 2) + Math.Pow(hinge.Vy[index].Last(), 2)));
									hinge.Mx[index].Add(result.row.line_hinge_moment_m_x);
								}

							}
						}
					}



					//Function to correct the positions
					//It appears that RFEM API does not always return the correct position of the value
					//In some cases (still unclear when), the second position is skipped, which creates prositions which are longer than the line itself.
					foreach (GH_Helper_HingeResults H in hingeResults)
					{
						if (H.PositionParams.Last() > H.BaseLine.Length)
						{
							double missingPosition = H.PositionParams[1] / 2;
							List<double> fixedPosition = new List<double>();
							H.PositionParams.Insert(1, missingPosition);

							H.PositionParams.RemoveAt(H.PositionParams.Count - 1);
						}


						//Update hinge orientation:
						H.UpdateOrientation(surfaces, lines, nodes);

						//Update positions points
						H.UpdatePositions();

					}



				}
				else
				{
					Console.WriteLine("Model has no results");
				}


				foreach (var H in hingeResults)
				{
					OutputLines.Add(H.BaseLine);
				}

				model.Close();

			}

			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);


			}

			#endregion


			//output definition
			DA.SetDataList(0, hingeResults);
			DA.SetDataList(1, OutputLines);

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
			get { return new Guid("4BCE562C-CDA3-46FA-9E8E-52F71D1DE781"); }
		}


		
	}


	//Structure to store the RFEM surface ID and the edges in a continuous direction
	public class SurfaceEdge
	{
		public int SrfID { get; set; }
		public List<Curve> Edges { get; set; }

		public SurfaceEdge(int srfID, List<Curve> edges)
		{
			SrfID = srfID;
			Edges = edges;	
		}
	}
}