using System;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace FriendsOfDT {
    public class StructureMapControllerFactory : DefaultControllerFactory {
        private readonly IContainer container;

        public StructureMapControllerFactory(IContainer container) {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType) {
            if (controllerType == null)
                return base.GetControllerInstance(requestContext, controllerType);
            var controller = (IController)container.GetInstance(controllerType);
            if (controller == null)
                return base.GetControllerInstance(requestContext, controllerType);
            return controller;
        }
    }
}