'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', 'localStorageService', function ($scope, $location, $timeout, authService, localStorageService) {

    $scope.savedSuccessfully = false;

    $scope.message = "";

    $scope.registration = {
        firstName: "",
        lastName: "",
        roleName: "Customer",
        password: "",
        confirmPassword: ""
    };

    $scope.signUp = function () {
        authService.saveRegistration($scope.registration).then(successRegistration, errorRegistration);

        function successRegistration(response) {
            $scope.savedSuccessfully = true;
            $scope.message = "User has been registered successfully. Check your email to confirm registration. You will be redicted to login page in 2 seconds.";
            startTimer();

        }

        function errorRegistration (response) {
            var errors = [];
            for (var key in response.data.modelState) {
                for (var i = 0; i < response.data.modelState[key].length; i++) {
                    errors.push(response.data.modelState[key][i]);
                }
            }
            $scope.message = "Failed to register user due to:" + errors.join('/n');
        }
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    };

}]);