using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDMS.Web.App_Code
{
    //https://blogs.msdn.microsoft.com/davidebb/2009/12/18/passing-anonymous-objects-to-mvc-views-and-accessing-them-using-dynamic/
    //public class DynamicViewPage : ViewPage
    //{
    //    // Hide the base Model property and replace it with a dynamic one
    //    public new dynamic Model { get; private set; }

    //    protected override void SetViewData(ViewDataDictionary viewData)
    //    {
    //        base.SetViewData(viewData);

    //        // Create a dynamic object that can do private reflection over the model object
    //        Model = new ReflectionDynamicObject() { RealObject = ViewData.Model };
    //    }

    //    class ReflectionDynamicObject : DynamicObject
    //    {
    //        internal object RealObject { get; set; }

    //        public override bool TryGetMember(GetMemberBinder binder, out object result)
    //        {
    //            // Get the property value
    //            result = RealObject.GetType().InvokeMember(
    //                binder.Name,
    //                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
    //                null,
    //                RealObject,
    //                null);

    //            // Always return true, since InvokeMember would have thrown if something went wrong
    //            return true;
    //        }
    //    }
    //}
}