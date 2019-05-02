'use strict';
app.controller('usersController', ['$scope', 'userService', 'localStorageService', function ($scope, userService, localStorageService) {

    $scope.init = function () {
        var isAdmin = localStorageService.get('authorizationData') !== null && localStorageService.get('authorizationData').roleName === "Admin";
        if (!isAdmin) {
            $location.path('/error');
        }
    };

    $scope.users = [];

    $scope.message = "";

    userService.getUsers().then(function (results) {

        $scope.users = results.data;

    }, function (error) {
        $scope.message = error.data.message;
        });


    $scope.deleteUser = function (id) {
        var userId = $scope.users[id].id;

        userService.deleteUser(userId).then(deleteUserSuccess, deleteUserError);
        function deleteUserSuccess(response) {
            $scope.users.splice(id, 1);
        }
        function deleteUserError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

}]);