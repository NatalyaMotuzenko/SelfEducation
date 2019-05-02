'use strict';
app.controller('usersAddController', ['$scope', 'userService', '$location', function ($scope, userService, $location) {

    $scope.registration = {
        firstName: "",
        lastName: "",
        roleName: "Customer",
        password: "",
        confirmPassword: ""
    };

    $scope.savedSuccessfully = false;

    $scope.message = "";

    $scope.signUp = function () {

        userService.addUser($scope.registration).then(addUserSuccess, addUserError);
        function addUserSuccess(response) {
            $location.path('/accounts');
        }
        function addUserError(response) {
            var errors = [];
            for (var key in response.data.modelState) {
                for (var i = 0; i < response.data.modelState[key].length; i++) {
                    errors.push(response.data.modelState[key][i]);
                }
            }
            $scope.message = "Failed to register user due to: " + errors.join(' ');
        }
    };

}]);