'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

    var serviceBase = 'http://localhost:59822/';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: "",
        roleName: ""
    };

    var _saveRegistration = function (registration) {
        _logOut();
        return $http.post(serviceBase + 'api/accounts', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {
        var data = "grant_type=password&username=" + loginData.email + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'oauth/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).then(successOauthToken, errorOauthToken);

        function successOauthToken (response) {
            
            var jwtData = response.data.access_token.split('.')[1];
            var decodedJwtJsonData = window.atob(jwtData);
            var decodedJwtData = JSON.parse(decodedJwtJsonData);
            var roleName = decodedJwtData.role;

            //The best way to store this token is to use AngularJS module named “angular-local-storage”
            localStorageService.set('authorizationData', { token: response.data.access_token, userName: loginData.email, roleName: roleName });


            _authentication.isAuth = true;
            _authentication.userName = loginData.email;
            _authentication.roleName = roleName;

            deferred.resolve(response);
        }

        function errorOauthToken (error) {
            _logOut();
            deferred.reject(error);
        }
        return deferred.promise;

    };

    var _logOut = function () {
        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.userName = "";
        _authentication.roleName = "";

    };

    var _fillAuthData = function () {
        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            _authentication.roleName = authData.roleName;
        }

    };

    var _hasPermission = function (permission, permissionRole){
        if (_authentication.isAuth.toString() === permission && (!permissionRole || permissionRole === "Any" || permissionRole === _authentication.roleName || (permissionRole === "NotAdmin" && _authentication.roleName!=="Admin"))) {
            return true;
        }
        else {
            return false;
        }
    };

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.hasPermission = _hasPermission;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
}]);