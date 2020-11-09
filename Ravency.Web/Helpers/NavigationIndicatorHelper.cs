using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Ravency.Web.Helpers
{
    public static class NavigationIndicatorHelper
    {
        public static string MakeClassActive(this IUrlHelper urlHelper, string controller, params string[] actions)
            => Make(urlHelper, "active", controller, actions);

        public static string MakeClassActive(this IUrlHelper urlHelper, string area)
            => Make(urlHelper, "active", area);

        public static string MakeTreeViewMenuOpen(this IUrlHelper urlHelper, string area)
            => Make(urlHelper, "menu-open", area);

        private static string Make(this IUrlHelper urlHelper, string returnValue, string controller, params string[] actions)
        {
            var controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
            var methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();

            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(methodName))
            {
                foreach (var action in actions)
                {
                    if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
                    {
                        if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                        {
                            return returnValue;
                        }
                    }
                }
            }

            return null;
        }

        private static string Make(this IUrlHelper urlHelper, string returnValue, string area)
        {
            var areaName = urlHelper.ActionContext.RouteData.Values["area"];
            if (areaName != null)
            {
                var areaNameStr = areaName.ToString();
                if (!string.IsNullOrEmpty(areaNameStr))
                {
                    if (areaNameStr.Equals(area, StringComparison.OrdinalIgnoreCase))
                    {
                        return returnValue;
                    }
                }
            }

            return null;
        }
    }
}
