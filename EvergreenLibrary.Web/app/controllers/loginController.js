'use strict';
app.controller('loginController', ['$scope', '$location', 'authService', 'localStorageService', function ($scope, $location, authService, localStorageService) {

    $scope.loginData = {
        email: "",
        password: ""
    };

    $scope.message = "";

    $scope.login = function () {

        authService.login($scope.loginData).then(loginSuccess, loginError);
        function loginSuccess(response) {
            if (localStorageService.get('authorizationData').roleName === "Admin")
                $location.path('/accounts');
            else
                $location.path('/books');
        }
        function loginError(err) {
            $scope.message = err.data.error_description;
        }
    };

}]);