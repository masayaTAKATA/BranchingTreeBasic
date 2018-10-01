using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace BranchingTreeBasic
{
    public class BranchingTreeBasicInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "BranchingTreeBasic";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("a2703265-d63e-41bc-96c1-010c838ce567");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "meenaxydesign";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
