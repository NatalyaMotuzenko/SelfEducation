(function () {
    'use strict';

    app.directive('permission', permission);

    permission.$inject = ['authService'];

    function permission(authService) {
        // Description:
        //     Is used to hide elements which user does not has access to
        // Params:
        //     permission: boolean - check if the user is auth (true or false)
        //     permission-level: string - set the user role (Admin, Librarian, Customer, Any, NotAdmin)
        // Usage:
        //     <div permission="True" permission-role="Any">...</div>
        var directive = {
            link: link,
            restrict: 'A'
        };
        return directive;

        function link(scope, element, attrs) {
            if (element[0]) {
                element = element[0];
            }
            if (!authService.hasPermission(attrs.permission, attrs.permissionRole)) {
                element.style.display = "none";
            }
        }
    }

})();