using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace BranchingTreeBasic
{
    public class BranchingTreeBasicComponent : GH_Component
    {
        //Constructor

        public BranchingTreeBasicComponent()
          : base("BranchingTreeBasic", "BranchingTreeBasic", "For prototype", "User", "Test")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Length", "L", "The length of trunk", GH_ParamAccess.item);
            pManager.AddNumberParameter("Branch Angle", "BA", "The Angle(degree) of branches", GH_ParamAccess.item);
            pManager.AddNumberParameter("Branch Scale", "BS", "The Scale of branches", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Branch Number", "N", "The number of branches", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Check", "C", "Check line list", GH_ParamAccess.item);
            pManager.AddLineParameter("Branches", "B", "Generated branching", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //1.Declare placeholder variables for the input data.
            double length = double.NaN;
            double branchAngle = double.NaN;
            double branchScale = double.NaN;
            int num = 0;

            //2.Retrieve input data.
            if (!DA.GetData(0, ref length)) { return; }
            if (!DA.GetData(1, ref branchAngle)) { return; }
            if (!DA.GetData(2, ref branchScale)) { return; }
            if (!DA.GetData(3, ref num)) { return; }

            //Set the branch angle by converting it from degs to radians
            double bAngRad = ((Math.PI / 100) * branchAngle);

            //Create a new line in the yDirection from the origin and add it to the list
            List<Line> lines = new List<Line>();
            Point3d stPt = new Point3d(0, 0, 0);
            Vector3d unitYvect = new Vector3d(0, 1, 0);
            Line ln0 = new Line(stPt, unitYvect, length);
            lines.Add(ln0);

            //Create two temporary lists and set the first equal to the lines list
            List<Line> tempList = new List<Line>();
            List<Line> tempList2 = new List<Line>();
            tempList = lines;

            int i = 0;
            while (i < num)
            {
                tempList2 = CreateBranch(tempList, branchScale, bAngRad);
                tempList = tempList2;
                lines.AddRange(tempList2);
                i++;
            }


            int count = lines.Count;

            DA.SetData(0, count);
            DA.SetDataList(1, lines);
        }

        //Method
        public List<Line> CreateBranch(List<Line> lines, double bScale, double bAng)
        {
            var newLines = new List<Line>();

            foreach (Line ln in lines)
            {
                //Get the length of the current trunk and create a new scaled branch.
                double newLength = ln.Length * bScale;

                //Get the endPt of the trunk and its tangent vector.
                Point3d endPt = ln.To;
                Vector3d unitTan = ln.UnitTangent;

                //Create two new lines and make the second line have a negative rotation anngle
                Line ln1 = new Line(endPt, unitTan, newLength);
                ln1.Transform(Rhino.Geometry.Transform.Rotation(bAng, endPt));
                Line ln2 = new Line(endPt, unitTan, newLength);
                ln2.Transform(Rhino.Geometry.Transform.Rotation(bAng * -1, endPt));

                //Add these new branches to the list
                newLines.Add(ln1);
                newLines.Add(ln2);
            }

            return newLines;

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                //>>Icon made by Freepik from www.flaticon.com 
                return BranchingTreeBasic.Properties.Resources.treeIcon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c9856bc7-6ce8-4d01-8f39-c97b80e6c950"); }
        }
    }
}
