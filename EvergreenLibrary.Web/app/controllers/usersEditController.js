'use strict';
app.controller('usersEditController', ['$scope', '$location', 'userService', '$routeParams', function ($scope, $location, userService, $routeParams) {

    $scope.init = function () {
            this.userId = $routeParams.userId;

            userService.getUserById($routeParams.userId).then(function (results) {

            $scope.editUserData = results.data;

        }, function (error) {
            $location.path('/error');
        });
    };
    $scope.init();

    $scope.editUserData = {};

    $scope.message = "";

}]);